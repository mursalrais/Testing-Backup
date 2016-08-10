using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Data;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetAcquisitionService : IAssetAcquisitionService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSACQ_LIST_NAME = "Asset Acquisition";
        const string SP_ASSACQDetails_LIST_NAME = "Asset Acquisition Details";
        const string SP_ACC_MEMO_LIST_NAME = "Acceptance Memo";
        const string SP_WBSMaster_LIST_NAME = "WBS Master";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetAcquisitionHeaderVM GetPopulatedModel(int? ID = default(int?))
        {
            var model = new AssetAcquisitionHeaderVM();
            model.TransactionType = Convert.ToString("Asset Acquisition");
            model.AccpMemo.Choices = GetChoicesFromList(SP_ACC_MEMO_LIST_NAME, "ID", "Title");

            return model;
        }

        private IEnumerable<string> GetChoicesFromList(string listname, string v1, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            foreach (var item in listItems)
            {
                if (v2 != null)
                {
                    _choices.Add(item[v1] + "-" + item[v2].ToString());
                }
                else
                {
                    _choices.Add(item[v1].ToString());
                }
            }
            return _choices.ToArray();
        }

        public int? CreateHeader(AssetAcquisitionHeaderVM viewmodel)
        {
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("Title", viewmodel.TransactionType);
            if (viewmodel.AccpMemo.Value == null)
            {
                return 0;
            }
            string[] memo = viewmodel.AccpMemo.Value.Split('-');
            //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);
            columnValues.Add("Acceptance_x0020_Memo_x0020_No", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
            columnValues.Add("Vendor", viewmodel.Vendor);
            columnValues.Add("PO_x0020_No", viewmodel.PoNo);
            if(viewmodel.PurchaseDate.HasValue)
            {
                columnValues.Add("Purchase_x0020_Date", viewmodel.PurchaseDate);
            }
            else
            {
                columnValues.Add("Purchase_x0020_Date", null);
            }
            
            columnValues.Add("Purchase_x0020_Description", viewmodel.PurchaseDescription);

            try
            {
                SPConnector.AddListItem(SP_ASSACQ_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetAcquisitionHeaderVM();
            entitiy = viewmodel;
            return SPConnector.GetLatestListItemID(SP_ASSACQ_LIST_NAME, _siteUrl);
        }

        public AssetAcquisitionHeaderVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_ASSACQ_LIST_NAME, ID, _siteUrl);
            var viewModel = new AssetAcquisitionHeaderVM();

            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
            viewModel.AccpMemo.Choices = GetChoicesFromList(SP_ACC_MEMO_LIST_NAME, "ID", "Title");
            if ((listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue) != null)
            {
                viewModel.AccpMemo.Value = (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupId.ToString();
                viewModel.AccpMemo.Text = (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupId.ToString() + "-" + (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupValue;
            }
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["Acceptance_x0020_Memo_x0020_No"]);
            viewModel.PoNo = Convert.ToString(listItem["PO_x0020_No"]);
            viewModel.Vendor = Convert.ToString(listItem["Vendor"]);
            if(Convert.ToDateTime(listItem["Purchase_x0020_Date"]) == DateTime.MinValue)
            {
                viewModel.PurchaseDate = null;
            }
            else
            {
                viewModel.PurchaseDate = Convert.ToDateTime(listItem["Purchase_x0020_Date"]);
            }
            viewModel.PurchaseDescription = Convert.ToString(listItem["Purchase_x0020_Description"]);
            viewModel.ID = ID;

            return viewModel;
        }

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
        {
            var models = new List<AssetMasterVM>();

            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            return models;
        }

        public IEnumerable<WBSMaterVM> GetWBS()
        {
            var models = new List<WBSMaterVM>();

            foreach (var item in SPConnector.GetList(SP_WBSMaster_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToModelWBS(item));
            }

            return models;
        }

        private AssetMasterVM ConvertToModelAssetSubAsset(ListItem item)
        {
            var viewModel = new AssetMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.AssetNoAssetDesc.Value = Convert.ToString(item["AssetID"]);
            viewModel.AssetDesc = Convert.ToString(item["Title"]);
            return viewModel;
        }

        private WBSMaterVM ConvertToModelWBS(ListItem item)
        {
            var viewModel = new WBSMaterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.WBSID.Value = Convert.ToString(item["Title"]);
            viewModel.WBSDesc = Convert.ToString(item["WBSDesc"]);
            return viewModel;
        }

        public void CreateDetails(int? headerID, IEnumerable<AssetAcquisitionItemVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_ASSACQDetails_LIST_NAME, item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("Asset_x0020_Acquisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("Asset_x002d_Sub_x0020_Asset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("WBS", new FieldLookupValue { LookupId = Convert.ToInt32(item.WBS.Value.Value) });
                updatedValues.Add("PO_x0020_Line_x0020_Item", item.POLineItem);
                updatedValues.Add("Cost_x0020_IDR", item.CostIDR);
                updatedValues.Add("Cost_x0020_USD", item.CostUSD);
                updatedValues.Add("Remarks", item.Remarks);
                updatedValues.Add("Status", "RUNNING");
                try
                {
                    SPConnector.AddListItem(SP_ASSACQDetails_LIST_NAME, updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        IEnumerable<AssetAcquisitionItemVM> IAssetAcquisitionService.GetDetails(int? headerID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Asset_x0020_Acquisition' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetAcquisitionItemVM>();
            foreach (var item in SPConnector.GetList(SP_ASSACQDetails_LIST_NAME, _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        private AssetAcquisitionItemVM ConvertToDetails(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["Asset_x002d_Sub_x0020_Asset"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["Asset_x002d_Sub_x0020_Asset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            var ListWBS = SPConnector.GetListItem("WBS Master", (item["WBS"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _wbs = new AjaxComboBoxVM();
            _wbs.Value = (item["WBS"] as FieldLookupValue).LookupId;
            _wbs.Text = Convert.ToString(ListWBS["Title"]) + " - " + Convert.ToString(ListWBS["WBSDesc"]);

            return new AssetAcquisitionItemVM
            {
                ID = Convert.ToInt32(item["ID"]),
                POLineItem = Convert.ToString(item["PO_x0020_Line_x0020_Item"]),
                AssetSubAsset = AssetAcquisitionItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                WBS = AssetAcquisitionItemVM.GetWBSDefaultValue(_wbs),
                CostIDR = Convert.ToInt32(item["Cost_x0020_IDR"]),
                CostUSD = Convert.ToInt32(item["Cost_x0020_USD"]),
                Remarks = Convert.ToString(item["Remarks"]),
                Status = Convert.ToString(item["Status"])
            };
        }

        public bool UpdateHeader(AssetAcquisitionHeaderVM viewmodel)
        {
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            //columnValues.add
            columnValues.Add("Title", viewmodel.TransactionType);
            string[] memo = viewmodel.AccpMemo.Value.Split('-');
            //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);
            columnValues.Add("Acceptance_x0020_Memo_x0020_No", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
            columnValues.Add("Vendor", viewmodel.Vendor);
            columnValues.Add("PO_x0020_No", viewmodel.PoNo);
            if (viewmodel.PurchaseDate.HasValue)
            {
                columnValues.Add("Purchase_x0020_Date", viewmodel.PurchaseDate);
            }
            else
            {
                columnValues.Add("Purchase_x0020_Date", null);
            }
            columnValues.Add("Purchase_x0020_Description", viewmodel.PurchaseDescription);

            try
            {
                SPConnector.UpdateListItem(SP_ASSACQ_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetAcquisitionHeaderVM();
            entitiy = viewmodel;
            return true;
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssetAcquisitionItemVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_ASSACQDetails_LIST_NAME, item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("Asset_x0020_Acquisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("Asset_x002d_Sub_x0020_Asset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("WBS", new FieldLookupValue { LookupId = Convert.ToInt32(item.WBS.Value.Value) });
                updatedValues.Add("PO_x0020_Line_x0020_Item", item.POLineItem);
                updatedValues.Add("Cost_x0020_IDR", item.CostIDR);
                updatedValues.Add("Cost_x0020_USD", item.CostUSD);
                updatedValues.Add("Remarks", item.Remarks);
                updatedValues.Add("Status", "RUNNING");
                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem(SP_ASSACQDetails_LIST_NAME, headerID, updatedValues, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_ASSACQDetails_LIST_NAME, updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPUpdateError);
                }
            }
        }

        public AssetAcquisitionItemVM GetPopulatedModelItem(int? ID = default(int?))
        {
            throw new NotImplementedException();
        }

        public int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            var rowTotal = CSVDataTable.Rows.Count;
            var columnTotal = CSVDataTable.Columns.Count;
            var columnTypes = new Type[columnTotal];
            var columnTechnicalNames = new string[columnTotal];

            // After Column Name, the first row should be Column Type
            for (int i = 0; i < columnTotal; i++)
            {
                //format header MUST be technicalname:type or technicalname_lookup:type technicalname_skip:type
                try
                {
                    columnTechnicalNames[i] = CSVDataTable.Columns[i].ColumnName;
                    columnTypes[i] = CSVDataTable.Columns[i].DataType;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
                }
            }

            var updatedValues = new Dictionary<string, object>();
            // Start from 1 since 0 is header 
            for (int i = 0; i < rowTotal; i++)
            {
                for (int j = 0; j < columnTotal; j++)
                {
                    if (isLookup(columnTechnicalNames[j]))
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                            columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: true, skip: false);
                    }
                    else if (isSkipped(columnTechnicalNames[j]))
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                           columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: false, skip: true);
                    }
                    else
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                          columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: false, skip: false);
                    }
                }
                try
                {
                    SPConnector.AddListItem(ListName, updatedValues, SiteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(string.Format("{0} at ID: {1}", e.Message, i + 1));
                    throw new Exception(string.Format("An error occured at ID: {0}. Therefore, data on ID: {0} and afterwards have not been submitted.", i + 1));

                }
                updatedValues = new Dictionary<string, object>();
            }
            return SPConnector.GetLatestListItemID(ListName, SiteUrl);
        }

        private bool isSkipped(string columnName)
        {
            return columnName.Contains("_")
                && string.Compare(columnName.Split('_')[1], "skip", StringComparison.OrdinalIgnoreCase) == 0;
        }

        private bool isLookup(string columnName)
        {
            return columnName.Contains("_")
               && string.Compare(columnName.Split('_')[1], "lookup", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public int? getIdOfColumn(string listname, string SiteUrl, string caml)
        {
            var getItem = SPConnector.GetList(listname, SiteUrl, caml);
            if (getItem.Count != 0 || getItem != null)
            {
                foreach (var item in getItem)
                {
                    return Convert.ToInt32(item["ID"]);
                }
            }
            else
            {
                return 0;
            }
            return 0;
        }

        Dictionary<int, string> IAssetAcquisitionService.getListIDOfList(string listName, string key, string value, string SiteUrl)
        {
            var caml = @"<View><Query />
                        <ViewFields>
                           <FieldRef Name='" + key + @"' />
                           <FieldRef Name='" + value + @"' />
                        </ViewFields>
                        <QueryOptions /></View>";

            var list = SPConnector.GetList(listName, SiteUrl, caml);
            Dictionary<int, string> ids = new Dictionary<int, string>();
            if (list.Count > 0)
            {
                foreach (var l in list)
                {
                    ids.Add(Convert.ToInt32(l[key]), Convert.ToString(l[value]));
                }
            }

            return ids;
        }

        public void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl)
        {
            SPConnector.DeleteListItem(listNameHeader, latestIDHeader, siteUrl);
        }

        public AcceptanceMemoVM GetAcceptanceMemoInfo(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem(SP_ACC_MEMO_LIST_NAME, ID, SiteUrl);
            var viewmodel = new AcceptanceMemoVM();
            viewmodel.ID = Convert.ToInt32(ID);
            viewmodel.VendorID = Convert.ToString(list["VendorID"]);
            viewmodel.VendorName = Convert.ToString(list["Vendor"]);
            viewmodel.PoNo = Convert.ToString(list["PoNo"]);

            return viewmodel;
        }
    }
}

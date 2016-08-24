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
using System.Text.RegularExpressions;

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
            model.CancelURL = _siteUrl + UrlResource.AssetAcquisition;

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

        public int? CreateHeader(AssetAcquisitionHeaderVM viewmodel, string mode = null, string SiteUrl = null)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetAcquisition;
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("Title", "Asset Acquisition");
            if (viewmodel.AccpMemo.Value == null)
            {
                return 0;
            }
            if(mode == null)
            {
                string[] memo = viewmodel.AccpMemo.Value.Split('-');
                //columnValues.Add("acceptancememono", memo[1]);
                columnValues.Add("acceptancememono", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
                var memoinfo = SPConnector.GetListItem(SP_ACC_MEMO_LIST_NAME, Convert.ToInt32(memo[0]), _siteUrl);
                columnValues.Add("vendorid", memoinfo["vendorid"]);
                columnValues.Add("vendorname", memoinfo["vendorname"]);
                columnValues.Add("pono", memoinfo["pono"]);
            }
            else
            {
                columnValues.Add("acceptancememono", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.AccpMemo.Value)});
                var memoinfo = GetAcceptanceMemoInfo(Convert.ToInt32(viewmodel.AccpMemo.Value), SiteUrl);
                columnValues.Add("vendorid", memoinfo.VendorID);
                columnValues.Add("vendorname", memoinfo.VendorName);
                columnValues.Add("pono", memoinfo.PoNo);
            }
            
            columnValues.Add("purchasedate", viewmodel.PurchaseDate);
            
            columnValues.Add("purchasedescription", viewmodel.PurchaseDescription);

            try
            {
                if(mode == null)
                {
                    SPConnector.AddListItem(SP_ASSACQ_LIST_NAME, columnValues, _siteUrl);
                }
                else
                {
                    SPConnector.AddListItem(SP_ASSACQ_LIST_NAME, columnValues, SiteUrl);
                }
                
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetAcquisitionHeaderVM();
            entitiy = viewmodel;
            if(mode == null)
            {
                return SPConnector.GetLatestListItemID(SP_ASSACQ_LIST_NAME, _siteUrl);
            }
            else
            {
                return SPConnector.GetLatestListItemID(SP_ASSACQ_LIST_NAME, SiteUrl);
            }
            
        }

        public AssetAcquisitionHeaderVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_ASSACQ_LIST_NAME, ID, _siteUrl);
            var viewModel = new AssetAcquisitionHeaderVM();

            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
            viewModel.AccpMemo.Choices = GetChoicesFromList(SP_ACC_MEMO_LIST_NAME, "ID", "Title");
            if ((listItem["acceptancememono"] as FieldLookupValue) != null)
            {
                viewModel.AccpMemo.Value = (listItem["acceptancememono"] as FieldLookupValue).LookupId.ToString();
                viewModel.AccpMemo.Text = (listItem["acceptancememono"] as FieldLookupValue).LookupId.ToString() + "-" + (listItem["acceptancememono"] as FieldLookupValue).LookupValue;
            }
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["acceptancememono"]);
            viewModel.PoNo = Convert.ToString(listItem["pono"]);
            viewModel.Vendor = Convert.ToString(listItem["vendorid"])+"-"+Convert.ToString(listItem["vendorname"]);

            viewModel.PurchaseDate = Convert.ToDateTime(listItem["purchasedate"]);
            //viewModel.Spesifications = Regex.Replace(listItem["Spesifications"].ToString(), "<.*?>", string.Empty);
            viewModel.PurchaseDescription = Regex.Replace(Convert.ToString(listItem["purchasedescription"]), "<.*?>", string.Empty);
            viewModel.ID = ID;

            viewModel.CancelURL = _siteUrl + UrlResource.AssetAcquisition;

            return viewModel;
        }

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
        {
            var models = new List<AssetMasterVM>();
            var caml = @"<View><Query>
                       <Where>
                          <Geq>
                             <FieldRef Name='AssetID' />
                             <Value Type='Text'>14</Value>
                          </Geq>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='AssetID' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='AssetID' />
                    </ViewFields>
                    <QueryOptions /></View>";
            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl, caml))
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

                if (item.AssetSubAsset.Value == null || item.WBS.Value == null || item.CostIDR == null || item.CostUSD == null)
                {
                    throw new Exception(ErrorResource.SPInsertError);
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetacquisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("wbs", new FieldLookupValue { LookupId = Convert.ToInt32(item.WBS.Value.Value) });
                updatedValues.Add("polineitem", item.POLineItem);
                updatedValues.Add("costidr", item.CostIDR);
                updatedValues.Add("costusd", item.CostUSD);
                updatedValues.Add("remarks", item.Remarks);
                updatedValues.Add("status", "RUNNING");
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
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetacquisition' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetAcquisitionItemVM>();
            foreach (var item in SPConnector.GetList(SP_ASSACQDetails_LIST_NAME, _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        private AssetAcquisitionItemVM ConvertToDetails(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            var ListWBS = SPConnector.GetListItem("WBS Master", (item["wbs"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _wbs = new AjaxComboBoxVM();
            _wbs.Value = (item["wbs"] as FieldLookupValue).LookupId;
            _wbs.Text = Convert.ToString(ListWBS["Title"]) + " - " + Convert.ToString(ListWBS["WBSDesc"]);

            return new AssetAcquisitionItemVM
            {
                ID = Convert.ToInt32(item["ID"]),
                POLineItem = Convert.ToString(item["polineitem"]),
                AssetSubAsset = AssetAcquisitionItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                WBS = AssetAcquisitionItemVM.GetWBSDefaultValue(_wbs),
                CostIDR = Convert.ToInt32(item["costidr"]),
                CostUSD = Convert.ToInt32(item["costusd"]),
                Remarks = Convert.ToString(item["remarks"]),
                Status = Convert.ToString(item["status"])
            };
        }

        public bool UpdateHeader(AssetAcquisitionHeaderVM viewmodel)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetAcquisition;
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            //columnValues.add
            columnValues.Add("Title", "Asset Acquisition");
            string[] memo = viewmodel.AccpMemo.Value.Split('-');
            var memoinfo = SPConnector.GetListItem(SP_ACC_MEMO_LIST_NAME, Convert.ToInt32(memo[0]), _siteUrl);
            //columnValues.Add("acceptancememono", memo[1]);
            columnValues.Add("acceptancememono", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
            columnValues.Add("vendorid", memoinfo["vendorid"]);
            columnValues.Add("vendorname", memoinfo["vendorname"]);
            columnValues.Add("pono", memoinfo["pono"]);
            columnValues.Add("purchasedate", Convert.ToDateTime(viewmodel.PurchaseDate));
            columnValues.Add("purchasedescription", viewmodel.PurchaseDescription);

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

                if (item.AssetSubAsset.Value == null || item.WBS.Value == null || item.CostIDR == null || item.CostUSD == null)
                {
                    throw new Exception(ErrorResource.SPInsertError);
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetacquisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("wbs", new FieldLookupValue { LookupId = Convert.ToInt32(item.WBS.Value.Value) });
                updatedValues.Add("polineitem", item.POLineItem);
                updatedValues.Add("costidr", item.CostIDR);
                updatedValues.Add("costusd", item.CostUSD);
                updatedValues.Add("remarks", item.Remarks);
                updatedValues.Add("status", "RUNNING");
                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem(SP_ASSACQDetails_LIST_NAME, item.ID, updatedValues, _siteUrl);
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
            int? latestHeaderID = 0;
            int latestDetailID = 0;
            if (ListName == SP_ASSACQ_LIST_NAME)
            {
                foreach(DataRow d in CSVDataTable.Rows)
                {
                    var model = new AssetAcquisitionHeaderVM();
                    model.TransactionType = d.ItemArray[0].ToString();
                    model.AccpMemo.Value = d.ItemArray[1].ToString();
                    model.VendorID = d.ItemArray[2].ToString();
                    model.Vendor = d.ItemArray[3].ToString();
                    model.PoNo = d.ItemArray[4].ToString();
                    model.PurchaseDate = Convert.ToDateTime(d.ItemArray[5]);
                    model.PurchaseDescription = d.ItemArray[6].ToString();

                    latestHeaderID =  CreateHeader(model, "upload", SiteUrl);
                }
            }

            if (ListName == SP_ASSACQDetails_LIST_NAME)
            {
                foreach (DataRow d in CSVDataTable.Rows)
                {
                    var model = new AssetAcquisitionItemVM();
                    model.POLineItem = Convert.ToString(d.ItemArray[1]);
                    model.AssetSubAsset.Value = Convert.ToInt32(d.ItemArray[2]);
                    model.WBS.Value = Convert.ToInt32(d.ItemArray[3]);
                    model.CostIDR = Convert.ToInt32(d.ItemArray[4]);
                    model.CostUSD = Convert.ToInt32(d.ItemArray[5]);
                    model.Remarks = Convert.ToString(d.ItemArray[6]);
                    model.Status = Convert.ToString(d.ItemArray[7]);

                    CreateDetails(Convert.ToInt32(d.ItemArray[0]), model, SiteUrl);
                }
            }

            return SPConnector.GetLatestListItemID(ListName, SiteUrl);
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
            viewmodel.VendorID = Convert.ToString(list["vendorid"]);
            viewmodel.VendorName = Convert.ToString(list["vendorname"]);
            viewmodel.PoNo = Convert.ToString(list["pono"]);

            return viewmodel;
        }

        public bool MassUploadBreakDown(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetSubAsst(string mainsubasset, string SiteUrl)
        {
            string caml = @"<View><Query>
                       <Where>
                          <Contains>
                             <FieldRef Name='AssetID' />
                             <Value Type='Text'>"+ mainsubasset + @"</Value>
                          </Contains>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='ID' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='AssetID' />
                       <FieldRef Name='Title' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var list = SPConnector.GetList("Asset Master", SiteUrl, caml);

            var viewmodel = new List<string>();
            foreach(var l in list)
            {
                if(Convert.ToString(l["AssetID"]).Length >14 )
                {
                    viewmodel.Add(Convert.ToString(l["AssetID"]) +"-"+ Convert.ToString(l["Title"]));
                }
            }
            
            return viewmodel;
        }

        public bool Syncronize(string SiteUrl)
        {
            var lists = SPConnector.GetList(SP_ACC_MEMO_LIST_NAME, SiteUrl);
            foreach(var l in lists)
            {
                var caml = @"<View><Query>
                            <Where>
                                <Eq>
                                    <FieldRef Name='acceptancememono' />
                                    <Value Type='Lookup'>"+l["Title"] +@"</Value> 
                                </Eq>
                            </Where>
                            </Query>
                            <ViewFields />
                            <QueryOptions /></View>";
                var getAsset = SPConnector.GetList(SP_ASSACQ_LIST_NAME, SiteUrl, caml);
                foreach(var ass in getAsset)
                {
                    var model = new Dictionary<string, object>();
                    model.Add("vendorname", Convert.ToString(l["vendorname"]));
                    model.Add("vendorid", Convert.ToString(l["vendorid"]));
                    model.Add("pono", Convert.ToString(l["pono"]));

                    SPConnector.UpdateListItem(SP_ASSACQ_LIST_NAME, Convert.ToInt32(ass["ID"]), model, SiteUrl);
                }                
            }

            return true;
        }

        public int? CreateDetails(int? headerID, AssetAcquisitionItemVM item, string SiteUrl = null)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("assetacquisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
            updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
            updatedValues.Add("wbs", new FieldLookupValue { LookupId = Convert.ToInt32(item.WBS.Value.Value) });
            updatedValues.Add("polineitem", item.POLineItem);
            updatedValues.Add("costidr", item.CostIDR);
            updatedValues.Add("costusd", item.CostUSD);
            updatedValues.Add("remarks", item.Remarks);
            updatedValues.Add("status", "RUNNING");
            try
            {
                SPConnector.AddListItem(SP_ASSACQDetails_LIST_NAME, updatedValues, SiteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(SP_ASSACQDetails_LIST_NAME, SiteUrl);
        }
    }
}

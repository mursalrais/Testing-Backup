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

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetAcquisitionService : IAssetAcquisitionService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/bo/";
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSACQ_LIST_NAME = "Asset Acquisition";
        const string SP_ASSACQDetails_LIST_NAME = "Asset Acquisition Details";
        const string SP_ACC_MEMO_LIST_NAME = "Acceptance Memo";

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
            string[] memo = viewmodel.AccpMemo.Value.Split('-');
            //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);
            columnValues.Add("Acceptance_x0020_Memo_x0020_No", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
            columnValues.Add("Vendor", viewmodel.Vendor);
            columnValues.Add("PO_x0020_No", viewmodel.PoNo);
            columnValues.Add("Purchase_x0020_Date", viewmodel.PurchaseDate);
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
                viewModel.AccpMemo.Text = (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupId.ToString()+"-"+(listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupValue;
            }
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["Acceptance_x0020_Memo_x0020_No"]);
            viewModel.PoNo = Convert.ToString(listItem["PO_x0020_No"]);
            viewModel.Vendor = Convert.ToString(listItem["Vendor"]);
            viewModel.PurchaseDate = Convert.ToDateTime(listItem["Purchase_x0020_Date"]);
            viewModel.PurchaseDescription = Convert.ToString(listItem["Purchase_x0020_Description"]);
            viewModel.ID = ID;

            return viewModel;
        }

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
        {
            var models = new List<AssetMasterVM>();

            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl))
            {
                models.Add(ConvertToModel(item));
            }

            return models;
        }

        private AssetMasterVM ConvertToModel(ListItem item)
        {
            var viewModel = new AssetMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.AssetNoAssetDesc.Value = Convert.ToString(item["AssetID"]);
            viewModel.AssetDesc = Convert.ToString(item["Title"]);
            return viewModel;
        }

        public AssetAcquisitionItemVM GetPopulatedModelItem(int? ID = default(int?))
        {
            var model = new AssetAcquisitionItemVM();
            //model.AssetSubAsset.Choices = GetChoicesFromList("Asset Master", "AssetID");

            return model;
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
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Asset_x0020_Acquisition' /><Value Type='Lookup'>"+headerID.ToString()+"</Value></Eq></Where></Query></View>";
            var details = new List<AssetAcquisitionItemVM>();
            foreach(var item in SPConnector.GetList(SP_ASSACQDetails_LIST_NAME, _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        private AssetAcquisitionItemVM ConvertToDetails(ListItem item)
        {
            var details = new AssetAcquisitionItemVM();
            details.ID = Convert.ToInt32(item["ID"]);
            //if (details.AssetSubAsset as FieldLookupValue != null)
            //{
            //    viewModel.AccpMemo.Value = (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupId.ToString();
            //    viewModel.AccpMemo.Text = (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupId.ToString() + "-" + (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupValue;
            //}
            details.AssetSubAsset = AssetAcquisitionItemVM.GetAssetSubAssetDefaultValue(FormatUtil.ConvertToInGridAjaxComboBox(item, "Asset_x002d_Sub_x0020_Asset"));

            return details;
            //return new AssetAcquisitionItemVM
            //{
            //    ID = Convert.ToInt32(item["ID"]),
            //    //viewModel.AccpMemo.Text = (listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupId.ToString()+"-"+(listItem["Acceptance_x0020_Memo_x0020_No"] as FieldLookupValue).LookupValue;
            //    AssetSubAsset = AssetAcquisitionItemVM.GetAssetSubAssetDefaultValue(FormatUtil.ConvertToInGridAjaxComboBox(item, "Asset_x002d_Sub_x0020_Asset"))
            //};
        }
    }
}

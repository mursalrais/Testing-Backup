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
        const string SP_ASSACQ_LIST_NAME = "Asset Acqusitiion";
        const string SP_ASSACQDetails_LIST_NAME = "Asset Acqusitiion Details";
        const string SP_ACC_MEMO_LIST_NAME = "Acceptance Memo";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetAcquisitionHeaderVM getPopulatedModel(int? id = default(int?))
        {
            var model = new AssetAcquisitionHeaderVM();
            model.AcceptanceMemoNo.Choices = GetChoiceFromList(SP_ACC_MEMO_LIST_NAME, "Title");

            return model;
        }

        private string[] GetChoiceFromList(string list, string fieldName)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(list, _siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item[fieldName].ToString());
            }
            return _choices.ToArray();
        }

        public AssetAcquisitionHeaderVM getHeader(int? ID)
        {
            var listitem = SPConnector.GetListItem(SP_ASSACQ_LIST_NAME, ID, _siteUrl);

            return ConvertToAssetAcquisition(listitem);
        }

        private AssetAcquisitionHeaderVM ConvertToAssetAcquisition(ListItem listitem)
        {
            
            var viewmodel = new AssetAcquisitionHeaderVM();
            viewmodel.ID = Convert.ToInt32(listitem["ID"]);
            viewmodel.TransactionType = Convert.ToString(listitem["TransactionType"]);
            viewmodel.AcceptanceMemoNo.Choices = GetChoiceFromList(SP_ACC_MEMO_LIST_NAME, "Title");
            viewmodel.Vendor = Convert.ToString(listitem["Vendor"]);
            viewmodel.PoNo = Convert.ToString(listitem["PoNo"]);
            viewmodel.PurchaseDate = Convert.ToDateTime(listitem["PurchaseDate"]);
            viewmodel.PurchaseDescription = Convert.ToString(listitem["PurchaseDescription"]);

            viewmodel.AssetAcquisitionItems = GetAssetAcqDetails(viewmodel.ID);

            return viewmodel;

        }

        private IEnumerable<AssetAcquisitionItemVM> GetAssetAcqDetails(int? iD)
        {
            var caml = "";

            var assetAcqDetails = new List<AssetAcquisitionItemVM>();
            foreach(var item in SPConnector.GetList(SP_ASSACQDetails_LIST_NAME, _siteUrl, caml))
            {
                assetAcqDetails.Add(ConvertToAssetAqcuisitionDetails(item));
            }

            return assetAcqDetails;
        }

        private AssetAcquisitionItemVM ConvertToAssetAqcuisitionDetails(ListItem item)
        {
            return new AssetAcquisitionItemVM
            {
                ID = Convert.ToInt32(item["ID"]),
                POLineItem = Convert.ToString(item["POLineItem"]),
                AssetSubAsset = Convert.ToString(item["AssetSubAsset"]),
                WBSNo = Convert.ToString(item["wbs"]),
                CostIDR = Convert.ToInt32(item["CostIDR"]),
                CostUSD = Convert.ToInt32(item["CostUSD"]),
                Remakrs = Convert.ToString(item["Remakrs"]),
                Status = Convert.ToString(item["Status"])
            };
        }

        public int createHeader(AssetAcquisitionHeaderVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("TransactionType", header.TransactionType);
            columnValues.Add("AcceptanceMemoNo", header.AcceptanceMemoNo);
            columnValues.Add("Vendor", header.Vendor);
            columnValues.Add("PoNo", header.PoNo);
            columnValues.Add("PurchaseDate", header.PurchaseDate);
            columnValues.Add("PurchaseDescription", header.PurchaseDescription);

            try
            {
                SPConnector.AddListItem(SP_ASSACQ_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_ASSACQ_LIST_NAME, _siteUrl);
        }

        public bool updateHeader(AssetAcquisitionHeaderVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? headerID = header.ID;
            columnValues.Add("TransactionType", header.TransactionType);
            columnValues.Add("AcceptanceMemoNo", header.AcceptanceMemoNo);
            columnValues.Add("Vendor", header.Vendor);
            columnValues.Add("PoNo", header.PoNo);
            columnValues.Add("PurchaseDate", header.PurchaseDate);
            columnValues.Add("PurchaseDescription", header.PurchaseDescription);

            try
            {
                SPConnector.UpdateListItem(SP_ASSACQ_LIST_NAME, headerID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            var entity = new AssetAcquisitionHeaderVM();
            entity = header;

            return true;
        }

        public void createAssetAcquisitionItems(int? headerID, IEnumerable<AssetAcquisitionItemVM> items)
        {
            //foreach(var viewmodel in items)
            //{
            //    if(Item.CheckIfSkipped(viewmodel))
            //    {
            //        continue;
            //    }

            //    if(Item.CheckIfDeleted(viewmodel))
            //    {
            //        try
            //        {
            //            //delete item
            //        }
            //        catch (Exception e)
            //        {
            //            logger.Error(e);
            //            throw e;
            //        }
            //        continue;
            //    }
            //    var updatedValues = new Dictionary<string, object>();
            //    updatedValues.Add("POLineItem", viewmodel.POLineItem);
            //    updatedValues.Add("POLineItem", viewmodel.POLineItem);

            //}
        }
    }
}

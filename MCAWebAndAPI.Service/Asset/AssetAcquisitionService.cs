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

        public AssetAcquisitionHeaderVM GetPopulatedModel(int? ID = default(int?))
        {
            var model = new AssetAcquisitionHeaderVM();

            return model;
        }

        public AssetAcquisitionHeaderVM GetHeader(int? ID)
        {
            var listitem = SPConnector.GetListItem(SP_ASSACQ_LIST_NAME, ID, _siteUrl);

            return ConvertToHeaderModel(listitem);
        }

        private AssetAcquisitionHeaderVM ConvertToHeaderModel(ListItem listitem)
        {
            var viewmodel = new AssetAcquisitionHeaderVM();

            viewmodel.ID = Convert.ToInt32(listitem["ID"]);
            viewmodel.TransactionType = Convert.ToString(listitem["TransactionType"]);
            viewmodel.AcceptanceMemoNoID.Value = FormatUtil.ConvertLookupToID(listitem, "");
            viewmodel.AcceptanceMemoNoString = FormatUtil.ConvertLookupToValue(listitem, "");
            viewmodel.Vendor = Convert.ToString(listitem["Vendor"]);
            viewmodel.PoNo = Convert.ToString(listitem["PoNo"]);
            viewmodel.PurchaseDate = Convert.ToDateTime(listitem["PurchaseDate"]);
            viewmodel.PurchaseDescription = Convert.ToString(listitem["PurchaseDescription"]);

            //viewmodel.AssetAcquisitionDetails = GetAssetDetails(viewmodel.ID);

            return viewmodel;
        }

        //private IEnumerable<AssetAcquisitionItemVM> GetAssetDetails(int? iD)
        //{
        //    var caml = "";

        //    var details = new List<AssetAcquisitionItemVM>();
        //    foreach(var item in SPConnector.GetList(SP_ASSACQDetails_LIST_NAME, _siteUrl, caml))
        //    {
        //        details.Add(ConvertToItemModel(item));
        //    }
        //    return details;
        //}

        //private AssetAcquisitionItemVM ConvertToItemModel(ListItem item)
        //{
        //    return new AssetAcquisitionItemVM
        //    {
        //        ID = Convert.ToInt32(item["ID"]),
        //        PoLineItem = Convert.ToString(item["PoLineItem"]),
        //        AssetSubAssetID = FormatUtil.ConvertLookupToID(item, "");


        //};
        //}

        //public int CreateHeader(AssetAcquisitionHeaderVM header)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool UpdateHeader(AssetAcquisitionHeaderVM header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CreateDetails(int? headerID, IEnumerable<AssetAcquisitionItemVM> details)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

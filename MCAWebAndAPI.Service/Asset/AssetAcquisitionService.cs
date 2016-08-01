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

        public AssetAcquisitionVM GetAssetAcquisition()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetAcquisition(AssetAcquisitionVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetAcquisition(AssetAcquisitionVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetAcquisitionVM> IAssetAcquisitionService.GetAssetAcquisition()
        {
            throw new NotImplementedException();
        }

        public AssetAcquisitionVM GetAssetAcquisitionItems_Dummy()
        {
            var viewModel = new AssetAcquisitionVM();



            return viewModel;
        }

        public bool CreateAssetAcquisition_Dummy(AssetAcquisitionItemVM assetAcquisition)
        {
            var entity = new AssetAcquisitionItemVM();
            entity = assetAcquisition;
            return true;
        }

        public bool UpdateAssetAcquisition_Dummy(AssetAcquisitionItemVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetAcquisition_Dummy(AssetAcquisitionItemVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

    }
}

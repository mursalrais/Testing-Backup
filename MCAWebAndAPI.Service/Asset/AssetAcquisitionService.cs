using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetAcquisitionService : IAssetAcquisitionService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetAcquisitionVM GetAssetAcquisitions()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetAcquisition(AssetAcquisitionVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetAcquisition_dummy(AssetAcquisitionVM assetAcquisition)
        {
            var entitiy = new AssetAcquisitionVM();
            entitiy = assetAcquisition;
            return true;
        }

        public bool CreateAssetAcquisitionItem_dummy(AssetAcquisitionItemVM assetAcquisition)
        {
            var entitiy = new AssetAcquisitionItemVM();
            entitiy = assetAcquisition;
            return true;
        }

        public bool UpdateAssetAcquisition(AssetAcquisitionVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetAcquisitionVM> IAssetAcquisitionService.GetAssetAcquisitions()
        {
            throw new NotImplementedException();
        }

        public AssetAcquisitionVM GetAssetAcquisition_Dummy()
        {
            var viewModel = new AssetAcquisitionVM();

            var list = new List<AssetAcquisitionItemVM>();
            list.Add(new AssetAcquisitionItemVM()
            {
                AssetDescription = "Asseasfas",
                CostUSD = 20000,
                New = "Newwww",
                Id = 1                
            });
            viewModel.Items = list;

            return viewModel;
        }
    }
}

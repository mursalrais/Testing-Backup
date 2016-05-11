using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetScrappingService : IAssetScrappingService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetScrappingVM GetAssetScrapping()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetScrapping(AssetScrappingVM assetScrapping)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetScrapping(AssetScrappingVM assetScrapping)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetScrappingVM> IAssetScrappingService.GetAssetScrapping()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetScrapping_Dummy(AssetScrappingItemVM assetScrapping)
        {
            var entity = new AssetScrappingItemVM();
            entity = assetScrapping;
            return true;
        }

        public bool UpdateAssetScrapping_Dummy(AssetScrappingItemVM assetScrapping)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetScrapping_Dummy(AssetScrappingItemVM assetScrapping)
        {
            throw new NotImplementedException();
        }

        public AssetScrappingVM GetssetScrappingItems_Dummy()
        {
            var viewModel = new AssetScrappingVM();

            var list = new List<AssetScrappingItemVM>();
            list.Add(new AssetScrappingItemVM()
            {
                NewAsset = "New",
                Item = "Chair",
                AssetDescription = "New Asset",
                Id = 1
            });
            viewModel.Header.TransactionType = "Cash";
            viewModel.Items = list;

            return viewModel;
        }
    }
}

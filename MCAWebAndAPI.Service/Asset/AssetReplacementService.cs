using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
     public class AssetReplacementService : IAssetReplacementService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetReplacementVM GetAssetReplacement()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetReplacement(AssetReplacementVM assetReplacement)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetReplacement(AssetReplacementVM assetReplacement)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetReplacementVM> IAssetReplacementService.GetAssetReplacement()
        {
            throw new NotImplementedException();
        }   

        public AssetReplacementVM GetAssetReplacementItems_Dummy()
        {
            var viewModel = new AssetReplacementVM();

            var list = new List<AssetReplacementItemVM>();
            list.Add(new AssetReplacementItemVM()
            {
                NewAsset = "New",
                Item = "Chair",
                WbsId = 123,
                Id = 1
            });
            viewModel.Header.TransactionType = "Cash";
            viewModel.Items = list;

            return viewModel;
        }

        public bool CreateAssetReplacement_Dummy(AssetReplacementItemVM assetReplacement)
        {
            var entity = new AssetReplacementItemVM();
            entity = assetReplacement;
            return true;
        }

        public bool UpdateAssetReplacement_Dummy(AssetReplacementItemVM assetReplacement)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetReplacement_Dummy(AssetReplacementItemVM assetReplacement)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetCheckResultService : IAssetCheckResultService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetCheckResultVM GetAssetCheckResult()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResult(AssetCheckResultVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetCheckResult(AssetCheckResultVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetCheckResultVM> IAssetCheckResultService.GetAssetCheckResult()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            var entity = new AssetCheckResultItemVM();
            entity = assetCheckResult;
            return true;
        }

        public bool UpdateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public AssetCheckResultVM GetAssetCheckResultItems_Dummy()
        {
            var viewModel = new AssetCheckResultVM();

            var list = new List<AssetCheckResultItemVM>();
            list.Add(new AssetCheckResultItemVM()
            {
                AssetNo = 34,
                Item = "Chair",
                AssetDescription = "New Asset",
                Id = 1
            });
            viewModel.Items = list;

            return viewModel;
        }
    }
}

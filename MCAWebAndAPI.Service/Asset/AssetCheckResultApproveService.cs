using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetCheckResultApproveService : IAssetCheckResultApproveService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetCheckResultApproveVM GetAssetCheckResultApprove()
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetCheckResultApproveVM> IAssetCheckResultApproveService.GetAssetCheckResultApprove()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResultApprove(AssetCheckResultApproveVM assetCheckResultApprove)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetCheckResultApprove(AssetCheckResultApproveVM assetCheckResultApprove)
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResultApprove_Dummy(AssetCheckResultApproveItemVM assetCheckResultApprove)
        {
            var entity = new AssetCheckResultApproveItemVM();
            entity = assetCheckResultApprove;
            return true;
        }

        public bool UpdateAssetCheckResultApprove_Dummy(AssetCheckResultApproveItemVM assetCheckResultApprove)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetCheckResultApprove_Dummy(AssetCheckResultApproveItemVM assetCheckResultApprove)
        {
            throw new NotImplementedException();
        }

        public AssetCheckResultApproveVM GetAssetCheckResultApproveItems_Dummy()
        {
            var viewModel = new AssetCheckResultApproveVM();

            var list = new List<AssetCheckResultApproveItemVM>();
            list.Add(new AssetCheckResultApproveItemVM()
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

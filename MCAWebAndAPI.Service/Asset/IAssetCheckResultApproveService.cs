using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetCheckResultApproveService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetCheckResultApproveVM> GetAssetCheckResultApprove();

        bool CreateAssetCheckResultApprove(AssetCheckResultApproveVM assetCheckResultApprove);

        bool UpdateAssetCheckResultApprove(AssetCheckResultApproveVM assetCheckResultApprove);

        bool CreateAssetCheckResultApprove_Dummy(AssetCheckResultApproveItemVM assetCheckResultApprove);

        bool UpdateAssetCheckResultApprove_Dummy(AssetCheckResultApproveItemVM assetCheckResultApprove);

        bool DestroyAssetCheckResultApprove_Dummy(AssetCheckResultApproveItemVM assetCheckResultApprove);

        AssetCheckResultApproveVM GetAssetCheckResultApproveItems_Dummy();
    }
}

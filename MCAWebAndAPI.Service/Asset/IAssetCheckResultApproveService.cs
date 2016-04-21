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

        bool CreateAssetCheckResultApprove(AssetReplacementVM assetCheckResultApprove);

        bool UpdateAssetCheckResultApprove(AssetReplacementVM assetCheckResultApprove);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetCheckResultService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetCheckResultVM> GetAssetCheckResult();

        bool CreateAssetCheckResult(AssetCheckResultVM assetCheckResult);

        bool UpdateAssetCheckResult(AssetCheckResultVM assetCheckResult);

        bool CreateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult);

        bool UpdateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult);

        bool DestroyAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult);

        AssetCheckResultVM GetAssetCheckResultItems_Dummy();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetReplacementService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetReplacementVM> GetAssetReplacement();

        bool CreateAssetReplacement(AssetReplacementVM assetReplacement);

        bool UpdateAssetReplacement(AssetReplacementVM assetReplacement);

        bool CreateAssetReplacement_Dummy(AssetReplacementItemVM assetReplacement);

        bool UpdateAssetReplacement_Dummy(AssetReplacementItemVM assetReplacement);

        bool DestroyAssetReplacement_Dummy(AssetReplacementItemVM assetReplacement);

        AssetReplacementVM GetAssetReplacementItems_Dummy();
    }
}

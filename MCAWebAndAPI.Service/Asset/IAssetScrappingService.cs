using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetScrappingService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetScrappingVM> GetAssetScrapping();

        bool CreateAssetScrapping(AssetScrappingVM assetScrapping);

        bool UpdateAssetScrapping(AssetScrappingVM assetScrapping);

        bool CreateAssetScrapping_Dummy(AssetScrappingItemVM assetScrapping);

        bool UpdateAssetScrapping_Dummy(AssetScrappingItemVM assetScrapping);

        bool DestroyAssetScrapping_Dummy(AssetScrappingItemVM assetScrapping);

        AssetScrappingVM GetssetScrappingItems_Dummy();
    }
}

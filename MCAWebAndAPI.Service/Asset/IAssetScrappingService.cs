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
    }
}

using System;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetDisposalService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetDisposalVM> GetAssetTransfers();

        int CreateHeader(AssetDisposalVM header);

        bool CreateAssetTransfer(AssetDisposalVM assetTransfer);

        bool UpdateAssetTransfer(AssetDisposalVM assetTransfer);

        AssetDisposalVM GetPopulatedModel(int? id = null);

        AssetDisposalVM GetAssetHolderFromInfo(int? ID, string siteUrl);
    }
}

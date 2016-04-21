using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetTransferService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetTransferVM> GetAssetTransfers();

        bool CreateAssetTransfer(AssetTransferVM assetTransfer);

        bool UpdateAssetTransfer(AssetTransferVM assetTransfer);
    }
}

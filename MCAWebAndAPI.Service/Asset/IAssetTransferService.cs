using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
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

        IEnumerable<AssetTransactionVM> GetAssetTransfers();

        int CreateHeader(AssetTransferVM header);

        bool CreateAssetTransfer(AssetTransactionVM assetTransfer);

        bool UpdateAssetTransfer(AssetTransactionVM assetTransfer);

        AssetTransferVM GetPopulatedModel(int? id = null);

        ProfessionalVM GetAssetHolderFromInfo(int? ID, string siteUrl);

        IEnumerable<LocationMasterVM> GetProvince();
    }
}

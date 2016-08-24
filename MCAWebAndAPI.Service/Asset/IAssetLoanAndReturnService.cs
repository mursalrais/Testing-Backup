using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetLoanAndReturnService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetLoanAndReturnItemVM> GetAssetTransfers();

        int CreateHeader(AssetLoanAndReturnHeaderVM header);

        int? CreateHeader(AssetLoanAndReturnHeaderVM viewmodel, string mode = null, string SiteUrl = null);

        bool CreateAssetTransfer(AssetLoanAndReturnItemVM assetTransfer);

        bool UpdateAssetTransfer(AssetLoanAndReturnItemVM assetTransfer);

        AssetLoanAndReturnHeaderVM GetPopulatedModel(int? id = null);

        ProfessionalsVM GetProfessionalInfo(int? ID, string SiteUrl);

        IEnumerable<AssetMasterVM> GetAssetSubAsset();
    }
}

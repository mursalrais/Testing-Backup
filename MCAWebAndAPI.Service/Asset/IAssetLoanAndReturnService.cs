using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
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

        // int CreateHeader(AssetLoanAndReturnHeaderVM header);

        int? CreateHeader(AssetLoanAndReturnHeaderVM viewmodel, string mode = null, string SiteUrl = null);

        //int? CreateDetails(int? headerID, AssetLoanAndReturnItemVM item, string SiteUrl = null);
        //void CreateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items);

        void CreateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items);
        int? CreateDetails(int? headerID, AssetLoanAndReturnItemVM item, string SiteUrl = null);

        void UpdateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items);

        //AssetLoanAndReturnHeaderVM GetHeader(int? ID);

        AssetLoanAndReturnHeaderVM GetHeader(int? ID = null, string SiteUrl = null);

        ProfessionalDataVM GetProfMasterInfo(string fullname, string SiteUrl);

        IEnumerable<AssetLoanAndReturnItemVM> GetDetails(int? headerID);

        bool CreateAssetTransfer(AssetLoanAndReturnItemVM assetTransfer);

        bool UpdateAssetTransfer(AssetLoanAndReturnItemVM assetTransfer);

        //AssetLoanAndReturnHeaderVM GetPopulatedModel(int? id = null);

        AssetLoanAndReturnHeaderVM GetPopulatedModel(string SiteUrl);

        ProfessionalsVM GetProfessionalInfo(int? ID, string SiteUrl);

        IEnumerable<AssetAcquisitionItemVM> GetAssetSubAsset();

        bool UpdateHeader(AssetLoanAndReturnHeaderVM viewmodel);
    }
}
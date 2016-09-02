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

        //create empty form
        AssetReplacementHeaderVM GetPopulatedModel(int? ID = null);

        int? CreateHeader(AssetReplacementHeaderVM viewmodel, int id, string SiteUrl);
        bool UpdateHeader(AssetReplacementHeaderVM viewmodel);

        bool Syncronize(string SiteUrl);

        void CreateDetails(int? headerID, IEnumerable<AssetReplacementItemVM> items);
        void UpdateDetails(int? headerID, IEnumerable<AssetReplacementItemVM> items);

        AssetReplacementHeaderVM GetHeader(int? ID);
        IEnumerable<AssetReplacementItemVM> GetDetails(int? headerID);

        AssetReplacementHeaderVM GetInfoFromAcquisitin(int? ID, string SiteUrl);
        IEnumerable<AssetReplacementItemVM> GetInfoFromAcquisitinDetail(int? ID, string SiteUrl);
        IEnumerable<AssetMasterVM> GetAssetSubAsset();

        void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl);
    }
}

using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetTransferService
    {
        void SetSiteUrl(string siteUrl);

        bool Syncronize(string SiteUrl);

        //create empty form
        AssetTransferVM GetPopulatedModel(string SiteUrl);
        AssetTransferVM GetHeader(int? ID = null, string SiteUrl = null);

        IEnumerable<AssetTransferDetailVM> GetDetails(int? headerID);
        IEnumerable<AssetTransferDetailVM> GetDetailsPrint(int? headerID);

        int? CreateHeader(AssetTransferVM viewmodel, string SiteUrl, string mode = null);
        bool UpdateHeader(AssetTransferVM viewmodel, string SiteUrl);
        void CreateDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, string SiteUrl);

        void CreateDetails(int? headerID, IEnumerable<AssetTransferDetailVM> items);
        void CreateDetails(int? headerID, AssetTransferDetailVM item, string SiteUrl);
        void UpdateDetails(int? headerID, IEnumerable<AssetTransferDetailVM> items);

        ProfessionalDataVM GetProfMasterInfo(string fullname, string SiteUrl);

        IEnumerable<AssignmentOfAssetDetailsVM> GetAssetSubAsset();
        IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetProvince();
        Model.ViewModel.Form.Asset.LocationMasterVM GetProvinceInfo(string province, string SiteUrl);
        IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetOfficeName(string SiteUrl, string province = null);
        IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetFloorList(string SiteUrl, string office = null);
        IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetRoomList(string SiteUrl, string floor = null);

        int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null);
        void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl);

        bool isExist(string listname, string caml, string SiteUrl);
    }
}

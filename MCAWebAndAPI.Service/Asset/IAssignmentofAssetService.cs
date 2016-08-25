using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssignmentOfAssetService
    {
        void SetSiteUrl(string siteUrl);

        //create empty form
        AssignmentOfAssetVM GetPopulatedModel(string SiteUrl);
        AssignmentOfAssetVM GetHeader(int? ID = null, string SiteUrl = null);

        IEnumerable<AssignmentOfAssetDetailsVM> GetDetails(int? headerID);

        int? CreateHeader(AssignmentOfAssetVM viewmodel, string SiteUrl, string mode = null);
        bool UpdateHeader(AssignmentOfAssetVM viewmodel, string SiteUrl);
        void CreateDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, string SiteUrl);

        void CreateDetails(int? headerID, IEnumerable<AssignmentOfAssetDetailsVM> items);
        void CreateDetails(int? headerID, AssignmentOfAssetDetailsVM item, string SiteUrl);
        void UpdateDetails(int? headerID, IEnumerable<AssignmentOfAssetDetailsVM> items);

        ProfessionalDataVM GetProfMasterInfo(string fullname,  string SiteUrl);

        IEnumerable<AssetAcquisitionItemVM> GetAssetSubAsset();
        IEnumerable<LocationMasterVM> GetProvince();
        LocationMasterVM GetProvinceInfo(string province, string SiteUrl);
        IEnumerable<LocationMasterVM> GetOfficeName(string SiteUrl, string province = null);
        IEnumerable<LocationMasterVM> GetFloorList(string SiteUrl, string office = null);
        IEnumerable<LocationMasterVM> GetRoomList(string SiteUrl, string floor = null);

        int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null);
        void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl);

        bool isExist(string listname, string caml, string SiteUrl);
    }
}

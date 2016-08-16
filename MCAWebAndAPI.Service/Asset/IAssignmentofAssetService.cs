using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System.Data;



namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssignmentofAssetService
    {

        void SetSiteUrl(string siteUrl);


        AssignmentofAssetVM GetPopulatedModel(int? ID = null);
        AssignmentofAssetDetailVM GetPopulatedModelItem(int? ID = null);

        int? CreateHeader(AssignmentofAssetVM viewmodel);
        bool UpdateHeader(AssignmentofAssetVM viewmodel);

        void CreateDetails(int? headerID, IEnumerable<AssignmentofAssetDetailVM> items);
        void UpdateDetails(int? headerID, IEnumerable<AssignmentofAssetDetailVM> items);

        AssignmentofAssetVM GetHeader(int? ID);

        IEnumerable<AssignmentofAssetDetailVM> GetDetails(int? headerID);

        IEnumerable<AssetMasterVM> GetAssetSubAsset();

        IEnumerable<LocationMasterVM> GetLocationMaster();


        IEnumerable<LocationMasterVM> GetOffice();

        IEnumerable<LocationMasterVM> GetFloor();

        IEnumerable<LocationMasterVM> GetRoom();

        IEnumerable<LocationMasterVM> GetRemark();

        int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null);

        int? getIdOfColumn(string listname, string SiteUrl, string caml);

        Dictionary<int, string> getListIDOfList(string listName, string key, string value, string SiteUrl);

        void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl);
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssignmentofAssetService : IAssignmentofAssetService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSOASS_LIST_NAME = "Assignment of Asset";
        const string SP_ASSOASSDetails_LIST_NAME = "Assignment of Asset Details";

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
        {
            var models = new List<AssetMasterVM>();

            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            return models;
        }

        private AssetMasterVM ConvertToModelAssetSubAsset(ListItem item)
        {
            var viewModel = new AssetMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.AssetNoAssetDesc.Value = Convert.ToString(item["AssetID"]);
            viewModel.AssetDesc = Convert.ToString(item["Title"]);
            return viewModel;
        }


        public IEnumerable<LocationMasterVM> GetLocationMaster()
        {
            var models = new List<LocationMasterVM>();

            foreach (var item in SPConnector.GetList("Location Master", _siteUrl))
            {
                models.Add(ConvertToLocationMasterModel_Light(item));
            }

            return models;
        }

        private LocationMasterVM ConvertToLocationMasterModel_Light(ListItem item)
        {

            var viewModel = new LocationMasterVM();

            viewModel.Province.Text = Convert.ToString(item["Province"]);
            viewModel.OfficeName = Convert.ToString(item["Title"]);
            viewModel.FloorName = Convert.ToInt32(item["Floor"]);
            viewModel.RoomName = Convert.ToString(item["Room"]);
            viewModel.Remarks = Convert.ToString(item["Remarks"]);
            return viewModel;
        }


        public IEnumerable<LocationMasterVM> GetOffice()
        {
            var models = new List<LocationMasterVM>();

            foreach (var item in SPConnector.GetList("Location Master", _siteUrl))
            {
                models.Add(ConvertToModelOffice(item));
            }

            return models;
        }

        private LocationMasterVM ConvertToModelOffice(ListItem item)
        {
            var viewModel = new LocationMasterVM();

            viewModel.OfficeName = Convert.ToString(item["Title"]);
            return viewModel;
        }

        public IEnumerable<LocationMasterVM> GetFloor()
        {
            var models = new List<LocationMasterVM>();

            foreach (var item in SPConnector.GetList("Location Master2", _siteUrl))
            {
                models.Add(ConvertToModelFloor(item));
            }

            return models;
        }

        private LocationMasterVM ConvertToModelFloor(ListItem item)
        {
            var viewModel = new LocationMasterVM();

            viewModel.FloorName = Convert.ToInt32(item["FloorName"]);
            return viewModel;
        }

        public IEnumerable<LocationMasterVM> GetRoom()
        {
            var models = new List<LocationMasterVM>();

            foreach (var item in SPConnector.GetList("Location Master2", _siteUrl))
            {
                models.Add(ConvertToModelRoom(item));
            }

            return models;
        }

        private LocationMasterVM ConvertToModelRoom(ListItem item)
        {
            var viewModel = new LocationMasterVM();

            viewModel.RoomName = Convert.ToString(item["RoomName"]);
            return viewModel;
        }

        public IEnumerable<LocationMasterVM> GetRemark()
        {
            var models = new List<LocationMasterVM>();

            foreach (var item in SPConnector.GetList("Location Master2", _siteUrl))
            {
                models.Add(ConvertToModelRemark(item));
            }

            return models;
        }

        private LocationMasterVM ConvertToModelRemark(ListItem item)
        {
            var viewModel = new LocationMasterVM();

            viewModel.Remarks = Convert.ToString(item["Remarks"]);
            return viewModel;
        }

        public void CreateDetails(int? headerID, IEnumerable<AssignmentofAssetDetailVM> items)
        {
            throw new NotImplementedException();
        }

        public int? CreateHeader(AssignmentofAssetVM viewmodel)
        {
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("Title", viewmodel.TransactionType);
            string[] memo = viewmodel.AssetHolder.Value.Split('-');
            //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);
            columnValues.Add("Date", viewmodel.Date);
            columnValues.Add("AssetHolder", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
            columnValues.Add("Project", viewmodel.Project);
            columnValues.Add("ContactNo", viewmodel.ContactNo);
            columnValues.Add("Attachments", viewmodel.Attachment);
            string status = viewmodel.CompletionStatus.Value;
            columnValues.Add("CompletionStatus", new FieldLookupValue { LookupId = Convert.ToInt32(status) });

            try
            {
                SPConnector.AddListItem(SP_ASSOASS_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssignmentofAssetVM();
            entitiy = viewmodel;
            return SPConnector.GetLatestListItemID(SP_ASSOASS_LIST_NAME, _siteUrl);
        }



        public IEnumerable<AssignmentofAssetDetailVM> GetDetails(int? headerID)
        {
            throw new NotImplementedException();
        }

        public AssignmentofAssetVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_ASSOASS_LIST_NAME, ID, _siteUrl);
            var viewModel = new AssignmentofAssetVM();

            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
            viewModel.Date = Convert.ToDateTime(listItem["Date"]);
            if ((listItem["Asset Holder"] as FieldLookupValue) != null)
            {
                viewModel.AssetHolder.Value = (listItem["AssetHolder"] as FieldLookupValue).LookupId.ToString();
                viewModel.AssetHolder.Text = (listItem["AssetHolder"] as FieldLookupValue).LookupId.ToString() + "-" + (listItem["AssetHolder"] as FieldLookupValue).LookupValue;
            }
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["Acceptance_x0020_Memo_x0020_No"]);
            viewModel.Project = Convert.ToString(listItem["Project"]);
            viewModel.ContactNo = Convert.ToString(listItem["ContactNo"]);
            //viewModel.Attachment = Convert.ToString(listItem["Attachments"]);
            if ((listItem["CompletionStatus"] as FieldLookupValue) != null)
            {
                viewModel.CompletionStatus.Value = (listItem["CompletionStatus"] as FieldLookupValue).LookupId.ToString();
                viewModel.CompletionStatus.Text = (listItem["CompletionStatus"] as FieldLookupValue).LookupId.ToString() + "-" + (listItem["CompletionStatus"] as FieldLookupValue).LookupValue;
            }
            viewModel.ID = ID;

            return viewModel;
        }

        public int? getIdOfColumn(string listname, string SiteUrl, string caml)
        {
            var getItem = SPConnector.GetList(listname, SiteUrl, caml);
            if (getItem.Count != 0 || getItem != null)
            {
                foreach (var item in getItem)
                {
                    return Convert.ToInt32(item["ID"]);
                }
            }
            else
            {
                return 0;
            }
            return 0;
        }

        public Dictionary<int, string> getListIDOfList(string listName, string key, string value, string SiteUrl)
        {
            var caml = @"<View><Query />
                        <ViewFields>
                           <FieldRef Name='" + key + @"' />
                           <FieldRef Name='" + value + @"' />
                        </ViewFields>
                        <QueryOptions /></View>";

            var list = SPConnector.GetList(listName, SiteUrl, caml);
            Dictionary<int, string> ids = new Dictionary<int, string>();
            if (list.Count > 0)
            {
                foreach (var l in list)
                {
                    ids.Add(Convert.ToInt32(l[key]), Convert.ToString(l[value]));
                }
            }

            return ids;
        }

        public AssignmentofAssetVM GetPopulatedModel(int? ID = default(int?))
        {
            var model = new AssignmentofAssetVM();
            model.TransactionType = "Assignment of Asset";
            model.Project = "GP";
            model.ContactNo = "081xxxxxxxx";

            return model;
        }

        public AssignmentofAssetDetailVM GetPopulatedModelItem(int? ID = default(int?))
        {
            throw new NotImplementedException();
        }


        public int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            throw new NotImplementedException();
        }

        public void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl)
        {
            SPConnector.DeleteListItem(listNameHeader, latestIDHeader, siteUrl);
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssignmentofAssetDetailVM> items)
        {
            throw new NotImplementedException();
        }

        public bool UpdateHeader(AssignmentofAssetVM viewmodel)
        {
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            //columnValues.add
            columnValues.Add("Title", viewmodel.TransactionType);
            string[] memo = viewmodel.AssetHolder.Value.Split('-');
            //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);
            columnValues.Add("Date", viewmodel.Date);
            columnValues.Add("AssetHolder", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0]) });
            columnValues.Add("Project", viewmodel.Project);
            columnValues.Add("ContactNo", viewmodel.ContactNo);
            columnValues.Add("Attachments", viewmodel.Attachment);
            columnValues.Add("CompletionStatus", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.CompletionStatus.Value) });

            try
            {
                SPConnector.UpdateListItem(SP_ASSOASS_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssignmentofAssetVM();
            entitiy = viewmodel;
            return true;
        }
    }
}

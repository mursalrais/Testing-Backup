using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssignmentofAssetService : IAssignmentofAssetService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_MON_FEE_LIST_NAME = "Monthly Fee";
        const string SP_MON_FEE_DETAIL_LIST_NAME = "Monthly Fee Detail";
        const string SP_PSA_LIST_NAME = "PSA";

       

        public AssignmentofAssetVM GetAssignmentofAsset()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssignmentofAsset(AssignmentofAssetDetailVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateAssignmentofAsset(AssignmentofAssetDetailVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        

        public bool CreateAssignmentofAsset_Dummy(AssignmentofAssetDetailVM assignmentofAsset)
        {
            var entity = new AssignmentofAssetDetailVM();
            entity = assignmentofAsset;
            return true;
        }

        public bool UpdateAssignmentofAsset_Dummy(AssignmentofAssetDetailVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssignmentofAsset_Dummy(AssignmentofAssetDetailVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public AssignmentofAssetVM GetAssignmentofAssetItems_Dummy()
        {
            var viewModel = new AssignmentofAssetVM();

            var list = new List<AssignmentofAssetDetailVM>();
            //list.Add(new AssignmentofAssetDetailVM()
            //{
            //    NewAsset = "New",
            //    Item = "Chair",
            //    AssetDescription = "New Asset",
            //    Id = 1
            //});
            //viewModel.TransactionType = "Cash";
            //viewModel.AssignmentofAssets = list;

            return viewModel;
        }

        public AssignmentofAssetVM GetPopulatedModel(int? id = null)
        {
            var model = new AssignmentofAssetVM();
            return model;
        }

        public AssignmentofAssetVM GetHeader(int? ID)
        {
            throw new NotImplementedException();
        }

        public int CreateHeader(AssignmentofAssetVM header)
        {
            //var columnValues = new Dictionary<string, object>();
            //columnValues.Add("professional", new aFieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            //columnValues.Add("ProjectOrUnit", header.ProjectUnit);
            //columnValues.Add("position", header.Position);
            //columnValues.Add("maritalstatus", header.Status);
            //columnValues.Add("joindate", header.JoinDate);
            //columnValues.Add("dateofnewpsa", header.DateOfNewPsa);
            //columnValues.Add("psaexpirydate", header.EndOfContract);
            //try
            //{
            //    SPConnector.AddListItem(SP_MON_FEE_LIST_NAME, columnValues, _siteUrl);
            //}
            //catch (Exception e)
            //{
            //    logger.Error(e.Message);
            //}

            //return SPConnector.GetLatestListItemID(SP_MON_FEE_LIST_NAME, _siteUrl);
            throw new NotImplementedException();
        }

        public bool UpdateHeader(AssignmentofAssetVM header)
        {
            throw new NotImplementedException();
        }

        public void CreateMonthlyFeeDetails(int? headerID, IEnumerable<AssignmentofAssetVM> monthlyFeeDetails)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssignmentofAssetVM> IAssignmentofAssetService.GetAssignmentofAsset()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool CreateAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }
    }
}

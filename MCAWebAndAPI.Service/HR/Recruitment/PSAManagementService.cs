using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using System.Web;
using MCAWebAndAPI.Service.Resources;


namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class PSAManagementService : IPSAManagementService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PSA_DOC_LIST_NAME = "PSA Documents";

        public int CreatePSAManagement(PSAManagementVM psaManagement)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("Title", psaManagement.psaNumber);
            updatedValues.Add("isrenewal", psaManagement.isrenewal.Value);
            updatedValues.Add("renewalnumber", psaManagement.renewalnumber);
            updatedValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit.Value);
            updatedValues.Add("position", new FieldLookupValue { LookupId =  psaManagement.position.Value});
            updatedValues.Add("professional", new FieldLookupValue { LookupId = psaManagement.professional.Value });
            updatedValues.Add("joindate", psaManagement.joinDate);
            updatedValues.Add("dateofnewpsa", psaManagement.dateofNewPSA);
            updatedValues.Add("tenure", psaManagement.tenure);
            //updatedValues.Add("psaexpirydate", psaManagement.pSAExpiryDate);

            try
            {
                SPConnector.AddListItem(SP_PSA_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetInsertedItemID(SP_PSA_LIST_NAME, _siteUrl);
        }

        public IEnumerable<PSAMaster> GetPSAs()
        {
            var models = new List<PSAMaster>();

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPSAModel(item));
            }

            return models;
        }
        
        private PSAMaster ConvertToPSAModel(ListItem item)
        {
            return new PSAMaster
            {
                ID = item["professional_x003a_ID"] == null ? "" :
               Convert.ToString((item["professional_x003a_ID"] as FieldLookupValue).LookupValue),
                JoinDate = Convert.ToString(item["joindate"]),
                DateOfNewPSA = Convert.ToString(item["dateofnewpsa"]),
                PsaExpiryDate = Convert.ToString(item["psaexpirydate"]),
                ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]),

            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public PSAManagementVM GetPSAManagement(int? ID)
        {
            var viewModel = new PSAManagementVM();
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_PSA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToPSAManagementVM(listItem);

            return viewModel;

        }

        private PSAManagementVM ConvertToPSAManagementVM(ListItem listItem)
        {
            var viewModel = new PSAManagementVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.psaNumber = Convert.ToString(listItem["Title"]);
            viewModel.isrenewal.DefaultValue = Convert.ToString(listItem["isrenewal"]);
            viewModel.renewalnumber = Convert.ToInt32(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.DefaultValue = Convert.ToString(listItem["ProjectOrUnit"]);
            //viewModel.position.DefaultValue = Convert.ToString(listItem["position"]);
            viewModel.position.DefaultValue = FormatUtil.ConvertLookupToID(listItem, "position") + string.Empty;
            //viewModel.professional.DefaultValue = Convert.ToString(listItem["professional"]);
            viewModel.professional.DefaultValue = FormatUtil.ConvertLookupToID(listItem, "professional") + string.Empty;
            viewModel.joinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.dateofNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.tenure = Convert.ToInt32(listItem["tenure"]);

            viewModel.pSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();

            //viewModel.Documents = GetDocuments(viewModel.ID);
            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.PSAManagementDocumentByID, _siteUrl);
        }
        
        public bool UpdatePSAManagement(PSAManagementVM psaManagement)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = psaManagement.ID.Value;

            columnValues.Add("Title", psaManagement.psaNumber);
            columnValues.Add("isrenewal", psaManagement.isrenewal);
            columnValues.Add("renewalnumber", psaManagement.renewalnumber);
            columnValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit);
            columnValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.position.Value) });
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.professional.Value) });
            columnValues.Add("joindate", psaManagement.joinDate.Value);
            columnValues.Add("dateofnewpsa", psaManagement.dateofNewPSA.Value);
            columnValues.Add("tenure", psaManagement.tenure);
            //columnValues.Add("psaexpirydate", psaManagement.pSAExpiryDate.Value);

            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            var entitiy = new PSAManagementVM();
            entitiy = psaManagement;
            return true;
        }

        private IEnumerable<HttpPostedFileBase> GetDocuments(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='psa' LookupId='True' /><Value Type='Lookup'>" + iD
               + @"</Value></Eq></Where> 
            </Query>
            <ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /><FieldRef Name='FileRef' /></ViewFields></View>";

            throw new NotImplementedException();
        }

        public void CreatePSAManagementDocuments(int? psaID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                try
                {
                    SPConnector.UploadDocument(SP_PSA_DOC_LIST_NAME, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }
    }
}

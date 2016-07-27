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
using System.Globalization;


namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class PSAManagementService : IPSAManagementService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PSA_DOC_LIST_NAME = "PSA Documents";

        

        public IEnumerable<PSAManagementVM> GetRenewalNumber(int? professionalID)
        {
            var renewalNumber = new List<PSAManagementVM>();

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl))
            {
                renewalNumber.Add(ConvertToRenewalPSANumber(item));
            }

            return renewalNumber;
        }

        private PSAManagementVM ConvertToRenewalPSANumber(ListItem item)
        {
            return new PSAManagementVM
            {
                ID = item["professional_x003a_ID"] == null ? 0 : Convert.ToInt32((item["professional_x003a_ID"] as FieldLookupValue).LookupId),
                Created = Convert.ToDateTime(item["Created"]),
                PSARenewalNumber = Convert.ToInt32(item["renewalnumber"]),
                ExpiryDateBefore = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime().ToShortDateString(),
                ExpireDateBefore = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime(),
                PSAId = Convert.ToInt32(item["ID"]),
                DateOfNewPSABefore = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime(),
                DateNewPSABefore = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime().ToShortDateString(),
                //ProfessionalMail = Convert.ToString(item["Professional_x0020_Name_x003a_Of"])
                ProfessionalMail = item["Professional_x0020_Name_x003a_Of"] == null? "" : Convert.ToString((item["Professional_x0020_Name_x003a_Of"] as FieldLookupValue).LookupValue)

            };
        }


        public IEnumerable<PSAManagementVM> GetJoinDate(int? professionalID)
        {
            var joindate = new List<PSAManagementVM>();

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl))
            {
                joindate.Add(ConvertToJoinDate(item));
            }

            return joindate;
        }

        private PSAManagementVM ConvertToJoinDate(ListItem item)
        {
            return new PSAManagementVM
            {
                ID = item["professional_x003a_ID"] == null ? 0 : Convert.ToInt32((item["professional_x003a_ID"] as FieldLookupValue).LookupId),
                StrJoinDate = Convert.ToDateTime(item["joindate"]).ToLocalTime().ToShortDateString(),
                PSANumber = Convert.ToString(item["Title"])
            };
        }

        public int CreatePSAManagement(PSAManagementVM psaManagement)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("isrenewal", psaManagement.IsRenewal.Value);
            updatedValues.Add("renewalnumber", psaManagement.PSARenewalNumber);
            updatedValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit.Value);
            updatedValues.Add("position", new FieldLookupValue { LookupId =  (int)psaManagement.Position.Value});
            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)psaManagement.Professional.Value });
            updatedValues.Add("joindate", psaManagement.JoinDate);
            updatedValues.Add("dateofnewpsa", psaManagement.DateOfNewPSA);
            updatedValues.Add("tenure", psaManagement.Tenure);
            updatedValues.Add("initiateperformanceplan", psaManagement.PerformancePlan.Value);
            updatedValues.Add("psastatus", psaManagement.PSAStatus.Value);
            updatedValues.Add("hiddenexpirydate", psaManagement.HiddenExpiryDate);
            updatedValues.Add("lastworkingdate", psaManagement.LastWorkingDate);


            try
            {
                SPConnector.AddListItem(SP_PSA_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return SPConnector.GetLatestListItemID(SP_PSA_LIST_NAME, _siteUrl);
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
                ID = Convert.ToInt32(item["ID"]),
                ProfessionalID = item["professional_x003a_ID"] == null ? string.Empty :
               Convert.ToString((item["professional_x003a_ID"] as FieldLookupValue).LookupValue),
                PSAID = Convert.ToString(item["Created"]),
                PSANumber = Convert.ToString(item["Title"]),
                JoinDate = Convert.ToDateTime(item["joindate"]).ToLocalTime().ToShortDateString(),
                DateOfNewPSA = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime().ToShortDateString(),
                PsaExpiryDate = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime().ToShortDateString(),
                ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]),
                Position = item["position"] == null ? "" :
               Convert.ToString((item["position"] as FieldLookupValue).LookupValue)
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
            {
                return viewModel;
            }                

            var listItem = SPConnector.GetListItem(SP_PSA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToPSAManagementVM(listItem);

            return viewModel;
        }

        private PSAManagementVM ConvertToPSAManagementVM(ListItem listItem)
        {
            var viewModel = new PSAManagementVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.IsRenewal.Text = Convert.ToString(listItem["isrenewal"]);
            viewModel.RenewalNumber = Convert.ToInt32(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Position.Value = FormatUtil.ConvertLookupToID(listItem, "position");
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.Tenure = Convert.ToInt32(listItem["tenure"]);
            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();
            viewModel.LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]).ToLocalTime();
            viewModel.PSAStatus.Text = Convert.ToString(listItem["psastatus"]);

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        public PSAManagementVM ViewPSAManagementData(int? ID)
        {
            var viewModel = new PSAManagementVM();
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_PSA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToViewPSAManagementVM(listItem);

            return viewModel;

        }

        private PSAManagementVM ConvertToViewPSAManagementVM(ListItem listItem)
        {
            var viewModel = new PSAManagementVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.PSANumber = Convert.ToString(listItem["Title"]);
            viewModel.IsRenewal.Value = Convert.ToString(listItem["isrenewal"]);
            viewModel.RenewalNumber = Convert.ToInt32(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.Position.Text = FormatUtil.ConvertLookupToValue(listItem, "position");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.TenureString = Convert.ToString(listItem["tenure"]);
            viewModel.PerformancePlan.Value = Convert.ToString(listItem["initiateperformanceplan"]);

            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.PSAManagementDocumentByID, _siteUrl, iD);
        }

        public bool UpdateStatusPSA(PSAManagementVM psaManagement)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = psaManagement.PSAId ;
            string psaStatus = psaManagement.PSAStatus.Value;
            DateTime hiddenexpirydate = psaManagement.HiddenExpiryDate.Value;

            columnValues.Add("psastatus", psaStatus);
            columnValues.Add("hiddenexpirydate", hiddenexpirydate);

            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }


        public bool UpdatePSAManagement(PSAManagementVM psaManagement)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = psaManagement.ID.Value;

            columnValues.Add("isrenewal", psaManagement.IsRenewal.Value);
            columnValues.Add("renewalnumber", psaManagement.RenewalNumber);
            columnValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit.Value);
            columnValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Position.Value) });
            columnValues.Add("joindate", psaManagement.JoinDate.Value);
            columnValues.Add("dateofnewpsa", psaManagement.DateOfNewPSA.Value);
            columnValues.Add("tenure", psaManagement.Tenure);
            columnValues.Add("initiateperformanceplan", psaManagement.PerformancePlan.Value);

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

        public void CreatePSAManagementDocuments(int? psaID, IEnumerable<HttpPostedFileBase> documents, PSAManagementVM psaManagmement)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();

                if (doc.FileName.IndexOf("MCC") >= 0)
                {
                    psaManagmement.DocumentType = "MCC No Objection Letter";
                }
                else
                {
                    psaManagmement.DocumentType = "PSA Document";
                }
                
                updateValue.Add("psa", new FieldLookupValue { LookupId = Convert.ToInt32(psaID) });
                updateValue.Add("documenttype", psaManagmement.DocumentType);

                try
                {
                    SPConnector.UploadDocument(SP_PSA_DOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void SendMailPerformancePlan(string professionalMail, string mailContent)
        {
            EmailUtil.Send(professionalMail, "Create Performance Plan", mailContent);
        }
    }
}

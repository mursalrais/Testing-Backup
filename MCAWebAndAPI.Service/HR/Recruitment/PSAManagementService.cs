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
        const string SP_PROF_LIST_NAME = "Professional Master";
        const string SP_POSITION_LIST_NAME = "Position Master";


        public IEnumerable<PSAManagementVM> GetRenewalNumber(int? professionalID)
        {
            var camlProfessional = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + professionalID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var professionalData = SPConnector.GetListItem(SP_PROF_LIST_NAME, professionalID, _siteUrl);
            string professionalFullName = Convert.ToString(professionalData["Title"]) + " " + Convert.ToString(professionalData["lastname"]);

            var camlPSA = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professionalfullname' /><Value Type='Text'>" + professionalFullName + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var renewalNumber = new List<PSAManagementVM>();

            foreach (var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, camlPSA))
            {
                renewalNumber.Add(ConvertToRenewalPSANumber(psaData));
            }
            return renewalNumber;
        }

        private PSAManagementVM ConvertToRenewalPSANumber(ListItem item)
        {
            var viewModel = new PSAManagementVM();

            viewModel.ID = FormatUtil.ConvertLookupToID(item, "professional_x003a_ID");
            viewModel.Created = Convert.ToDateTime(item["Created"]);
            viewModel.PSARenewalNumber = Convert.ToInt32(item["renewalnumber"]);
            viewModel.ExpiryDateBefore = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime().ToShortDateString();
            viewModel.ExpireDateBefore = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime();
            viewModel.PSAId = Convert.ToInt32(item["ID"]);
            viewModel.DateOfNewPSABefore = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime();
            viewModel.DateNewPSABefore = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime().ToShortDateString();
            viewModel.JoinDate = Convert.ToDateTime(item["joindate"]).ToLocalTime();
            viewModel.StrJoinDate = Convert.ToDateTime(item["joindate"]).ToLocalTime().ToShortDateString();

            var professionalData = SPConnector.GetListItem(SP_PROF_LIST_NAME, viewModel.ID, _siteUrl);
            viewModel.ProfessionalMail = Convert.ToString(professionalData["officeemail"]);
            viewModel.ProfessionalFullName = Convert.ToString(professionalData["Title"]) + " " + Convert.ToString(professionalData["lastname"]);

            viewModel.StrPSARenewal = Convert.ToString(item["renewalnumber"]);
            viewModel.ProjectUnit = Convert.ToString(item["ProjectOrUnit"]);
            viewModel.PositionID = Convert.ToInt32((item["position"] as FieldLookupValue).LookupId);
            
            return viewModel;
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
            updatedValues.Add("renewalnumber", psaManagement.StrPSARenewal);
            updatedValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit.Value);
            updatedValues.Add("position", new FieldLookupValue { LookupId = (int)psaManagement.PositionBasedProject.Value });
            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)psaManagement.Professional.Value });
            updatedValues.Add("joindate", psaManagement.JoinDate);
            updatedValues.Add("dateofnewpsa", psaManagement.DateOfNewPSA);
            updatedValues.Add("tenure", psaManagement.Tenure);
            updatedValues.Add("initiateperformanceplan", psaManagement.PerformancePlan.Value);
            updatedValues.Add("psastatus", psaManagement.PSAStatus.Value);
            updatedValues.Add("psaexpirydates", psaManagement.PSAExpiryDates);

            if (Convert.ToString(psaManagement.ProfessionalFullName) == null)
            {
                var professionalData = SPConnector.GetListItem(SP_PROF_LIST_NAME, psaManagement.Professional.Value, _siteUrl);
                psaManagement.ProfessionalFullName = Convert.ToString(professionalData["Title"]) + " " + Convert.ToString(professionalData["lastname"]);
            }

            updatedValues.Add("professionalfullname", psaManagement.ProfessionalFullName);
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

        /// <summary>
        /// To return all PSAs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSAMaster> GetPSAs()
        {
            var models = new List<PSAMaster>();

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPSAModel(item));
            }

            return models;
        }

        /// <summary>
        /// To return all PSAs having last working date more or equal than given period
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public IEnumerable<PSAMaster> GetPSAs(DateTime period)
        {
            var startTimeUniversalString = period.ToUniversalTime().ToString("o");
            var caml = @"<View><Query><Where><Geq><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>"
              + startTimeUniversalString
              + @"</Value></Geq></Where><OrderBy><FieldRef Name='renewalnumber' Ascending='False' /></OrderBy></Query></View>";

            var models = new List<PSAMaster>();

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, caml))
            {
                models.Add(ConvertToPSAModel(item));
            }

            return models;
        }
        
        private PSAMaster ConvertToPSAModel(ListItem item)
        {
            var psaMaster = new PSAMaster();
            psaMaster.ID = Convert.ToInt32(item["ID"]);
            psaMaster.ProfessionalID = FormatUtil.ConvertLookupToID(item, "professional") + string.Empty;
            psaMaster.PSANumber = Convert.ToString(item["Title"]);
            psaMaster.ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]);
            psaMaster.Position = FormatUtil.ConvertLookupToValue(item, "position");


            psaMaster.JoinDate = Convert.ToDateTime(item["joindate"]).ToLocalTime();
            psaMaster.DateOfNewPSA = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime();
            psaMaster.PSAExpiryDate = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime();
            psaMaster.LastWorkingDate = Convert.ToDateTime(item["lastworkingdate"]).ToLocalTime();

            psaMaster.JoinDateString = psaMaster.JoinDate.ToShortDateString();
            psaMaster.DateOfNewPSAString = psaMaster.DateOfNewPSA.ToShortDateString();
            psaMaster.PsaExpiryDateString = psaMaster.PSAExpiryDate.ToShortDateString();

            return psaMaster;

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
            viewModel.StrPSARenewal = Convert.ToString(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.PositionBasedProject.Value = FormatUtil.ConvertLookupToID(listItem, "position");
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.Professional.Value = FormatUtil.ConvertLookupToID(listItem, "professional");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.Tenure = Convert.ToInt32(listItem["tenure"]);
            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();
            viewModel.PSAExpiryDates = Convert.ToDateTime(listItem["psaexpirydates"]).ToLocalTime();
            viewModel.LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]).ToLocalTime();
            viewModel.PSAStatus.Text = Convert.ToString(listItem["psastatus"]);
            viewModel.ProfessionalFullName = Convert.ToString(listItem["professionalfullname"]);

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
            viewModel.StrPSARenewal = Convert.ToString(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.ProfessionalFullName = Convert.ToString(listItem["professionalfullname"]);
            viewModel.PositionBasedProject.Text = FormatUtil.ConvertLookupToValue(listItem, "position");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]);
            viewModel.TenureString = Convert.ToString(listItem["tenure"]);
            viewModel.PerformancePlan.Value = Convert.ToString(listItem["initiateperformanceplan"]);
            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();
            viewModel.PSAExpiryDates = Convert.ToDateTime(listItem["psaexpirydates"]).ToLocalTime();
            viewModel.LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]);

            DateTime today = DateTime.UtcNow;
                       
            if((today < viewModel.DateOfNewPSA) || (today > viewModel.LastWorkingDate))
            {
                viewModel.PSAStatus.Text = "Inactive";
            }
            else if((today >= viewModel.DateOfNewPSA) && (today <= viewModel.LastWorkingDate))
            {
                viewModel.PSAStatus.Text = "Active";
            }

            bool updateData = UpdatePSAStatus(viewModel.ID, viewModel.PSAStatus.Text);

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        private bool UpdatePSAStatus(int? ID, string psaStatus)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("psastatus", psaStatus);

            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, ID, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.PSAManagementDocumentByID, _siteUrl, iD);
        }

        public bool UpdateStatusPSA(PSAManagementVM psaManagement)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = psaManagement.PSAId;
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
            columnValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.PositionBasedProject.Value) });
            columnValues.Add("joindate", psaManagement.JoinDate.Value);
            columnValues.Add("dateofnewpsa", psaManagement.DateOfNewPSA.Value);
            columnValues.Add("tenure", psaManagement.Tenure);
            columnValues.Add("initiateperformanceplan", psaManagement.PerformancePlan.Value);
            columnValues.Add("professionalfullname", psaManagement.ProfessionalFullName);
            columnValues.Add("psaexpirydates", psaManagement.PSAExpiryDates.Value);

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

        public bool UpdateProfessionalFromPSA(PSAManagementVM psaManagement, int? psaID)
        {
            var updateValues = new Dictionary<string, object>();

            var psaData = SPConnector.GetListItem(SP_PSA_LIST_NAME, psaID, _siteUrl);
            string psaNumber = Convert.ToString(psaData["Title"]);
            int? professionalID = FormatUtil.ConvertLookupToID(psaData, "professional");
            
            updateValues.Add("Join_x0020_Date", psaManagement.JoinDate.Value);
            updateValues.Add("Project_x002f_Unit", psaManagement.ProjectOrUnit.Value);
            updateValues.Add("Position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.PositionBasedProject.Value) });
            updateValues.Add("PSAnumber", psaNumber);
            updateValues.Add("PSAstartdate", psaManagement.DateOfNewPSA.Value);
            updateValues.Add("PSAexpirydate", psaManagement.PSAExpiryDates.Value);
            updateValues.Add("lastworkingdate", psaManagement.PSAExpiryDates.Value);
                        
            try
            {
                SPConnector.UpdateListItem(SP_PROF_LIST_NAME, professionalID, updateValues, _siteUrl);
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

        public void SendMailPerformancePlan(int? professionalID, string siteUrl, DateTime today)
        {
            var currentYear = today.Year;
            
            var professionalData = SPConnector.GetListItem(SP_PROF_LIST_NAME, professionalID, _siteUrl);
            string professionalMail = Convert.ToString(professionalData["officeemail"]);
            string professionalFullName = Convert.ToString(professionalData["Title"]) + " " + Convert.ToString(professionalData["lastname"]);

            string mailSubject = string.Format("Initiation Performance Plan for Period {0}", currentYear);
            string mailContent = string.Format("Dear Mr./Mrs. {0}, {1}{2}This email is sent to you to notify that you are required to create Performance Plan for period {3}. Creating and approval plan process will take maximum 5 working days. Therefore, do prepare your plan accordingly. {4}{5}To Create the performance plan, please click the following link: {6}{7}/NewForm_Custom.aspx {8}{9}Thank you for your attention.", professionalFullName, Environment.NewLine, Environment.NewLine, currentYear, Environment.NewLine, Environment.NewLine, siteUrl, UrlResource.ProfessionalPerformancePlan, Environment.NewLine, Environment.NewLine);

            EmailUtil.Send(professionalMail, mailSubject, mailContent);
        }

        public string GetProfessionalFullName(int? professionalID)
        {
            var item = SPConnector.GetListItem(SP_PROF_LIST_NAME, professionalID, _siteUrl);

            string professionalFullName = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]);

            return professionalFullName;
        }

        public int GetPSALatestID(string _siteUrl)
        {
            int psaID = SPConnector.GetLatestListItemID(SP_PSA_LIST_NAME, _siteUrl, null);

            return psaID;
        }

        public bool UpdateStatusPSABefore(int? psaRenewalNumberMinusOne, string professionalName)
        {
            var camlPSABefore = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq><Eq><FieldRef Name='renewalnumber' /><Value Type='Number'>" + psaRenewalNumberMinusOne + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int psaID = SPConnector.GetLatestListItemID(SP_PSA_LIST_NAME, _siteUrl, camlPSABefore);

            var updateValues = new Dictionary<string, object>();
            string psaStatus = "In Active";

            updateValues.Add("psastatus", psaStatus);

            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, psaID, updateValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public string GetProfessionalName(int? professionalID)
        {
            var professionalData = SPConnector.GetListItem(SP_PROF_LIST_NAME, professionalID, _siteUrl);

            string professionalName = Convert.ToString(professionalData["Title"]);

            return professionalName;
        }

        public IEnumerable<PositionMaster> GetPosition(string ProjectUnit)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='projectunit' /><Value Type='Choice'>" + ProjectUnit + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var positions = new List<PositionMaster>();
            foreach (var item in SPConnector.GetList(SP_POSITION_LIST_NAME, _siteUrl, caml))
            {
                positions.Add(ConvertToLocationModel(item));
            }
            return positions;
        }

        PositionMaster ConvertToLocationModel(ListItem item)
        {
            return new PositionMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                PositionName = Convert.ToString(item["Title"]),
                IsKeyPosition = Convert.ToString(item["iskeyposition"])
        };
        }
    }
}

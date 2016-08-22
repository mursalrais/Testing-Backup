using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.Common;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRManpowerRequisitionService : IHRManpowerRequisitionService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_MANPOW_LIST_NAME = "Manpower Requisition";
        const string SP_WORKRE_LIST_NAME = "Manpower Requisition Working Relationship";
        const string SP_MANDOC_LIST_NAME = "Manpower Requisition Documents";
        const string SP_POSITION_MAST = "Position Master";
        const string SP_MANPOW_WORKFLOW = "Manpower Requisition Workflow";
        const string SP_PROFESSIONAL_MAST = "Professional Master";

        public int CreateManpowerRequisition(ManpowerRequisitionVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();
            string emailApprover;
            FieldUserValue visibleTo = SPConnector.GetUser(viewModel.Username, _siteUrl, "hr");
            FieldUserValue Approver;
            if ((viewModel.Status.Value == "Pending Approval"))
            {
                if (viewModel.IsKeyPosition)
                {
                    emailApprover = GetApprover("Executive Director");
                }
                else
                {
                    emailApprover = GetApprover("Deputy ED");
                }
                Approver = SPConnector.GetUser(emailApprover, _siteUrl, "hr");
                updatedValue.Add("currentapprover", Approver);
            }
            else
            {
                viewModel.Status.Value = "Draft";
            }
            updatedValue.Add("expectedjoindate", viewModel.ExpectedJoinDate);
            updatedValue.Add("requestdate", viewModel.DateRequested);
            updatedValue.Add("numberofperson", viewModel.NoOfPerson);
            updatedValue.Add("Tenure", viewModel.Tenure);
            updatedValue.Add("personnelmgmt", viewModel.PersonnelManagement);
            updatedValue.Add("budgetmgmt", viewModel.BudgetManagement);
            updatedValue.Add("isonbehalfof", viewModel.IsOnBehalfOf);
            if (viewModel.IsOnBehalfOf)
            {
                updatedValue.Add("onbehalfof", new FieldLookupValue { LookupId = (int)viewModel.OnBehalfOf.Value });
            }
            updatedValue.Add("istravelrequired", viewModel.IsTravellingRequired);
            updatedValue.Add("iskeyposition", viewModel.IsKeyPosition);
            updatedValue.Add("projectunit", viewModel.DivisionProjectUnit.Value);
            updatedValue.Add("positionrequested", new FieldLookupValue { LookupId = (int)viewModel.Position.Value });
            updatedValue.Add("reportingto", new FieldLookupValue { LookupId = (int)viewModel.ReportingTo.Value });
            updatedValue.Add("joblocation", new FieldLookupValue { LookupId = (int)viewModel.JobLocation.Value });
            if (viewModel.SecondaryReportingTo.Value.Value == 0)
            {
                updatedValue.Add("secondaryreportingto", null);

            }
            else
            {
                updatedValue.Add("secondaryreportingto", new FieldLookupValue { LookupId = (int)viewModel.SecondaryReportingTo.Value });
            }
            updatedValue.Add("Objectives", viewModel.PositionObjectives);
            updatedValue.Add("totalyrsofexperience", viewModel.TotalYrsOfExperience);
            updatedValue.Add("minimumeducation", viewModel.MinimumEducation);
            updatedValue.Add("Industry", viewModel.Industry);
            updatedValue.Add("minimumyrsofrelatedexperience", viewModel.MinimumYrsOfExperienceInRelatedField);
            updatedValue.Add("specifictechnicalskill", viewModel.SpecificTechnicalSkillQualification);
            updatedValue.Add("personalattributes", viewModel.PersonalAttributesCompetencies);
            updatedValue.Add("otherrequirements", viewModel.OtherRequirements);
            updatedValue.Add("remarks", viewModel.Remarks);
            updatedValue.Add("manpowerrequeststatus", viewModel.Status.Value);
            updatedValue.Add("visibleto", visibleTo);

            try
            {
                SPConnector.AddListItem(SP_MANPOW_LIST_NAME, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }

            int IDManpower = SPConnector.GetLatestListItemID(SP_MANPOW_LIST_NAME, _siteUrl);
            updatedValue = new Dictionary<string, object>();

            updatedValue.Add("Title", IDManpower);


            try
            {
                SPConnector.UpdateListItem(SP_MANPOW_LIST_NAME, IDManpower, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }

            return IDManpower;
        }

        public bool UpdateStatus(ManpowerRequisitionVM viewModel)
        {

            var updatedValue = new Dictionary<string, object>();
            int ID = viewModel.ID.Value;
            updatedValue.Add("manpowerrequeststatus", viewModel.Status.Value);


            try
            {
                SPConnector.UpdateListItem(SP_MANPOW_LIST_NAME, ID, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            //var entitiy = new PSAManagementVM();
            //entitiy = psaManagement;
            return true;
        }

        public bool UpdateManpowerRequisition(ManpowerRequisitionVM viewModel)
        {
            // check if status changed to active and checked if mcc aproval document already uploaded
            if ((viewModel.Status.Value == "Active") && (viewModel.IsKeyPosition))
            {
                string caml = @"<View><Query><Where><And><Eq><FieldRef Name='documenttype' /><Value Type='Choice'>MCC Approval Letter</Value></Eq><Eq><FieldRef Name='manpowerrequestid' /><Value Type='Lookup'>" + viewModel.ID + @"</Value></Eq></And></Where></Query><ViewFields /><QueryOptions /></View>";
                var document = SPConnector.GetList(SP_MANDOC_LIST_NAME, _siteUrl, caml);
                if (document.Count == 0)
                {
                    logger.Debug("Document MCC Aprroval must be uploaded first!");
                    throw new Exception("Document MCC Aprroval must be uploaded first!");
                }
            }
            var updatedValue = new Dictionary<string, object>();
            int ID = viewModel.ID.Value;

            updatedValue.Add("expectedjoindate", viewModel.ExpectedJoinDate);
            updatedValue.Add("requestdate", viewModel.DateRequested);
            updatedValue.Add("numberofperson", viewModel.NoOfPerson);
            updatedValue.Add("Tenure", viewModel.Tenure);
            updatedValue.Add("personnelmgmt", viewModel.PersonnelManagement);
            updatedValue.Add("budgetmgmt", viewModel.BudgetManagement);
            updatedValue.Add("isonbehalfof", viewModel.IsOnBehalfOf);
            updatedValue.Add("istravelrequired", viewModel.IsTravellingRequired);
            updatedValue.Add("iskeyposition", viewModel.IsKeyPosition);
            if (viewModel.IsOnBehalfOf)
            {
                updatedValue.Add("onbehalfof", new FieldLookupValue { LookupId = (int)viewModel.OnBehalfOf.Value });
            }
            else
            {
                updatedValue.Add("onbehalfof", "");
            }

            if (!(string.IsNullOrEmpty(viewModel.Status.Value)))
            {
                updatedValue.Add("manpowerrequeststatus", viewModel.Status.Value);
            }
            updatedValue.Add("projectunit", viewModel.DivisionProjectUnit.Value);
            updatedValue.Add("positionrequested", new FieldLookupValue { LookupId = (int)viewModel.Position.Value });
            updatedValue.Add("reportingto", new FieldLookupValue { LookupId = (int)viewModel.ReportingTo.Value });
            updatedValue.Add("joblocation", new FieldLookupValue { LookupId = (int)viewModel.JobLocation.Value });
            if (viewModel.SecondaryReportingTo.Value.Value == 0)
            {
                updatedValue.Add("secondaryreportingto", null);
            }
            else
            {
                updatedValue.Add("secondaryreportingto", new FieldLookupValue { LookupId = (int)viewModel.SecondaryReportingTo.Value });
            }
            updatedValue.Add("Objectives", viewModel.PositionObjectives);
            updatedValue.Add("totalyrsofexperience", viewModel.TotalYrsOfExperience);
            updatedValue.Add("minimumeducation", viewModel.MinimumEducation);
            updatedValue.Add("Industry", viewModel.Industry);
            updatedValue.Add("minimumyrsofrelatedexperience", viewModel.MinimumYrsOfExperienceInRelatedField);
            updatedValue.Add("specifictechnicalskill", viewModel.SpecificTechnicalSkillQualification);
            updatedValue.Add("personalattributes", viewModel.PersonalAttributesCompetencies);
            updatedValue.Add("otherrequirements", viewModel.OtherRequirements);
            updatedValue.Add("remarks", viewModel.Remarks);
            FieldUserValue Approver;
            string emailApprover;
            if ((viewModel.Status.Value == "Pending Approval"))
            {
                if (viewModel.IsKeyPosition)
                {
                    emailApprover = GetApprover("Executive Director");
                }
                else
                {
                    emailApprover = GetApprover("Deputy ED");
                }
                Approver = SPConnector.GetUser(emailApprover, _siteUrl, "hr");
                updatedValue.Add("currentapprover", Approver);
            }

            try
            {
                SPConnector.UpdateListItem(SP_MANPOW_LIST_NAME, ID, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            //var entitiy = new PSAManagementVM();
            //entitiy = psaManagement;
            return true;
        }

        public void CreateManpowerRequisitionDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                var type = doc.FileName.Split('-')[0].Trim();
                if (type == "MCC")
                {
                    updateValue.Add("documenttype", "MCC Approval Letter");
                }
                updateValue.Add("manpowerrequestid", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                try
                {
                    SPConnector.UploadDocument(SP_MANDOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public ManpowerRequisitionVM GetManpowerRequisition(int? ID)
        {
            var viewModel = new ManpowerRequisitionVM();
            var checkBoxItem = SPConnector.GetChoiceFieldValues(SP_MANPOW_LIST_NAME, "Workplan", _siteUrl);
            //viewModel.DivisionProjectUnit.Choices = SPConnector.GetChoiceFieldValues(SP_MANPOW_LIST_NAME, "projectunit", _siteUrl);


            var tempList = new List<CheckBoxItemVM>();
            foreach (var item in checkBoxItem)
            {
                tempList.Add(new CheckBoxItemVM
                {
                    Text = item,
                    Value = false
                });
            }

            viewModel.Workplan = tempList;
            viewModel.ID = ID;
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_MANPOW_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToManpowerRequisitionVM(listItem, viewModel);



            return viewModel;

        }
        public async Task<ManpowerRequisitionVM> GetManpowerRequisitionAsync(int? ID)
        {            
            var viewModel = new ManpowerRequisitionVM();
            var checkBoxItem = SPConnector.GetChoiceFieldValues(SP_MANPOW_LIST_NAME, "Workplan", _siteUrl);
            //viewModel.DivisionProjectUnit.Choices = SPConnector.GetChoiceFieldValues(SP_MANPOW_LIST_NAME, "projectunit", _siteUrl);
            var tempList = new List<CheckBoxItemVM>();
            foreach (var item in checkBoxItem)
            {
                tempList.Add(new CheckBoxItemVM
                {
                    Text = item,
                    Value = false
                });
            }

            viewModel.Workplan = tempList;
            viewModel.ID = ID;
            if (ID == null)
            {
                return viewModel;
            }


            var listItem = SPConnector.GetListItem(SP_MANPOW_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToManpowerRequisitionVM(listItem, viewModel);

            viewModel.WorkingRelationshipDetails = GetWorkingRelationshipDetails(viewModel.ID);

            //viewModel.DocumentUrl =  GetDocumentUrl(viewModel.ID);

            return await GetManpowerRequisitionDetailsAsync(viewModel);
        }

        private ManpowerRequisitionVM GetManpowerRequisitionDetail(ManpowerRequisitionVM viewModel)
        {
            viewModel.WorkingRelationshipDetails = GetWorkingRelationshipDetails(viewModel.ID);
            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        private async Task<ManpowerRequisitionVM> GetManpowerRequisitionDetailsAsync(ManpowerRequisitionVM viewModel)
        {
            Task<IEnumerable<WorkingRelationshipDetailVM>> getWorkingRelationshipTask = GetWorkingRelationshipDetailsAsyn(viewModel.ID);

            Task allTasks = Task.WhenAll(getWorkingRelationshipTask, getWorkingRelationshipTask);
            await allTasks;
            viewModel.WorkingRelationshipDetails = await getWorkingRelationshipTask;
            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        private async Task<IEnumerable<WorkingRelationshipDetailVM>> GetWorkingRelationshipDetailsAsyn(int? iD)
        {
            return GetWorkingRelationshipDetails(iD);
        }

        private ManpowerRequisitionVM ConvertToManpowerRequisitionVM(ListItem listItem, ManpowerRequisitionVM viewModel)
        {
            //var viewModel = new ManpowerRequisitionVM();

            viewModel.ExpectedJoinDate = Convert.ToDateTime(listItem["expectedjoindate"]).ToLocalTime();
            viewModel.DateRequested = Convert.ToDateTime(listItem["requestdate"]).ToLocalTime();

            viewModel.NoOfPerson = Convert.ToInt32(listItem["numberofperson"]);
            viewModel.Tenure = Convert.ToInt32(listItem["Tenure"]);
            viewModel.PersonnelManagement = Convert.ToInt32(listItem["personnelmgmt"]);
            viewModel.BudgetManagement = Convert.ToInt32(listItem["budgetmgmt"]);

            viewModel.IsOnBehalfOf = Convert.ToBoolean(listItem["isonbehalfof"]);
            viewModel.IsTravellingRequired = Convert.ToBoolean(listItem["istravelrequired"]);
            viewModel.IsKeyPosition = Convert.ToBoolean(listItem["iskeyposition"]);

            if ((listItem["onbehalfof"] as FieldLookupValue) != null)
            {
                viewModel.OnBehalfOf.Value = (listItem["onbehalfof"] as FieldLookupValue).LookupId;
                viewModel.OnBehalfOf.Text = GetFullNameandPosition((listItem["onbehalfof"] as FieldLookupValue).LookupId);
            }

            viewModel.Status.Value = Convert.ToString(listItem["manpowerrequeststatus"]);
            //viewModel.workplanItem.Value;


            //viewModel.DivisionProjectUnit.Text = Convert.ToString(listItem["projectunit"]);
            viewModel.DivisionProjectUnit.Value = Convert.ToString(listItem["projectunit"]);

            viewModel.Position.Value = (listItem["positionrequested"] as FieldLookupValue).LookupId;
            viewModel.Position.Text = (listItem["positionrequested"] as FieldLookupValue).LookupValue;

            //viewModel.DivisionProjectUnit.Text = (listItem["projectunit"] as FieldLookupValue).LookupValue;
            viewModel.ReportingTo.Value = (listItem["reportingto"] as FieldLookupValue).LookupId;
            viewModel.ReportingTo.Text = GetFullNameandPosition((listItem["reportingto"] as FieldLookupValue).LookupId);
            viewModel.JobLocation.Value = (listItem["joblocation"] as FieldLookupValue).LookupId;
            viewModel.JobLocation.Text = (listItem["joblocation"] as FieldLookupValue).LookupValue;
            if ((listItem["secondaryreportingto"] as FieldLookupValue) != null)
            {
                viewModel.SecondaryReportingTo.Value = (listItem["secondaryreportingto"] as FieldLookupValue).LookupId;
                viewModel.SecondaryReportingTo.Text = GetFullNameandPosition((listItem["secondaryreportingto"] as FieldLookupValue).LookupId);

            }


            viewModel.PositionObjectives = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["Objectives"]));
            viewModel.TotalYrsOfExperience = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["totalyrsofexperience"]));
            viewModel.MinimumEducation = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["minimumeducation"]));
            viewModel.Industry = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["Industry"]));
            viewModel.MinimumYrsOfExperienceInRelatedField = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["minimumyrsofrelatedexperience"]));
            viewModel.SpecificTechnicalSkillQualification = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["specifictechnicalskill"]));
            viewModel.PersonalAttributesCompetencies = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["personalattributes"]));
            viewModel.OtherRequirements = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["otherrequirements"]));
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["remarks"]));


            return viewModel;

        }

        private string GetFullNameandPosition(int lookupId)
        {
            var listitem = SPConnector.GetListItem(SP_PROFESSIONAL_MAST,lookupId,_siteUrl);
            return Convert.ToString(listitem["Title"]) + " - " + (listitem["Position"] as FieldLookupValue).LookupValue;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.ManpowerDocumentByID, _siteUrl, iD);
        }

        private IEnumerable<HttpPostedFileBase> GetDocuments(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='application' LookupId='True' /><Value Type='Lookup'>" + iD
               + @"</Value></Eq></Where> 
            </Query>
            <ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /><FieldRef Name='FileRef' /></ViewFields></View>";

            throw new NotImplementedException();
        }

        private IEnumerable<WorkingRelationshipDetailVM> GetWorkingRelationshipDetails(int? ID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='manpowerrequisition' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";
            var WorkingRelationshipDetails = new List<WorkingRelationshipDetailVM>();
            foreach (var item in SPConnector.GetList(SP_WORKRE_LIST_NAME, _siteUrl, caml))
            {
                WorkingRelationshipDetails.Add(ConvertToWorkingRelationshipDetailVM(item));
            }

            return WorkingRelationshipDetails;
        }

        private WorkingRelationshipDetailVM ConvertToWorkingRelationshipDetailVM(ListItem item)
        {
            var _frequencyArray = (string[])item["frequency"];
            string _frequency = string.Join(",", _frequencyArray);

            var _relationshipArray = (string[])item["relationship"];
            string _relationship = string.Join(",", _relationshipArray);

            var ListPosition = SPConnector.GetListItem(SP_POSITION_MAST, (item["position"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _positionWorking = new AjaxComboBoxVM();
            _positionWorking.Value = (item["position"] as FieldLookupValue).LookupId;
            _positionWorking.Text = Convert.ToString(ListPosition["projectunit"]) + " - " + Convert.ToString(ListPosition["Title"]);
            return new WorkingRelationshipDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                PositionWorking = WorkingRelationshipDetailVM.GetPositionDefaultValue(_positionWorking),
                Frequency = WorkingRelationshipDetailVM.GetFrequencyDefaultValue(new InGridMultiSelectVM { Text = _frequency }),
                Relationship = WorkingRelationshipDetailVM.GetRelationshipDefaultValue(new InGridMultiSelectVM { Text = _relationship })
            };
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void CreateManpowerRequisitionDetails(int? headerID, IEnumerable<EducationDetailVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_WORKRE_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("university", viewModel.University);
                updatedValue.Add("yearofgraduation", FormatUtil.ConvertToDateString(viewModel.YearOfGraduation));
                updatedValue.Add("remarks", viewModel.Remarks);
                updatedValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_WORKRE_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_WORKRE_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void CreateWorkingRelationshipDetails(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails)
        {
            ////update title become header ID
            //var updatedValue = new Dictionary<string, object>();
            //updatedValue.Add("Title", headerID);

            //SPConnector.UpdateListItem(SP_MANPOW_LIST_NAME, headerID, updatedValue);
            foreach (var viewModel in workingRelationshipDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_WORKRE_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>();
                string[] _frequency = viewModel.Frequency.Text.Split(',');
                string[] _relationship = viewModel.Relationship.Text.Split(',');
                updatedValue.Add("manpowerrequisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.PositionWorking.Value.Value) });
                updatedValue.Add("frequency", _frequency);
                updatedValue.Add("relationship", _relationship);
                try
                {
                    SPConnector.AddListItem(SP_WORKRE_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public ManpowerRequisitionVM GetRequestStatus()
        {
            var viewModel = new ManpowerRequisitionVM();

            viewModel.Status.Choices = new string[]
            {
                "Pending MCC Approval",
                "Rejected by MCC",
                "Approved by MCC",
                "Active",
                "Filled",
                "Cancelled"
            };


            return viewModel;
        }

        public IEnumerable<ManpowerRequisitionVM> GetManpowerRequisitionAll()
        {
            var models = new List<ManpowerRequisitionVM>();
            var itemModel = new ManpowerRequisitionVM();
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl))
            {
                itemModel = new ManpowerRequisitionVM();
                itemModel.ID = Convert.ToInt32(item["ID"]);
                itemModel.Position.Text = (item["positionrequested"] as FieldLookupValue).LookupValue;
                models.Add(itemModel);
            }

            return models;
        }

        public string GetPosition(string username, string siteUrl)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + username + @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", siteUrl, caml);
            string position = "";
            foreach (var item in listItem)
            {
                position = FormatUtil.ConvertLookupToValue(item, "Position");
            }
            if (position == null)
            {
                position = "";
            }
            return position;
        }

        public async Task CreateWorkingRelationshipDetailsAsync(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails)
        {
            CreateWorkingRelationshipDetails(headerID, workingRelationshipDetails);
        }

        public async Task CreateManpowerRequisitionDocumentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateManpowerRequisitionDocuments(headerID, documents);
        }

        public string getEmailOnBehalf(int? ID)
        {

            return "ad";
        }

        public string GetApprover(string Position)
        {
            string email = "anugerahseptian@gmail.com";
            string caml = @"<View><Query><Where><Eq><FieldRef Name='Position' /><Value Type='Lookup'>" + Position + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='officeemail' /></ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList(SP_PROFESSIONAL_MAST, _siteUrl, caml))
            {
                email = Convert.ToString(item["officeemail"]);
            }
            return email;
        }
    }
}

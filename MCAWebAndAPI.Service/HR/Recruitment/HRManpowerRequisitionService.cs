using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRManpowerRequisitionService : IHRManpowerRequisitionService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_MANPOW_LIST_NAME = "Manpower Requisition";
        const string SP_WORKRE_LIST_NAME = "Manpower Requisition Working Relationship";
        const string SP_MANDOC_LIST_NAME = "Manpower Requisition Documents";

        public int CreateManpowerRequisition(ManpowerRequisitionVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();            

            updatedValue.Add("expectedjoindate", viewModel.ExpectedJoinDate);
            updatedValue.Add("requestdate", viewModel.DateRequested);

            updatedValue.Add("numberofperson", viewModel.NoOfPerson);
            updatedValue.Add("Tenure", viewModel.Tenure);
            updatedValue.Add("personnelmgmt", viewModel.PersonnelManagement);
            updatedValue.Add("budgetmgmt", viewModel.BudgetManagement);

            updatedValue.Add("isonbehalfof", viewModel.IsOnBehalfOf);
            updatedValue.Add("istravelrequired", viewModel.IsTravellingRequired);
            updatedValue.Add("iskeyposition", viewModel.IsKeyPosition);

            updatedValue.Add("onbehalfof", new FieldLookupValue { LookupId = (int)viewModel.OnBehalfOf.Value });

            updatedValue.Add("projectunit", viewModel.DivisionProjectUnit.Value);

            updatedValue.Add("positionrequested", new FieldLookupValue { LookupId = (int)viewModel.Position.Value });
            updatedValue.Add("reportingto", new FieldLookupValue { LookupId = (int)viewModel.ReportingTo.Value });
            updatedValue.Add("joblocation", new FieldLookupValue { LookupId = (int)viewModel.JobLocation.Value });
            updatedValue.Add("secondaryreportingto", new FieldLookupValue { LookupId = (int)viewModel.SecondaryReportingTo.Value });

            updatedValue.Add("Objectives", viewModel.PositionObjectives);
            updatedValue.Add("totalyrsofexperience", viewModel.TotalYrsOfExperience);
            updatedValue.Add("minimumeducation", viewModel.MinimumEducation);
            updatedValue.Add("Industry", viewModel.Industry);
            updatedValue.Add("minimumyrsofrelatedexperience", viewModel.MinimumYrsOfExperienceInRelatedField);
            updatedValue.Add("specifictechnicalskill", viewModel.SpecificTechnicalSkillQualification);
            updatedValue.Add("personalattributes", viewModel.PersonalAttributesCompetencies);
            updatedValue.Add("otherrequirements", viewModel.OtherRequirements);
            updatedValue.Add("remarks", viewModel.Remarks);

            try
            {
                SPConnector.AddListItem(SP_MANPOW_LIST_NAME, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetInsertedItemID(SP_MANPOW_LIST_NAME, _siteUrl);
        }

        public bool UpdateManpowerRequisition(ManpowerRequisitionVM viewModel)
        {
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

            updatedValue.Add("onbehalfof", new FieldLookupValue { LookupId = (int)viewModel.OnBehalfOf.Value });

            updatedValue.Add("projectunit", viewModel.DivisionProjectUnit.Value);

            updatedValue.Add("positionrequested", new FieldLookupValue { LookupId = (int)viewModel.Position.Value });
            updatedValue.Add("reportingto", new FieldLookupValue { LookupId = (int)viewModel.ReportingTo.Value });
            updatedValue.Add("joblocation", new FieldLookupValue { LookupId = (int)viewModel.JobLocation.Value });
            updatedValue.Add("secondaryreportingto", new FieldLookupValue { LookupId = (int)viewModel.SecondaryReportingTo.Value });

            updatedValue.Add("Objectives", viewModel.PositionObjectives);
            updatedValue.Add("totalyrsofexperience", viewModel.TotalYrsOfExperience);
            updatedValue.Add("minimumeducation", viewModel.MinimumEducation);
            updatedValue.Add("Industry", viewModel.Industry);
            updatedValue.Add("minimumyrsofrelatedexperience", viewModel.MinimumYrsOfExperienceInRelatedField);
            updatedValue.Add("specifictechnicalskill", viewModel.SpecificTechnicalSkillQualification);
            updatedValue.Add("personalattributes", viewModel.PersonalAttributesCompetencies);
            updatedValue.Add("otherrequirements", viewModel.OtherRequirements);
            updatedValue.Add("remarks", viewModel.Remarks);


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
       
        public void CreateWorkingRelationshipDetails(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails)
        {
            foreach (var viewModel in workingRelationshipDetails)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("manpowerrequisition", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

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

        public ManpowerRequisitionVM GetManpowerRequisition(int? ID)
        {
            var viewModel = new ManpowerRequisitionVM();
            var checkBoxItem = SPConnector.GetChoiceFieldValues(SP_MANPOW_LIST_NAME, "Workplan", _siteUrl);
            viewModel.DivisionProjectUnit.Choices = SPConnector.GetChoiceFieldValues(SP_MANPOW_LIST_NAME, "projectunit", _siteUrl);
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

            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_MANPOW_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToManpowerRequisitionVM(listItem,viewModel);
            viewModel.ID = ID;
            
           
            return viewModel;

        }
               
        private ManpowerRequisitionVM ConvertToManpowerRequisitionVM(ListItem listItem, ManpowerRequisitionVM viewModel)
        {
            //var viewModel = new ManpowerRequisitionVM();
            
            viewModel.ExpectedJoinDate = Convert.ToDateTime(listItem["expectedjoindate"]);
            viewModel.DateRequested = Convert.ToDateTime(listItem["requestdate"]);

            viewModel.NoOfPerson = Convert.ToInt32(listItem["numberofperson"]);
            viewModel.Tenure = Convert.ToInt32(listItem["Tenure"]);
            viewModel.PersonnelManagement = Convert.ToInt32(listItem["personnelmgmt"]);
            viewModel.BudgetManagement = Convert.ToInt32(listItem["budgetmgmt"]);

            viewModel.IsOnBehalfOf = Convert.ToBoolean(listItem["isonbehalfof"]);
            viewModel.IsTravellingRequired = Convert.ToBoolean(listItem["istravelrequired"]);
            viewModel.IsKeyPosition = Convert.ToBoolean(listItem["iskeyposition"]);

            viewModel.OnBehalfOf.Value = FormatUtil.ConvertLookupToID(listItem, "onbehalfof");
            viewModel.DivisionProjectUnit.Value = Convert.ToString(listItem["projectunit"]);
            //viewModel.workplanItem.Value;

            viewModel.Position.Value = FormatUtil.ConvertLookupToID(listItem, "positionrequested");
            viewModel.ReportingTo.Value = FormatUtil.ConvertLookupToID(listItem, "reportingto");
            viewModel.JobLocation.Value = FormatUtil.ConvertLookupToID(listItem, "joblocation");
            viewModel.SecondaryReportingTo.Value = FormatUtil.ConvertLookupToID(listItem, "secondaryreportingto");

            

            viewModel.PositionObjectives = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["Objectives"]));
            viewModel.TotalYrsOfExperience = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["totalyrsofexperience"]));
            viewModel.MinimumEducation = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["minimumeducation"]));
            viewModel.Industry = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["Industry"]));
            viewModel.MinimumYrsOfExperienceInRelatedField = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["minimumyrsofrelatedexperience"]));
            viewModel.SpecificTechnicalSkillQualification = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["specifictechnicalskill"]));
            viewModel.PersonalAttributesCompetencies = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["personalattributes"]));
            viewModel.OtherRequirements = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["otherrequirements"]));
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["remarks"]));


            // Convert Details
            viewModel.WorkingRelationshipDetails = GetWorkingRelationshipDetails(viewModel.ID);

            return viewModel;

        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.PSAManagementDocumentByID, _siteUrl, iD);
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
            var caml = @"";

            var WorkingRelationshipDetails = new List<WorkingRelationshipDetailVM>();
            foreach (var item in SPConnector.GetList(SP_WORKRE_LIST_NAME, _siteUrl, caml))
            {
                WorkingRelationshipDetails.Add(ConvertToWorkingRelationshipDetailVM(item));
            }

            return WorkingRelationshipDetails;
        }

        private WorkingRelationshipDetailVM ConvertToWorkingRelationshipDetailVM(ListItem item)
        {
            return new WorkingRelationshipDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Position = WorkingRelationshipDetailVM.GetPositionDefaultValue(FormatUtil.ConvertToInGridLookup(item, "position")),
                Frequency = WorkingRelationshipDetailVM.GetFrequencyDefaultValue(FormatUtil.ConvertToInGridLookup(item, "frequency")),
                Relationship = WorkingRelationshipDetailVM.GetRelationshipDefaultValue(FormatUtil.ConvertToInGridLookup(item, "relationship"))
            };
        }


        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRPerformancePlanService : IHRPerformancePlanService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_PPPIG_LIST_NAME = "PPP Individual Goal";
        const string SP_PPP_LIST_NAME = "Professional Performance Plan";
        const string SP_PP_LIST_NAME = "Performance Plan";
        const string SP_PM_LIST_NAME = "Professional Master";
        const string SP_PPPW_LIST_NAME = "Professional Performance Plan Workflow";
        const string SP_PPE_LIST_NAME = "Professional Performance Evaluation";

        public int CreateHeader(string requestor, ProfessionalPerformancePlanVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("majorstrength", header.MajorStrength);
            columnValues.Add("performancearea", header.PerformanceArea);
            columnValues.Add("recommendedactivities", header.RecommendedActivities);
            columnValues.Add("professional", new FieldLookupValue { LookupId = (int)header.NameID });
            columnValues.Add("Position", new FieldLookupValue { LookupId = (int)header.PositionAndDepartementID });
            columnValues.Add("performanceplan", new FieldLookupValue { LookupId = (int)header.PerformancePeriodID });
            columnValues.Add("visibleto", SPConnector.GetUser(requestor, _siteUrl, "hr"));
            if (header.StatusForm == "DraftInitiated")
            {
                columnValues.Add("pppstatus", "Draft");
            }
            if (header.StatusForm == null)
            {
                columnValues.Add("pppstatus", "Pending Approval 1 of 2");
            }
            try
            {
                SPConnector.AddListItem(SP_PPP_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_PPP_LIST_NAME, _siteUrl);

        }

        public void CreatePerformancePlanDetails(int? headerID, int? performanceID, string email, string status, IEnumerable<ProjectOrUnitGoalsDetailVM> PerformancePlanDetails)
        {
            foreach (var viewModel in PerformancePlanDetails)
            {
                if (email != null)
                {
                    if (Item.CheckIfSkipped(viewModel))
                        continue;
                    if (Item.CheckIfDeleted(viewModel))
                    {
                        try
                        {
                            SPConnector.DeleteListItem(SP_PPPIG_LIST_NAME, viewModel.ID, _siteUrl);

                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                            throw e;
                        }
                        continue;
                    }
                    var updatedValue = new Dictionary<string, object>();
                    updatedValue.Add("projectunitgoals", viewModel.ProjectOrUnitGoals);
                    updatedValue.Add("professionalperformanceplan", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                    updatedValue.Add("individualgoalcategory", viewModel.Category.Text);
                    updatedValue.Add("individualgoalplan", viewModel.IndividualGoalAndPlan);
                    updatedValue.Add("individualgoalweight", viewModel.Weight);

                    if (viewModel.Remarks != null)
                    {
                        updatedValue.Add("individualgoalremarks", viewModel.Remarks);
                    }
                    try
                    {
                        if (Item.CheckIfUpdated(viewModel))
                        {
                            SPConnector.UpdateListItem(SP_PPPIG_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                        }
                        else
                        {
                            updatedValue.Add("officeemail", email);
                            updatedValue.Add("idperformanceplan", performanceID);
                            SPConnector.AddListItem(SP_PPPIG_LIST_NAME, updatedValue, _siteUrl);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw new Exception(ErrorResource.SPInsertError);
                    }
                }

                if (email == null && status == "Pending Approval 2 of 2" && PerformancePlanDetails.Count() != 0)
                {
                    var updatedValue = new Dictionary<string, object>();
                    updatedValue.Add("status", "Approved");
                    try
                    {
                        SPConnector.UpdateListItem(SP_PPPIG_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw new Exception(ErrorResource.SPInsertError);
                    }
                }
            }
        }

        public ProfessionalPerformancePlanVM GetHeader(int? ID, string requestor)
        {
            var listItem = SPConnector.GetListItem(SP_PPP_LIST_NAME, ID, _siteUrl);
            return ConvertToProfessionalPerformancePlanModel(listItem, ID, requestor);
        }

        private ProfessionalPerformancePlanVM ConvertToProfessionalPerformancePlanModel(ListItem listItem, int? ID, string requestor)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='professionalperformanceplan' /><Value Type='Lookup'>" + ID + @"</Value></Eq><Eq><FieldRef Name='approver0' /><Value Type='Text'>" + requestor + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var viewModel = new ProfessionalPerformancePlanVM();
            string firstName;
            string lastName;
            int level = 0;
            int count = SPConnector.GetList(SP_PPPW_LIST_NAME, _siteUrl, caml).Count();
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            firstName = FormatUtil.ConvertLookupToValue(listItem, "professional");
            lastName = FormatUtil.ConvertLookupToValue(listItem, "professional_x003a_Last_x0020_Na");
            viewModel.Name = string.Format("{0} {1}", firstName, lastName);
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID");
            viewModel.PositionAndDepartement = FormatUtil.ConvertLookupToValue(listItem, "Position");
            viewModel.PerformancePeriod = FormatUtil.ConvertLookupToValue(listItem, "performanceplan");
            viewModel.PerformancePeriodID = FormatUtil.ConvertLookupToID(listItem, "performanceplan");
            viewModel.StatusForm = Convert.ToString(listItem["pppstatus"]);
            viewModel.MajorStrength = Convert.ToString(listItem["majorstrength"]);
            viewModel.PerformanceArea = Convert.ToString(listItem["performancearea"]);
            viewModel.RecommendedActivities = viewModel.PerformanceArea = Convert.ToString(listItem["recommendedactivities"]);
            viewModel.TypeForm = "Professional";

            foreach (var item in SPConnector.GetList(SP_PPPW_LIST_NAME, _siteUrl, caml))
            {
                if (count != 0)
                {
                    level = Convert.ToInt32(item["approverlevel"]);
                    if (viewModel.StatusForm == "Pending Approval 1 of 2" && level == 1)
                    {
                        viewModel.TypeForm = "Approver1";
                    }
                    if (viewModel.StatusForm == "Pending Approval 2 of 2" && level == 2)
                    {
                        viewModel.TypeForm = "Approver2";
                    }
                    if (viewModel.TypeForm == "Professional")
                    {
                        viewModel.TypeForm = "AcessDenied";
                    }
                }
            }

            // Convert Details
            viewModel.ProjectOrUnitGoalsDetails = GetPerformancePlanDetails(viewModel.ID);

            return viewModel;
        }

        private IEnumerable<ProjectOrUnitGoalsDetailVM> GetPerformancePlanDetails(int? ID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='professionalperformanceplan' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            var ProjectOrUnitGoalsDetail = new List<ProjectOrUnitGoalsDetailVM>();
            foreach (var item in SPConnector.GetList(SP_PPPIG_LIST_NAME, _siteUrl, caml))
            {
                ProjectOrUnitGoalsDetail.Add(ConvertToPerformancePlanDetail(item));
            }

            return ProjectOrUnitGoalsDetail;
        }

        private ProjectOrUnitGoalsDetailVM ConvertToPerformancePlanDetail(ListItem item)
        {
            return new ProjectOrUnitGoalsDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                ProjectOrUnitGoals = Convert.ToString(item["projectunitgoals"]),
                Category = IndividualGoalDetailVM.GetCategoryDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["individualgoalcategory"])
                    }),
                IndividualGoalAndPlan = Convert.ToString(item["individualgoalplan"]),
                Weight = Convert.ToInt32(item["individualgoalweight"]),
                Remarks = Convert.ToString(item["individualgoalremarks"]),
            };
        }

        public ProfessionalPerformancePlanVM GetPopulatedModel(string requestor = null)
        {
            var model = new ProfessionalPerformancePlanVM();
            string period = null;
            DateTime dateTemp = DateTime.UtcNow;
            DateTime? createDate = null;
            int idTemp = 0;

            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + requestor + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var models = new List<ProfessionalPerformancePlanVM>();
            foreach (var item in SPConnector.GetList(SP_PP_LIST_NAME, _siteUrl))
            {
                dateTemp = Convert.ToDateTime(item["Created"]).ToLocalTime();

                if (createDate == null)
                {
                    createDate = Convert.ToDateTime(item["Created"]).ToLocalTime();
                }
                if (dateTemp >= createDate)
                {
                    createDate = dateTemp;
                    period = Convert.ToString(item["Title"]);
                    idTemp = Convert.ToInt32(item["ID"]);
                }
            }

            model.PerformancePeriod = period;
            model.PerformancePeriodID = idTemp;

            foreach (var item in SPConnector.GetList(SP_PM_LIST_NAME, _siteUrl, caml))
            {
                model.Name = Convert.ToString(item["Title"]);
                model.NameID = Convert.ToInt32(item["ID"]);
                model.PositionAndDepartement = FormatUtil.ConvertLookupToValue(item, "Position");
                model.PositionAndDepartementID = FormatUtil.ConvertLookupToID(item, "Position");
            }

            return model;

        }

        public bool UpdateHeader(ProfessionalPerformancePlanVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;
            if (header.StatusForm == "Initiated")
            {
                columnValues.Add("pppstatus", "Pending Approval 1 of 2");
            }
            if (header.StatusForm == "Draft")
            {
                columnValues.Add("pppstatus", "Pending Approval 1 of 2");
            }
            if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 2")
            {
                columnValues.Add("pppstatus", "Pending Approval 2 of 2");
            }
            if (header.TypeForm == "Approver2" && header.StatusForm == "Pending Approval 2 of 2")
            {
                columnValues.Add("pppstatus", "Approved");
            }
            if (header.StatusForm == "DraftInitiated" || header.StatusForm == "DraftDraft")
            {
                columnValues.Add("pppstatus", "Draft");
            }
            if (header.StatusForm == "Reject1" || header.StatusForm == "Reject2")
            {
                columnValues.Add("pppstatus", "Rejected");
            }

            columnValues.Add("majorstrength", header.MajorStrength);
            columnValues.Add("performancearea", header.PerformanceArea);
            columnValues.Add("recommendedactivities", header.RecommendedActivities);
            try
            {
                SPConnector.UpdateListItem(SP_PPP_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            var entitiy = new ProfessionalPerformancePlanVM();
            entitiy = header;
            return true;
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public async Task CreatePerformancePlanDetailsAsync(int? headerID, int? performanceID, string email, string status, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails)
        {
            CreatePerformancePlanDetails(headerID, performanceID, email, status, performancePlanDetails);
        }

        public void SendEmail(ProfessionalPerformancePlanVM header, string workflowTransactionListName, string transactionLookupColumnName, int headerID, int level, string messageForApprover, string messageForRequestor)
        {
            var camlprof = @"<View>  
          <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + headerID + @" </Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /><FieldRef Name='professional_x003a_Office_x0020_' /></ViewFields> 
            </View>";

            var caml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='" + transactionLookupColumnName + @"' />
               <Value Type='Lookup'>" + headerID + @"</Value></Eq><Eq>
               <FieldRef Name='approverlevel' /><Value Type='Choice'>" + level + @"</Value></Eq></And></Where> 
            </Query> 
            </View>";

            string emails = null;
            string professionalEmail = null;
            var columnValues = new Dictionary<string, object>();
            if (header.Requestor != null)
            {
                if (header.StatusForm == "Initiated" || header.StatusForm == "Pending Approval 1 of 2" || header.StatusForm == "Pending Approval 2 of 2" || header.StatusForm == null || header.StatusForm == "Draft")
                {
                    foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                    {
                        emails = Convert.ToString(item["approver0"]);

                        EmailUtil.Send(emails, "Ask for Approval", messageForApprover);
                        //SPConnector.SendEmail(item, message, "Ask for Approval Level 2", _siteUrl);

                        columnValues.Add("visibletoapprover1", SPConnector.GetUser(emails, _siteUrl, "hr"));
                        try
                        {
                            SPConnector.UpdateListItem(SP_PPP_LIST_NAME, headerID, columnValues, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                        }
                    }
                }
            }
            if (header.Requestor == null && header.StatusForm == "Pending Approval 1 of 2")
            {
                foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                {
                    emails = Convert.ToString(item["approver0"]);

                    EmailUtil.Send(emails, "Ask for Approval", messageForApprover);
                    //SPConnector.SendEmail(item, message, "Ask for Approval Level 2", _siteUrl);

                    columnValues.Add("visibletoapprover2", SPConnector.GetUser(emails, _siteUrl, "hr"));
                    try
                    {
                        SPConnector.UpdateListItem(SP_PPP_LIST_NAME, headerID, columnValues, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                    }
                }

                foreach (var item in SPConnector.GetList(SP_PPP_LIST_NAME, _siteUrl, camlprof))
                {
                    professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                    Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                    EmailUtil.Send(professionalEmail, "Plan Status", messageForRequestor);
                    //SPConnector.SendEmail(item, "Approved by Level 1", _siteUrl);
                }
            }

            if (header.Requestor == null)
            {
                if (header.StatusForm == "Pending Approval 2 of 2" || header.StatusForm == "Reject1" || header.StatusForm == "Reject2")
                {
                    foreach (var item in SPConnector.GetList(SP_PPP_LIST_NAME, _siteUrl, camlprof))
                    {
                        professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                       Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                        EmailUtil.Send(professionalEmail, "Plan Status", messageForRequestor);
                        //SPConnector.SendEmail(item, message, "Ask for Approval", _siteUrl);
                    }
                }
            }
        }
    }
}

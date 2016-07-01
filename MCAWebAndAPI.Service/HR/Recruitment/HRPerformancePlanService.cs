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

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRPerformancePlanService : IHRPerformancePlanService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_PPPIG_LIST_NAME = "PPP Individual Goal";
        const string SP_PPP_LIST_NAME = "Professional Performance Plan";
        const string SP_PP_LIST_NAME = "Performance Plan";
        const string SP_PPE_LIST_NAME = "Professional Performance Evaluation";

        public int CreateHeader(ProfessionalPerformancePlanVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("majorstrength", header.MajorStrength);
            columnValues.Add("performancearea", header.PerformanceArea);
            columnValues.Add("recommendedactivities", header.RecommendedActivities);
            columnValues.Add("professional", new FieldLookupValue { LookupId = (int)header.NameID });
            columnValues.Add("Position", new FieldLookupValue { LookupId = (int)header.PositionAndDepartementID });
            columnValues.Add("performanceplan", new FieldLookupValue { LookupId = (int)header.PerformancePeriodID });
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

        public void CreatePerformancePlanDetails(int? headerID, IEnumerable<ProjectOrUnitGoalsDetailVM> PerformancePlanDetails)
        {
            foreach (var viewModel in PerformancePlanDetails)
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
                updatedValue.Add("individualgoalremarks", viewModel.Remarks);
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_PPPIG_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_PPPIG_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public ProfessionalPerformancePlanVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_PPP_LIST_NAME, ID, _siteUrl);
            return ConvertToProfessionalPerformancePlanModel(listItem);
        }

        private ProfessionalPerformancePlanVM ConvertToProfessionalPerformancePlanModel(ListItem listItem)
        {
            var viewModel = new ProfessionalPerformancePlanVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Name = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID");
            viewModel.PositionAndDepartement = FormatUtil.ConvertLookupToValue(listItem, "Position");
            viewModel.PerformancePeriod = FormatUtil.ConvertLookupToValue(listItem, "performanceplan");
            viewModel.StatusForm = Convert.ToString(listItem["pppstatus"]);
            viewModel.MajorStrength = Convert.ToString(listItem["majorstrength"]);
            viewModel.PerformanceArea = Convert.ToString(listItem["performancearea"]);
            viewModel.RecommendedActivities = viewModel.PerformanceArea = Convert.ToString(listItem["recommendedactivities"]);

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
                Category = ProjectOrUnitGoalsDetailVM.GetCategoryDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM
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

            int dateTemp;
            string emailTemp;
            var models = new List<ProfessionalPerformancePlanVM>();
            foreach (var item in SPConnector.GetList(SP_PP_LIST_NAME, _siteUrl))
            {
                dateTemp = Convert.ToInt32(item["Title"]);
                if (model.DatetimeCaml == dateTemp)
                {
                    model.PerformancePeriod = Convert.ToString(item["Title"]);
                    model.PerformancePeriodID = Convert.ToInt32(item["ID"]);
                }
            }

            foreach (var item in SPConnector.GetList(SP_PPP_LIST_NAME, _siteUrl))
            {
                emailTemp = FormatUtil.ConvertLookupToValue(item, "professional_x003a_Office_x0020_");
                if (requestor == emailTemp)
                {
                    model.Name = FormatUtil.ConvertLookupToValue(item, "professional");
                    model.NameID = FormatUtil.ConvertLookupToID(item, "professional");
                    model.PositionAndDepartement = FormatUtil.ConvertLookupToValue(item, "Position");
                    model.PositionAndDepartementID = FormatUtil.ConvertLookupToID(item, "Position");
                }
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
            if (header.Requestor == null && header.StatusForm == "Pending Approval 1 of 2")
            {
                columnValues.Add("pppstatus", "Pending Approval 2 of 2");
            }
            if (header.Requestor == null && header.StatusForm == "Pending Approval 2 of 2")
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

        public async Task CreatePerformancePlanDetailsAsync(int? headerID, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails)
        {
            CreatePerformancePlanDetails(headerID, performancePlanDetails);
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

            var emails = new List<string>();
            var professionalEmail = new List<string>();

            if (header.StatusForm == "Initiated" || header.StatusForm == "Pending Approval 1 of 2"|| header.StatusForm == null)
            {
                foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                {
                    emails.Add(Convert.ToString(item["approver0"]));
                }
                foreach (var item in emails)
                {
                    EmailUtil.Send(item, "Ask for Approval", messageForApprover);
                    //SPConnector.SendEmail(item, message, "Ask for Approval Level 2", _siteUrl);
                }

                if (header.StatusForm == "Pending Approval 1 of 2")
                {
                    foreach (var item in SPConnector.GetList(SP_PPP_LIST_NAME, _siteUrl, camlprof))
                    {
                        professionalEmail.Add(item["professional_x003a_Office_x0020_"] == null ? "" :
                        Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));
                    }

                    foreach (var item in professionalEmail)
                    {
                        EmailUtil.Send(item, "Approved by Level 1", messageForRequestor);
                        //SPConnector.SendEmail(item, "Approved by Level 1", _siteUrl);
                    }
                }
            }
            else
            {
                foreach (var item in SPConnector.GetList(SP_PPP_LIST_NAME, _siteUrl, camlprof))
                {
                    professionalEmail.Add(item["professional_x003a_Office_x0020_"] == null ? "" :
                   Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));
                }
                foreach (var item in professionalEmail)
                {
                    EmailUtil.Send(item, "Status", messageForRequestor);
                    //SPConnector.SendEmail(item, message, "Ask for Approval", _siteUrl);
                }
            }
        }
    }
}

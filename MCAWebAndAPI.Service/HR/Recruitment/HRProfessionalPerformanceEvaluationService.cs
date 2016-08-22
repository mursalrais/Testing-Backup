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
    public class HRProfessionalPerformanceEvaluationService : IHRProfessionalPerformanceEvaluationService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_PPEO_LIST_NAME = "Professional Performance Evaluation Output";
        const string SP_PP_LIST_NAME = "Performance Evaluation";
        const string SP_PPE_LIST_NAME = "Professional Performance Evaluation";
        const string SP_PPEW_LIST_NAME = "Professional Performance Evaluation Workflow";

        public int CreateHeader(ProfessionalPerformanceEvaluationVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("professional", new FieldLookupValue { LookupId = (int)header.NameID });
            columnValues.Add("Position", new FieldLookupValue { LookupId = (int)header.PositionAndDepartementID });
            columnValues.Add("performanceplan", new FieldLookupValue { LookupId = (int)header.PerformancePeriodID });
            columnValues.Add("ppestatus", "Pending Approval 1 of 2");
            try
            {
                SPConnector.AddListItem(SP_PPE_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_PPE_LIST_NAME, _siteUrl);
        }

        public void CreatePerformanceEvaluationDetails(int? headerID, IEnumerable<ProfessionalPerformanceEvaluationDetailVM> performanceEvaluationDetails)
        {
            foreach (var viewModel in performanceEvaluationDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PPEO_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("professionalperformanceevaluatio", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add("individualgoalcategory", viewModel.Category.Text);
                updatedValue.Add("individualgoalplan", viewModel.IndividualGoalAndPlan);
                updatedValue.Add("individualgoalweight", viewModel.PlannedWeight);
                updatedValue.Add("individualgoalplanactualweight", viewModel.ActualWeight);
                updatedValue.Add("individualgoalplanscore", viewModel.Score);
                updatedValue.Add("individualgoalplantotalscore", viewModel.TotalScore);
                updatedValue.Add("output", viewModel.Output);

                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_PPEO_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_PPEO_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public async Task CreatePerformanceEvaluationDetailsAsync(int? headerID, IEnumerable<ProfessionalPerformanceEvaluationDetailVM> performanceEvaluationDetails)
        {
            CreatePerformanceEvaluationDetails(headerID, performanceEvaluationDetails);
        }

        public ProfessionalPerformanceEvaluationVM GetHeader(int? ID, string requestor)
        {
            var listItem = SPConnector.GetListItem(SP_PPE_LIST_NAME, ID, _siteUrl);
            return ConvertToProfessionalPerformancePlanModel(listItem, ID, requestor);
        }

        private ProfessionalPerformanceEvaluationVM ConvertToProfessionalPerformancePlanModel(ListItem listItem, int? ID, string requestor)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='professionalperformanceevaluatio' /><Value Type='Lookup'>" + ID + @"</Value></Eq><Eq><FieldRef Name='approver0' /><Value Type='Text'>" + requestor + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var viewModel = new ProfessionalPerformanceEvaluationVM();
            string firstName;
            string lastName;
            int level = 0;
            int count = SPConnector.GetList(SP_PPEW_LIST_NAME, _siteUrl, caml).Count();
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            firstName = FormatUtil.ConvertLookupToValue(listItem, "professional");
            lastName = FormatUtil.ConvertLookupToValue(listItem, "professional_x003a_Last_x0020_Na");
            viewModel.Name = string.Format("{0} {1}", firstName, lastName);
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID");
            viewModel.PositionAndDepartement = FormatUtil.ConvertLookupToValue(listItem, "Position");
            viewModel.PerformancePeriod = FormatUtil.ConvertLookupToValue(listItem, "performanceevaluation");
            viewModel.StatusForm = Convert.ToString(listItem["ppestatus"]);
            viewModel.TypeForm = "Professional";
            viewModel.OverallTotalScore = Convert.ToDecimal(listItem["overalltotalscore"]); 

            foreach (var item in SPConnector.GetList(SP_PPEW_LIST_NAME, _siteUrl, caml))
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
            viewModel.ProfessionalPerformanceEvaluationDetails = GetProfessionalPerformanceDetails(viewModel.ID);

            return viewModel;
        }

        private IEnumerable<ProfessionalPerformanceEvaluationDetailVM> GetProfessionalPerformanceDetails(int? ID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='professionalperformanceevaluatio' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            var ProfessionalPerformanceEvaluationDetails = new List<ProfessionalPerformanceEvaluationDetailVM>();
            foreach (var item in SPConnector.GetList(SP_PPEO_LIST_NAME, _siteUrl, caml))
            {
                ProfessionalPerformanceEvaluationDetails.Add(ConvertToProfessionalPerformanceDetails(item));
            }

            return ProfessionalPerformanceEvaluationDetails;
        }

        private ProfessionalPerformanceEvaluationDetailVM ConvertToProfessionalPerformanceDetails(ListItem item)
        {
            return new ProfessionalPerformanceEvaluationDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                ProfessionalPerformanceEvaluationId = item["professionalperformanceevaluatio"] == null ? 0 :
                        Convert.ToInt32((item["professionalperformanceevaluatio"] as FieldLookupValue).LookupValue),
                Category = IndividualGoalDetailVM.GetCategoryDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["individualgoalcategory"])
                    }),
                IndividualGoalAndPlan = Convert.ToString(item["individualgoalplan"]),
                ProjectOrUnitGoals = Convert.ToString(item["projectunitgoals"]),
                PlannedWeight = Convert.ToInt32(item["individualgoalweight"]),
                ActualWeight = Convert.ToInt32(item["individualgoalplanactualweight"]),
                Score = Convert.ToDecimal(item["individualgoalplanscore"]),
                TotalScore = Convert.ToDecimal(item["individualgoalplantotalscore"]),
                Output = Convert.ToString(item["output"]),
            };
        }

        public ProfessionalPerformanceEvaluationVM GetPopulatedModel(string requestor = null)
        {
            var model = new ProfessionalPerformanceEvaluationVM();

            int dateTemp;
            string emailTemp;
            var models = new List<ProfessionalPerformanceEvaluationVM>();
            foreach (var item in SPConnector.GetList(SP_PP_LIST_NAME, _siteUrl))
            {
                dateTemp = Convert.ToInt32(item["Title"]);
                if (model.DatetimeCaml == dateTemp)
                {
                    model.PerformancePeriod = Convert.ToString(item["Title"]);
                    model.PerformancePeriodID = Convert.ToInt32(item["ID"]);
                }
            }

            foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl))
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

        public void SendEmail(ProfessionalPerformanceEvaluationVM header, string workflowTransactionListName, string transactionLookupColumnName, int headerID, int level, string messageForApprover, string messageForRequestor)
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

            if (header.StatusForm == "Initiated" || header.StatusForm == null)
            {
                foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                {
                    emails = Convert.ToString(item["approver0"]);

                    EmailUtil.Send(emails, "Ask for Approval", messageForApprover);

                    if (header.StatusForm == "Initiated" || header.StatusForm == null)
                    {
                        columnValues.Add("visibletoapprover1", SPConnector.GetUser(emails, _siteUrl, "hr"));
                        try
                        {
                            SPConnector.UpdateListItem(SP_PPE_LIST_NAME, headerID, columnValues, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                        }
                    }
                }
            }
            if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 2")
            {
                foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                {
                    emails = Convert.ToString(item["approver0"]);

                    EmailUtil.Send(emails, "Ask for Approval", messageForApprover);
                    //SPConnector.SendEmail(item, message, "Ask for Approval Level 2", _siteUrl);

                    columnValues.Add("visibletoapprover2", SPConnector.GetUser(emails, _siteUrl, "hr"));
                    try
                    {
                        SPConnector.UpdateListItem(SP_PPE_LIST_NAME, headerID, columnValues, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                    }
                }

                foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                {
                    professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                    Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                    EmailUtil.Send(professionalEmail, "Evaluation Plan Status", messageForRequestor);
                    //SPConnector.SendEmail(item, "Approved by Level 1", _siteUrl);
                }
            }
            if (header.TypeForm == "Approver2" && header.StatusForm == "Pending Approval 2 of 2")
                {
                    foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                    {
                        professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                        Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                        EmailUtil.Send(professionalEmail, "Evaluation Plan Status", messageForRequestor);
                        //SPConnector.SendEmail(item, "Approved by Level 1", _siteUrl);PConnector.SendEmail(item, "Approved by Level 1", _siteUrl);
                    }
                }
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateHeader(ProfessionalPerformanceEvaluationVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;
            if (header.StatusForm == "Initiated")
            {
                columnValues.Add("ppestatus", "Pending Approval 1 of 2");
            }
            if (header.StatusForm == "Draft")
            {
                columnValues.Add("ppestatus", "Pending Approval 1 of 2");
            }
            if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 2")
            {
                columnValues.Add("ppestatus", "Pending Approval 2 of 2");
            }
            if (header.TypeForm == "Approver2" && header.StatusForm == "Pending Approval 2 of 2")
            {
                columnValues.Add("ppestatus", "Approved");
            }
            if (header.StatusForm == "DraftInitiated" || header.StatusForm == "DraftDraft")
            {
                columnValues.Add("ppestatus", "Draft");
            }
            if (header.StatusForm == "Reject1" || header.StatusForm == "Reject2")
            {
                columnValues.Add("ppestatus", "Rejected");
            }
            columnValues.Add("overalltotalscore", header.OverallTotalScore);
            try
            {
                SPConnector.UpdateListItem(SP_PPE_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            var entitiy = new ProfessionalPerformanceEvaluationVM();
            entitiy = header;
            return true;
        }
    }
}

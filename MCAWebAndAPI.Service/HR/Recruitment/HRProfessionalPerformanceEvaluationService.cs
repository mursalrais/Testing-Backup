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
using MCAWebAndAPI.Service.Common;

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

        public async Task<ProfessionalPerformanceEvaluationVM> GetHeader(int? ID, string requestor, string listName, string listNameWorkflow, string columnName)
        {
            var listItem = SPConnector.GetListItem(SP_PPE_LIST_NAME, ID, _siteUrl);
            return await ConvertToProfessionalPerformancePlanModel(listItem, ID, requestor, listName, listNameWorkflow, columnName);
        }

        private async Task<ProfessionalPerformanceEvaluationVM> ConvertToProfessionalPerformancePlanModel(ListItem listItem, int? ID, string requestor, string listName, string listNameWorkflow, string columnName)
        {
            var caml = @"<View><Query><Where><And><Eq><FieldRef Name='professionalperformanceevaluatio' /><Value Type='Lookup'>" + ID + "</Value></Eq><Eq><FieldRef Name='approvername_x003a_Office_x0020_' /><Value Type='Lookup'>" + requestor + "</Value></Eq></And></Where></Query><ViewFields><FieldRef Name='approverlevel' /><FieldRef Name='approvername_x003a_Office_x0020_' /><FieldRef Name='professionalperformanceevaluatio' /></ViewFields><QueryOptions /></View>";

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
                    if (level == 1)
                    {
                        if (viewModel.StatusForm == "Pending Approval 1 of 2" || viewModel.StatusForm == "Pending Approval 1 of 1" || viewModel.StatusForm == "Pending Approval 1 of 3")
                        {
                            viewModel.TypeForm = "Approver1";
                        }
                    }
                    if (level == 2)
                    {
                        if (viewModel.StatusForm == "Pending Approval 2 of 2" || viewModel.StatusForm == "Pending Approval 2 of 3")
                        {
                            viewModel.TypeForm = "Approver2";
                        }
                    }
                    if (level == 3)
                    {
                        if (viewModel.StatusForm == "Pending Approval 3 of 3")
                        {
                            viewModel.TypeForm = "Approver3";
                        }
                    }
                    if (viewModel.TypeForm == "Professional" && viewModel.StatusForm != "Draft")
                    {
                        viewModel.TypeForm = "AcessDenied";
                    }
                }
            }

            // Convert Details
            viewModel.ProfessionalPerformanceEvaluationDetails = GetProfessionalPerformanceDetails(viewModel.ID);

            var _workflow = new WorkflowService();
            _workflow.SetSiteUrl(_siteUrl);
            var intID = Convert.ToInt32(ID);
            var Check = await _workflow.CheckWorkflow(intID, listNameWorkflow, columnName);
            if (Check.Count() != 0)
            {
                viewModel.WorkflowItems = Check;
            }
            if (Check.Count() == 0)
            {
                viewModel.WorkflowItems = await _workflow.GetWorkflowDetails(requestor, listName);
            }

            viewModel.ApproverCount = viewModel.WorkflowItems.Count();

            foreach (var item in viewModel.WorkflowItems)
            {
                var lvl = item.Level;
                if (lvl == "1")
                {
                    viewModel.Approver1 = item.ApproverNameText;
                }

                if (lvl == "2")
                {
                    viewModel.Approver2 = item.ApproverNameText;
                }
            }

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
                    emails = FormatUtil.ConvertLookupToValue(item, "approvername_x003a_Office_x0020_");

                    columnValues.Add("visibletoapprover1", SPConnector.GetUser(emails, _siteUrl));
                    try
                    {
                        SPConnector.UpdateListItem(SP_PPE_LIST_NAME, headerID, columnValues, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                    }

                    try
                    {
                        EmailUtil.Send(emails, "Request for Approval of Performance Evaluation Form", messageForApprover);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
            }

            if (header.ApproverCount == 1)
            {
                if (header.TypeForm == "Approver1")
                {
                    if (header.StatusForm == "Pending Approval 1 of 1")
                    {
                        foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                        {
                            professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                           Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));
                            try
                            {
                                EmailUtil.Send(professionalEmail, "Approval of Performance Plan Form", messageForRequestor);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }

            if (header.ApproverCount == 2)
            {
                if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 2")
                {
                    foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                    {
                        emails = FormatUtil.ConvertLookupToValue(item, "approvername_x003a_Office_x0020_");

                        columnValues.Add("visibletoapprover2", SPConnector.GetUser(emails, _siteUrl));
                        try
                        {
                            SPConnector.UpdateListItem(SP_PPE_LIST_NAME, headerID, columnValues, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }

                        try
                        {
                            EmailUtil.Send(emails, "Request for Approval of Performance Plan Form", messageForApprover);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }

                    foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                    {
                        professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                        Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                        try
                        {
                            EmailUtil.Send(emails, "Approval of Performance Plan Form", messageForRequestor);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }

                if (header.TypeForm == "Approver2")
                {
                    if (header.StatusForm == "Pending Approval 2 of 2")
                    {
                        foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                        {
                            professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                           Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));
                            try
                            {
                                EmailUtil.Send(professionalEmail, "Approval of Performance Plan Form", messageForRequestor);
                            }
                            catch (Exception e)
                            {
                                logger.Error(e.Message);
                                throw e;
                            }
                        }
                    }
                }
            }

            if (header.ApproverCount == 3)
            {
                if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 3")
                {
                    foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                    {
                        emails = FormatUtil.ConvertLookupToValue(item, "approvername_x003a_Office_x0020_");

                        columnValues.Add("visibletoapprover2", SPConnector.GetUser(emails, _siteUrl));
                        try
                        {
                            SPConnector.UpdateListItem(SP_PPE_LIST_NAME, headerID, columnValues, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }

                        try
                        {
                            EmailUtil.Send(emails, "Request for Approval of Performance Plan Form", messageForApprover);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }

                    foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                    {
                        professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                        Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                        try
                        {
                            EmailUtil.Send(emails, "Approval of Performance Plan Form", messageForRequestor);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }

                if (header.TypeForm == "Approver2" && header.StatusForm == "Pending Approval 2 of 3")
                {
                    foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
                    {
                        emails = FormatUtil.ConvertLookupToValue(item, "approvername_x003a_Office_x0020_");

                        columnValues.Add("visibletoapprover3", SPConnector.GetUser(emails, _siteUrl));
                        try
                        {
                            SPConnector.UpdateListItem(SP_PPE_LIST_NAME, headerID, columnValues, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }

                        try
                        {
                            EmailUtil.Send(emails, "Request for Approval of Performance Plan Form", messageForApprover);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }

                    foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                    {
                        professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                        Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));

                        try
                        {
                            EmailUtil.Send(emails, "Approval of Performance Plan Form", messageForRequestor);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }

                if (header.TypeForm == "Approver3")
                {
                    if (header.StatusForm == "Pending Approval 3 of 3")
                    {
                        foreach (var item in SPConnector.GetList(SP_PPE_LIST_NAME, _siteUrl, camlprof))
                        {
                            professionalEmail = (item["professional_x003a_Office_x0020_"] == null ? "" :
                           Convert.ToString((item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue));
                            try
                            {
                                EmailUtil.Send(professionalEmail, "Approval of Performance Plan Form", messageForRequestor);
                            }
                            catch (Exception e)
                            {
                                logger.Error(e.Message);
                                throw e;
                            }
                        }
                    }
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
            if (header.ApproverCount == 1)
            {
                if (header.StatusForm == "Initiated")
                {
                    columnValues.Add("ppestatus", "Pending Approval 1 of 1");
                }
                if (header.StatusForm == "Draft")
                {
                    columnValues.Add("ppestatus", "Pending Approval 1 of 1");
                }
                if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 1")
                {
                    columnValues.Add("ppestatus", "Approved");
                }
            }
            if (header.ApproverCount == 2)
            {
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
            }
            if (header.ApproverCount == 3)
            {
                if (header.StatusForm == "Initiated")
                {
                    columnValues.Add("ppestatus", "Pending Approval 1 of 3");
                }
                if (header.StatusForm == "Draft")
                {
                    columnValues.Add("ppestatus", "Pending Approval 1 of 3");
                }
                if (header.TypeForm == "Approver1" && header.StatusForm == "Pending Approval 1 of 3")
                {
                    columnValues.Add("ppestatus", "Pending Approval 2 of 3");
                }
                if (header.TypeForm == "Approver2" && header.StatusForm == "Pending Approval 2 of 3")
                {
                    columnValues.Add("ppestatus", "Pending Approval 3 of 3");
                }
                if (header.TypeForm == "Approver3" && header.StatusForm == "Pending Approval 3 of 3")
                {
                    columnValues.Add("ppestatus", "Approved");
                }
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

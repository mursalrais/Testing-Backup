using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class PerformanceEvaluationService : IPerformanceEvaluationService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_LIST_NAME = "Performance Evaluation";
        const string SP_LIST_PLAN = "Performance Plan";
        const string SP_DETAIL_LIST_NAME = "Professional Performance Evaluation";
        const string SP_DETAIL_LIST_PLAN = "Professional Performance Plan";
        const string SP_PPP_INDIVIDUAL_PLAN = "PPP Individual Goal";
        const string SP_PROF_PERFM_EVAL = "Professional Performance Evaluation Output";

        public int CreatePerformanceEvaluation(PerformanceEvaluationVM PerformanceEvaluation)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("Title", PerformanceEvaluation.Period.Value);
            updatedValues.Add("latestdateforcreation", PerformanceEvaluation.LatestCreationDate.Value);
            updatedValues.Add("latestdateforapproval1", PerformanceEvaluation.LatestDateApproval1.Value);
            updatedValues.Add("latestdateforapproval2", PerformanceEvaluation.LatestDateApproval2.Value);
            updatedValues.Add("pestatus", "Open");

            try
            {
                SPConnector.AddListItem(SP_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }



            return SPConnector.GetLatestListItemID(SP_LIST_NAME, _siteUrl);
        }

        public bool UpdatePerformanceEvaluation(PerformanceEvaluationVM PerformanceEvaluation)
        {
            var updatedValues = new Dictionary<string, object>();
            if (PerformanceEvaluation.EditType == "Edit")
            {
                updatedValues.Add("Title", PerformanceEvaluation.Period.Value);
                updatedValues.Add("latestdateforcreation", PerformanceEvaluation.LatestCreationDate.Value);
                updatedValues.Add("latestdateforapproval1", PerformanceEvaluation.LatestDateApproval1.Value);
                updatedValues.Add("latestdateforapproval2", PerformanceEvaluation.LatestDateApproval2.Value);
            }
            else
            {
                updatedValues.Add("pestatus", "Closed");
                updatedValues.Add("closingdate", DateTime.UtcNow);
            }
            try
            {
                SPConnector.UpdateListItem(SP_LIST_NAME, PerformanceEvaluation.ID, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            if (PerformanceEvaluation.EditType != "Edit")
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='performanceevaluation_x003a_ID' /><Value Type='Lookup'>" + PerformanceEvaluation.ID + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /></ViewFields><QueryOptions /></View>";
                var listItem = SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl, caml);
                foreach (var item in listItem)
                {
                    updatedValues = new Dictionary<string, object>();
                    updatedValues.Add("pestatus", "Closed");
                    try
                    {
                        SPConnector.UpdateListItem(SP_DETAIL_LIST_NAME, Convert.ToInt32(item["ID"]), updatedValues, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Debug(e.Message);
                        return false;
                    }

                }
            }            

            return true;
        }

        public void CreatePerformanceEvaluationDetails(int? headerID, string emailMessage)
        {
            //Get All Active Professional
            var caml = @"<View><Query><Where><Or><IsNull><FieldRef Name='lastworkingdate' /></IsNull><Gt><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "T23:57:44Z</Value></Gt></Or></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='officeemail' /><FieldRef Name='Position_x003a_ID' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", _siteUrl, caml);
            var updatedValues = new Dictionary<string, object>();
            string emailTo;
            int IdDetail;
            int IDHeaderPLan = SPConnector.GetLatestListItemID(SP_LIST_PLAN, _siteUrl);
            FieldUserValue visibleTo;
            foreach (var item in listItem)
            {
                updatedValues = new Dictionary<string, object>();
                updatedValues.Add("performanceevaluation", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("ppestatus", "Initiated");
                updatedValues.Add("pestatus", "Open");
                if (((item["Position_x003a_ID"] as FieldLookupValue)) != null)
                {
                    updatedValues.Add("Position", (item["Position_x003a_ID"] as FieldLookupValue).LookupId);
                }
                updatedValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(item["ID"]) });
                emailTo = Convert.ToString(item["officeemail"]);
                visibleTo = SPConnector.GetUser(emailTo, _siteUrl);
                updatedValues.Add("visibleto", visibleTo);
                try
                {
                    SPConnector.AddListItem(SP_DETAIL_LIST_NAME, updatedValues, _siteUrl);
                }

                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
                }


                //send email to professional
                try
                {
                    if (emailTo == "" || emailTo == null)
                    {
                        emailTo = "anugerahseptian@gmail.com";
                    }

                    IdDetail = SPConnector.GetLatestListItemID(SP_DETAIL_LIST_NAME, _siteUrl);
                    
                    EmailUtil.Send(emailTo, "Notify to initiate Performance Evaluation", string.Format(emailMessage, _siteUrl, IdDetail));

                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
                }

                caml = @"<View><Query><Where><And><Eq><FieldRef Name='idperformanceplan' /><Value Type='Number'>" + IDHeaderPLan.ToString() + "</Value></Eq><And><Eq><FieldRef Name='status' /><Value Type='Text'>Approved</Value></Eq><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + emailTo + "</Value></Eq></And></And></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='Edit' /><FieldRef Name='LinkTitleNoMenu' /><FieldRef Name='LinkTitle' /><FieldRef Name='DocIcon' /><FieldRef Name='AppAuthor' /><FieldRef Name='AppEditor' /><FieldRef Name='individualgoalcategory' /><FieldRef Name='individualgoalplan' /><FieldRef Name='individualgoalweight' /><FieldRef Name='individualgoalremarks' /><FieldRef Name='professionalperformanceplandetai' /><FieldRef Name='professionalperformanceplan' /><FieldRef Name='projectunitgoals' /></ViewFields><QueryOptions /></View>";
                var placeItem = SPConnector.GetList(SP_PPP_INDIVIDUAL_PLAN, _siteUrl, caml);

                foreach (var pppIndividual in placeItem)
                {
                    updatedValues = new Dictionary<string, object>();
                    updatedValues.Add("individualgoalplan", Convert.ToString(pppIndividual["individualgoalplan"]));
                    updatedValues.Add("individualgoalcategory", Convert.ToString(pppIndividual["individualgoalcategory"]));
                    updatedValues.Add("individualgoalweight", Convert.ToString(pppIndividual["individualgoalweight"]));
                    updatedValues.Add("professionalperformanceevaluatio", new FieldLookupValue { LookupId = Convert.ToInt32(IdDetail) });
                    updatedValues.Add("projectunitgoals", Convert.ToString(pppIndividual["projectunitgoals"]));

                    try
                    {
                        SPConnector.AddListItem(SP_PROF_PERFM_EVAL, updatedValues, _siteUrl);
                    }

                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                }
            }
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public PerformanceEvaluationVM GetPerformanceEvaluation(int? ID)
        {
            var viewModel = new PerformanceEvaluationVM();
            if (ID == null)
            {
                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToPerformanceEvaluationVM(listItem);

            return viewModel;
        }

        private PerformanceEvaluationVM ConvertToPerformanceEvaluationVM(ListItem listItem)
        {
            var viewModel = new PerformanceEvaluationVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Period.Value = Convert.ToString(listItem["Title"]);
            DateTime _lastCreationDate = Convert.ToDateTime(listItem["latestdateforcreation"]).ToLocalTime();
            viewModel.LatestCreationDate = new DateTime(_lastCreationDate.Year,_lastCreationDate.Month,_lastCreationDate.Day);
            DateTime _lastDateApproval = Convert.ToDateTime(listItem["latestdateforapproval1"]).ToLocalTime();
            viewModel.LatestDateApproval1 = new DateTime(_lastDateApproval.Year,_lastDateApproval.Month,_lastDateApproval.Day);
            DateTime _lastDateApproval2 = Convert.ToDateTime(listItem["latestdateforapproval2"]).ToLocalTime();
            viewModel.LatestDateApproval2 = new DateTime(_lastDateApproval2.Year,_lastDateApproval2.Month,_lastDateApproval2.Day);
            viewModel.Status = Convert.ToString(listItem["pestatus"]);
            viewModel.IntiationDate = Convert.ToDateTime(listItem["Created"]);

            DateTime Now = DateTime.UtcNow;

            //get Detail
            var caml = @"<View><Query><Where><Eq><FieldRef Name='performanceevaluation_x003a_ID' /><Value Type='Lookup'>" + viewModel.ID.Value.ToString() + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='professional' /><FieldRef Name='Name='performanceevaluation' /><FieldRef Name='ppestatus' /><FieldRef Name='professional_x003a_Last_x0020_Na' /></ViewFields><QueryOptions /></View>";

            var PerformancePlanEvaluationDetails = new List<PerformanceEvaluationDetailVM>();
            string color;
            foreach (var item in SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl, caml))
            {
                if ((Convert.ToString(item["ppestatus"]) == "Approved") || (Convert.ToString(item["ppestatus"]) == "Rejected"))
                {
                    color = "green";
                }
                else if ((Convert.ToString(item["ppestatus"]) == "Initiated") || (Convert.ToString(item["ppestatus"]) == "Draft"))
                {
                    if (Now <= viewModel.LatestCreationDate)
                    {
                        color = "green";
                    }
                    else if ((Now > viewModel.LatestCreationDate) && (Now <= viewModel.LatestDateApproval1))
                    {
                        color = "yellow";
                    }
                    else
                    {
                        color = "red";
                    }
                }
                else if ((Convert.ToString(item["ppestatus"]) == "Pending Approval 1 of 2"))
                {
                    if (Now <= viewModel.LatestDateApproval2)
                    {
                        color = "green";
                    }
                    else
                    {
                        color = "red";
                    }
                }
                else if ((Convert.ToString(item["ppestatus"]) == "Pending Approval 2 of 2"))
                {
                    if (Now <= viewModel.LatestDateApproval2)
                    {
                        color = "green";
                    }
                    else
                    {
                        color = "red";
                    }
                }
                else
                {
                    color = "red";
                }

                PerformancePlanEvaluationDetails.Add(new PerformanceEvaluationDetailVM
                {
                    ID = Convert.ToInt32(item["ID"]),
                    EmployeeName = Convert.ToString((item["professional"] as FieldLookupValue).LookupValue) + " " + Convert.ToString((item["professional_x003a_Last_x0020_Na"] as FieldLookupValue).LookupValue),
                    EvaluationStatus = Convert.ToString(item["ppestatus"]),
                    EvaluationIndicator = color
                });
            }

            viewModel.PerformanceEvaluationDetails = PerformancePlanEvaluationDetails;
            return viewModel;
        }


    }
}

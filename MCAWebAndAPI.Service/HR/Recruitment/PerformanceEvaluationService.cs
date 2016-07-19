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
        const string SP_DETAIL_LIST_NAME = "Professional Performance Evaluation";

        public int CreatePerformanceEvaluation(PerformanceEvaluationVM PerformanceEvaluation)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("Title", PerformanceEvaluation.Period.Value);
            updatedValues.Add("latestdateforcreation", PerformanceEvaluation.LatestCreationDate.Value);
            updatedValues.Add("latestdateforapproval1", PerformanceEvaluation.LatestDateApproval1.Value);
            updatedValues.Add("latestdateforapproval2", PerformanceEvaluation.LatestDateApproval2.Value);
            updatedValues.Add("pestatus", "Open");
            //updatedValues.Add("initiationdate", DateTime.UtcNow);

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

            updatedValues.Add("pestatus", "Closed");
            updatedValues.Add("closingdate", DateTime.UtcNow.ToLocalTime());
            try
            {
                SPConnector.UpdateListItem(SP_LIST_NAME, PerformanceEvaluation.ID, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }


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

            return true;
        }

        public void CreatePerformanceEvaluationDetails(int? headerID, string emailMessage)
        {
            //Get All Active Professional
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Professional_x0020_Status' /><Value Type='Choice'>Active</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='officeemail' /><FieldRef Name='Position_x003a_ID' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", _siteUrl, caml);
            var updatedValues = new Dictionary<string, object>();
            string emailTo;
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
                    EmailUtil.Send(emailTo, "Notify to initiate Performance Plan", string.Format(emailMessage, UrlResource.ProfessionalPerformancePlan));

                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
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
            viewModel.LatestCreationDate = Convert.ToDateTime(listItem["latestdateforcreation"]).ToLocalTime();
            viewModel.LatestDateApproval1 = Convert.ToDateTime(listItem["latestdateforapproval1"]).ToLocalTime();
            viewModel.LatestDateApproval2 = Convert.ToDateTime(listItem["latestdateforapproval2"]).ToLocalTime();
            viewModel.Status = Convert.ToString(listItem["pestatus"]);
            viewModel.IntiationDate = Convert.ToDateTime(listItem["Created"]).ToLocalTime();

            DateTime Now = DateTime.UtcNow.ToLocalTime();

            //get Detail
            var caml = @"<View><Query><Where><Eq><FieldRef Name='performanceevaluation_x003a_ID' /><Value Type='Lookup'>" + viewModel.ID.Value.ToString() + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='professional' /><FieldRef Name='Name='performanceevaluation' /><FieldRef Name='ppestatus' /></ViewFields><QueryOptions /></View>";

            var PerformancePlanEvaluationDetails = new List<PerformanceEvaluationDetailVM>();
            string color;
            foreach (var item in SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl, caml))
            {
                if ((Convert.ToString(item["ppestatus"]) == "Approved") || (Convert.ToString(item["ppestatus"]) == "Rejected"))
                {
                    color = "green";
                }
                else if((Convert.ToString(item["ppestatus"]) == "Initiated") || (Convert.ToString(item["ppestatus"]) == "Draft"))
                {
                    if (Now<=viewModel.LatestCreationDate)
                    {
                        color = "green";
                    }
                    else if((Now>viewModel.LatestCreationDate) && (Now <= viewModel.LatestDateApproval1))
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
                    if (Now<=viewModel.LatestDateApproval2)
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
                    EmployeeName = Convert.ToString((item["professional"] as FieldLookupValue).LookupValue),
                    EvaluationStatus = Convert.ToString(item["ppestatus"]),
                    EvaluationIndicator = color
                });
            }

            viewModel.PerformanceEvaluationDetails = PerformancePlanEvaluationDetails;
            return viewModel;
        }


    }
}

using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class PerformanceMonitoringService : IPerformanceMonitoringService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_LIST_NAME = "Performance Plan";
        const string SP_DETAIL_LIST_NAME = "Professional Performance Plan";

        public int CreatePerformanceMonitoring(PerformanceMonitoringVM PerformanceMonitoring)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("Title", PerformanceMonitoring.Period.Value);
            updatedValues.Add("latestdateforcreation", PerformanceMonitoring.LatestCreationDate.Value);
            updatedValues.Add("latestdateforapproval1", PerformanceMonitoring.LatestDateApproval1.Value);
            updatedValues.Add("latestdateforapproval2", PerformanceMonitoring.LatestDateApproval2.Value);
            updatedValues.Add("ppstatus", "Open");
            updatedValues.Add("initiationdate", DateTime.UtcNow);

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

        public bool UpdatePerformanceMonitoring(PerformanceMonitoringVM PerformanceMonitoring)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("ppstatus", "Closed");
            updatedValues.Add("closingdate", DateTime.UtcNow.ToLocalTime());

            try
            {
                SPConnector.UpdateListItem(SP_LIST_NAME, PerformanceMonitoring.ID, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            

            var caml = @"<View><Query><Where><Eq><FieldRef Name='performanceplan_x003a_ID' /><Value Type='Lookup'>"+ PerformanceMonitoring.ID + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl, caml);
            foreach (var item in listItem)
            {
                updatedValues = new Dictionary<string, object>();
                updatedValues.Add("ppstatus", "Closed");
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

        public void CreatePerformanceMonitoringDetails(int? headerID, string emailMessage)
        {
            //Get All Active Professional
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Professional_x0020_Status' /><Value Type='Choice'>Active</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='officeemail' /><FieldRef Name='Position_x003a_ID' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", _siteUrl, caml);
            var updatedValues = new Dictionary<string, object>();
            string emailTo;
            foreach (var item in listItem)
            {
                updatedValues = new Dictionary<string, object>();
                updatedValues.Add("performanceplan", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("pppstatus", "Initiated");
                updatedValues.Add("ppstatus", "Open");
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

        public PerformanceMonitoringVM GetPerformanceMonitoring(int? ID)
        {
            var viewModel = new PerformanceMonitoringVM();
            if (ID == null)
            {
                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToPerformanceMonitoringVM(listItem);

            return viewModel;
        }

        private PerformanceMonitoringVM ConvertToPerformanceMonitoringVM(ListItem listItem)
        {
            var viewModel = new PerformanceMonitoringVM();

            DateTime Now = DateTime.UtcNow.ToLocalTime();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Period.Value = Convert.ToString(listItem["Title"]);
            viewModel.LatestCreationDate = Convert.ToDateTime(listItem["latestdateforcreation"]).ToLocalTime();
            viewModel.LatestDateApproval1 = Convert.ToDateTime(listItem["latestdateforapproval1"]).ToLocalTime();
            viewModel.LatestDateApproval2 = Convert.ToDateTime(listItem["latestdateforapproval2"]).ToLocalTime();
            viewModel.Status = Convert.ToString(listItem["ppstatus"]);
            viewModel.IntiationDate = Convert.ToDateTime(listItem["Created"]).ToLocalTime();


            //get Detail
            var caml = @"<View><Query><Where><Eq><FieldRef Name='performanceplan_x003a_ID' /><Value Type='Lookup'>" + viewModel.ID.Value.ToString() + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='professional' /><FieldRef Name='performanceplan' /><FieldRef Name='pppstatus' /></ViewFields><QueryOptions /></View>";

            var PerformancePlanMonitoringDetails = new List<PerformanceMonitoringDetailVM>();
            string color;
            foreach (var item in SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl, caml))
            {
                if ((Convert.ToString(item["pppstatus"]) == "Approved"))
                {
                    color = "green";
                }
                else if ((Convert.ToString(item["pppstatus"]) == "Initiated") || (Convert.ToString(item["pppstatus"]) == "Draft"))
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
                else if ((Convert.ToString(item["pppstatus"]) == "Pending Approval 1 of 2"))
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
                else if ((Convert.ToString(item["pppstatus"]) == "Pending Approval 2 of 2"))
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

                //if ((Convert.ToString(item["pppstatus"]) != "Approved") && (viewModel.LatestDateApproval2.Value >= Now))
                //{
                //    color = "yellow";
                //}
                //else if ((Convert.ToString(item["pppstatus"]) == "Approved"))
                //{
                //    color = "green";
                //}
                //else
                //{
                //    color = "red";
                //}
                PerformancePlanMonitoringDetails.Add(new PerformanceMonitoringDetailVM
                {
                    ID = Convert.ToInt32(item["ID"]),
                    EmployeeName = Convert.ToString((item["professional"] as FieldLookupValue).LookupValue),
                    PlanStatus = Convert.ToString(item["pppstatus"]),
                    PlanIndicator = color
                });
            }

            viewModel.PerformanceMonitoringDetails = PerformancePlanMonitoringDetails;
            return viewModel;
        }


    }
}

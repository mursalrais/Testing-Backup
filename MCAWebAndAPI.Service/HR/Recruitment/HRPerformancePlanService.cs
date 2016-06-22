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

        public int CreateHeader(ProfessionalPerformancePlanVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("majorstrength", header.MajorStrength);
            columnValues.Add("performancearea", header.PerformanceArea);
            columnValues.Add("recommendedactivities", header.RecommendedActivities);
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
            viewModel.PerformancePeriod = FormatUtil.ConvertLookupToValue(listItem, "performanceplan");
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
                Category = IndividualGoalDetailVM.GetCategoryDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["individualgoalcategory"])
                    }),
                IndividualGoalAndPlan = Convert.ToString(item["individualgoalplan"]),
                Weight = Convert.ToInt32(item["individualgoalweight"]),
                Remarks = Convert.ToString(item["individualgoalremarks"]),
            };
        }

        public ProfessionalPerformancePlanVM GetPopulatedModel(int? id = null)
        {
            var model = new ProfessionalPerformancePlanVM();
            return model;
        }

        public bool UpdateHeader(ProfessionalPerformancePlanVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;
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
    }
}

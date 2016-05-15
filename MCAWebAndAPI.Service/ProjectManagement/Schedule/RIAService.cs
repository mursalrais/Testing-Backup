using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Model.ViewModel.Chart;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class RIAService : IRIAService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string RISK_SP_LIST_NAME = "Risks";
        const string ISSUE_SP_LIST_NAME = "Issues";
        const string ACTION_SP_LIST_NAME = "Actions";

        const string OPEN_STATUS = "Open";
        const string PENDING_STATUS = "Pending";
        const string SOLVED_STATUS = "Solved";

        //const string HIGH_PRIORITY = "High";
        //const string NORMAL_PRIORITY = "Normal";
        //const string LOW_PRIORITY = "Low";

        const string UNASSIGNED = "Unassigned";

        string _siteUrl = null;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }


        private RIABase ConvertToModel(ListItem item, string listName)
        {
            var result = new RIABase()
            {
                Id = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                Status = Convert.ToString(item["Status"]),
                AssignedTo = (FieldUserValue)item["AssignedTo"] == null ? UNASSIGNED : 
                    Convert.ToString(((FieldUserValue)item["AssignedTo"]).LookupValue),
                //Priority = Convert.ToString(item["Priority"])
            };

            if (string.IsNullOrEmpty(result.AssignedTo))
                result.AssignedTo = UNASSIGNED;

            // In case there is a need to add child-specific columns 
            return result;
        }

        public IEnumerable<Model.ProjectManagement.Schedule.Action> GetAllAction()
        {
            var result = new List<Model.ProjectManagement.Schedule.Action>();

            foreach (var item in SPConnector.GetList(ACTION_SP_LIST_NAME, _siteUrl))
            {
                result.Add(ConvertToModel(item, ACTION_SP_LIST_NAME) as Model.ProjectManagement.Schedule.Action);
            }

            return result;
        }


        public IEnumerable<Issue> GetAllIssues()
        {
            var result = new List<Model.ProjectManagement.Schedule.Issue>();

            foreach (var item in SPConnector.GetList(ISSUE_SP_LIST_NAME, _siteUrl))
            {
                result.Add(ConvertToModel(item, ISSUE_SP_LIST_NAME) as Issue);
            }

            return result;
        }

        public IEnumerable<Risk> GetAllRisks()
        {
            var result = new List<Model.ProjectManagement.Schedule.Risk>();

            foreach (var item in SPConnector.GetList(RISK_SP_LIST_NAME, _siteUrl))
            {
                result.Add(ConvertToModel(item, RISK_SP_LIST_NAME) as Model.ProjectManagement.Schedule.Risk);
            }

            return result;
        }
        
        public IEnumerable<RIABase> GetRIAListItems(string listName)
        {
            var result = new List<RIABase>();

            foreach (var item in SPConnector.GetList(listName, _siteUrl))
            {
                result.Add(ConvertToModel(item, listName));
            }

            return result;
        }

        IEnumerable<StackedBarChartVM> IRIAService.GetOverallRIAChart()
        {
            var actions = GetRIAListItems(ACTION_SP_LIST_NAME);
            var issues = GetRIAListItems(ISSUE_SP_LIST_NAME);
            var risks = GetRIAListItems(RISK_SP_LIST_NAME);

            var viewModel = new List<StackedBarChartVM>();
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = RISK_SP_LIST_NAME,
                GroupName = SOLVED_STATUS,
                Value = risks.Count(e => string.Compare(e.Status, SOLVED_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.GREEN
            });
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = RISK_SP_LIST_NAME,
                GroupName = PENDING_STATUS,
                Value = risks.Count(e => string.Compare(e.Status, PENDING_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.YELLOW
            });

            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ISSUE_SP_LIST_NAME,
                GroupName = SOLVED_STATUS,
                Value = issues.Count(e => string.Compare(e.Status, SOLVED_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.GREEN
            });
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ISSUE_SP_LIST_NAME,
                GroupName = PENDING_STATUS,
                Value = issues.Count(e => string.Compare(e.Status, PENDING_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.YELLOW
            });

            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ACTION_SP_LIST_NAME,
                GroupName = SOLVED_STATUS,
                Value = actions.Count(e => string.Compare(e.Status, SOLVED_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.GREEN
            });
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ACTION_SP_LIST_NAME,
                GroupName = PENDING_STATUS,
                Value = actions.Count(e => string.Compare(e.Status, PENDING_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.YELLOW
            });

            return viewModel;
        }

        public IEnumerable<StackedBarChartVM> GetRIAResourceChart(string riaType)
        {
            var items = new List<RIABase>();

            foreach (var item in SPConnector.GetList(riaType, _siteUrl))
            {
                items.Add(ConvertToModel(item, riaType));
            }

            return items.Select(e => new StackedBarChartVM
            {
                CategoryName = e.AssignedTo,
                GroupName = e.Status,
                Color = e.Status == PENDING_STATUS ? GraphicUtil.YELLOW : GraphicUtil.GREEN, 
                Value = 1
            });
        }

        public IEnumerable<DonutsChartVM> GetRIAStatusChart(string riaType)
        {
            var items = new List<RIABase>();

            foreach (var item in SPConnector.GetList(riaType, _siteUrl))
            {
                items.Add(ConvertToModel(item, riaType));
            }

            var results = new List<DonutsChartVM>();

            results.Add(new DonutsChartVM()
            {
                Label = SOLVED_STATUS,
                Value = items.Count(e => string.Compare(e.Status, SOLVED_STATUS, StringComparison.OrdinalIgnoreCase) == 0), 
                Color = GraphicUtil.GREEN
            });
            results.Add(new DonutsChartVM()
            {
                Label = PENDING_STATUS,
                Value = items.Count(e => string.Compare(e.Status, PENDING_STATUS, StringComparison.OrdinalIgnoreCase) == 0), 
                Color = GraphicUtil.YELLOW
            });

            return results;
        }

        //public IEnumerable<DonutsChartVM> GetRIAPriorityChart(string riaType)
        //{
        //    var items = new List<RIABase>();

        //    foreach (var item in SPConnector.GetList(riaType, _siteUrl))
        //    {
        //        items.Add(ConvertToModel(item, riaType));
        //    }

        //    var results = new List<DonutsChartVM>();

        //    results.Add(new DonutsChartVM()
        //    {
        //        Label = HIGH_PRIORITY,
        //        Value = items.Count(e => string.Compare(e.Priority, HIGH_PRIORITY, StringComparison.OrdinalIgnoreCase) == 0),
        //        Color = GraphicUtil.RED
        //    });
        //    results.Add(new DonutsChartVM()
        //    {
        //        Label = NORMAL_PRIORITY,
        //        Value = items.Count(e => string.Compare(e.Priority, NORMAL_PRIORITY, StringComparison.OrdinalIgnoreCase) == 0),
        //        Color = GraphicUtil.YELLOW
        //    });
        //    results.Add(new DonutsChartVM()
        //    {
        //        Label = LOW_PRIORITY,
        //        Value = items.Count(e => string.Compare(e.Status, LOW_PRIORITY, StringComparison.OrdinalIgnoreCase) == 0),
        //        Color = GraphicUtil.GREEN
        //    });

        //    return results;
        //}

        public IEnumerable<BarChartVM> GetIssuesAgeingChart()
        {
            var issuesAgeing = new List<BarChartVM>();
            return issuesAgeing;
        }

    }
}

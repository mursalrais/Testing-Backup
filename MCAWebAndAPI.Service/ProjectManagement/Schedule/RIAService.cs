using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Model.ViewModel.Chart;
using NLog;
using MCAWebAndAPI.Service.SPUtil;
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
        const string CLOSED_STATUS = "Closed";

        const string HIGH_PRIORITY = "High";
        const string NORMAL_PRIORITY = "Normal";
        const string LOW_PRIORITY = "Low";

        string _siteUrl = null;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }


        private RIABase ConvertToModel(ListItem item, string listName)
        {
            var result = new RIABase()
            {
                Id = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                Status = Convert.ToString(item["Status"]),
                AssignedTo = Convert.ToString(item["AssignedTo"]),
                Priority = Convert.ToString(item["Priority"])
            };

            // In case there is a need to add child-specific columns 
            switch (listName)
            {
                case ACTION_SP_LIST_NAME:
                    return result as Model.ProjectManagement.Schedule.Action;
                case RISK_SP_LIST_NAME:
                    return result as Risk;
                case ISSUE_SP_LIST_NAME:
                    return result as Issue;
                default: return null;
            }
        }

        public IEnumerable<Model.ProjectManagement.Schedule.Action> GetAllAction()
        {
            var result = new List<Model.ProjectManagement.Schedule.Action>();

            foreach (var item in SPConnector.GetList(ACTION_SP_LIST_NAME))
            {
                result.Add(ConvertToModel(item, ACTION_SP_LIST_NAME) as Model.ProjectManagement.Schedule.Action);
            }

            return result;
        }


        public IEnumerable<Issue> GetAllIssues()
        {
            var result = new List<Model.ProjectManagement.Schedule.Issue>();

            foreach (var item in SPConnector.GetList(ISSUE_SP_LIST_NAME))
            {
                result.Add(ConvertToModel(item, ISSUE_SP_LIST_NAME) as Issue);
            }

            return result;
        }

        public IEnumerable<Risk> GetAllRisks()
        {
            var result = new List<Model.ProjectManagement.Schedule.Risk>();

            foreach (var item in SPConnector.GetList(RISK_SP_LIST_NAME))
            {
                result.Add(ConvertToModel(item, RISK_SP_LIST_NAME) as Model.ProjectManagement.Schedule.Risk);
            }

            return result;
        }

        IEnumerable<StackedBarChartVM> IRIAService.GetOverallRIAChart()
        {
            var actions = GetAllAction();
            var issues = GetAllIssues();
            var risks = GetAllRisks();

            var viewModel = new List<StackedBarChartVM>();
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = RISK_SP_LIST_NAME,
                GroupName = CLOSED_STATUS,
                Value = risks.Count(e => string.Compare(e.Status, CLOSED_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
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
                CategoryName = RISK_SP_LIST_NAME,
                GroupName = OPEN_STATUS,
                Value = risks.Count(e => string.Compare(e.Status, OPEN_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.RED
            });

            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ISSUE_SP_LIST_NAME,
                GroupName = CLOSED_STATUS,
                Value = issues.Count(e => string.Compare(e.Status, CLOSED_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
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
                CategoryName = ISSUE_SP_LIST_NAME,
                GroupName = OPEN_STATUS,
                Value = issues.Count(e => string.Compare(e.Status, OPEN_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.RED
            });


            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ACTION_SP_LIST_NAME,
                GroupName = CLOSED_STATUS,
                Value = actions.Count(e => string.Compare(e.Status, CLOSED_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.GREEN
            });
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ACTION_SP_LIST_NAME,
                GroupName = PENDING_STATUS,
                Value = actions.Count(e => string.Compare(e.Status, PENDING_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.YELLOW
            });
            viewModel.Add(new StackedBarChartVM
            {
                CategoryName = ACTION_SP_LIST_NAME,
                GroupName = OPEN_STATUS,
                Value = actions.Count(e => string.Compare(e.Status, OPEN_STATUS, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.RED
            });

            return viewModel;
        }

        public IEnumerable<StackedBarChartVM> GetRIAResourceChart(string riaType)
        {
            var items = new List<RIABase>();

            foreach (var item in SPConnector.GetList(riaType))
            {
                items.Add(ConvertToModel(item, riaType));
            }

            return items.Select(e => new StackedBarChartVM
            {
                CategoryName = e.AssignedTo,
                GroupName = e.Status,
                Color = e.Status == OPEN_STATUS ? GraphicUtil.RED : (e.Status == PENDING_STATUS ? GraphicUtil.YELLOW : GraphicUtil.GREEN), 
                Value = 1
            });
        }

        public IEnumerable<DonutsChartVM> GetRIAStatusChart(string riaType)
        {
            var items = new List<RIABase>();

            foreach (var item in SPConnector.GetList(riaType))
            {
                items.Add(ConvertToModel(item, riaType));
            }

            var results = new List<DonutsChartVM>();

            results.Add(new DonutsChartVM()
            {
                Label = OPEN_STATUS,
                Value = items.Count(e => string.Compare(e.Status, OPEN_STATUS, StringComparison.OrdinalIgnoreCase) == 0), 
                Color = GraphicUtil.RED
            });
            results.Add(new DonutsChartVM()
            {
                Label = PENDING_STATUS,
                Value = items.Count(e => string.Compare(e.Status, PENDING_STATUS, StringComparison.OrdinalIgnoreCase) == 0), 
                Color = GraphicUtil.YELLOW
            });
            results.Add(new DonutsChartVM()
            {
                Label = CLOSED_STATUS,
                Value = items.Count(e => string.Compare(e.Status, CLOSED_STATUS, StringComparison.OrdinalIgnoreCase) == 0), 
                Color = GraphicUtil.GREEN
            });

            return results;
        }

        public IEnumerable<DonutsChartVM> GetRIAPriorityChart(string riaType)
        {
            var items = new List<RIABase>();

            foreach (var item in SPConnector.GetList(riaType))
            {
                items.Add(ConvertToModel(item, riaType));
            }

            var results = new List<DonutsChartVM>();

            results.Add(new DonutsChartVM()
            {
                Label = HIGH_PRIORITY,
                Value = items.Count(e => string.Compare(e.Priority, HIGH_PRIORITY, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.RED
            });
            results.Add(new DonutsChartVM()
            {
                Label = NORMAL_PRIORITY,
                Value = items.Count(e => string.Compare(e.Priority, NORMAL_PRIORITY, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.YELLOW
            });
            results.Add(new DonutsChartVM()
            {
                Label = LOW_PRIORITY,
                Value = items.Count(e => string.Compare(e.Status, LOW_PRIORITY, StringComparison.OrdinalIgnoreCase) == 0),
                Color = GraphicUtil.GREEN
            });

            return results;
        }
    }
}

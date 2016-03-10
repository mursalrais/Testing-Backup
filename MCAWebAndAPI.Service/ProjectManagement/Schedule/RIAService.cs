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
        string _siteUrl = null;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }


        private RIABase ConvertToModel(ListItem item, string listName)
        {
            switch (listName)
            {
                case ACTION_SP_LIST_NAME:
                    return new Model.ProjectManagement.Schedule.Action()
                    {
                        Id = Convert.ToInt32(item["ID"]),
                        Title = Convert.ToString(item["Title"]),
                        Status = Convert.ToString(item["Status"])
                    };
                case RISK_SP_LIST_NAME:
                    return new Risk()
                    {
                        Id = Convert.ToInt32(item["ID"]),
                        Title = Convert.ToString(item["Title"]),
                        Status = Convert.ToString(item["Status"])
                    };
                case ISSUE_SP_LIST_NAME:
                    return new Issue()
                    {
                        Id = Convert.ToInt32(item["ID"]),
                        Title = Convert.ToString(item["Title"]),
                        Status = Convert.ToString(item["Status"])
                    };
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
                result.Add(ConvertToModel(item, ISSUE_SP_LIST_NAME) as Model.ProjectManagement.Schedule.Issue);
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

        public IEnumerable<OverallRIAChartVM> GetOverallRIAChart()
        {
            var actions = GetAllAction();
            var issues = GetAllIssues();
            var risks = GetAllRisks();

            var viewModel = new List<OverallRIAChartVM>();
            viewModel.Add(new OverallRIAChartVM {
                Name = "Closed",
                Data = { risks.Count(e => string.Compare(e.Status, "Closed", StringComparison.OrdinalIgnoreCase) == 0),
                    actions.Count(e => string.Compare(e.Status, "Closed", StringComparison.OrdinalIgnoreCase) == 0),
                    issues.Count(e => string.Compare(e.Status, "Closed", StringComparison.OrdinalIgnoreCase) == 0) },
                Color = "#069F16"
            });
            viewModel.Add(new OverallRIAChartVM
            {
                Name = "Pending",
                Data = { risks.Count(e => string.Compare(e.Status, "Pending", StringComparison.OrdinalIgnoreCase) == 0),
                    actions.Count(e => string.Compare(e.Status, "Pending", StringComparison.OrdinalIgnoreCase) == 0),
                    issues.Count(e => string.Compare(e.Status, "Pending", StringComparison.OrdinalIgnoreCase) == 0) },
                Color = "#FFFF25"
            });
            viewModel.Add(new OverallRIAChartVM
            {
                Name = "Open",
                Data = { risks.Count(e => string.Compare(e.Status, "Open", StringComparison.OrdinalIgnoreCase) == 0),
                       actions.Count(e => string.Compare(e.Status, "Open", StringComparison.OrdinalIgnoreCase) == 0),
                       issues.Count(e => string.Compare(e.Status, "Open", StringComparison.OrdinalIgnoreCase) == 0) },
                Color = "#FF0000"
            });

            return viewModel;
        }
    }
}

using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Web.Helpers
{
    public class WorkflowHelper
    {
        static IWorkflowService _service = new WorkflowService();

        public async static Task CreateTransactionWorkflowAsync(string workflowTransactionListName, string transactionLookupColumnName,
            int headerID)
        {
            CreateTransactionWorkflow(workflowTransactionListName, transactionLookupColumnName, headerID);
        }

        public static void CreateTransactionWorkflow(string workflowTransactionListName, string transactionLookupColumnName,
            int headerID)
        {
            var workflowItems = SessionManager.Get<IEnumerable<WorkflowItemVM>>("WorkflowItems");
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var userLogin = SessionManager.Get<string>("UserLogin");

            _service.SetSiteUrl(siteUrl);
            _service.CreateTransactionWorkflow(workflowTransactionListName, transactionLookupColumnName, headerID, workflowItems, userLogin);
        }

        public async static Task SendApprovalRequestAsync(string workflowTransactionListName, string transactionLookupColumnName,
            int headerID, int level, string message)
        {
            SendApprovalRequest(workflowTransactionListName, transactionLookupColumnName, headerID, level, message);
        }

        public static void SendApprovalRequest(string workflowTransactionListName, string transactionLookupColumnName,
            int headerID, int level, string message)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            _service.SendApprovalRequest(workflowTransactionListName, transactionLookupColumnName, headerID, level, message);
        }
    }
}
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
        private static IWorkflowService _service;
        public WorkflowHelper()
        {
            _service = new WorkflowService();
        }

        public async static Task CreateTransactionWorkflowAsync(string workflowTransactionListName, string transactionLookupColumnName,
            int headerID)
        {
            CreateTransactionWorkflow(workflowTransactionListName, transactionLookupColumnName, headerID);
        }

        public static void CreateTransactionWorkflow(string workflowTransactionListName, string transactionLookupColumnName,
            int headerID)
        {
            var workflowItems = SessionManager.Get<IEnumerable<WorkflowItemVM>>("");

            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            _service.CreateTransactionWorkflow(workflowTransactionListName, transactionLookupColumnName, headerID, workflowItems);
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
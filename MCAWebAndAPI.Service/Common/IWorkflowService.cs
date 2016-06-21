using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.Common
{
    public interface IWorkflowService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<PositionMaster> GetPositionsInWorkflow(string listName, string requestorPosition, 
            string requestorUnit);

        IEnumerable<ProfessionalMaster> GetApproverNames(string position);

        WorkflowRouterVM GetWorkflowRouter(string listName, string requestor);

        void CreateTransactionWorkflow(string workflowTransactionListName, string transactionLookupColumnName, 
            int headerID, IEnumerable<WorkflowItemVM> workflowItems);

        void SendApprovalRequest(string workflowTransactionListName, string transactionLookupColumnName, int headerID, int level, string message);
    }
}

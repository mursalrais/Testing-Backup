using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Common
{
    public interface IWorkflowService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<PositionMaster> GetPositionsInWorkflow(string listName,
            string approverUnit,
            string requestorPosition, 
            string requestorUnit);

        IEnumerable<ProfessionalMaster> GetApproverNames(string unit, string position);

        Task<IEnumerable<PendingApprovalItemVM>> GetPendingApprovalItemsAsync(string userLogin);

        Task<WorkflowRouterVM> GetWorkflowRouter(string listName, string requestor);

        void CreateTransactionWorkflow(string workflowTransactionListName, string transactionLookupColumnName, 
            int headerID, IEnumerable<WorkflowItemVM> workflowItems, string requestor = null);

        void CreateExitProcedureChecklistWorkflow(string workflowTransactionListName, string transactionLookupColumnName,
            int exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist, string requestor = null);

        void SendApprovalRequest(string workflowTransactionListName, string transactionLookupColumnName, 
            int headerID, int level, string message);

        string GetPositionName(int position);

        string GetUnitName(int unit);
    }
}

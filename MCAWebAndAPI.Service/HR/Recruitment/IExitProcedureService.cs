using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Web;
using Microsoft.SharePoint.Client;


namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IExitProcedureService
    {

        void SetSiteUrl(string siteUrl);

        //Display Exit Procedure Data based on ID
        ExitProcedureVM GetExitProcedure(int? ID, string siteUrl, string requestor, string listName, string user);

        ExitProcedureVM GetExitProcedure(int? ID);

        ExitProcedureForApproverVM GetExitProcedureApprover(int? ID, string siteUrl, string requestor, int? level);

        int CreateExitProcedure(ExitProcedureVM exitProcedure);

        bool UpdateExitProcedure(ExitProcedureVM exitProcedure);

        bool UpdateExitChecklistStatus(ExitProcedureForApproverVM exitProcedureForApprover);

        ExitProcedureVM ViewExitProcedure(int? ID);

        void CreateExitProcedureDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, ExitProcedureVM exitProcedure);

        //Task<ExitProcedureVM> GetWorkflowRouterExitProcedure(string listName, string requestor);

        ExitProcedureVM GetWorkflowExitProcedure(string listName, string requestor, string user);

        IEnumerable<PositionMaster> GetPositionsInWorkflow(string listName,
            string approverUnit,
            string requestorPosition,
            string requestorUnit);

        string GetPositionName(int position);

        IEnumerable<ProfessionalMaster> GetApproverNames(string position);

        Task CreateExitProcedureChecklistAsync(int? exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist, string requestorposition, string requestorunit);

        void SendEmail(ExitProcedureVM header, string workflowTransactionListName, string transactionLookupColumnName,
            int exitProcID, string messageForApprover, string messageForRequestor);

        void SendMailDocument(string requestorMail, string documentExitProcedure);

        ExitProcedureVM GetExitProcedureForApprove(int? ID, string siteUrl, string approver);
    }
}

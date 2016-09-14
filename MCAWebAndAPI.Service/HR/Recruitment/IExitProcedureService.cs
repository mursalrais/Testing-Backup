using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Web;
using System;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IExitProcedureService
    {

        void SetSiteUrl(string siteUrl);

        //Display Exit Procedure Data based on ID
        ExitProcedureVM GetExitProcedure(int? ID, string siteUrl, string requestor, string listName, string user);

        ExitProcedureVM GetExitProcedureHR(int? ID, string siteUrl);

        ExitProcedureVM GetExitProcedure(int? ID);

        ExitProcedureVM GetExitChecklistForHR(int? ID, string siteUrl, string professionalMail, string listName);

        ExitProcedureForApproverVM GetExitProcedureApprover(int? ID, string siteUrl, string requestor, int? level);

        int CreateExitProcedure(ExitProcedureVM exitProcedure);

        bool UpdateExitProcedureHR(ExitProcedureVM exitProcedure);

        bool UpdateExitProcedure(ExitProcedureVM exitProcedure);

        bool UpdateExitChecklist(ExitProcedureVM exitProcedure, IEnumerable<ExitProcedureChecklistVM> ExitProcedureChecklist);

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

        Task CreateExitProcedureChecklistAsync(ExitProcedureVM exitProcedure, int? exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist, string requestorposition, string requestorunit, int? positionID);

        void SendEmail(ExitProcedureVM header, string workflowTransactionListName, string transactionLookupColumnName,
            int exitProcID, string _siteUrl, string urlResource, string requestorMail, string messageForRequestor);

        void SendMailDocument(string requestorMail, string documentExitProcedure);

        ExitProcedureVM GetExitProcedureForApprove(int? ID, string siteUrl, string approver);

        bool CheckPendingApproval(int? id, string checklistStatusApproved);

        bool UpdateExitProcedureStatus(int? id, string statusExitProcedure);

        string GetPSANumberOnExitProcedure(int? id);

        int GetPSAId(string psaNumber);

        System.DateTime GetLastWorkingDate(int? exitProcID);

        bool UpdateLastWorkingDateOnPSA(int? psaID, System.DateTime lastWorkingDate);

        int GetPositionID(string requestorposition, string requestorunit, int positionID, int number);

        string GetExitProcedureStatus(int? exitProcID);

        string GetProjectUnit(string requestor);

        bool UpdateLastWorkingDateOnProfessional(int? professionalID, System.DateTime lastWorkingDate);

        string GetProfessionalData(int? professionalID);

        int GetProfessionalIDNumber(string professionalName, string projectUnit, string positionName);

        string GetProfessionalName(int? exitProcID);

        string GetUnitBasedExitID(int? exitProcID);

        string GetPositionBasedExitID(int? exitProcID);
    }
}

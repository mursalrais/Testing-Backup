using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IExitProcedureService
    {

        void SetSiteUrl(string siteUrl);

        //Display Exit Procedure Data based on ID
        ExitProcedureVM GetExitProcedure(int? ID, string siteUrl, string requestor, string listName);

        ExitProcedureVM GetExitProcedure(int? ID);

        int CreateExitProcedure(ExitProcedureVM exitProcedure);

        bool UpdateExitProcedure(ExitProcedureVM exitProcedure);

        ExitProcedureVM ViewExitProcedure(int? ID);

        void CreateExitProcedureDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, ExitProcedureVM exitProcedure);

        //Task<ExitProcedureVM> GetWorkflowRouterExitProcedure(string listName, string requestor);

        ExitProcedureVM GetWorkflowExitProcedure(string listName, string requestor);

        IEnumerable<PositionMaster> GetPositionsInWorkflow(string listName,
            string approverUnit,
            string requestorPosition,
            string requestorUnit);

        string GetPositionName(int position);

        IEnumerable<ProfessionalMaster> GetApproverNames(string position);

        Task CreateExitProcedureChecklistAsync(int? exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist);
    }
}

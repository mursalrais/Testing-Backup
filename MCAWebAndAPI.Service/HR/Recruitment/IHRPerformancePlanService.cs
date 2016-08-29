using System;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Common;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRPerformancePlanService
    {
        void SetSiteUrl(string siteUrl = null);

        Task<ProfessionalPerformancePlanVM> GetPopulatedModel(string requestor, string listName);

        Task<ProfessionalPerformancePlanVM> GetHeader(int? ID, string requestor, string listName, string listNameWorkflow, string columnName);

        int CreateHeader(string requestor, ProfessionalPerformancePlanVM header);

        bool UpdateHeader(ProfessionalPerformancePlanVM header);

        void CreatePerformancePlanDetails(int? headerID, int? performanceID, string email, string status, string type, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails);

        Task CreatePerformancePlanDetailsAsync(int? headerID, int? performanceID, string email, string status, string type, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails);

        void SendEmail(ProfessionalPerformancePlanVM header, string workflowTransactionListName, string transactionLookupColumnName,
            int headerID, int level, string messageForApprover, string messageForRequestor);
    }
}

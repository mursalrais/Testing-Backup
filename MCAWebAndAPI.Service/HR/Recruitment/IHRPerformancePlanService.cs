using System;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRPerformancePlanService
    {
        void SetSiteUrl(string siteUrl = null);

        ProfessionalPerformancePlanVM GetPopulatedModel(string requestor = null);

        ProfessionalPerformancePlanVM GetHeader(int? ID);

        int CreateHeader(ProfessionalPerformancePlanVM header);

        bool UpdateHeader(ProfessionalPerformancePlanVM header);

        void CreatePerformancePlanDetails(int? headerID, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails);

        Task CreatePerformancePlanDetailsAsync(int? headerID, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails);

        void SendEmail(ProfessionalPerformancePlanVM header, string workflowTransactionListName, string transactionLookupColumnName,
            int headerID, int level, string messageForApprover, string messageForRequestor);
    }
}

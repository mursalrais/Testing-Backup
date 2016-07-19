using System;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRProfessionalPerformanceEvaluationService
    {
        void SetSiteUrl(string siteUrl = null);

        ProfessionalPerformanceEvaluationVM GetPopulatedModel(string requestor = null);

        ProfessionalPerformanceEvaluationVM GetHeader(int? ID);

        int CreateHeader(ProfessionalPerformanceEvaluationVM header);

        bool UpdateHeader(ProfessionalPerformanceEvaluationVM header);

        void CreatePerformanceEvaluationDetails(int? headerID, IEnumerable<ProfessionalPerformanceEvaluationDetailVM> performanceEvaluationDetails);

        Task CreatePerformanceEvaluationDetailsAsync(int? headerID, IEnumerable<ProfessionalPerformanceEvaluationDetailVM> performanceEvaluationDetails);

        void SendEmail(ProfessionalPerformanceEvaluationVM header, string workflowTransactionListName, string transactionLookupColumnName,
            int headerID, int level, string messageForApprover, string messageForRequestor);
    }
}

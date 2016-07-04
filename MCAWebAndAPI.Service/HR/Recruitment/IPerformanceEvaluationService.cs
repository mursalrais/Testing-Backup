using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IPerformanceEvaluationService
    {
        int CreatePerformanceEvaluation(PerformanceEvaluationVM PerformanceEvaluation);
        bool UpdatePerformanceEvaluation(PerformanceEvaluationVM viewModel);
        void CreatePerformanceEvaluationDetails(int? headerID, string emailMessage);
        void SetSiteUrl(string siteUrl);
        PerformanceEvaluationVM GetPerformanceEvaluation(int? ID);
    }
}
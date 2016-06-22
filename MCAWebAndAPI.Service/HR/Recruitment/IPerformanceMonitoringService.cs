using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IPerformanceMonitoringService
    {
        int CreatePerformanceMonitoring(PerformanceMonitoringVM PerformanceMonitoring);
        bool UpdatePerformanceMonitoring(PerformanceMonitoringVM viewModel);
        void CreatePerformanceMonitoringDetails(int? headerID);
        void SetSiteUrl(string siteUrl);
        PerformanceMonitoringVM GetPerformanceMonitoring(int? ID);
    }
}
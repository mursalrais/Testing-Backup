using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IPerformanceMonitoringService
    {
        int CreatePerformanceMonitoring(PerformanceMonitoringVM PerformanceMonitoring);
        bool UpdatePerformanceMonitoring(PerformanceMonitoringVM viewModel);
        void CreatePerformanceMonitoringDetails(int? headerID, string emailMessage);
        void SetSiteUrl(string siteUrl);
        PerformanceMonitoringVM GetPerformanceMonitoring(int? ID);
    }
}
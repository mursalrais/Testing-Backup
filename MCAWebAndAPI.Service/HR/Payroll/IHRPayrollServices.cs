using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public interface IHRPayrollServices
    {
        void SetSiteUrl(string siteUrl);

        MonthlyFeeVM GetPopulatedModel(int? id = null);

        MonthlyFeeVM GetHeader(int? ID);

        int CreateHeader(MonthlyFeeVM header);

        bool UpdateHeader(MonthlyFeeVM header);

        void CreateMonthlyFeeDetails(int? headerID, IEnumerable<MonthlyFeeDetailVM> monthlyFeeDetails);

        IEnumerable<PayrollDetailVM> GetPayrollDetails(DateTime period);

        IEnumerable<PayrollWorksheetDetailVM> GetPayrollWorksheetDetails(DateTime? period, bool isSummary = false);
    }
}

using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MCAWebAndAPI.Service.HR.Payroll
{
    public interface IHRPayrollServices
    {
        void SetSiteUrl(string siteUrl);

        MonthlyFeeVM GetPopulatedModel(int? id = null);

        MonthlyFeeVM GetHeader();

        MonthlyFeeVM GetHeader(int ID);

        int CreateHeader(MonthlyFeeVM header);

        bool UpdateHeader(MonthlyFeeVM header);
    }
}

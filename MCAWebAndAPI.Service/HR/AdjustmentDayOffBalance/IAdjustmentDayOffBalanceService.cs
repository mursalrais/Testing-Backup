using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.HR.AdjustmentDayOffBalance
{
    public interface IAdjustmentDayOffBalanceService
    {
        void SetSiteUrl(string siteUrl);

        string GetProfessionalName(int? professionalID);

        AdjustmentDayOffBalanceVM GetAdjustmentDayOffBalance(int? iD);

        int GetLastBalanceCompensatory(int? professionalID, string dayOffType);

        int GetLastBalanceAnnualDayOff(int? professionalID, string dayOffType);

        int GetLastBalanceSpecialDayOff(int? professionalID, string dayOffType);

        int GetLastBalancePaternity(int? professionalID, string dayOffType);

        int CreateAdjustmentDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance);

        bool UpdateAdjustmentDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance);

        bool UpdateAnnualDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance);

        bool UpdateSpecialDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance);

        bool UpdatePaternityBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance);

        bool UpdateCompensatoryBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance);


    }
}

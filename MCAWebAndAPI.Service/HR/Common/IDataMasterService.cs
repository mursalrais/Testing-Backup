using MCAWebAndAPI.Model.HR.DataMaster;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IDataMasterService
    {
        void SetSiteUrl(string siteUrl);

        PositionMaster GetPosition(int id);

        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<ProfessionalMaster> GetProfessionalsActive();

        IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFees();

        IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFeesEdit();

        IEnumerable<AdjustmentMaster> GetAdjustemnt(IEnumerable<int> professionalIDs);

        IEnumerable<PositionMaster> GetPositions();

        IEnumerable<PositionMaster> GetPositionsManpower(string Level);
        
        IEnumerable<DependentMaster> GetDependents();

        IEnumerable<DependentMaster> GetDependentsForInsurance(int? id);

        IEnumerable<MonthlyFeeMaster> GetMonthlyFees(int[] professionalIDs);

        string GetProfessionalPosition(string userLogin);

        string GetProfessionalOfficeEmail(int professionalID);
    }
}

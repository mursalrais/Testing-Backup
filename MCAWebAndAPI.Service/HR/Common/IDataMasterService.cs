using MCAWebAndAPI.Model.HR.DataMaster;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IDataMasterService
    {
        void SetSiteUrl(string siteUrl);

        PositionMaster GetPosition(int id);

        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFees();

        IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFeesEdit();

        IEnumerable<PositionMaster> GetPositions();

        IEnumerable<PositionMaster> GetPositionsManpower(string Level);
        
        IEnumerable<DependentMaster> GetDependents();

        IEnumerable<DependentMaster> GetDependentsForInsurance();

        IEnumerable<MonthlyFeeMaster> GetMonthlyFees(int[] professionalIDs);

        string GetProfessionalPosition(string userLogin);

        string GetProfessionalOfficeEmail(int professionalID);
    }
}

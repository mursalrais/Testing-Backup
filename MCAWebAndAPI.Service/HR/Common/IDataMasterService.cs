using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.Common;
using System.Threading.Tasks;

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

        string GetProfessionalPosition(string userLogin);

        string GetProfessionalOfficeEmail(int professionalID);


    }
}

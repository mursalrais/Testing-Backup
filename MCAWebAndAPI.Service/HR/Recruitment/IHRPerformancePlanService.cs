using System;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRPerformancePlanService
    {
        void SetSiteUrl(string siteUrl = null);

        ProfessionalPerformancePlanVM GetPopulatedModel(int? id = null);

        ProfessionalPerformancePlanVM GetHeader(int? ID);

        int CreateHeader(ProfessionalPerformancePlanVM header);

        bool UpdateHeader(ProfessionalPerformancePlanVM header);

        void CreatePerformancePlanDetails(int? headerID, IEnumerable<ProjectOrUnitGoalsDetailVM> performancePlanDetails);

        ProfessionalPerformancePlanVM GetProfessionalEmail(int? ID);


    }
}

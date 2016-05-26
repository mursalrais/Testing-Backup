using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRApplicationService
    {
        void SetSiteUrl(string siteUrl = null);

        ApplicationDataVM GetApplicationData(int? ID);

        int CreateApplicationData(ApplicationDataVM viewModel);

        void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> educationDetails);

        void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails);

        void CreateWorkingExperienceDetails(int? headerID, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails);

        void CreateApplicationDocument(int? headerID, IEnumerable<HttpPostedFileBase> documents);
    }
}

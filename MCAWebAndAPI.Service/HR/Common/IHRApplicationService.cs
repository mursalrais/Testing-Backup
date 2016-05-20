using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IHRApplicationService
    {
        void SetSiteUrl(string siteUrl = null);

        ApplicationDataVM GetBlankApplicationDataForm();

        int CreateApplicationData(ApplicationDataVM viewModel);

        bool CreateEducationDetails(int headerID, IEnumerable<EducationDetailVM> educationDetails);

        bool CreateTrainingDetails(int headerID, IEnumerable<TrainingDetailVM> trainingDetails);

        bool CreateWorkingExperienceDetails(int headerID, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails);

        void CreateProfessionalDocuments(int headerID, IEnumerable<HttpPostedFileBase> documents);
    }
}

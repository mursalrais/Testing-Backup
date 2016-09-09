using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRApplicationService
    {
        void SetSiteUrl(string siteUrl = null);

        IEnumerable<PositionMaster> GetVacantPositions();

        IEnumerable<ApplicationDataVM> GetApplications();

        ApplicationDataVM GetApplication(int? ID);
        Task<ApplicationDataVM> GetApplicationAsync(int? ID);


        int CreateApplication(ApplicationDataVM viewModel);

        void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> educationDetails);

        Task CreateEducationDetailsAsync(int? headerID, IEnumerable<EducationDetailVM> educationDetails);

        void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails);

        Task CreateTrainingDetailsAsync(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails);

        void CreateWorkingExperienceDetails(int? headerID, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails);

        Task CreateWorkingExperienceDetailsAsync(int? headerID, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails);

        Dictionary<int, string> GetIDCardType();

        void CreateApplicationDocument(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        Task CreateApplicationDocumentAsync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        void SetApplicationStatus(ApplicationDataVM viewModel);

        int? CreateProfessionalData(ApplicationDataVM viewModel);
    }
}
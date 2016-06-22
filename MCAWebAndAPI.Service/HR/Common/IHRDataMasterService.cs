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
    public interface IHRDataMasterService
    {
        void SetSiteUrl(string siteUrl);

        /// <summary>
        /// Used to get professional master form
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        ProfessionalDataVM GetProfessionalData(int? ID);
        Task<ProfessionalDataVM> GetProfessionalDataAsync(int? ID);

        ProfessionalDataVM GetProfessionalData(string userLoginName = null);

        /// <summary>
        /// Used to get lightweight professional data, e.g., name, position.
        /// Suitable for combobox
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFees(); 

        IEnumerable<PositionMaster> GetPositions();

        int? EditProfessionalData(ProfessionalDataVM viewModel);
        
        void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> educationDetails);

        Task CreateEducationDetailsAsync(int? headerID, IEnumerable<EducationDetailVM> educationDetails);

        void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails);

        Task CreateTrainingDetailsAsync(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails);

        void CreateDependentDetails(int? headerID, IEnumerable<DependentDetailVM> documents);

        Task CreateDependentDetailsAsync(int? headerID, IEnumerable<DependentDetailVM> documents);

        void CreateOrganizationalDetails(int? headerID, IEnumerable<OrganizationalDetailVM> organizationalDetails);
        Task CreateOrganizationalDetailsAsync(int? headerID, IEnumerable<OrganizationalDetailVM> organizationalDetails);


        void UpdateValidation(int? ID, string status);
        void SendEmailValidation(string emailTo, string emailMessages, bool isApproved);
        void SendEmailValidation(string emailTo, string emailMessages);
        PositionMaster GetPosition(int id);
        void SetValidationStatus(int? id, Workflow.ProfessionalValidationStatus validationStatus);
    }
}

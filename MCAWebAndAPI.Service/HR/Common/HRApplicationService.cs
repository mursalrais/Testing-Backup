using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class HRApplicationService : IHRApplicationService
    {
        string _siteUrl;

        public int CreateApplicationData(ApplicationDataVM viewModel)
        {
            throw new NotImplementedException();
        }

        public bool CreateEducationDetails(int headerID, IEnumerable<EducationDetailVM> educationDetails)
        {
            throw new NotImplementedException();
        }

        public void CreateProfessionalDocuments(int headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            throw new NotImplementedException();
        }

        public bool CreateTrainingDetails(int headerID, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            throw new NotImplementedException();
        }

        public bool CreateWorkingExperienceDetails(int headerID, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails)
        {
            throw new NotImplementedException();
        }

        public ApplicationDataVM GetBlankApplicationDataForm()
        {
            return new ApplicationDataVM();
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}

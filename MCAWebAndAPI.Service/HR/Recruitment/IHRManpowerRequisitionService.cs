using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRManpowerRequisitionService
    {
        void SetSiteUrl(string siteUrl = null);

        ManpowerRequisitionVM GetManpowerRequisition(int? ID);

        ManpowerRequisitionVM GetRequestStatus();

        IEnumerable<ManpowerRequisitionVM> GetManpowerRequisitionAll();

        string GetApprover(string Position);

        int CreateManpowerRequisition(ManpowerRequisitionVM viewModel);

        void CreateWorkingRelationshipDetails(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails);        

        void CreateManpowerRequisitionDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        bool UpdateManpowerRequisition(ManpowerRequisitionVM viewModel);

        bool UpdateStatus(ManpowerRequisitionVM viewModel);

        string GetPosition(string username,string siteUrl);
        Task<ManpowerRequisitionVM> GetManpowerRequisitionAsync(int? iD);
        Task CreateWorkingRelationshipDetailsAsync(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails);
        Task CreateManpowerRequisitionDocumentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);
        string getEmailOnBehalf(int? ID);
        List<string> GetEmailHR();
    }
}

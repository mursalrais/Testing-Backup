using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRInterviewService
    {
        void SetSiteUrl(string siteUrl = null);

        IEnumerable<ApplicationShortlistVM> GetInterviewlists();

        ApplicationShortlistVM GetInterviewlist(string position, string username, string useraccess);

        ApplicationShortlistVM GetResultlistInterview(int? ID);

        void CreateInterviewDataDetail(int? headerID, ApplicationShortlistVM list);

        void CreateInputIntvResult(int? headerID, ApplicationShortlistVM list);

        void SendEmailValidation(string emailto, string emailmessage);

        Task CreateInterviewDocumentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        IEnumerable<ShortlistDetailVM> GetUpdatedata(string url);

        void UpdateManualDataDetail(ShortlistDetailVM list, int? manID);
        
    }
}


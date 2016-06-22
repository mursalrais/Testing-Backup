using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
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
        
    }
}


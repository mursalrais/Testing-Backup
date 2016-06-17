using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRInterviewService
    {
        void SetSiteUrl(string siteUrl = null);

        IEnumerable<ApplicationShortlistVM> GetShortlists();

        ApplicationShortlistVM GetInterviewlist(string position, string username, string useraccess);

        ApplicationShortlistVM GetShortlistSend(int? ID);

        void UpdateShortlistDataDetail(int? headerID, IEnumerable<ShortlistDetailVM> ShortlistDetails);

        void CreateShortlistInviteIntv(int? headerID, ApplicationShortlistVM ShortlistDetails);

        void CreateShorlistSendintv(int? headerID, ApplicationShortlistVM ShortlistDetails);

    }
}


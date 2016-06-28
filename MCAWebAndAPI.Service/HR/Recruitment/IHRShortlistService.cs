using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRShortlistService
    {
        void SetSiteUrl(string siteUrl = null);

        IEnumerable<ApplicationShortlistVM> GetShortlists();

        ApplicationShortlistVM GetShortlist(int? position, string username, string useraccess);

        ApplicationShortlistVM GetShortlistSend(int? ID);

        void UpdateShortlistDataDetail(int? headerID, IEnumerable<ShortlistDetailVM> ShortlistDetails);

        void CreateShortlistInviteIntv(int? headerID, ApplicationShortlistVM ShortlistDetails, string mailsubject);

        void CreateShorlistSendintv(int? headerID, ApplicationShortlistVM ShortlistDetails);

        void SendEmailValidation(string mailto, string mailmessage);

        IEnumerable<PositionMaster> GetPositions();
    }
}

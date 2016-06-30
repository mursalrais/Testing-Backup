using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRCompensatoryService
    {
        void SetSiteUrl(string siteUrl = null);

        CompensatoryVM GetComplist(int? ID);

        void UpdateShortlistDataDetail(int? headerID, IEnumerable<ShortlistDetailVM> ShortlistDetails);

        IEnumerable<PositionMaster> GetPositions();
    }
}

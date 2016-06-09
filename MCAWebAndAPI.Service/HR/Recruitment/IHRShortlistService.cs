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

        ApplicationShortlistVM GetShortlist(string Position);

        int? EditShortlistData(ShortlistDetailVM viewModel);

        void CreateShortlistDataDetail(int? headerID, IEnumerable<ShortlistDetailVM> educationDetails);
    }
}

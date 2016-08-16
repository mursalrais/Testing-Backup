using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    public interface IOutstandingAdvanceService
    {
        void SetSiteUrl(string siteUrl);

        int Save(OutstandingAdvanceVM viewModel);

        Task SaveAttachmentAsync(int? ID, string sphlNo, IEnumerable<HttpPostedFileBase> documents);

        OutstandingAdvanceVM Get(int? ID);
    }
}

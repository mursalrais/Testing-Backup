using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    public interface IOutstandingAdvanceService
    {
        void SetSiteUrl(string siteUrl);

        OutstandingAdvanceVM Get(Operations op, int? id = default(int?));

        int Save(OutstandingAdvanceVM viewModel);

        Task SaveAttachmentAsync(int? ID, string sphlNo, IEnumerable<HttpPostedFileBase> documents);

        OutstandingAdvanceVM Get(int? ID);

        void SendEmail(string emailTo, string message);
    }
}

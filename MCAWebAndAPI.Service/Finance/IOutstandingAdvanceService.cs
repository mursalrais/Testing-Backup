using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    public interface IOutstandingAdvanceService
    {
        void SetSiteUrl(string siteUrl);

        void SendEmail(string emailTo, string message);

        int Save(OutstandingAdvanceVM viewModel);

        OutstandingAdvanceVM Get(Operations op, int? id = default(int?));

        OutstandingAdvanceVM Get(int? ID);
        
        Task SaveAttachmentAsync(int? ID, string sphlNo, IEnumerable<HttpPostedFileBase> documents);

        Task SendEmailToProfessional(string message, OutstandingAdvanceVM viewModel);

        Task SendEmailToGrantees(string message, OutstandingAdvanceVM viewModel);

        Task<List<CSVErrorLogVM>> ProcessCSVFilesAsync(IEnumerable<HttpPostedFileBase> documents, IEnumerable<VendorVM> vendors);
    }
}

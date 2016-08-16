using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ITaxReimbursementService
    {
        void SetSiteUrl(string siteUrl);

        TaxReimbursementVM Get(int? id = null);

        int Create(TaxReimbursementVM viewModel);

        bool Update(TaxReimbursementVM viewModel);

        Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);
    }
}

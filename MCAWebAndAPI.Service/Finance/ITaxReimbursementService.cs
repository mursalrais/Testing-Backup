using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ITaxReimbursementService
    {
        void SetSiteUrl(string siteUrl);

        TaxReimbursementVM Get(Operations op, int? id = null);

        int Create(TaxReimbursementVM viewModel);

        bool Update(TaxReimbursementVM viewModel);

        Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);
    }
}

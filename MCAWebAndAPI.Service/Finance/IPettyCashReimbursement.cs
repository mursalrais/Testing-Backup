using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface IPettyCashReimbursement
    {
        void SetSiteUrl(string siteUrl);

        int? Create(PettyCashReimbursementVM sphl);

        Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);
    }
}

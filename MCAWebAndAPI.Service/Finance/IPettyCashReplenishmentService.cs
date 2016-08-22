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
    /// FIN14: Petty Cash Replenishment
    /// </summary>

    public interface IPettyCashReplenishmentService
    {
        void SetSiteUrl(string siteUrl);

        int Save(PettyCashReplenishmentVM viewModel);

        Task SaveAttachmentAsync(int? id, string name, IEnumerable<HttpPostedFileBase> documents);

        PettyCashReplenishmentVM Get(int? id);
    }
}

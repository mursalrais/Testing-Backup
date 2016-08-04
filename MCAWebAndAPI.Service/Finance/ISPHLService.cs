using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ISPHLService
    {
        void SetSiteUrl(string siteUrl);

        int? CreateSPHL(SPHLVM sphl);

        Task CreateSPHLAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);
    }
}

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

        int? Create(SPHLVM sphl);

        bool UpdateSPHL(SPHLVM viewMOdel);

        bool CheckExistingSPHLNo(string no);

        SPHLVM GetDataSPHL(int? ID);

        Task CreateSPHLAttachmentAsync(int? ID, string sphlNo, IEnumerable<HttpPostedFileBase> attachment);
    }
}

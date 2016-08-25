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
    public interface IPettyCashSettlementService
    {
        void SetSiteUrl(string siteUrl);

        PettyCashSettlementVM Get(Operations op, int? id = default(int?));

        int? Save(PettyCashSettlementVM sphl);
        void SavePettyCashAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents);
    }
}

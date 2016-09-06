using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ISCAReimbursementService
    {
        void SetSiteUrl(string siteUrl);

        SCAReimbursementVM Get(Operations op, int? id = default(int?));

        int? Save(SCAReimbursementVM scaSettlement);

    }
}

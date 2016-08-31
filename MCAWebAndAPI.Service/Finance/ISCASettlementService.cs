using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wirefram FIN07: SCA Settlement
    /// </summary>

    public interface ISCASettlementService
    {
        void SetSiteUrl(string siteUrl);

        SCASettlementVM Get(Operations op, int? id = default(int?));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Form.Finance;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public interface IPettyCashStatementService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<PettyCashTransactionItem> GetPettyCashStatements(DateTime dateFrom, DateTime dateTo);
    }
}

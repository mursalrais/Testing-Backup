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
    /// Wireframe FIN13: Petty Cash Journal
    /// </summary>

    public interface IPettyCashJournalService
    {
        void SetSiteUrl(string siteUrl);
        int? Create(PettyCashJournalVM viewModel);
        Task CreateItems(int? ID, PettyCashJournalVM viewModel);
        PettyCashJournalVM Get(Operations op, int? id = default(int?));
        IEnumerable<PettyCashJournalItemVM> Get(int? id = default(int?));
        IEnumerable<PettyCashJournalItemVM> GetPettyCashTransactions(string dateFrom, string dateTo);
    }
}

using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN04: Event Budget
    /// </summary>
    
    public interface IEventBudgetService
    {
        EventBudgetVM Get(int? ID);

        Task<EventBudgetVM> GetASync(int? ID);

        int Create(EventBudgetVM eventBudget);

        bool Update(EventBudgetVM eventBudget);

        IEnumerable<EventBudgetVM> GetEventBudgetList();

        IEnumerable<EventBudgetItemVM> GetItems(int eventBudgetID);

        Task CreateItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> items);

        Task CreateAttachmentsAsync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        Task UpdateRequisitionNoteAsync(string siteUrl = null, int id = 0);

        Task UpdateSCAVoucherAsync(string siteUrl = null, int id = 0);
    }
}

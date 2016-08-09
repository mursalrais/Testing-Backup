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
    public interface IEventBudgetService
    {
        void SetSiteUrl(string siteUrl);

        EventBudgetVM Get(int? ID);


        int CreateEventBudget(EventBudgetVM eventBudget);

        bool Update(EventBudgetVM eventBudget);

        IEnumerable<EventBudgetVM> GetEventBudgetList();

        Task CreateItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> noteItems);

        Task CreateAttachmentsAsync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        Task EditItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> noteItems);
        Task EditAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);
    }
}

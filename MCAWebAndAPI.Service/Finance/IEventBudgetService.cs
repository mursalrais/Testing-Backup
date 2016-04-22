using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Finance
{
    public interface IEventBudgetService
    {
        void SetSiteUrl(string siteUrl);

        EventBudgetVM GetEventBudget();

        EventBudgetVM GetEventBudget_Dummy();


        bool CreateEventBudget(EventBudgetVM eventBudget);

        bool UpdateEventBudget(EventBudgetVM eventBudget);


    }
}

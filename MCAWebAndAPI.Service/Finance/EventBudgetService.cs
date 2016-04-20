using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class EventBudgetService : IEventBudgetService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public IEnumerable<EventBudgetVM> GetEventBudget()
        {
            throw new NotImplementedException();
        }

        public bool CreateEventBudget(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEventBudget(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }
    }
}

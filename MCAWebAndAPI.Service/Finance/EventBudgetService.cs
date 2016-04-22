using System;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using NLog;
using System.Collections.Generic;

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

        public EventBudgetVM GetEventBudget()
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

        public EventBudgetVM GetEventBudget_Dummy()
        {
            var viewModel = new EventBudgetVM();

            var list  = new List<EventBudgetItemVM>();
            list.Add(new EventBudgetItemVM()
            {
                AmountPerItem = 1212,
                DirectPayment = 122,
                Remarks = "asass"
            });
            viewModel.Items = list;

            return viewModel;
        }
    }
}

using System;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using NLog;
using System.Collections.Generic;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Finance
{
    public class EventBudgetService : IEventBudgetService
    {
        const string ListName_EventBudget = "Event Budget";
        const string ListName_EventBudgetItem = "Event Budget Item";
        const string ListName_Activity = "Activity";

        const string EventBudgetFieldName_ID = "ID";
        const string EventBudgetFieldName_No = "No";
        const string EventBudgetFieldName_EventName = "Event Name";
        const string EventBudgetFieldName_DateFrom = "Date From";
        const string EventBudgetFieldName_DateTo = "Date To";
        const string EventBudgetFieldName_Project = "Project";
        const string EventBudgetFieldName_ActivityID = "Activity ID";
        const string EventBudgetFieldName_Venue = "Venue";
        const string EventBudgetFieldName_ExhangeRate = "Exhange Rate";
        const string EventBudgetFieldName_TotalDirectPaymentIDR = "Total Direct Payment (IDR)";

        const string ActivityFieldName_Name = "Title";

        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public EventBudgetVM GetEventBudget(int? ID)
        {
            EventBudgetVM eventBudget;

            if (ID == null)
            {
                eventBudget = CreateNewEventBudgetVM();
            }
            else
            {
                var listItem = SPConnector.GetListItem(ListName_EventBudget, ID, _siteUrl);

                eventBudget = ConvertToEventBudgetVM(listItem);
                eventBudget.ItemDetails = GetEventBudgetItem();
            }
            return eventBudget;
        }

        public bool CreateEventBudget(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEventBudget(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventBudgetItemVM> GetEventBudgetItem()
        {
            var eventBudgets = new List<EventBudgetItemVM>();
            foreach (var item in SPConnector.GetList(ListName_EventBudgetItem, _siteUrl))
            {
                eventBudgets.Add(ConvertToEventBudgetModel(item));
            }


            return eventBudgets;
        }

        EventBudgetItemVM ConvertToEventBudgetModel(ListItem item)
        {
            return new EventBudgetItemVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                Quantity = Convert.ToInt32(item["Quantity"]),
                UoMQty = Convert.ToString(item["UoMQuantity"]),
            };
        }

        private EventBudgetVM CreateNewEventBudgetVM()
        {
            var eventBudget = new EventBudgetVM();
            eventBudget.Activity.Choices = GetActivities(ActivityFieldName_Name); 

            return eventBudget;
        }


        private EventBudgetVM ConvertToEventBudgetVM(ListItem listItem)
        {
            var eventBudget = new EventBudgetVM();

            eventBudget.ID = Convert.ToInt32(listItem[EventBudgetFieldName_ID]);
            eventBudget.EventName = Convert.ToString(listItem[EventBudgetFieldName_EventName]);
            eventBudget.DateFrom = Convert.ToDateTime(listItem[EventBudgetFieldName_DateFrom]);
            eventBudget.DateTo = Convert.ToDateTime(listItem[EventBudgetFieldName_DateTo]);

            eventBudget.Project.Text = Convert.ToString(listItem[EventBudgetFieldName_Project]);

            eventBudget.Activity.Choices = GetActivities("whatever");
            eventBudget.Activity.Text = Convert.ToString(listItem[EventBudgetFieldName_ActivityID]);

            return eventBudget;
        }


        private string[] GetEvents(string listName)
        {
            List<string> _eventNames = new List<string>();
            var listItems = SPConnector.GetList(ListName_EventBudget, _siteUrl);

            foreach (var item in listItems)
            {
                _eventNames.Add(item[listName].ToString());
            }

            return _eventNames.ToArray();
        }

        private string[] GetActivities(string columnName)
        {
            List<string> eventNames = new List<string>();
            var listItems = SPConnector.GetList(ListName_Activity, _siteUrl);

            foreach (var item in listItems)
            {
                eventNames.Add(item[columnName].ToString());
            }
            
            return eventNames.ToArray();
        }

    }
}
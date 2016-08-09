using System;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using NLog;
using System.Collections.Generic;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using System.Threading.Tasks;
using System.Web;

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
        const string EventBudgetFieldName_DateFrom = "Date_x0020_From";
        const string EventBudgetFieldName_DateTo = "Date_x0020_To";
        const string EventBudgetFieldName_Project = "Project";
        const string EventBudgetFieldName_ActivityID = "Activity_x0020_ID0";
        const string EventBudgetFieldName_Venue = "Venue";
        const string EventBudgetFieldName_ExhangeRate = "Exhange Rate";
        const string EventBudgetFieldName_TotalDirectPaymentIDR = "Total Direct Payment (IDR)";
        const string EventBudgetItemFieldName_EventBudgetID = "Event_x0020_Budget_x0020_ID";
        const string EventBudgetFieldName_ActivityName = "Activity_x0020_ID_x003a_Name";
        const string ActivityFieldName_Name = "Title";

        const string DocumentNoMask = "EB/{0}-{1}/";

        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public EventBudgetVM Get(int? ID)
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
                eventBudget.ItemDetails = GetItem(ID.Value);
            }
            return eventBudget;
        }

        public int Create(EventBudgetVM eventBudget)
        {
            var updatedValue = new Dictionary<string, object>();
            string DocumentNo = string.Format(ActivityFieldName_Name, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0:D5}";

            updatedValue.Add(EventBudgetFieldName_EventName, eventBudget.EventName);
            updatedValue.Add(EventBudgetFieldName_DateFrom, eventBudget.DateFrom);
            updatedValue.Add(EventBudgetFieldName_DateTo, eventBudget.DateTo);
            updatedValue.Add(EventBudgetFieldName_Project, eventBudget.Project.Value);
            updatedValue.Add(EventBudgetFieldName_ActivityID, eventBudget.Activity.Value);

      
            updatedValue.Add(EventBudgetFieldName_No, DocumentNumbering.Create(_siteUrl, DocumentNo));



            try
            {
                SPConnector.AddListItem(ListName_EventBudget, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(ListName_EventBudget, _siteUrl);
        }

        public bool Update(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventBudgetItemVM> GetItem(int eventBudgetID)
        {
            var eventBudgets = new List<EventBudgetItemVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='" + EventBudgetItemFieldName_EventBudgetID + "' /><Value Type='Lookup'>" + eventBudgetID.ToString() + "</Value></Eq></Where></Query></View>";


            foreach (var item in SPConnector.GetList(ListName_EventBudgetItem, _siteUrl, caml))
            {
                eventBudgets.Add(ConvertToItemVM(item));
            }


            return eventBudgets;
        }

        EventBudgetItemVM ConvertToItemVM(ListItem item)
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
            eventBudget.EventName = Convert.ToString(listItem[ActivityFieldName_Name]);

            eventBudget.DateFrom = Convert.ToDateTime(listItem[EventBudgetFieldName_DateFrom]);
            eventBudget.DateTo = Convert.ToDateTime(listItem[EventBudgetFieldName_DateTo]);

            eventBudget.Project.Text = Convert.ToString(listItem[EventBudgetFieldName_Project]);

            eventBudget.Activity.Choices = GetActivities("Title");
            eventBudget.Activity.Text = (listItem[EventBudgetFieldName_ActivityName] as FieldLookupValue).LookupValue; 
            eventBudget.Activity.Value = Convert.ToString((listItem[EventBudgetFieldName_ActivityID] as FieldLookupValue).LookupId);

            return eventBudget;
        }


        private string[] GetList(string listName)
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


        public IEnumerable<EventBudgetVM> GetEventBudgetList()
        {
            var eventBudgets = new List<EventBudgetVM>();
            foreach (var item in SPConnector.GetList(ListName_EventBudget, _siteUrl, null))
            {
                eventBudgets.Add(ConvertToEventBudgetVMForList(item));
            }
            

            return eventBudgets;
        }

        private EventBudgetVM ConvertToEventBudgetVMForList(ListItem listItem)
        {
            var eventBudget = new EventBudgetVM();

            eventBudget.ID = Convert.ToInt32(listItem[EventBudgetFieldName_ID]);
            eventBudget.Title = Convert.ToString(listItem[ActivityFieldName_Name]);
            eventBudget.No = Convert.ToString(listItem[EventBudgetFieldName_No]);
            return eventBudget;
        }

        int IEventBudgetService.CreateEventBudget(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }

        public Task CreateItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> noteItems)
        {
            throw new NotImplementedException();
        }

        public Task CreateAttachmentsAsync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            throw new NotImplementedException();
        }

        public Task EditItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> noteItems)
        {
            throw new NotImplementedException();
        }

        public Task EditAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            throw new NotImplementedException();
        }
    }
}
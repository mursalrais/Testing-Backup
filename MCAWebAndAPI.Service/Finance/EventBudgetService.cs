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
    /// <summary>
    /// Wireframe FIN04: Event Budget
    /// </summary>
    
    public class EventBudgetService : IEventBudgetService
    {
        const string ListName_EventBudget = "Event Budget";
        const string ListName_EventBudgetItem = "Event Budget Item";
        const string ListName_Activity = "Activity";

        const string EventBudgetFieldName_ID = "ID";
        const string EventBudgetFieldName_No = "No";
        const string EventBudgetFieldName_EventName = "Title";
        const string EventBudgetFieldName_DateFrom = "Date_x0020_From";
        const string EventBudgetFieldName_DateTo = "Date_x0020_To";
        const string EventBudgetFieldName_Project = "Project";
        const string EventBudgetFieldName_ActivityID = "Activity_x0020_ID0";
        const string EventBudgetFieldName_Venue = "Venue";
        const string EventBudgetFieldName_ExhangeRate = "Exchange_x0020_Rate";
        const string EventBudgetFieldName_TotalDirectPaymentIDR = "Total Direct Payment (IDR)";
        const string EventBudgetItemFieldName_EventBudgetID = "Event_x0020_Budget_x0020_ID";
        const string EventBudgetFieldName_ActivityName = "Activity_x0020_ID_x003a_Name";
        const string ActivityFieldName_Name = "Title";

        const string DocumentNoMask = "EB/{0}-{1}/";

        string siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
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
                var listItem = SPConnector.GetListItem(ListName_EventBudget, ID, siteUrl);

                eventBudget = ConvertToEventBudgetVM(listItem);
                eventBudget.ItemDetails = GetItem(ID.Value);
            }

            return eventBudget;
        }
        

        public bool Update(EventBudgetVM eventBudget)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventBudgetItemVM> GetItem(int eventBudgetID)
        {
            var eventBudgets = new List<EventBudgetItemVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='" + EventBudgetItemFieldName_EventBudgetID + "' /><Value Type='Lookup'>" + eventBudgetID.ToString() + "</Value></Eq></Where></Query></View>";


            foreach (var item in SPConnector.GetList(ListName_EventBudgetItem, siteUrl, caml))
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
            //eventBudget.Activity.Choices = GetActivities(ActivityFieldName_Name); 

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

            return eventBudget;
        }


        private string[] GetList(string listName)
        {
            List<string> eventNames = new List<string>();
            var listItems = SPConnector.GetList(ListName_EventBudget, siteUrl);

            foreach (var item in listItems)
            {
                eventNames.Add(item[listName].ToString());
            }

            return eventNames.ToArray();
        }

        private string[] GetActivities(string columnName)
        {
            List<string> eventNames = new List<string>();
            var listItems = SPConnector.GetList(ListName_Activity, siteUrl);

            foreach (var item in listItems)
            {
                eventNames.Add(item[columnName].ToString());
            }
            
            return eventNames.ToArray();
        }


        public IEnumerable<EventBudgetVM> GetEventBudgetList()
        {
            var eventBudgets = new List<EventBudgetVM>();
            foreach (var item in SPConnector.GetList(ListName_EventBudget, siteUrl, null))
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

        int IEventBudgetService.Create(EventBudgetVM eventBudget)
        {

            var updatedValue = new Dictionary<string, object>();
            string DocumentNo = string.Format(DocumentNoMask, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            updatedValue.Add(EventBudgetFieldName_EventName, eventBudget.EventName);
            updatedValue.Add(EventBudgetFieldName_DateFrom, eventBudget.DateFrom);
            updatedValue.Add(EventBudgetFieldName_DateTo, eventBudget.DateTo);
            updatedValue.Add(EventBudgetFieldName_Project, eventBudget.Project.Value);
            updatedValue.Add(EventBudgetFieldName_ActivityID, eventBudget.Activity.Value);

            updatedValue.Add(EventBudgetFieldName_No, DocumentNumbering.Create(siteUrl, DocumentNo, 5));

            try
            {
                SPConnector.AddListItem(ListName_EventBudget, updatedValue, siteUrl);
            }
            catch (ServerException e)
            {
                var errMsg = e.Message + Environment.NewLine + e.ServerErrorValue;
                logger.Error(errMsg);

#if DEBUG
                throw new Exception(errMsg);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

#if DEBUG
                throw new Exception(e.Message);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }


            return SPConnector.GetLatestListItemID(ListName_EventBudget, siteUrl);

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
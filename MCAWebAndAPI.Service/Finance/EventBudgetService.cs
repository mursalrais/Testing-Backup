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
        private const string ListName_EventBudget = "Event Budget";
        private const string ListName_EventBudgetItem = "Event Budget Item";
        private const string ListName_Attachment = "Event Budget Documents";
        private const string ListName_Activity = "Activity";

        private const string EventBudgetFieldName_ID = "ID";
        private const string EventBudgetFieldName_No = "No";
        private const string EventBudgetFieldName_EventName = "Title";
        private const string EventBudgetFieldName_DateFrom = "Date_x0020_From";
        private const string EventBudgetFieldName_DateTo = "Date_x0020_To";
        private const string EventBudgetFieldName_Project = "Project";
        private const string EventBudgetFieldName_ActivityID = "Activity_x0020_ID0";
        private const string EventBudgetFieldName_ActivityName = "Activity_x0020_ID_x003a_Name";
        private const string EventBudgetFieldName_Venue = "Venue";
        private const string EventBudgetFieldName_ExhangeRate = "Exchange_x0020_Rate";
        private const string EventBudgetFieldName_TotalDirectPaymentIDR = "Total Direct Payment (IDR)";

        private const string ActivityFieldName_Name = "Title";

        private const string EventBudgetItemFieldName_EventBudgetID = "Event_x0020_Budget_x0020_ID";
        private const string EventBudgetItemFieldName_WBSId = "WBS_x0020_Master_x002e_ID";
        private const string EventBudgetItemFieldName_WBSName = "WBS_x0020_Master_x002e_ID_x003a_";
        private const string EventBudgetItemFieldName_GLID = "GL_x0020_Master_x002e_ID";
        private const string EventBudgetItemFieldName_GLNo = "GL_x0020_Master_x002e_ID_x003a_G";
        private const string EventBudgetItemFieldName_GLDescription = "GL_x0020_Master_x002e_ID_x003a_G0";


        private const string DocumentNoMask = "EB/{0}-{1}/";

        private string siteUrl = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public EventBudgetVM Get(int? ID)
        {
            EventBudgetVM eventBudget;

            if (ID == null)
            {
                eventBudget = new EventBudgetVM();
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
            EventBudgetItemVM detail = new EventBudgetItemVM();
            detail.ID = Convert.ToInt32(item["ID"]);
            detail.Title = Convert.ToString(item["Title"]);
            detail.Quantity = Convert.ToInt32(item["Quantity"]);
            detail.UoMQty = Convert.ToString(item["UoMQuantity"]);
            detail.AmountPerItem = Convert.ToDecimal(item["AmountPerItem"]);
            detail.Frequency = Convert.ToInt32(item["Frequency"]);
            detail.UnitPrice = Convert.ToDecimal(item["UnitPrice"]);

            detail.WBS.Value = (item[EventBudgetItemFieldName_WBSId] as FieldLookupValue).LookupId;
            detail.WBS.Text = string.Format("{0}-{1}", (item[EventBudgetItemFieldName_WBSId] as FieldLookupValue).LookupValue, (item[EventBudgetItemFieldName_WBSName] as FieldLookupValue).LookupValue);

            detail.GL.Value = (item[EventBudgetItemFieldName_GLID] as FieldLookupValue).LookupId;
            detail.GL.Text = string.Format("{0}-{1}", (item[EventBudgetItemFieldName_GLNo] as FieldLookupValue).LookupValue, (item[EventBudgetItemFieldName_GLDescription] as FieldLookupValue).LookupValue);

            return detail;
        }


        private EventBudgetVM ConvertToEventBudgetVM(ListItem listItem)
        {
            var eventBudget = new EventBudgetVM();

            eventBudget.ID = Convert.ToInt32(listItem[EventBudgetFieldName_ID]);
            eventBudget.EventName = Convert.ToString(listItem[ActivityFieldName_Name]);
            eventBudget.No = Convert.ToString(listItem[EventBudgetFieldName_No]);

            eventBudget.DateFrom = Convert.ToDateTime(listItem[EventBudgetFieldName_DateFrom]);
            eventBudget.DateTo = Convert.ToDateTime(listItem[EventBudgetFieldName_DateTo]);

            eventBudget.Project.Text = Convert.ToString(listItem[EventBudgetFieldName_Project]);
            eventBudget.Project.Value = Convert.ToString(listItem[EventBudgetFieldName_Project]);

            eventBudget.Activity.Value = (listItem[EventBudgetFieldName_ActivityName] as FieldLookupValue).LookupId;
            eventBudget.Activity.Text = (listItem[EventBudgetFieldName_ActivityName] as FieldLookupValue).LookupValue;

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
            eventBudget.Project.Text = Convert.ToString(listItem[EventBudgetFieldName_Project]);

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

        public async Task CreateAttachmentsAsync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateRequisitionNoteAttachments(headerID, documents);
        }

        public Task EditItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> noteItems)
        {
            throw new NotImplementedException();
        }

        public Task EditAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            throw new NotImplementedException();
        }

        public async Task CreateItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add(EventBudgetItemFieldName_EventBudgetID, headerID);
                updatedValue.Add(EventBudgetItemFieldName_WBSId, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBS.Value) });
                updatedValue.Add(EventBudgetItemFieldName_GLID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });

                try
                {
                    SPConnector.AddListItem(ListName_EventBudgetItem, updatedValue, siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void CreateRequisitionNoteAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                var type = doc.FileName.Split('-')[0].Trim();

                updateValue.Add(EventBudgetItemFieldName_EventBudgetID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                try
                {
                    SPConnector.UploadDocument(ListName_Attachment, updateValue, doc.FileName, doc.InputStream, siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }
    }
}
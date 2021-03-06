﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;


namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN04: Event Budget
    /// </summary>

    public class EventBudgetService : IEventBudgetService
    {
        public enum GetChoice  {All, DirectPayment, SCA}

        #region Constants
        
        private const string ListName_EventBudget = "Event Budget";
        public const string ListName_EventBudgetItem = "Event Budget Item";
        private const string ListName_Attachment = "Event Budget Documents";
        private const string ListName_Activity = "Activity";
        private const string ListNameLandingPageBudgetAactualDisbursement = "Budget VS Actual Disbursement MCDR";
        private const string ListNameWBSMapping = "WBS Mapping";

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
        private const string EventBudgetFieldName_TotalDirectPaymentIDR = "Total_x0020_Direct_x0020_Payment";
        private const string EventBudgetFieldName_TotalSCAIDR = "Total_x0020_SCA_x0020__x0028_IDR";
        private const string EventBudgetFieldName_TotalIDR = "Total_x0020__x0028_IDR_x0029_";
        private const string EventBudgetFieldName_TotalUSD = "Total_x0020__x0028_USD_x0029_";
        private const string EventBudgetFieldName_TransactionStatus = "TransactionStatus";
        private const string EventBudgetFieldName_UserEmail = "UserEmail";
        private const string EventBudgetFieldName_VisibleTo = "VisibleTo";

        private const string ActivityFieldName_Name = "Title";

        private const string EventBudgetItemFieldName_ID = "ID";
        private const string EventBudgetItemFieldName_EventBudgetID = "Event_x0020_Budget_x0020_ID";

        //public const string EventBudgetItemFieldName_WBSId = "WBS_x0020_Master_x002e_ID";
        //private const string EventBudgetItemFieldName_WBSName = "WBS_x0020_Master_x002e_ID_x003a_";

        public const string EventBudgetItemFieldName_WBSId = "WBSID";
        private const string EventBudgetItemFieldName_WBSDescription = "WBSDescription";


        public const string EventBudgetItemFieldName_GLID = "GL_x0020_Master_x002e_ID";
        private const string EventBudgetItemFieldName_GLNo = "GL_x0020_Master_x002e_ID_x003a_G";
        private const string EventBudgetItemFieldName_GLDescription = "GL_x0020_Master_x002e_ID_x003a_G0";
        public const string EventBudgetItemFieldName_Quantity = "Quantity";
        private const string EventBudgetItemFieldName_UoMQuantity = "UoMQuantity";
        private const string EventBudgetItemFieldName_UoMFrequency = "UoMFrequency";
        public const string EventBudgetItemFieldName_Frequency = "Frequency";
        public const string EventBudgetItemFieldName_UnitPrice = "UnitPrice";
        private const string EventBudgetItemFieldName_TypeOfExpense = "Type_x0020_of_x0020_Expense";
        private const string EventBudgetItemFieldName_AmountPerItem = "AmountPerItem";
        private const string EventBudgetItemFieldName_DirectPayment = "DirectPayment";
        private const string EventBudgetItemFieldName_SCA = "SCA";
        private const string EventBudgetItemFieldName_Remarks = "Remarks";
        private const string EventBudgetItemFieldName_Description = "Title";

        private const string EventBudgetDocuments_EventBudgetId = "Event_x0020_Budget";
        private const string FINEventBudgetDocumentByID = "{0}/Event%20Budget%20Documents/Forms/AllItems.aspx#InplviewHash5093bda1-84bf-4cad-8652-286653d6a83f=FilterField1%3Dpsa%255Fx003a%255FID-FilterValue1%3D{1}";

        private const string BudgetActualDisbursementMonth = "Month";
        private const string BudgetActualDisbursementYear = "Year";
        private const string BudgetActualDisbursementBudget = "Budget";
        private const string BudgetActualDisbursementActual = "Actual";
        private const string BudgetActualDisbursementMonthPercent = "MonthPercentComplete";
        private const string BudgetActualDisbursementWBSID = "WBSID";

        private const string WBSMappingWBSID = "WBS_x0020_ID";
        private const string WBSMappingActivity = "Activity";
       
        #endregion

        private const string DocumentNoMask = "EB/{0}-{1}/";

        private string siteUrl = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static object SessionManager { get; private set; }

        public EventBudgetService(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }
        
        public async Task<EventBudgetVM> GetASync(int? ID)
        {
            return Get(ID);
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
                eventBudget.ItemDetails = GetItems(ID.Value);
            }

            return eventBudget;
        }   

        public static ListItemCollection GetAllListItems(GetChoice choice, string siteUrl)
        {
            string caml = @"<View><Query>
                            <Where>
                                <Gt>
                                    <FieldRef Name='{0}' />
                                    <Value Type='Number'>0</Value> 
                                </Gt>
                            </Where>
                            </Query>
                            <QueryOptions /></View>";

            switch (choice)
            {
                case GetChoice.DirectPayment:
                    caml = String.Format(caml, EventBudgetFieldName_TotalDirectPaymentIDR);
                    break;

                case GetChoice.SCA:
                    caml = String.Format(caml, EventBudgetFieldName_TotalSCAIDR);
                    break;

                default:
                    caml = string.Empty;
                    break;
            }

            return SPConnector.GetList(ListName_EventBudget, siteUrl, caml);
        }

        public static IEnumerable<AjaxComboBoxVM> GetAllAjaxComboBoxVMs(EventBudgetService.GetChoice choice, string siteUrl)
        {
            var models = new List<AjaxComboBoxVM>();
            foreach (var item in EventBudgetService.GetAllListItems(choice, siteUrl))
            {
                models.Add(
                    new AjaxComboBoxVM
                    {
                        Value = Convert.ToInt32(item[EventBudgetFieldName_ID]),
                        Text = Convert.ToString(item[EventBudgetFieldName_No]) + " - " + Convert.ToString(item[EventBudgetFieldName_EventName])
                    }
                );
            }

            return models;
        }

        public bool Update(EventBudgetVM eventBudget)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(EventBudgetFieldName_EventName, eventBudget.EventName);
            updatedValue.Add(EventBudgetFieldName_DateFrom, eventBudget.DateFrom);
            updatedValue.Add(EventBudgetFieldName_DateTo, eventBudget.DateTo);
            updatedValue.Add(EventBudgetFieldName_Project, eventBudget.Project.Value);
            updatedValue.Add(EventBudgetFieldName_ActivityID, eventBudget.Activity.Value);
            updatedValue.Add(EventBudgetFieldName_Venue, eventBudget.Venue);
            updatedValue.Add(EventBudgetFieldName_ExhangeRate, eventBudget.Rate);
            updatedValue.Add(EventBudgetFieldName_TotalDirectPaymentIDR, eventBudget.TotalDirectPayment);
            updatedValue.Add(EventBudgetFieldName_TotalSCAIDR, eventBudget.TotalSCA);
            updatedValue.Add(EventBudgetFieldName_TotalIDR, eventBudget.TotalIDR);
            updatedValue.Add(EventBudgetFieldName_TotalUSD, eventBudget.TotalUSD);
            updatedValue.Add(EventBudgetFieldName_TransactionStatus, eventBudget.TransactionStatus.Value);
            updatedValue.Add(EventBudgetFieldName_UserEmail, eventBudget.UserEmail);

            updatedValue.Add(EventBudgetFieldName_VisibleTo, SPConnector.GetUser(eventBudget.UserEmail, siteUrl));

            try
            {
                SPConnector.UpdateListItem(ListName_EventBudget, eventBudget.ID, updatedValue, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return true;
        }
        
        public static IEnumerable<EventBudgetItemVM> GetItems(string siteUrl,int eventBudgetID)
        {
            IEventBudgetService service = new EventBudgetService(siteUrl);
    
            return service.GetItems(eventBudgetID);
        }

        public IEnumerable<EventBudgetItemVM> GetItems(int eventBudgetID)
        {
            var eventBudgets = new List<EventBudgetItemVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='" + EventBudgetItemFieldName_EventBudgetID + "' /><Value Type='Lookup'>" + eventBudgetID.ToString() + "</Value></Eq></Where></Query></View>";


            foreach (var item in SPConnector.GetList(ListName_EventBudgetItem, siteUrl, caml))
            {
                if (item != null)
                {
                    eventBudgets.Add(ConvertToItemVM(item));
                }
            }


            return eventBudgets;
        }

        EventBudgetItemVM ConvertToItemVM(ListItem item)
        {
            EventBudgetItemVM detail = new EventBudgetItemVM();
            detail.ID = Convert.ToInt32(item[EventBudgetItemFieldName_ID]);
            detail.Description = Convert.ToString(item[EventBudgetItemFieldName_Description]);
            detail.TypeOfExpense = Convert.ToString(item[EventBudgetItemFieldName_TypeOfExpense]);
            detail.Quantity = Convert.ToInt32(item[EventBudgetItemFieldName_Quantity]);
            detail.UoMQty = Convert.ToString(item[EventBudgetItemFieldName_UoMQuantity]);
            detail.Frequency = Convert.ToInt32(item[EventBudgetItemFieldName_Frequency]);
            detail.UoMFreq = Convert.ToString(item[EventBudgetItemFieldName_UoMFrequency]);
            detail.UnitPrice = Convert.ToDecimal(item[EventBudgetItemFieldName_UnitPrice]);
            detail.AmountPerItem = Convert.ToDecimal(item[EventBudgetItemFieldName_AmountPerItem]);
            detail.DirectPayment = Convert.ToDecimal(item[EventBudgetItemFieldName_DirectPayment]);
            detail.SCA = Convert.ToDecimal(item[EventBudgetItemFieldName_SCA]);
            detail.Remarks = Convert.ToString(item[EventBudgetItemFieldName_Remarks]);

            detail.WBSId = Convert.ToInt32(item[EventBudgetItemFieldName_WBSId]);
            if (detail.WBSId > 0)
            {
                WBS wbsMapping = Common.WBSService.Get(siteUrl, detail.WBSId);
                detail.WBS.Value = detail.WBSId;
                detail.WBS.Text = wbsMapping.WBSIDDescription;
                detail.WBSDesription = string.Format("{0}-{1}", wbsMapping.WBSID, wbsMapping.WBSIDDescription);
            }

            detail.GL.Value = (item[EventBudgetItemFieldName_GLID] as FieldLookupValue).LookupId;
            detail.GL.Text = string.Format("{0}-{1}", (item[EventBudgetItemFieldName_GLNo] as FieldLookupValue).LookupValue, (item[EventBudgetItemFieldName_GLDescription] as FieldLookupValue).LookupValue);

            detail.EditMode = (int)Item.Mode.UPDATED;

            return detail;
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

        
        int IEventBudgetService.Create(EventBudgetVM eventBudget)
        {

            var newObject = new Dictionary<string, object>();
            string DocumentNo = string.Format(DocumentNoMask, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            newObject.Add(EventBudgetFieldName_EventName, eventBudget.EventName);
            newObject.Add(EventBudgetFieldName_DateFrom, eventBudget.DateFrom);
            newObject.Add(EventBudgetFieldName_DateTo, eventBudget.DateTo);
            newObject.Add(EventBudgetFieldName_Project, eventBudget.Project.Value);
            newObject.Add(EventBudgetFieldName_ActivityID, eventBudget.Activity.Value);
            newObject.Add(EventBudgetFieldName_Venue, eventBudget.Venue);
            newObject.Add(EventBudgetFieldName_ExhangeRate, eventBudget.Rate);
            newObject.Add(EventBudgetFieldName_TotalDirectPaymentIDR, eventBudget.TotalDirectPayment);
            newObject.Add(EventBudgetFieldName_TotalSCAIDR, eventBudget.TotalSCA);
            newObject.Add(EventBudgetFieldName_TotalIDR, eventBudget.TotalIDR);
            newObject.Add(EventBudgetFieldName_TotalUSD, eventBudget.TotalUSD);
            newObject.Add(EventBudgetFieldName_TransactionStatus, eventBudget.TransactionStatus.Value);
            newObject.Add(EventBudgetFieldName_UserEmail, eventBudget.UserEmail);

            newObject.Add(EventBudgetFieldName_VisibleTo, SPConnector.GetUser(eventBudget.UserEmail, siteUrl));

            eventBudget.No = DocumentNumbering.Create(siteUrl, DocumentNo, 5);
            newObject.Add(EventBudgetFieldName_No, eventBudget.No);

            try
            {
                SPConnector.AddListItem(ListName_EventBudget, newObject, siteUrl);
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
            CreateAttachments(headerID, documents);
        }

        public async Task CreateItemsAsync(int? headerID, IEnumerable<EventBudgetItemVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if(Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(ListName_EventBudgetItem, viewModel.ID, siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add(EventBudgetItemFieldName_EventBudgetID, headerID);

                var wbsId = Convert.ToInt32(viewModel.WBS.Value);
                WBS wbsMapping = Common.WBSService.Get(siteUrl, wbsId);

                updatedValue.Add(EventBudgetItemFieldName_WBSId, wbsId);
                updatedValue.Add(EventBudgetItemFieldName_WBSDescription, wbsMapping.WBSIDDescription);

                updatedValue.Add(EventBudgetItemFieldName_GLID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(EventBudgetItemFieldName_Quantity, viewModel.Quantity);
                updatedValue.Add(EventBudgetItemFieldName_UoMQuantity, viewModel.UoMQty);
                updatedValue.Add(EventBudgetItemFieldName_UoMFrequency, viewModel.UoMFreq);
                updatedValue.Add(EventBudgetItemFieldName_Frequency, viewModel.Frequency);
                updatedValue.Add(EventBudgetItemFieldName_UnitPrice, viewModel.UnitPrice);
                updatedValue.Add(EventBudgetItemFieldName_TypeOfExpense, viewModel.TypeOfExpense);
                updatedValue.Add(EventBudgetItemFieldName_AmountPerItem, viewModel.AmountPerItem);
                updatedValue.Add(EventBudgetItemFieldName_DirectPayment, viewModel.DirectPayment);
                updatedValue.Add(EventBudgetItemFieldName_SCA, viewModel.SCA);
                updatedValue.Add(EventBudgetItemFieldName_Remarks, viewModel.Remarks);
                updatedValue.Add(EventBudgetItemFieldName_Description, viewModel.Description);

                try
                {
                    if (Item.CheckIfCreated(viewModel))
                    {
                        SPConnector.AddListItem(ListName_EventBudgetItem, updatedValue, siteUrl);
                    }
                    else
                    {
                        SPConnector.UpdateListItem(ListName_EventBudgetItem, viewModel.ID, updatedValue, siteUrl);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void CreateAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                var type = doc.FileName.Split('-')[0].Trim();

                updateValue.Add(EventBudgetDocuments_EventBudgetId, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
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

        public async Task UpdateRequisitionNoteAsync(string siteUrl = null, int id = 0)
        {
            UpdateRequisitionNote(siteUrl, id);
        }
        public async Task UpdateSCAVoucherAsync(string siteUrl = null, int id = 0)
        {
            UpdateSCAVoucher(siteUrl, id);
        }

        /// <summary>
        /// Automatically updates existing Requsition Note based based on a change in an event budget
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="id">Request Note Id</param>
        public void UpdateRequisitionNote(string siteUrl = null, int id = 0)
        {
            if (id == 0)
            {
                throw new Exception("Invalid parameters.");
            }

            IRequisitionNoteService reqNoteService = new RequisitionNoteService(siteUrl);
            
            RequisitionNoteVM rnHeader = reqNoteService.Get(id);
            EventBudgetVM ebHeader = Get(rnHeader.EventBudgetNo.Value);

            //if (ebHeader.TransactionStatus.Value == TransactionStatusComboBoxVM.Locked)
            //    throw new Exception("Cannot update Requisition Note for a Locked Event Budget");

            // copy EB updated values to RN
            rnHeader.Project.Value = ebHeader.Project.Value;
            rnHeader.Total = ebHeader.TotalDirectPayment;
            rnHeader.TransactionStatus = ebHeader.TransactionStatus.Text;

            // delete all existing details in RN
            foreach (var rnDetail in rnHeader.ItemDetails)
            {
                reqNoteService.DeleteDetail((int)rnDetail.ID);
            }

            //copy all new details from EB
            List<RequisitionNoteItemVM> detail = new List<RequisitionNoteItemVM>();
            foreach (var ebDetail in ebHeader.ItemDetails)
            {
                if (ebDetail.DirectPayment > 0)
                {
                    detail.Add(new RequisitionNoteItemVM()
                    {
                        Activity = new AjaxComboBoxVM() { Value = Convert.ToInt32(ebHeader.Activity.Value), Text = ebHeader.Activity.Text },
                        WBS = new AjaxComboBoxVM() { Value = ebDetail.WBS.Value, Text = ebDetail.WBS.Text },
                        GL = new AjaxComboBoxVM() { Value = ebDetail.GL.Value, Text = ebDetail.GL.Text },
                        Specification = ebDetail.Description,
                        Quantity = ebDetail.Quantity,
                        Frequency = ebDetail.Frequency,
                        Price = ebDetail.UnitPrice,
                        EditMode = (int)Item.Mode.CREATED,
                        IsFromEventBudget = true,
                        Total = ebDetail.Frequency * ebDetail.UnitPrice * ebDetail.Quantity
                    });
                }
            }

            try
            {
                reqNoteService.UpdateRequisitionNote(rnHeader);
                reqNoteService.CreateRequisitionNoteItems(rnHeader.ID, detail);
            }
            catch (Exception e)
            {
                throw new Exception("Update Requisition note failed.");
            }
            // attachment?
        }


        public void UpdateSCAVoucher(string siteUrl = null, int id = 0)
        {
            if (id == 0)
            {
                throw new Exception("Invalid parameters.");
            }

            ISCAVoucherService scaVoucherService = new SCAVoucherService(siteUrl);
            
            SCAVoucherVM scaVoucherHeader = scaVoucherService.Get(id);
            EventBudgetVM ebHeader = Get(scaVoucherHeader.EventBudgetID);

            //if (ebHeader.TransactionStatus.Value == TransactionStatusComboBoxVM.Locked)
            //    throw new Exception("Cannot update Requisition Note for a Locked Event Budget");

            // copy EB updated values to RN
            scaVoucherHeader.Project = ebHeader.Project.Value;
			scaVoucherHeader.TotalAmount = ebHeader.TotalSCA;
            scaVoucherHeader.TransactionStatus.Value = ebHeader.TransactionStatus.Value;

            // delete all existing details in RN
            foreach (var rnDetail in scaVoucherHeader.EventBudgetItems)
            {
                scaVoucherService.DeleteDetail((int)rnDetail.ID);
            }

            //copy all new details from EB
            List<SCAVoucherItemsVM> d = new List<SCAVoucherItemsVM>();
            foreach (var ebDetail in ebHeader.ItemDetails)
            {
                if (ebDetail.SCA > 0)
                {
                    d.Add(new SCAVoucherItemsVM()
                    {
                        WBSID = ebDetail.WBS.Value.Value,
                        WBS = ebDetail.WBS.Text,
                        GLID = ebDetail.GL.Value.Value,
                        GL = ebDetail.GL.Text,
                        Amount = ebDetail.Frequency * ebDetail.UnitPrice * ebDetail.Quantity
                    });
                }
            }

            try
            {
                scaVoucherService.UpdateSCAVoucher(scaVoucherHeader);
                scaVoucherService.CreateSCAVoucherItems(siteUrl, scaVoucherHeader.ID, d);
            }
            catch(Exception ex)
            {
                throw new Exception("Update SCA Voucher failed.");
            }
            // attachment?
        }

        #region Supply data to Landing Page

        public static IEnumerable<LPBudgetVsActualDisbursementVM> GetLatestMonthBudgetActualDisbursement(DateTime period, string mcaDevSiteURL)
        {
            var result = new List<LPBudgetVsActualDisbursementVM>();
            var caml = @"<View><Query><Where><And> <Eq><FieldRef Name='" + 
                        BudgetActualDisbursementYear + "' /><Value Type='Lookup'>" + 
                        period.Year.ToString() + "</Value></Eq> " +
                        "<Eq><FieldRef Name='" +
                        BudgetActualDisbursementMonth  + "' /><Value Type='Lookup'>" +
                        period.ToString("MMMM", CultureInfo.InvariantCulture) + "</Value></Eq>"
                        + "</And></Where></Query></View>";

            string valuesText = string.Empty;
            string camlGetActiviy = string.Empty;

            foreach (var item in SPConnector.GetList(ListNameLandingPageBudgetAactualDisbursement, mcaDevSiteURL, caml))
            {
                if (item[BudgetActualDisbursementWBSID] != null)
                {
                    string wbsIdValue = (item[BudgetActualDisbursementWBSID] as FieldLookupValue).LookupValue;
                    decimal budgetUsd = Convert.ToDecimal(item[BudgetActualDisbursementBudget]);
                    decimal actualUSD = Convert.ToDecimal(item[BudgetActualDisbursementActual]);

                    if (budgetUsd > 0 || actualUSD > 0)
                    {
                        LPBudgetVsActualDisbursementVM existingData = result.FirstOrDefault(m => m.WBSID == wbsIdValue);
                        if (existingData != null)
                        {
                            existingData.BudgetUSD += budgetUsd;
                            existingData.ActualUSD += actualUSD;
                        }
                        else
                        {
                            LPBudgetVsActualDisbursementVM itemBAD = new LPBudgetVsActualDisbursementVM();
                            itemBAD.WBSID = wbsIdValue;
                            itemBAD.BudgetUSD = budgetUsd;
                            itemBAD.ActualUSD = actualUSD;
                            result.Add(itemBAD);
                            valuesText += "<Value Type='Lookup'>" + wbsIdValue + "</Value>";
                        }
                    }
                }
            }

            if (result.Count > 0)
            {
                camlGetActiviy = @"<View><Query><Where><In><FieldRef Name='" + WBSMappingWBSID + "' /><Values>" +
                                   valuesText + "</Values></In></Where></Query></View>";
                //get activity name
                foreach (var item in SPConnector.GetList(ListNameWBSMapping, mcaDevSiteURL, camlGetActiviy))
                {
                    string wbsId = Convert.ToString(item[WBSMappingWBSID]);
                    var listMatch = result.Where(p => p.WBSID == wbsId).ToList();
                    if (listMatch.Count > 0)
                    {
                        foreach (var itemMatch in listMatch)
                        {
                            itemMatch.ProjectOrActivity = Convert.ToString(item[WBSMappingActivity]);
                        }
                    }
                }
            }
            return result;
        }
        #endregion


        private EventBudgetVM ConvertToEventBudgetVM(ListItem listItem)
        {
            var eventBudget = new EventBudgetVM();

            eventBudget.ID = Convert.ToInt32(listItem[EventBudgetFieldName_ID]);
            eventBudget.EventName = Convert.ToString(listItem[EventBudgetFieldName_EventName]);
            eventBudget.No = Convert.ToString(listItem[EventBudgetFieldName_No]);

            eventBudget.DateFrom = Convert.ToDateTime(listItem[EventBudgetFieldName_DateFrom]);
            eventBudget.DateTo = Convert.ToDateTime(listItem[EventBudgetFieldName_DateTo]);

            eventBudget.Project.Text = Convert.ToString(listItem[EventBudgetFieldName_Project]);
            eventBudget.Project.Value = Convert.ToString(listItem[EventBudgetFieldName_Project]);

            if (listItem[EventBudgetFieldName_ActivityName] != null)
            {
                eventBudget.Activity.Value = (listItem[EventBudgetFieldName_ActivityName] as FieldLookupValue).LookupId;
                eventBudget.Activity.Text = (listItem[EventBudgetFieldName_ActivityName] as FieldLookupValue).LookupValue;
            }

            eventBudget.Venue = Convert.ToString(listItem[EventBudgetFieldName_Venue]);
            eventBudget.Rate = Convert.ToDecimal(listItem[EventBudgetFieldName_ExhangeRate]);
            eventBudget.TotalDirectPayment = Convert.ToDecimal(listItem[EventBudgetFieldName_TotalDirectPaymentIDR]);
            eventBudget.TotalSCA = Convert.ToDecimal(listItem[EventBudgetFieldName_TotalSCAIDR]);
            eventBudget.TotalIDR = Convert.ToDecimal(listItem[EventBudgetFieldName_TotalIDR]);
            eventBudget.TotalUSD = Convert.ToDecimal(listItem[EventBudgetFieldName_TotalUSD]);

            if (eventBudget.Rate > 0)
            {
                eventBudget.TotalDirectPaymentUSD = eventBudget.TotalDirectPayment / eventBudget.Rate;
                eventBudget.TotalSCAUSD = eventBudget.TotalSCA / eventBudget.Rate;
            }

            eventBudget.TransactionStatus.Value = Convert.ToString(listItem[EventBudgetFieldName_TransactionStatus]);
            eventBudget.UserEmail = Convert.ToString(listItem[EventBudgetFieldName_UserEmail]);
            eventBudget.DocumentUrl = GetDocumentUrl(siteUrl, eventBudget.ID);

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

        private EventBudgetVM ConvertToEventBudgetVMForList(ListItem listItem)
        {
            var eventBudget = new EventBudgetVM();

            eventBudget.ID = Convert.ToInt32(listItem[EventBudgetFieldName_ID]);
            eventBudget.Title = Convert.ToString(listItem[ActivityFieldName_Name]);
            eventBudget.No = Convert.ToString(listItem[EventBudgetFieldName_No]);
            eventBudget.Project.Text = Convert.ToString(listItem[EventBudgetFieldName_Project]);

            return eventBudget;
        }

        private string GetDocumentUrl(string siteUrl, int? iD)
        {
            return string.Format(FINEventBudgetDocumentByID, siteUrl, iD);
        }

    }
}
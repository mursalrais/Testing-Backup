using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class SCAVoucherService : ISCAVoucherService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        private const string LIST_NAME_SCAVOUCHER = "SCA Voucher";
        private const string LIST_NAME_SCAVOUCHER_ITEM = "SCA Voucher Item";
        private const string LIST_NAME_EVENT_BUDGET = "Event Budget";
        private const string LIST_NAME_EVENT_BUDGET_ITEM = "Event Budget Item";
        private const string DOC_LIST_NAME = "SCA Voucher Documents";

        private const string FIELD_NAME_SCAVOUCHER = "SCAVoucher";
        private const string FIELD_NAME_SCA_NO = "Title";
        private const string FIELD_NAME_DATE = "rmxv";
        private const string FIELD_NAME_SDOID = "SDO_x0020_ID";
        private const string FIELD_NAME_SDO_NAME = "SDO_x0020_ID_x003a_Name";
        private const string FIELD_NAME_POSITION = "SDO_x0020_ID_x003a_Position";
        private const string FIELD_NAME_EBUDGET_ID = "Event_x0020_Budget_x0020_ID";
        private const string FIELD_NAME_CURRENCY = "dz1b";
        private const string FIELD_NAME_TOTAL_AMOUNT = "q34t";
        private const string FIELD_NAME_TA_WORDS = "df4j";
        private const string FIELD_NAME_PURPOSE = "fsjg";
        private const string FIELD_NAME_PROJECT = "_x0066_iq1";
        private const string FIELD_NAME_ACTIVITY_ID = "Activity_x0020_ID";
        private const string FIELD_NAME_ACTIVITY_NAME = "Activity_x0020_ID_x003a_Name";
        private const string FIELD_NAME_SUB_ACTIVITY_ID = "Sub_x0020_Activity_x0020_ID";
        private const string FIELD_NAME_SUB_ACTIVITY_NAME = "Sub_x0020_Activity_x0020_ID_x003";
        private const string FIELD_NAME_FUND = "lzhg";
        private const string FIELD_NAME_REFFERENCE_NO = "cicv";
        private const string FIELD_NAME_REMARKS = "p7up";
        private const string FIELD_NAME_WBS = "WBS_x0020_Master_x0020_ID";
        private const string FIELD_NAME_GL = "GL_x0020_Master_x0020_ID";
        private const string FIELD_NAME_TITLE = "Title";
        private const string FIELD_NAME_AMOUNT = "Amount";
        private const string LIST_NAME_SCAVOUCHER_DOC = "SCA_x0020_Voucher";

        private const string EventBudgetFieldName_DateFrom = "Date_x0020_From";
        private const string EventBudgetFieldName_DateTo = "Date_x0020_To";
        private const string EventBudgetFieldName_Project = "Project";
        private const string EventBudgetFieldName_ActivityID = "Activity_x0020_ID0";
        private const string EventBudgetFieldName_ActivityName = "Activity_x0020_ID_x003a_Name";

        private string _siteUrl = string.Empty;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public int GetActivityIDByEventBudgetID(int eventBudgetID)
        {
            int activityID = 0;
            if (eventBudgetID > 0)
            {
                var list = SPConnector.GetListItem(LIST_NAME_EVENT_BUDGET, eventBudgetID, _siteUrl);
                activityID = list["Activity_x0020_ID0"] == null ? 0 :
                        Convert.ToInt32((list["Activity_x0020_ID0"] as FieldLookupValue).LookupId);
            }

            return activityID;
        }

        public int? CreateSCAVoucher(SCAVoucherVM scaVoucher)
        {
            int? result = null;
            DateTime today = DateTime.Now;
            string scaNo = DocumentNumbering.Create(_siteUrl, string.Format("SCA/{0}-{1}/", DateTimeExtensions.GetMonthInRoman(today), today.ToString("yy")) + "{0}");
            var columnValues = new Dictionary<string, object>
            {
                {FIELD_NAME_SCA_NO,scaNo},
                {FIELD_NAME_DATE,scaVoucher.SCAVoucherDate},
                {FIELD_NAME_SDOID,new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.SDO.Value) }},
                {FIELD_NAME_EBUDGET_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.EventBudgetNo.Value) }},
                {FIELD_NAME_CURRENCY,scaVoucher.Currency},
                {FIELD_NAME_TOTAL_AMOUNT,scaVoucher.TotalAmount},
                {FIELD_NAME_TA_WORDS,scaVoucher.TotalAmountInWord},
                {FIELD_NAME_PURPOSE,scaVoucher.Purpose},
                {FIELD_NAME_PROJECT,scaVoucher.Project},
                {FIELD_NAME_ACTIVITY_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.ActivityID) } },
                {FIELD_NAME_SUB_ACTIVITY_ID,new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.SubActivity.Value) }},
                {FIELD_NAME_FUND,scaVoucher.Fund},
                {FIELD_NAME_REFFERENCE_NO,scaVoucher.RefferenceNo},
                {FIELD_NAME_REMARKS,scaVoucher.Remarks}
            };

            try
            {
                SPConnector.AddListItem(LIST_NAME_SCAVOUCHER, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(LIST_NAME_SCAVOUCHER, _siteUrl);

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public async Task CreateSCAVoucherItem(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                var columnValues = new Dictionary<string, object>
                {
                    {FIELD_NAME_TITLE,scaVoucherID},
                    {FIELD_NAME_SCAVOUCHER,scaVoucherID},
                    {FIELD_NAME_WBS,new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBSID) }},
                    {FIELD_NAME_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GLID) }},
                    {FIELD_NAME_AMOUNT,viewModel.Amount}
                };
                try
                {
                    SPConnector.AddListItem(LIST_NAME_SCAVOUCHER_ITEM, columnValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public async Task CreateSCAVoucherDocumentAsync(int? ID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateSCAVoucherAttachment(ID, documents);
        }

        public SCAVoucherVM GetSCAVoucherVMData(int? ID)
        {
            var viewModel = new SCAVoucherVM();
            var viewModels = new List<SCAVoucherItemsVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='SCAVoucher' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            if (ID != null)
            {
                var list = SPConnector.GetListItem(LIST_NAME_SCAVOUCHER, ID, _siteUrl);
                viewModel = ConvertToSCAVoucherVM(list);
            }

            return viewModel;
        }

        public IEnumerable<SCAVoucherItemsVM> GetSCAVoucherItems(int scaVoucherID)
        {
            var scaVoucherItemsVM = new List<SCAVoucherItemsVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='SCAVoucher' /><Value Type='Lookup'>" + scaVoucherID.ToString() + "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(LIST_NAME_SCAVOUCHER_ITEM, _siteUrl, caml))
            {
                scaVoucherItemsVM.Add(
                    new SCAVoucherItemsVM
                    {
                        ID = Convert.ToInt32(item["ID"]),
                        WBS = string.Format("{0} - {1}", (item["WBS_x0020_Master_x0020_ID_x003a_"] as FieldLookupValue).LookupValue.ToString(), (item["WBS_x0020_Master_x0020_ID_x003a_0"] as FieldLookupValue).LookupValue.ToString()),
                        GL = string.Format("{0} - {1}", (item["GL_x0020_Master_x0020_ID_x003a_G"] as FieldLookupValue).LookupValue.ToString(), (item["GL_x0020_Master_x0020_ID_x003a_G0"] as FieldLookupValue).LookupValue.ToString()),
                        Amount = Convert.ToInt32(item[FIELD_NAME_AMOUNT])
                    }
                );
            }

            return scaVoucherItemsVM;
        }

        public IEnumerable<SCAVoucherItemsVM> GetEventBudgetItems(int eventBudgetID)
        {
            var scaVoucherItemsVM = new List<SCAVoucherItemsVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Event_x0020_Budget_x0020_ID' /><Value Type='Lookup'>" + eventBudgetID.ToString() + "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(LIST_NAME_EVENT_BUDGET_ITEM, _siteUrl, caml))
            {
                scaVoucherItemsVM.Add(
                    new SCAVoucherItemsVM
                    {
                        ID = Convert.ToInt32(item["ID"]),
                        WBSID = Convert.ToInt32((item["WBS_x0020_Master_x002e_ID"] as FieldLookupValue).LookupId),
                        WBS = string.Format("{0} - {1}", (item["WBS_x0020_Master_x002e_ID"] as FieldLookupValue).LookupValue.ToString(), (item["WBS_x0020_Master_x002e_ID_x003a_"] as FieldLookupValue).LookupValue.ToString()),
                        GLID = Convert.ToInt32((item["GL_x0020_Master_x002e_ID_x003a_G"] as FieldLookupValue).LookupId),
                        GL = string.Format("{0} - {1}", (item["GL_x0020_Master_x002e_ID_x003a_G"] as FieldLookupValue).LookupValue.ToString(), (item["GL_x0020_Master_x002e_ID_x003a_G0"] as FieldLookupValue).LookupValue.ToString()),
                        Amount = Convert.ToDecimal(item["Quantity"]) * Convert.ToDecimal(item["UoMQuantity"]),
                    }
                );
            }

            return scaVoucherItemsVM;
        }

        public SCAVoucherVM GetEventBudget(int? ID)
        {
            SCAVoucherVM scaVoucherVM = new SCAVoucherVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(LIST_NAME_EVENT_BUDGET, ID, _siteUrl);

                scaVoucherVM = ConvertFromEventBudget(listItem);
            }

            return scaVoucherVM;
        }

        private SCAVoucherVM ConvertToSCAVoucherVM(ListItem ListItem)
        {
            return new SCAVoucherVM()
            {
                SCAVoucherNo = ListItem[FIELD_NAME_SCA_NO].ToString(),
                SCAVoucherDate = Convert.ToDateTime(ListItem[FIELD_NAME_DATE].ToString()),
                EventBudgetID = Convert.ToInt32((ListItem[FIELD_NAME_EBUDGET_ID] as FieldLookupValue).LookupId.ToString()),
                SDOName = (ListItem[FIELD_NAME_SDO_NAME] as FieldLookupValue).LookupValue.ToString(),
                Position = (ListItem[FIELD_NAME_POSITION] as FieldLookupValue).LookupValue.ToString(),
                TotalAmount = Convert.ToDecimal(ListItem[FIELD_NAME_TOTAL_AMOUNT].ToString()),
                TotalAmountInWord = ListItem[FIELD_NAME_TA_WORDS].ToString(),
                Purpose = ListItem[FIELD_NAME_PURPOSE].ToString(),
                Project = ListItem[FIELD_NAME_PROJECT].ToString(),
                ActivityID = Convert.ToInt32((ListItem[FIELD_NAME_ACTIVITY_NAME] as FieldLookupValue).LookupId.ToString()),
                ActivityName = (ListItem[FIELD_NAME_ACTIVITY_NAME] as FieldLookupValue).LookupValue.ToString(),
                SubActivityName = (ListItem[FIELD_NAME_SUB_ACTIVITY_NAME] as FieldLookupValue).LookupValue.ToString(),
                Fund = Convert.ToDecimal(ListItem[FIELD_NAME_FUND])
            };
        }

        private SCAVoucherVM ConvertFromEventBudget(ListItem listItem)
        {
            return new SCAVoucherVM()
            {
                Currency = "IDR",
                Purpose = string.Format("{0} - {1} - {2}", Convert.ToString(listItem[FIELD_NAME_TITLE]), Convert.ToDateTime(listItem[EventBudgetFieldName_DateFrom]).ToString("dd/MM/yyyy"), Convert.ToDateTime(listItem[EventBudgetFieldName_DateTo]).ToString("dd/MM/yyyy")),
                Project = Convert.ToString(listItem[EventBudgetFieldName_Project]),
                ActivityID = Convert.ToInt32((listItem[EventBudgetFieldName_ActivityID] as FieldLookupValue).LookupId.ToString()),
                ActivityName = (listItem[EventBudgetFieldName_ActivityName] as FieldLookupValue).LookupValue.ToString()
            };
        }

        private void CreateSCAVoucherAttachment(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(LIST_NAME_SCAVOUCHER_DOC, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });
                        try
                        {
                            SPConnector.UploadDocument(DOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }
            }
        }
    }
}

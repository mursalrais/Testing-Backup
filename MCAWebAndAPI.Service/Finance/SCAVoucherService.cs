using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN06: SCA Voucher
    ///     i.e.: Special Cash Advance Voucher
    /// </summary>

    public class SCAVoucherService : ISCAVoucherService
    {
        #region List Definition

        private const string LIST_NAME_SCAVOUCHER = "SCA Voucher";
        private const string LIST_NAME_SCAVOUCHER_ITEM = "SCA Voucher Item";
        private const string LIST_NAME_EVENT_BUDGET = "Event Budget";
        private const string LIST_NAME_EVENT_BUDGET_ITEM = "Event Budget Item";
        private const string LIST_NAME_SCA_DOCUMENT = "SCA Voucher Documents";
        private const string LIST_NAME_SCAVOUCHER_DOC = "SCA_x0020_Voucher";

        #endregion List Definition

        #region List Field Definition

        private const string FIELD_NAME_ID = "ID";
        private const string FIELD_NAME_SCAVOUCHER = "SCAVoucher";
        private const string FIELD_NAME_SCA_NO = "Title";
        private const string FIELD_NAME_DATE = "rmxv";
        private const string FIELD_NAME_SDOID = "SDOID";
        private const string FIELD_NAME_SDO_NAME = "SDOName";
        private const string FIELD_NAME_SDO_POSITION = "SDOPosition";
        private const string FIELD_NAME_EBUDGET_ID = "Event_x0020_Budget_x0020_ID";
        private const string FIELD_NAME_EBUDGET_NO = "Event_x0020_Budget_x0020_ID_x0031";
        private const string FIELD_NAME_EBUDGET_NAME = "Event_x0020_Budget_x0020_ID_x0030";
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
        private const string FIELD_NAME_USER_EMAIL = "UserEmail";
        private const string FieldName_VisibleTo = "VisibleTo";
        private const string FIELD_NAME_GL = "GL_x0020_Master_x0020_ID";
        private const string FIELD_NAME_TITLE = "Title";
        private const string FIELD_NAME_AMOUNT = "Amount";
        private const string FIELD_NAME_TRANSTATUS = "Transaction_x0020_Status";
        private const string FIELD_NAME_SCA_GL_ID = "GL_x0020_Master_x0020_ID_x003a_G";
        private const string FIELD_NAME_SCA_GL_VALUE = "GL_x0020_Master_x0020_ID_x003a_G0";

        private const string FieldNameItem_WBSID = "WBSID";
        private const string FieldNameItem_WBSDescription = "WBSDescription";

        private const string EVENT_BUDGET_FIELD_NAME_DATE_FROM = "Date_x0020_From";
        private const string EVENT_BUDGET_FIELD_NAME_DATE_TO = "Date_x0020_To";
        private const string EVENT_BUDGET_FIELD_NAME_PROJECT = "Project";
        private const string EVENT_BUDGET_FIELD_NAME_ACTIVITY_ID = "Activity_x0020_ID0";
        private const string EVENT_BUDGET_FIELD_NAME_ACTIVITY_NAME = "Activity_x0020_ID_x003a_Name";
        private const string EVENT_BUDGET_FIELD_WBS_ID = "WBS_x0020_Master_x002e_ID";
        private const string EVENT_BUDGET_FIELD_WBS_VALUE = "WBS_x0020_Master_x002e_ID_x003a_";
        private const string EVENT_BUDGET_FIELD_GL_ID = "GL_x0020_Master_x002e_ID_x003a_G";
        private const string EVENT_BUDGET_FIELD_GL_VALUE = "GL_x0020_Master_x002e_ID_x003a_G0";
        private const string EVENT_BUDGET_FIELD_QUANTITY = "Quantity";
        private const string EVENT_BUDGET_FIELD_UOMQUANTITY = "UoMQuantity";
        private const string EVENT_BUDGET_FIELD_SCA = "SCA";
        private const string EVENT_BUDGET_FIELD_SCA_VALUE = "0";

        #endregion List Field Definition

        public SCAVoucherService(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        private string siteUrl = string.Empty;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public int GetActivityIDByEventBudgetID(int eventBudgetID)
        {
            int activityID = 0;
            if (eventBudgetID > 0)
            {
                var list = SPConnector.GetListItem(LIST_NAME_EVENT_BUDGET, eventBudgetID, siteUrl);
                activityID = list[EVENT_BUDGET_FIELD_NAME_ACTIVITY_ID] == null ? 0 :
                        Convert.ToInt32((list[EVENT_BUDGET_FIELD_NAME_ACTIVITY_ID] as FieldLookupValue).LookupId);
            }

            return activityID;
        }

        public int? CreateSCAVoucher(ref SCAVoucherVM scaVoucher, IEnumerable<ProfessionalMaster> professionals = null)
        {
            int? result = null;
            DateTime today = DateTime.Now;
            string scaNo = DocumentNumbering.Create(siteUrl, string.Format("SCA/{0}-{1}/", DateTimeExtensions.GetMonthInRoman(today), today.ToString("yy")) + "{0}", 5);
            scaVoucher.SCAVoucherNo = scaNo;

            string professionalName = string.Empty;
            string professionalPosition = string.Empty;
            var professionalId = scaVoucher.SDO.Value == null ? 0 : scaVoucher.SDO.Value;

            if (professionalId != 0)
            {
                professionalName = professionals.ToList().Find(p => p.ID == professionalId).Name;
                professionalPosition = professionals.ToList().Find(p => p.ID == professionalId).Position;
            }

            var columnValues = new Dictionary<string, object>();

            columnValues.Add(FIELD_NAME_SCA_NO, scaNo);
            columnValues.Add(FIELD_NAME_DATE, scaVoucher.SCAVoucherDate);
            columnValues.Add(FIELD_NAME_SDOID, professionalId);
            columnValues.Add(FIELD_NAME_SDO_NAME, professionalName);
            columnValues.Add(FIELD_NAME_SDO_POSITION, professionalPosition);
            columnValues.Add(FIELD_NAME_EBUDGET_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.EventBudget.Value) });
            columnValues.Add(FIELD_NAME_CURRENCY, scaVoucher.Currency.Value);
            columnValues.Add(FIELD_NAME_TOTAL_AMOUNT, scaVoucher.TotalAmount);
            columnValues.Add(FIELD_NAME_TA_WORDS, scaVoucher.TotalAmountInWord);
            columnValues.Add(FIELD_NAME_PURPOSE, scaVoucher.Purpose);
            columnValues.Add(FIELD_NAME_PROJECT, scaVoucher.Project);
            columnValues.Add(FIELD_NAME_ACTIVITY_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.ActivityID) });
            columnValues.Add(FIELD_NAME_SUB_ACTIVITY_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.SubActivity.Value) });
            columnValues.Add(FIELD_NAME_FUND, scaVoucher.Fund);
            columnValues.Add(FIELD_NAME_REFFERENCE_NO, scaVoucher.ReferenceNo);
            columnValues.Add(FIELD_NAME_REMARKS, scaVoucher.Remarks);
            columnValues.Add(FIELD_NAME_USER_EMAIL, scaVoucher.UserEmail);
            columnValues.Add(FieldName_VisibleTo, SPConnector.GetUser(scaVoucher.UserEmail, siteUrl));

            try
            {
                SPConnector.AddListItem(LIST_NAME_SCAVOUCHER, columnValues, siteUrl);
                result = SPConnector.GetLatestListItemID(LIST_NAME_SCAVOUCHER, siteUrl);
            }
            catch (ServerException e)
            {
                logger.Error(e.Message + " " + e.ServerErrorValue);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public bool UpdateSCAVoucher(SCAVoucherVM scaVoucher, IEnumerable<ProfessionalMaster> professionals = null)
        {
            bool result = false;
            var user = SPConnector.GetUser(scaVoucher.UserEmail, siteUrl);

            var columnValues = new Dictionary<string, object>
            {
                { FIELD_NAME_DATE,scaVoucher.SCAVoucherDate},
                { FIELD_NAME_EBUDGET_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.EventBudget.Value) }},
                { FIELD_NAME_CURRENCY,scaVoucher.Currency.Value},
                { FIELD_NAME_TOTAL_AMOUNT,scaVoucher.TotalAmount},
                { FIELD_NAME_TA_WORDS,scaVoucher.TotalAmountInWord},
                { FIELD_NAME_PURPOSE,scaVoucher.Purpose},
                { FIELD_NAME_PROJECT,scaVoucher.Project},
                { FIELD_NAME_ACTIVITY_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.ActivityID) } },
                { FIELD_NAME_SUB_ACTIVITY_ID,new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.SubActivity.Value) }},
                { FIELD_NAME_FUND,scaVoucher.Fund},
                { FIELD_NAME_REFFERENCE_NO,scaVoucher.ReferenceNo},
                { FIELD_NAME_REMARKS,scaVoucher.Remarks},
                { FIELD_NAME_USER_EMAIL,scaVoucher.UserEmail},
                { FieldName_VisibleTo, user }
            };

            if (professionals != null)
            {
                string professionalName = string.Empty;
                string professionalPosition = string.Empty;
                int? professionalId = 0;
                professionalId = scaVoucher.SDO.Value == null ? 0 : scaVoucher.SDO.Value;

                if (professionalId != 0)
                {
                    professionalName = professionals.ToList().Find(p => p.ID == professionalId).Name;
                    professionalPosition = professionals.ToList().Find(p => p.ID == professionalId).Position;
                }

                columnValues.Add(FIELD_NAME_SDOID, professionalId);
                columnValues.Add(FIELD_NAME_SDO_NAME, professionalName);
                columnValues.Add(FIELD_NAME_SDO_POSITION, professionalPosition);
            }

            if (scaVoucher.Action == SCAVoucherVM.ActionType.approve.ToString())
            {
                columnValues.Add(FIELD_NAME_TRANSTATUS, scaVoucher.TransactionStatus.Value);
            }

            try
            {
                SPConnector.UpdateListItem(LIST_NAME_SCAVOUCHER, scaVoucher.ID, columnValues, siteUrl);
                result = true;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public bool UpdateStatusSCAVoucher(SCAVoucherVM scaVoucher)
        {
            bool result = false;
            var columnValues = new Dictionary<string, object>
            {
                {FIELD_NAME_TRANSTATUS, scaVoucher.TransactionStatus.Value }
            };

            try
            {
                SPConnector.UpdateListItem(LIST_NAME_SCAVOUCHER, scaVoucher.ID, columnValues, siteUrl);
                result = true;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public async Task CreateSCAVoucherItemAsync(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels)
        {
            CreateSCAVoucherItems(siteUrl, scaVoucherID, viewModels);
        }

        public async Task CreateSCAVoucherAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateSCAVoucherAttachment(siteUrl, ID, documents);
        }

        public async Task UpdateSCAVoucherItem(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels)
        {
            try
            {
                var listItemID = GetIDItemDetails(siteUrl, (int)scaVoucherID);
                SPConnector.DeleteMultipleListItemAsync(LIST_NAME_SCAVOUCHER_ITEM, listItemID, siteUrl);

                CreateSCAVoucherItems(siteUrl, scaVoucherID, viewModels);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }
        }

        public IEnumerable<SCAVoucherVM> GetAll()
        {
            var result = new List<SCAVoucherVM>();

            foreach (var item in SPConnector.GetList(LIST_NAME_SCAVOUCHER, siteUrl))
            {
                result.Add(ConvertToVM(item));
            }

            return result;
        }

        public SCAVoucherVM Get(int? id)
        {
            var viewModel = new SCAVoucherVM();
            var viewModels = new List<SCAVoucherItemsVM>();
            var caml = CamlQueryUtil.Generate(FIELD_NAME_SCAVOUCHER, "Lookup", id.ToString());

            if (id != null)
            {
                var list = SPConnector.GetListItem(LIST_NAME_SCAVOUCHER, id, siteUrl);
                viewModel = ConvertToVM(list);
                viewModel.DocumentUrl = GetDocumentUrl(id);
                viewModel.SCAVoucherItems = GetSCAVoucherItems(id);
            }

            return viewModel;
        }

        public IEnumerable<SCAVoucherItemsVM> GetSCAVoucherItems(int? scaVoucherID)
        {
            var scaVoucherItemsVM = new List<SCAVoucherItemsVM>();
            //var caml = @"<View><Query><Where><Eq><FieldRef Name='SCAVoucher' /><Value Type='Lookup'>" + scaVoucherID.ToString() + "</Value></Eq></Where></Query></View>";
            var caml = CamlQueryUtil.Generate(FIELD_NAME_SCAVOUCHER, "Lookup", scaVoucherID.ToString());

            foreach (var item in SPConnector.GetList(LIST_NAME_SCAVOUCHER_ITEM, siteUrl, caml))
            {
                SCAVoucherItemsVM scvItem = new SCAVoucherItemsVM();

                scvItem.ID = Convert.ToInt32(item[FIELD_NAME_ID]);
                scvItem.GLID = Convert.ToInt32((item[FIELD_NAME_GL] as FieldLookupValue).LookupId.ToString());

                scvItem.WBSID = Convert.ToInt32(item[FieldNameItem_WBSID]);
                WBS wbs = Common.WBSService.Get(siteUrl, scvItem.WBSID);
                scvItem.WBS = wbs.WBSIDDescription;

                scvItem.GL = string.Format("{0} - {1}", (item[FIELD_NAME_SCA_GL_ID] as FieldLookupValue).LookupValue.ToString(), (item[FIELD_NAME_SCA_GL_VALUE] as FieldLookupValue).LookupValue.ToString());
                scvItem.Amount = Convert.ToInt32(item[FIELD_NAME_AMOUNT]);

                scaVoucherItemsVM.Add(scvItem);
            }

            return scaVoucherItemsVM;
        }

        public IEnumerable<SCAVoucherItemsVM> GetEventBudgetItems(int eventBudgetID)
        {
            IEnumerable<EventBudgetItemVM> eventBudgetItems = EventBudgetService.GetItems(siteUrl, eventBudgetID).Where(eb => eb.SCA > 0);

            var scaVoucherItemVMs = new List<SCAVoucherItemsVM>();

            foreach (var ebItem in eventBudgetItems)
            {
                scaVoucherItemVMs.Add(new SCAVoucherItemsVM
                {
                    WBSID = Convert.ToInt32(ebItem.WBS.Value),
                    WBS = Convert.ToString(ebItem.WBS.Text),
                    GLID = Convert.ToInt32(ebItem.GL.Value),
                    GL = Convert.ToString(ebItem.GL.Text),
                    Amount = Convert.ToDecimal(ebItem.SCA)
                });
            }

            return scaVoucherItemVMs;
        }

        public SCAVoucherVM GetEventBudget(int? ID)
        {
            SCAVoucherVM scaVoucherVM = new SCAVoucherVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(LIST_NAME_EVENT_BUDGET, ID, siteUrl);

                scaVoucherVM = ConvertToVMShort(listItem);
            }

            return scaVoucherVM;
        }

        public void DeleteDetail(int id)
        {
            SPConnector.DeleteListItem(LIST_NAME_SCAVOUCHER, id, siteUrl);
        }

        public void CreateSCAVoucherItems(string siteUrl, int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels)
        {
            if (viewModels != null)
            {
                foreach (var viewModel in viewModels)
                {
                    var columnValues = new Dictionary<string, object>
                    {
                        { FIELD_NAME_TITLE,scaVoucherID},
                        { FIELD_NAME_SCAVOUCHER,scaVoucherID},
                        { FieldNameItem_WBSID, Convert.ToInt32(viewModel.WBSID) },
                        { FIELD_NAME_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GLID) }},
                        { FIELD_NAME_AMOUNT,viewModel.Amount}
                    };

                    try
                    {
                        SPConnector.AddListItem(LIST_NAME_SCAVOUCHER_ITEM, columnValues, siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
            }
        }

        public IEnumerable<AjaxComboBoxVM> GetAllAjaxComboBoxVM()
        {
            var result = new List<AjaxComboBoxVM>();
            var vms = GetAll();

            foreach (var item in vms)
            {
                result.Add(
                    new AjaxComboBoxVM
                    {
                        Value = item.ID,
                        Text = item.SCAVoucherNo + " - " + item.Purpose
                    }
                );
            }

            return result;
        }

        public Tuple<int, string> GetIdAndNoByEventBudgetID(int eventBudgetId)
        {
            var sca = GetAll().FirstOrDefault(s => s.EventBudgetID == eventBudgetId);

            return sca == null ? new Tuple<int, string>(0, string.Empty) : new Tuple<int, string>((int)sca.ID, sca.SCAVoucherNo);
        }

        private string GetDocumentUrl(int? ID)
        {
            return string.Format(UrlResource.SCAVoucherDocumentByID, siteUrl, ID);
        }

        private SCAVoucherVM ConvertToVM(ListItem ListItem)
        {
            SCAVoucherVM model = new SCAVoucherVM();
            model.ID = Convert.ToInt32(ListItem[FIELD_NAME_ID]);
            model.SCAVoucherNo = ListItem[FIELD_NAME_SCA_NO].ToString();
            model.SCAVoucherDate = Convert.ToDateTime(ListItem[FIELD_NAME_DATE].ToString());
            model.EventBudgetID = Convert.ToInt32((ListItem[FIELD_NAME_EBUDGET_ID] as FieldLookupValue).LookupId.ToString());
            model.EventBudgetNo = string.Format("{0} - {1}", (ListItem[FIELD_NAME_EBUDGET_NO] as FieldLookupValue).LookupValue.ToString(), (ListItem[FIELD_NAME_EBUDGET_NAME] as FieldLookupValue).LookupValue.ToString());
            model.TotalAmount = Convert.ToDecimal(ListItem[FIELD_NAME_TOTAL_AMOUNT].ToString());
            model.TotalAmountInWord = ListItem[FIELD_NAME_TA_WORDS].ToString();
            model.Purpose = ListItem[FIELD_NAME_PURPOSE].ToString();
            model.Project = ListItem[FIELD_NAME_PROJECT].ToString();
            model.ActivityID = Convert.ToInt32((ListItem[FIELD_NAME_ACTIVITY_NAME] as FieldLookupValue).LookupId.ToString());
            model.ActivityName = (ListItem[FIELD_NAME_ACTIVITY_NAME] as FieldLookupValue).LookupValue.ToString();
            model.SubActivityID = Convert.ToInt32((ListItem[FIELD_NAME_SUB_ACTIVITY_NAME] as FieldLookupValue).LookupId.ToString());
            model.SubActivityName = (ListItem[FIELD_NAME_SUB_ACTIVITY_NAME] as FieldLookupValue).LookupValue.ToString();
            model.Fund = Convert.ToString(ListItem[FIELD_NAME_FUND]);
            model.ReferenceNo = ListItem[FIELD_NAME_REFFERENCE_NO] == null ? "" : ListItem[FIELD_NAME_REFFERENCE_NO].ToString();
            model.Remarks = ListItem[FIELD_NAME_REMARKS] == null ? "" : ListItem[FIELD_NAME_REMARKS].ToString();
            model.UserEmail = ListItem[FIELD_NAME_USER_EMAIL] == null ? "" : ListItem[FIELD_NAME_USER_EMAIL].ToString();
            model.TransactionStatus.Value = ListItem[FIELD_NAME_TRANSTATUS].ToString();
            model.EventBudget.Value = Convert.ToInt32((ListItem[FIELD_NAME_EBUDGET_ID] as FieldLookupValue).LookupId.ToString());

            if (ListItem[FIELD_NAME_SDOID] != null)
            {
                model.SDO.Value = Convert.ToInt32((ListItem[FIELD_NAME_SDOID]));
                model.SDOPosition = Convert.ToString(ListItem[FIELD_NAME_SDO_POSITION]);
                model.SDO.Text = string.Format("{0} - {1}", (Convert.ToString(ListItem[FIELD_NAME_SDO_NAME])), model.SDOPosition);
                model.SDOName = Convert.ToString(ListItem[FIELD_NAME_SDO_NAME]);
            }
            model.SDOPosition = Convert.ToString(ListItem[FIELD_NAME_SDO_POSITION]);
            model.SubActivity.Value = Convert.ToInt32((ListItem[FIELD_NAME_SUB_ACTIVITY_NAME] as FieldLookupValue).LookupId.ToString());
            model.Currency.Value = ListItem[FIELD_NAME_CURRENCY] == null ? "" : ListItem[FIELD_NAME_CURRENCY].ToString();

            return model;
        }

        private SCAVoucherVM ConvertToVMShort(ListItem listItem)
        {
            var model = new SCAVoucherVM();
            //model.Currency.Value = listItem[FIELD_NAME_CURRENCY] == null ? "" : listItem[FIELD_NAME_CURRENCY].ToString();
            model.Purpose = string.Format("{0} - {1} - {2}", Convert.ToString(listItem[FIELD_NAME_TITLE]), Convert.ToDateTime(listItem[EVENT_BUDGET_FIELD_NAME_DATE_FROM]).ToString("dd/MM/yyyy"), Convert.ToDateTime(listItem[EVENT_BUDGET_FIELD_NAME_DATE_TO]).ToString("dd/MM/yyyy"));
            model.Project = Convert.ToString(listItem[EVENT_BUDGET_FIELD_NAME_PROJECT]);
            model.ActivityID = Convert.ToInt32((listItem[EVENT_BUDGET_FIELD_NAME_ACTIVITY_ID] as FieldLookupValue).LookupId.ToString());
            model.ActivityName = (listItem[EVENT_BUDGET_FIELD_NAME_ACTIVITY_NAME] as FieldLookupValue).LookupValue.ToString();

            return model;
        }

        private static void CreateSCAVoucherAttachment(string siteUrl, int? ID, IEnumerable<HttpPostedFileBase> attachment)
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
                            SPConnector.UploadDocument(LIST_NAME_SCA_DOCUMENT, updateValue, doc.FileName, doc.InputStream, siteUrl);
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

        private static List<string> GetIDItemDetails(string siteUrl, int headerID)
        {
            List<string> details = new List<string>();

            if (headerID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FIELD_NAME_SCAVOUCHER + "' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";

                foreach (var item in SPConnector.GetList(LIST_NAME_SCAVOUCHER_ITEM, siteUrl, caml))
                {
                    details.Add(item[FIELD_NAME_ID].ToString());
                }
            }

            return details;
        }
    }
}
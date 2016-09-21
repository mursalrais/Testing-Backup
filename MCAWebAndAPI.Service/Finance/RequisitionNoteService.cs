using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance.RequisitionNote
{
    /// <summary>
    /// Wireframe FIN05: Requisition Note
    ///     i.e.: Purchase Requisition Note
    /// </summary>

    public class RequisitionNoteService : IRequisitionNoteService
    {
        #region Constants

        private const string ListName_GLMaster = "GL Master";
        private const string ListName_WBSMaster = "WBS Master";
        private const string ListName_Activity = "Activity";
        private const string ListName_SubActivity = "Sub Activity";
        private const string ListName_RequisitionNote = "Requisition Note";
        private const string ListName_RequisitionNoteItem = "Requisition Note item";
        private const string ListName_Attachment = "Requisition Note Documents";

        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";
        private const string FIELD_GL_DESCRIPTION = "yyxi";
        private const string FIELD_WBS_DESCRIPTION = "WBSDesc";

        private const string FIELD_FORMAT_DOC = "RN/{0}-{1}/";
        private const string FIELD_REQUISITION_CATEGORY = "Category";
        private const string FIELD_REQUISITION_DATE = "Date";
        private const string FIELD_REQUISITION_EVENTBUDGETNO = "Event_x0020_Budget";
        private const string FIELD_REQUISITION_PROJECT = "Project";
        private const string FIELD_REQUISITION_FUND = "Fund";
        private const string FIELD_REQUISITION_CURRENCY = "Currency";
        private const string FIELD_REQUISITION_TOTAL = "Total";
        private const string FIELD_MODIFIED = "Modified";
        private const string FIELD_CREATED = "Created";
        private const string FIELD_USER_EMAIL = "UserEmail";
        private const string FieldName_VisibleTo = "VisibleTo";

        private const string FIELD_RN_HEADERID = "Requisition_x0020_Note_x0020_ID";
        private const string FIELD_RN_ACTIVITY = "Activity";
        private const string FIELD_RN_WBS = "WBS_x0020_ID";
        private const string FIELD_RN_WBS_ID = "WBS_x0020_ID_x003a_WBS_x0020_ID_";
        private const string FIELD_RN_WBS_DESC = "WBS_x0020_ID_x003a_WBS_x0020_Des";
        private const string FIELD_RN_GL = "GL_x0020_ID";
        private const string FIELD_RN_GL_ID = "GL_x0020_ID_x003a_GL_x0020_No_x0";
        private const string FIELD_RN_GL_DESC = "GL_x0020_ID_x003a_GL_x0020_Descr";
        private const string FIELD_RN_QUANTITY = "Quantity";
        private const string FIELD_RN_FREQUENCY = "Frequency";
        private const string FIELD_RN_PRICE = "Price";
        private const string FIELD_RN_TOTAL = "Total_x0020_Per_x0020_Item";
        private const string FIELD_RN_DOCUMENTS_HEADERID = "_x0027_Requisition_x0020_Note_x0027_";
        private const string FIELD_RN_EDITOR = "Editor";
        private const string ACTIVTY_PROJECT_NAME = "Project";
        private const string ACTIVITYID_SUBACTIVITY = "Activity_x003a_ID";
        private const string WBS_SUBACTIVITY_ID = "Sub_x0020_Activity_x003a_ID";
        private const string FieldName_TransactionStatus = "TransactionStatus";

        private const string FieldNameItem_WBSID = "WBSID";
        private const string FieldNameItem_WBSIdDescription = "WBSIdDescription";

        #endregion Constants

        private string siteUrl = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public RequisitionNoteService(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public async Task<RequisitionNoteVM> GetAsync(int? ID)
        {
            return Get(ID);
        }

        public RequisitionNoteVM Get(int? ID)
        {
            var viewModel = new RequisitionNoteVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(ListName_RequisitionNote, ID, siteUrl);
                viewModel = ConvertToRequisitionNoteVM(listItem);

                viewModel.ItemDetails = GetRequisitionNoteItemDetails(ID.Value);

                if (viewModel.EventBudgetNo.Value.HasValue)
                {
                    foreach (var item in viewModel.ItemDetails)
                    {
                        item.IsFromEventBudget = true;
                    }
                }
            }

            return viewModel;
        }

        public IEnumerable<GLMasterVM> GetGLMasters()
        {
            var glMasters = new List<GLMasterVM>();

            foreach (var item in SPConnector.GetList(ListName_GLMaster, siteUrl, null))
            {
                glMasters.Add(ConvertToGLMasterModel(item));
            }

            return glMasters;
        }

        //[Obsolete]
        //public IEnumerable<WBSMasterVM> GetWBSMaster(string activity)
        //{
        //    var camlGetSubactivity = @"<View><Query><Where><Eq><FieldRef Name='" + ACTIVITYID_SUBACTIVITY + "' /><Value Type='Lookup'>" +
        //         (activity == null ? string.Empty : activity.ToString()) + "</Value></Eq></Where></Query></View>";

        //    string valuesText = string.Empty;
        //    ListItemCollection subActivityLits = SPConnector.GetList(ListName_SubActivity, siteUrl, camlGetSubactivity);
        //    string camlGetWbs = string.Empty;

        //    if (subActivityLits.Count > 0)
        //    {
        //        foreach (var item in subActivityLits)
        //        {
        //            valuesText += "<Value Type='Lookup'>" + Convert.ToString(item[FIELD_ID]) + "</Value>";
        //        }
        //    }
        //    else
        //    {
        //        //if isempty
        //        valuesText += "<Value Type='Lookup'>-1</Value>";
        //    }

        //    camlGetWbs = @"<View><Query><Where><In><FieldRef Name='" + WBS_SUBACTIVITY_ID + "' /><Values>" +
        //                        valuesText + "</Values></In></Where></Query></View>";
        //    var wbsMasters = new List<WBSMasterVM>();

        //    foreach (var item in SPConnector.GetList(ListName_WBSMaster, siteUrl, camlGetWbs))
        //    {
        //        wbsMasters.Add(ConvertToWBSMasterModel(item));
        //    }

        //    return wbsMasters;
        //}

        public int CreateRequisitionNote(RequisitionNoteVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();
            string documentNoFormat = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            updatedValue.Add(FIELD_REQUISITION_CATEGORY, viewModel.Category.Value);
            updatedValue.Add(FIELD_REQUISITION_DATE, viewModel.Date);

            if (viewModel.EventBudgetNo.Value.HasValue)
            {
                updatedValue.Add(FIELD_REQUISITION_EVENTBUDGETNO, viewModel.EventBudgetNo.Value);
            }

            updatedValue.Add(FIELD_REQUISITION_PROJECT, viewModel.Project.Value);
            updatedValue.Add(FIELD_REQUISITION_FUND, viewModel.Fund);
            updatedValue.Add(FIELD_REQUISITION_CURRENCY, viewModel.Currency.Value);
            updatedValue.Add(FIELD_REQUISITION_TOTAL, viewModel.Total);
            updatedValue.Add(FIELD_USER_EMAIL, viewModel.UserEmail);
            updatedValue.Add(FieldName_TransactionStatus, TransactionStatusComboBoxVM.NotLocked);

            updatedValue.Add(FieldName_VisibleTo, SPConnector.GetUser(viewModel.UserEmail, siteUrl));

            string docNo = DocumentNumbering.Create(siteUrl, documentNoFormat, 5);
            updatedValue.Add(FIELD_TITLE, docNo);

            viewModel.Title = docNo;

            try
            {
                SPConnector.AddListItem(ListName_RequisitionNote, updatedValue, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(ListName_RequisitionNote, siteUrl);
        }

        public bool UpdateRequisitionNote(RequisitionNoteVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FIELD_REQUISITION_CATEGORY, viewModel.Category.Value);
            updatedValue.Add(FIELD_REQUISITION_DATE, viewModel.Date);

            if (viewModel.EventBudgetNo.Value.HasValue)
            {
                updatedValue.Add(FIELD_REQUISITION_EVENTBUDGETNO, viewModel.EventBudgetNo.Value);
            }

            updatedValue.Add(FIELD_REQUISITION_PROJECT, viewModel.Project.Value);
            updatedValue.Add(FIELD_REQUISITION_FUND, viewModel.Fund);
            updatedValue.Add(FIELD_REQUISITION_CURRENCY, viewModel.Currency.Value);
            updatedValue.Add(FIELD_REQUISITION_TOTAL, viewModel.Total);
            updatedValue.Add(FIELD_USER_EMAIL, viewModel.UserEmail);
            updatedValue.Add(FieldName_TransactionStatus, viewModel.TransactionStatus);

            updatedValue.Add(FieldName_VisibleTo, SPConnector.GetUser(viewModel.UserEmail, siteUrl));

            try
            {
                SPConnector.UpdateListItem(ListName_RequisitionNote, viewModel.ID, updatedValue, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return true;
        }

        public void CreateRequisitionNoteItems(int? headerID, IEnumerable<RequisitionNoteItemVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add(FIELD_TITLE, viewModel.Specification);
                updatedValue.Add(FIELD_RN_HEADERID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add(FIELD_RN_ACTIVITY, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Activity.Value) });

                updatedValue.Add(FIELD_RN_WBS, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBS.Value) });

                var wbs = Common.WBSMasterService.Get(siteUrl, Convert.ToInt32(viewModel.WBS.Value));
                updatedValue.Add(FieldNameItem_WBSID, Convert.ToInt32(viewModel.WBS.Value));
                updatedValue.Add(FieldNameItem_WBSIdDescription, wbs.WBSIDDescription);

                updatedValue.Add(FIELD_RN_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(FIELD_RN_QUANTITY, viewModel.Quantity);
                updatedValue.Add(FIELD_RN_FREQUENCY, viewModel.Frequency);
                updatedValue.Add(FIELD_RN_PRICE, viewModel.Price);
                updatedValue.Add(FIELD_RN_TOTAL, viewModel.Total);

                try
                {
                    SPConnector.AddListItem(ListName_RequisitionNoteItem, updatedValue, siteUrl);
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

                updateValue.Add(FIELD_RN_DOCUMENTS_HEADERID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
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

        public void EditRequisitionNoteItems(int? headerID, IEnumerable<RequisitionNoteItemVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(ListName_RequisitionNoteItem, viewModel.ID, siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add(FIELD_TITLE, viewModel.Specification);
                updatedValue.Add(FIELD_RN_HEADERID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add(FIELD_RN_ACTIVITY, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Activity.Value) });

                updatedValue.Add(FIELD_RN_WBS, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBS.Value) });

                var wbs = Common.WBSMasterService.Get(siteUrl, Convert.ToInt32(viewModel.WBS.Value));
                updatedValue.Add(FieldNameItem_WBSID, Convert.ToInt32(viewModel.WBS.Value));
                updatedValue.Add(FieldNameItem_WBSIdDescription, wbs.WBSIDDescription);

                updatedValue.Add(FIELD_RN_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(FIELD_RN_QUANTITY, viewModel.Quantity);
                updatedValue.Add(FIELD_RN_FREQUENCY, viewModel.Frequency);
                updatedValue.Add(FIELD_RN_PRICE, viewModel.Price);
                updatedValue.Add(FIELD_RN_TOTAL, viewModel.Total);

                try
                {
                    if (Item.CheckIfCreated(viewModel))
                    {
                        SPConnector.AddListItem(ListName_RequisitionNoteItem, updatedValue, siteUrl);
                    }
                    else
                    {
                        SPConnector.UpdateListItem(ListName_RequisitionNoteItem, viewModel.ID, updatedValue, siteUrl);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void EditRequisitionNoteAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                var type = doc.FileName.Split('-')[0].Trim();

                updateValue.Add(FIELD_RN_DOCUMENTS_HEADERID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
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

        public async Task CreateRequisitionNoteItemsAsync(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems)
        {
            CreateRequisitionNoteItems(headerID, noteItems);
        }

        public async Task CreateRequisitionNoteAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateRequisitionNoteAttachments(headerID, documents);
        }

        public async Task EditRequisitionNoteItemsAsync(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems)
        {
            EditRequisitionNoteItems(headerID, noteItems);
        }

        public async Task EditRequisitionNoteAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            EditRequisitionNoteAttachments(headerID, documents);
        }

        public void DeleteDetail(int id)
        {
            SPConnector.DeleteListItem(ListName_RequisitionNoteItem, id, siteUrl);
        }

        public Tuple<int, string> GetIdAndNoByEventBudgetID(int eventBudgetId)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FIELD_REQUISITION_EVENTBUDGETNO + "' /><Value Type='Lookup'>" + eventBudgetId.ToString() + "</Value></Eq></Where></Query> <RowLimit>1</RowLimit> </View>";
            string number = string.Empty;
            int id = 0;

            foreach (var item in SPConnector.GetList(ListName_RequisitionNote, siteUrl, caml))
            {
                number = Convert.ToString(item[FIELD_TITLE]);
                id = Convert.ToInt32(item[FIELD_ID]);
            }

            return new Tuple<int, string>(id, number);
        }

        private ActivityVM ConvertToActivityModel(ListItem item)
        {
            return new ActivityVM
            {
                ID = Convert.ToInt32(item[FIELD_ID]),
                Title = Convert.ToString(item[FIELD_TITLE])
            };
        }

        private GLMasterVM ConvertToGLMasterModel(ListItem item)
        {
            return new GLMasterVM
            {
                ID = Convert.ToInt32(item[FIELD_ID]),
                Title = Convert.ToString(item[FIELD_TITLE]),

                GLDescription = Convert.ToString(item[FIELD_GL_DESCRIPTION]),
            };
        }

        private WBSMasterVM ConvertToWBSMasterModel(ListItem item)
        {
            return new WBSMasterVM
            {
                ID = Convert.ToInt32(item[FIELD_ID]),
                Title = Convert.ToString(item[FIELD_TITLE]),
                WBSDescription = Convert.ToString(item[FIELD_WBS_DESCRIPTION])
            };
        }

        private RequisitionNoteVM ConvertToRequisitionNoteVM(ListItem listItem)
        {
            RequisitionNoteVM viewModel = new RequisitionNoteVM();

            viewModel.ID = Convert.ToInt32(listItem[FIELD_ID]);
            viewModel.Title = Convert.ToString(listItem[FIELD_TITLE]);
            viewModel.Category.Value = Convert.ToString(listItem[FIELD_REQUISITION_CATEGORY]);
            viewModel.Date = Convert.ToDateTime(listItem[FIELD_REQUISITION_DATE]);

            if (listItem[FIELD_REQUISITION_EVENTBUDGETNO] != null)
            {
                viewModel.EventBudgetNo.Value = (listItem[FIELD_REQUISITION_EVENTBUDGETNO] as FieldLookupValue).LookupId;
                viewModel.EventBudgetNo.Text = (listItem[FIELD_REQUISITION_EVENTBUDGETNO] as FieldLookupValue).LookupValue;
            }

            viewModel.Project.Value = Convert.ToString(listItem[FIELD_REQUISITION_PROJECT]);
            viewModel.Project.Text = Convert.ToString(listItem[FIELD_REQUISITION_PROJECT]);
            viewModel.Currency.Value = Convert.ToString(listItem[FIELD_REQUISITION_CURRENCY]);
            viewModel.Total = Convert.ToDecimal(listItem[FIELD_REQUISITION_TOTAL]);
            viewModel.EditMode = (int)Item.Mode.UPDATED;
            viewModel.Modified = Convert.ToDateTime(listItem[FIELD_MODIFIED]);
            viewModel.Created = Convert.ToDateTime(listItem[FIELD_CREATED]);

            viewModel.Editor = Convert.ToString((listItem[FIELD_RN_EDITOR] as FieldUserValue).LookupValue);
            viewModel.UserEmail = Convert.ToString(listItem[FIELD_USER_EMAIL]);
            viewModel.TransactionStatus = Convert.ToString(listItem[FieldName_TransactionStatus]);

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
            return viewModel;
        }

        private IEnumerable<RequisitionNoteItemVM> GetRequisitionNoteItemDetails(int HeaderID)
        {
            List<RequisitionNoteItemVM> details = null;

            if (HeaderID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FIELD_RN_HEADERID + "' /><Value Type='Lookup'>" + HeaderID.ToString() + "</Value></Eq></Where></Query></View>";

                details = new List<RequisitionNoteItemVM>();

                foreach (var item in SPConnector.GetList(ListName_RequisitionNoteItem, siteUrl, caml))
                {
                    details.Add(ConvertToRequisitionNoteItemVM(item));
                }
            }

            return details;
        }

        private RequisitionNoteItemVM ConvertToRequisitionNoteItemVM(ListItem listItem)
        {
            RequisitionNoteItemVM viewModel = new RequisitionNoteItemVM();

            viewModel.ID = Convert.ToInt32(listItem[FIELD_ID]);
            viewModel.Specification = Convert.ToString(listItem[FIELD_TITLE]);

            viewModel.Activity.Value = (listItem[FIELD_RN_ACTIVITY] as FieldLookupValue).LookupId;
            viewModel.Activity.Text = (listItem[FIELD_RN_ACTIVITY] as FieldLookupValue).LookupValue;

            var wbs = WBSMasterService.Get(siteUrl, Convert.ToInt32((listItem[FIELD_RN_WBS] as FieldLookupValue).LookupId));
            viewModel.WBS.Value = (listItem[FIELD_RN_WBS] as FieldLookupValue).LookupId;
            viewModel.WBS.Text = wbs.WBSIDDescription;

            viewModel.GL.Value = (listItem[FIELD_RN_GL] as FieldLookupValue).LookupId;
            viewModel.GL.Text = string.Format("{0}-{1}", (listItem[FIELD_RN_GL_ID] as FieldLookupValue).LookupValue, (listItem[FIELD_RN_GL_DESC] as FieldLookupValue).LookupValue);

            viewModel.Quantity = Convert.ToInt32(listItem[FIELD_RN_QUANTITY]);
            viewModel.Frequency = Convert.ToInt32(listItem[FIELD_RN_FREQUENCY]);
            viewModel.Price = Convert.ToDecimal(listItem[FIELD_RN_PRICE]);
            viewModel.Total = Convert.ToDecimal(listItem[FIELD_RN_TOTAL]);
            viewModel.EditMode = (int)Item.Mode.UPDATED;
            return viewModel;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.RequisitionNoteDocumentByID, siteUrl, iD);
        }
    }
}
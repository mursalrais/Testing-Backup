﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance.RequisitionNote
{
    public class RequisitionNoteService : IRequisitionNote
    {
        private const string GLMASTER_SITE_LIST = "GL Master";
        private const string WBSMASTER_SITE_LIST = "WBS Master";
        private const string ACTIVITY_SITE_LIST = "Activity";
        private const string REQUISITION_SITE_LIST = "Requisition Note";
        private const string REQUISITION_ITEM_SITE_LIST = "Requisition Note item";
        private const string REQUISITION_ATTACHMENTS_SITE_LIST = "Requisition Note Documents";
        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";
        private const string FIELD_GL_DESCRIPTION = "yyxi";
        private const string FIELD_WBS_DESCRIPTION = "WBSDesc";
        private const string FIELD_FORMAT_DOC = "RN/{0}-{1}/";
        private const string FIELD_REQUISITION_CATEGORY = "Category";
        private const string FIELD_REQUISITION_DATE = "Date";
        private const string FIELD_REQUISITION_EVENTBUDGETNO = "Event_x0020_Budget_x0020_No"; 
        private const string FIELD_REQUISITION_PROJECT = "Project"; 
        private const string FIELD_REQUISITION_FUND = "Fund"; 
        private const string FIELD_REQUISITION_CURRENCY = "Currency";
        private const string FIELD_REQUISITION_TOTAL = "Total";
        private const string FIELD_RN_HEADERID = "Requisition_x0020_Note_x0020_ID";
        private const string FIELD_RN_ACTIVITY = "Activity";
        private const string FIELD_RN_WBS = "WBS_x0020_ID";
        private const string FIELD_RN_GL = "GL_x0020_ID";
        private const string FIELD_RN_QUANTITY = "Quantity";
        private const string FIELD_RN_PRICE = "Price";
        private const string FIELD_RN_TOTAL = "Total_x0020_Per_x0020_Item";
        private const string FIELD_RN_DOCUMENTS_HEADERID = "_x0027_Requisition_x0020_Note_x0027_";

        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;  
        }

        public RequisitionNoteVM GetRequisitionNote(int? ID)
        {
            var viewModel = new RequisitionNoteVM();
            
            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(REQUISITION_SITE_LIST, ID, _siteUrl);
                viewModel = ConvertToRequisitionNoteVM(listItem);

                viewModel.ItemDetails = GetRequisitionNoteItemDetails(ID.Value);
                
            }
           

            return viewModel;

        }

        public IEnumerable<GLMasterVM> GetGLMaster()
        {
            var glMasters = new List<GLMasterVM>();

            foreach (var item in SPConnector.GetList(GLMASTER_SITE_LIST, _siteUrl, null))
            {
                glMasters.Add(ConvertToGLMasterModel(item));
            }

            return glMasters;
        }

        public IEnumerable<WBSMasterVM> GetWBSMaster()
        {
            var wbsMasters = new List<WBSMasterVM>();

            foreach (var item in SPConnector.GetList(WBSMASTER_SITE_LIST, _siteUrl, null))
            {
                wbsMasters.Add(ConvertToWBSMasterModel(item));
            }

            return wbsMasters;
        }

        public IEnumerable<ActivityVM> GetActivity()
        {
            var activities = new List<ActivityVM>();

            foreach (var item in SPConnector.GetList(ACTIVITY_SITE_LIST, _siteUrl, null))
            {
                activities.Add(ConvertToActivityModel(item));
            }

            
            return activities;
        }

        public int CreateRequisitionNote(RequisitionNoteVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();
            string DocumentNo = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            updatedValue.Add(FIELD_REQUISITION_CATEGORY, viewModel.Category.Value);
            updatedValue.Add(FIELD_REQUISITION_DATE, viewModel.Date);
            updatedValue.Add(FIELD_REQUISITION_EVENTBUDGETNO, viewModel.EventBudgetNo.Text);
            updatedValue.Add(FIELD_REQUISITION_PROJECT, viewModel.Project.Value);
            updatedValue.Add(FIELD_REQUISITION_FUND, viewModel.Fund);
            updatedValue.Add(FIELD_REQUISITION_CURRENCY, viewModel.Currency.Value);
            updatedValue.Add(FIELD_REQUISITION_TOTAL, viewModel.Total);
            updatedValue.Add(FIELD_TITLE, DocumentNumbering.Create(_siteUrl, DocumentNo));

            try
            {
                SPConnector.AddListItem(REQUISITION_SITE_LIST, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(REQUISITION_SITE_LIST, _siteUrl);
        }

        public bool UpdateRequisitionNote(RequisitionNoteVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FIELD_REQUISITION_CATEGORY, viewModel.Category.Value);
            updatedValue.Add(FIELD_REQUISITION_DATE, viewModel.Date);
            updatedValue.Add(FIELD_REQUISITION_EVENTBUDGETNO, viewModel.EventBudgetNo.Text);
            updatedValue.Add(FIELD_REQUISITION_PROJECT, viewModel.Project.Value);
            updatedValue.Add(FIELD_REQUISITION_FUND, viewModel.Fund);
            updatedValue.Add(FIELD_REQUISITION_CURRENCY, viewModel.Currency.Value);
            updatedValue.Add(FIELD_REQUISITION_TOTAL, viewModel.Total);

            try
            {
                SPConnector.UpdateListItem(REQUISITION_SITE_LIST, viewModel.ID, updatedValue, _siteUrl);
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
                updatedValue.Add(FIELD_RN_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(FIELD_RN_QUANTITY, viewModel.Quantity);
                updatedValue.Add(FIELD_RN_PRICE, viewModel.Price);
                updatedValue.Add(FIELD_RN_TOTAL, viewModel.Total);
             
                try
                {
                    SPConnector.AddListItem(REQUISITION_ITEM_SITE_LIST, updatedValue, _siteUrl);
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
                    SPConnector.UploadDocument(REQUISITION_ATTACHMENTS_SITE_LIST, updateValue, doc.FileName, doc.InputStream, _siteUrl);
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
                        SPConnector.DeleteListItem(REQUISITION_ITEM_SITE_LIST, viewModel.ID, _siteUrl);

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
                updatedValue.Add(FIELD_RN_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(FIELD_RN_QUANTITY, viewModel.Quantity);
                updatedValue.Add(FIELD_RN_PRICE, viewModel.Price);
                updatedValue.Add(FIELD_RN_TOTAL, viewModel.Total);

                try
                {
                    SPConnector.UpdateListItem(REQUISITION_ITEM_SITE_LIST,viewModel.ID, updatedValue, _siteUrl);
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
                    SPConnector.UploadDocument(REQUISITION_ATTACHMENTS_SITE_LIST, updateValue, doc.FileName, doc.InputStream, _siteUrl);
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
            
            viewModel.EventBudgetNo.Text = Convert.ToString(listItem[FIELD_REQUISITION_EVENTBUDGETNO]);
            viewModel.Project.Value = Convert.ToString(listItem[FIELD_REQUISITION_PROJECT]);
            viewModel.Fund = Convert.ToDecimal(listItem[FIELD_REQUISITION_FUND]);
            viewModel.Currency.Value = Convert.ToString(listItem[FIELD_REQUISITION_CURRENCY]);
            viewModel.Total = Convert.ToDecimal(listItem[FIELD_REQUISITION_TOTAL]);
            viewModel.EditMode = (int)Item.Mode.UPDATED;

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
            return viewModel;
        }

        private IEnumerable<RequisitionNoteItemVM> GetRequisitionNoteItemDetails(int HeaderID)
        {
            List<RequisitionNoteItemVM> details = null;

            if (HeaderID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='"+FIELD_RN_HEADERID+"' /><Value Type='Lookup'>" + HeaderID.ToString() + "</Value></Eq></Where></Query></View>";

                details = new List<RequisitionNoteItemVM>();

                foreach (var item in SPConnector.GetList(REQUISITION_ITEM_SITE_LIST, _siteUrl, caml))
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

            viewModel.WBS.Value = (listItem[FIELD_RN_WBS] as FieldLookupValue).LookupId;
            viewModel.WBS.Text = (listItem[FIELD_RN_WBS] as FieldLookupValue).LookupValue;

            viewModel.GL.Value = (listItem[FIELD_RN_GL] as FieldLookupValue).LookupId;
            viewModel.GL.Text = (listItem[FIELD_RN_GL] as FieldLookupValue).LookupValue;

            viewModel.Quantity = Convert.ToInt32(listItem[FIELD_RN_QUANTITY]);
            viewModel.Price = Convert.ToDecimal(listItem[FIELD_RN_PRICE]);
            viewModel.Total = Convert.ToDecimal(listItem[FIELD_RN_TOTAL]);
            viewModel.EditMode = (int)Item.Mode.UPDATED;
            return viewModel;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.RequisitionNoteDocumentByID, _siteUrl, iD);
        }
    }
}
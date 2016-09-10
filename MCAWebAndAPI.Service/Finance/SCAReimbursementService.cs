using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN08: SCA Reimbursement
    /// </summary>

    public class SCAReimbursementService : ISCAReimbursementService
    {
        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public const string ListName = "SCA Reimbursement";
        private const string ListName_SCASettlementItem = "SCA Reimbursement Item";

        private const string FIELD_FORMAT_DOC = "RSCA/{0}-{1}/";
        private const int DIGIT_DOCUMENTNO = 5;

        private const string FieldNameId = "ID";
        private const string FieldNameSCAReimbursementNO = "Title";
        private const string FieldNameEventBudget = "EventBudget";
        private const string FieldNameDescription = "Description";
        private const string FieldNameFund = "Fund";
        private const string FieldNameCurrency = "Currency";
        private const string FieldNameTotalAmount = "TotalAmountReimbursed";
        private const string FieldNameDetailReceiptDate = "ReceiptDate";
        private const string FieldNameDetailReceiptNo = "ReceiptNo";
        private const string FieldNameDetailPayee = "Payee";
        private const string FieldNameDetailDescription = "DescriptionOfExpenses";
        private const string FieldNameDetailWBS = "WBSMasterId";
        private const string FieldNameDetailGL = "GLMasterId";
        private const string FieldNameDetailAmount = "AmontPerItem";
        private const string FieldNameDetailSCAReimbursementHeaderID = "SCAReimbursementId";
        private const string FieldNameDetailWBSNo = "WBSMasterId_x003a_WBS_x0020_ID";
        private const string FieldNameDetailWBSDesc = "WBSMasterId_x003a_WBS_x0020_Desc";
        private const string FieldNameDetailGLNo = "GLMasterId_x003a_GL_x0020_No";
        private const string FieldNameDetailGLDesc = "GLMasterId_x003a_GL_x0020_Descri";

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public SCAReimbursementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new SCAReimbursementVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToSCAReimbursementVM(listItem);

                viewModel.ItemDetails = GetSCAReimbursementItemDetails(id.Value);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public int? Save(SCAReimbursementVM scaReimbursement)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();
            var documentNoFormat = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            columnValues.Add(FieldNameEventBudget, new FieldLookupValue { LookupId = Convert.ToInt32(scaReimbursement.EventBudget.Value) });
            columnValues.Add(FieldNameTotalAmount, scaReimbursement.Amount);
            columnValues.Add(FieldNameDescription, scaReimbursement.Description);
            columnValues.Add(FieldNameFund, scaReimbursement.Fund);
            columnValues.Add(FieldNameCurrency, scaReimbursement.Currency.Text);

            try
            {
                if (scaReimbursement.Operation == Operations.c)
                {
                    scaReimbursement.DocNo = DocumentNumbering.Create(siteUrl, documentNoFormat, DIGIT_DOCUMENTNO);
                    columnValues.Add(FieldNameSCAReimbursementNO, scaReimbursement.DocNo);

                    SPConnector.AddListItem(ListName, columnValues, siteUrl);

                    result = SPConnector.GetLatestListItemID(ListName, siteUrl);

                    scaReimbursement.ID = result;

                }
                else if (scaReimbursement.Operation == Operations.e)
                {
                    SPConnector.UpdateListItem(ListName, scaReimbursement.ID, columnValues, siteUrl);
                    result = scaReimbursement.ID;
                }

                SaveSCAReimbursementDetailItems(scaReimbursement.ID, scaReimbursement.ItemDetails);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }
            return result;
        }

        private void SaveSCAReimbursementDetailItems(int? headerID, IEnumerable<SCAReimbursementItemVM> itemDetails)
        {
            foreach (var viewModel in itemDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(ListName_SCASettlementItem, viewModel.ID, siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }


                var updatedValue = new Dictionary<string, object>();

                updatedValue.Add(FieldNameDetailSCAReimbursementHeaderID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add(FieldNameDetailWBS, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBS.Value) });
                updatedValue.Add(FieldNameDetailGL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(FieldNameDetailReceiptDate, viewModel.ReceiptDate);
                updatedValue.Add(FieldNameDetailReceiptNo, viewModel.ReceiptNo);
                updatedValue.Add(FieldNameDetailPayee, viewModel.Payee);
                updatedValue.Add(FieldNameDetailDescription, viewModel.DescriptionOfExpense);
                updatedValue.Add(FieldNameDetailAmount, viewModel.Amount);

                try
                {
                    if (Item.CheckIfCreated(viewModel))
                    {
                        SPConnector.AddListItem(ListName_SCASettlementItem, updatedValue, siteUrl);
                    }
                    else
                    {
                        SPConnector.UpdateListItem(ListName_SCASettlementItem, viewModel.ID, updatedValue, siteUrl);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        private IEnumerable<SCAReimbursementItemVM> GetSCAReimbursementItemDetails(int HeaderID)
        {
            List<SCAReimbursementItemVM> details = null;

            if (HeaderID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FieldNameDetailSCAReimbursementHeaderID + "' /><Value Type='Lookup'>" + HeaderID.ToString() + "</Value></Eq></Where></Query></View>";

                details = new List<SCAReimbursementItemVM>();

                foreach (var item in SPConnector.GetList(ListName_SCASettlementItem, siteUrl, caml))
                {
                    details.Add(ConvertToSCASettlementItemVM(item));
                }
            }

            return details;
        }

        private SCAReimbursementItemVM ConvertToSCASettlementItemVM(ListItem listItem)
        {
            SCAReimbursementItemVM viewModel = new SCAReimbursementItemVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldNameId]);
            viewModel.ReceiptDate = Convert.ToDateTime(listItem[FieldNameDetailReceiptDate]);
            viewModel.ReceiptNo = Convert.ToString(listItem[FieldNameDetailReceiptNo]);
            viewModel.Payee = Convert.ToString(listItem[FieldNameDetailPayee]);
            viewModel.DescriptionOfExpense = Convert.ToString(listItem[FieldNameDetailDescription]);

            viewModel.WBS.Value = (listItem[FieldNameDetailWBS] as FieldLookupValue).LookupId;
            viewModel.WBS.Text = string.Format("{0}-{1}", (listItem[FieldNameDetailWBSNo] as FieldLookupValue).LookupValue, (listItem[FieldNameDetailWBSDesc] as FieldLookupValue).LookupValue);

            viewModel.GL.Value = (listItem[FieldNameDetailGL] as FieldLookupValue).LookupId;
            viewModel.GL.Text = string.Format("{0}-{1}", (listItem[FieldNameDetailGLNo] as FieldLookupValue).LookupValue, (listItem[FieldNameDetailGLDesc] as FieldLookupValue).LookupValue);

            viewModel.Amount = Convert.ToDecimal(listItem[FieldNameDetailAmount]);

            viewModel.EditMode = (int)Item.Mode.UPDATED;
            return viewModel;
        }

        private SCAReimbursementVM ConvertToSCAReimbursementVM(ListItem listItem)
        {
            SCAReimbursementVM viewModel = new SCAReimbursementVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldNameId]);
            viewModel.DocNo = Convert.ToString(listItem[FieldNameSCAReimbursementNO]);

            if (listItem[FieldNameEventBudget] != null)
            {
                viewModel.EventBudget.Value = (listItem[FieldNameEventBudget] as FieldLookupValue).LookupId;
                viewModel.EventBudget.Text = (listItem[FieldNameEventBudget] as FieldLookupValue).LookupValue;
            }

            viewModel.Description = Convert.ToString(listItem[FieldNameDescription]);
            viewModel.Fund = Convert.ToString(listItem[FieldNameFund]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldNameCurrency]);
            viewModel.EditMode = (int)Item.Mode.UPDATED;
            viewModel.Amount = Convert.ToDecimal(listItem[FieldNameTotalAmount]);

            return viewModel;
        }

  }
}

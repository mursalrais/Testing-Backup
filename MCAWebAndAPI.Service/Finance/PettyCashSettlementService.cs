using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.PettyCashTransactionItem;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN11: Petty Cash Settlement
    /// 
    ///     Petty Cash Settlement is a transaction for settlement-reimbursement of petty cash where 
    ///     user has already asked for petty cash advance previously. 
    ///     
    ///     Through this feature, user will create the settlement-reimbursement of 
    ///     petty cash which results whether user needs to return the excess petty cash advance or 
    ///     receive the reimbursement in the case where the actual expense for 
    ///     petty cash exceeds the petty cash advance given. 
    ///     
    ///     It is created and maintained by finance. 																									
    ///
    /// </summary>

    public class PettyCashSettlementService : IPettyCashSettlementService
    {
        public const string ListName = "Petty Cash Settlement";

        private const string LISTNAME_DOCUMENTS = "Petty Cash Settlement Documents";

        private const string FIELD_FORMAT_DOC = "SPC/{0}-{1}/";
        private const int DIGIT_DOCUMENTNO = 5;

        private const string FieldName_ID = "ID";
        private const string FieldName_Date = "Settlement_x0020_Date";
        private const string FieldName_PettyCashVoucherId = "PettyCashVoucherId";
        private const string FieldName_PettyCashVoucherAdvanceReceivedDate = "PettyCashVoucherId_x003a_Advance";
        private const string FieldName_PettyCashVoucherNo = "PettyCashVoucherId_x003a_Voucher";
        private const string FieldName_AmountLiquidated = "Amount_x0020_Liquidated";
        private const string FieldName_AmountReimbursedOrReturned = "Amount_x0020_Reimbursed_x002f_Re";
        private const string FieldName_Remarks = "Remarks";
        private const string FieldName_DocumentNo = "Title";
        private const string FieldName_Status = "Status";
        private const string FieldName_PaidTo = "Paid_x0020_To";
        private const string FieldName_Currency = "Currency";
        private const string FieldName_AmountPaid = "Amount_x0020_Paid";
        private const string FieldName_AmountPaidInWord = "Amount_x0020_Paid_x0020_in_x0020";
        private const string FieldName_Reason = "Reason_x0020_of_x0020_Payment";
        private const string FieldName_Fund = "Fund";
        private const string FieldName_WBS = "WBS";
        private const string FieldName_GL = "GL";

        private const string FIELD_PCID_DOCUMENTS = "Petty_x0020_Cash_x0020_Settlement";
        private const string FINPettyCashSettlementDocumentByID = "{0}/Petty%20Cash%20Settlement%20Documents/Forms/AllItems.aspx#InplviewHash5093bda1-84bf-4cad-8652-286653d6a83f=FilterField1%3Dpsa%255Fx003a%255FID-FilterValue1%3D{1}";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int? Save(PettyCashSettlementVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();

            columnValues.Add(FieldName_Date, viewModel.Date);
            columnValues.Add(FieldName_PettyCashVoucherId, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.PettyCashVoucher.Value) });
            columnValues.Add(FieldName_Status, viewModel.Status);
            columnValues.Add(FieldName_PaidTo, viewModel.PaidTo);
            columnValues.Add(FieldName_AmountPaid, viewModel.AmountPaid);
            columnValues.Add(FieldName_Currency, viewModel.Currency.Value);
            columnValues.Add(FieldName_AmountPaidInWord, viewModel.AmountPaidInWords);
            columnValues.Add(FieldName_Reason, viewModel.ReasonOfPayment);
            columnValues.Add(FieldName_Fund, viewModel.Fund);
            columnValues.Add(FieldName_WBS, viewModel.WBS);
            columnValues.Add(FieldName_GL, viewModel.GL);
            columnValues.Add(FieldName_AmountLiquidated, viewModel.AmountLiquidated);
            columnValues.Add(FieldName_AmountReimbursedOrReturned, viewModel.Amount);
            columnValues.Add(FieldName_Remarks, viewModel.Remarks);

            try
            {
                if (viewModel.Operation == Operations.c)
                {
                    var documentNoFormat = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

                    viewModel.TransactionNo = DocumentNumbering.Create(siteUrl, documentNoFormat, DIGIT_DOCUMENTNO);
                    columnValues.Add(FieldName_DocumentNo, viewModel.TransactionNo);

                    SPConnector.AddListItem(ListName, columnValues, siteUrl);
                }
                else if (viewModel.Operation == Operations.e)
                {
                    SPConnector.UpdateListItem(ListName, viewModel.ID, columnValues, siteUrl);
                }

                result = SPConnector.GetLatestListItemID(ListName, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return result;
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public PettyCashSettlementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new PettyCashSettlementVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToVM(siteUrl, listItem);

                viewModel.Amount = viewModel.AmountLiquidated - viewModel.AmountPaid;
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public void SavePettyCashAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                var type = doc.FileName.Split('-')[0].Trim();

                updateValue.Add(FIELD_PCID_DOCUMENTS, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                try
                {
                    SPConnector.UploadDocument(LISTNAME_DOCUMENTS, updateValue, doc.FileName, doc.InputStream, siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public static IEnumerable<PettyCashTransactionItem> GetPettyCashTransaction(string siteUrl, DateTime dateFrom, DateTime dateTo, Post sign)
        {
            return SharedService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, ListName, FieldName_Date, sign, ConvertToVMShort);
        }

        public delegate PettyCashTransactionItem ConvertToVMDelegate(string siteUrl, ListItem listItem, Post sign);

        private static PettyCashSettlementVM ConvertToVMShort(string siteUrl, ListItem listItem, Post sign)
        {
            PettyCashSettlementVM viewModel = new PettyCashSettlementVM();

            int multiplier = sign == Post.DR ? 1 : -1;

            viewModel.ID = Convert.ToInt32(listItem[FieldName_ID]);
            viewModel.Date = Convert.ToDateTime(listItem[FieldName_Date]);
            viewModel.TransactionNo = Convert.ToString(listItem[FieldName_DocumentNo]);
            viewModel.Amount = multiplier * Convert.ToDecimal(listItem[FieldName_AmountReimbursedOrReturned]);

            return viewModel;
        }

        private static PettyCashSettlementVM ConvertToVM(string siteUrl, ListItem listItem)
        {
            PettyCashSettlementVM viewModel = ConvertToVMShort(siteUrl, listItem, Post.DR);

            viewModel.PettyCashVoucher = new Model.ViewModel.Control.AjaxComboBoxVM();
            viewModel.PettyCashVoucher.Value = (listItem[FieldName_PettyCashVoucherNo] as FieldLookupValue).LookupId;
            viewModel.PettyCashVoucher.Text = (listItem[FieldName_PettyCashVoucherNo] as FieldLookupValue).LookupValue;
            viewModel.AdvanceReceivedDate = Convert.ToDateTime((listItem[FieldName_PettyCashVoucherAdvanceReceivedDate] as FieldLookupValue).LookupValue);
            viewModel.Status = Convert.ToString(listItem[FieldName_Status]);
            viewModel.PaidTo = Convert.ToString(listItem[FieldName_PaidTo]);
            viewModel.AmountPaid = Convert.ToDecimal(listItem[FieldName_AmountPaid]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.AmountPaidInWords = Convert.ToString(listItem[FieldName_AmountPaidInWord]);
            viewModel.ReasonOfPayment = Convert.ToString(listItem[FieldName_Reason]);
            viewModel.Fund = Convert.ToString(listItem[FieldName_Fund]);
            viewModel.WBS = Convert.ToString(listItem[FieldName_WBS]);
            viewModel.GL = Convert.ToString(listItem[FieldName_GL]);
            viewModel.AmountLiquidated = Convert.ToDecimal(listItem[FieldName_AmountLiquidated]);
            viewModel.Remarks = Convert.ToString(listItem[FieldName_GL]);
            viewModel.AmountLiquidated = Convert.ToDecimal(listItem[FieldName_AmountLiquidated]);
            viewModel.Amount = Convert.ToDecimal(listItem[FieldName_AmountReimbursedOrReturned]);
            viewModel.Remarks = Convert.ToString(listItem[FieldName_Remarks]);
            viewModel.DocumentUrl = GetDocumentUrl(siteUrl, viewModel.ID);

            return viewModel;
        }

        private static string GetDocumentUrl(string siteUrl, int? iD)
        {
            return string.Format(FINPettyCashSettlementDocumentByID, siteUrl, iD);
        }
    }
}
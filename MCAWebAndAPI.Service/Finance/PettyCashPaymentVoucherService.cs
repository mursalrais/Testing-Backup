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

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN10: Petty Cash Voucher
    ///     a.k.a.: Petty Cash Payment Voucher
    ///     a.k.a.: Petty Cash Advance Voucher
    /// </summary>

    public class PettyCashPaymentVoucherService : IPettyCashPaymentVoucherService
    {
        private const string LISTNAME = "Petty Cash Payment Voucher";
        private const string LISTNAME_DOCUMENTS = "Petty Cash Payment Voucher Documents";

        private const string FIELD_FORMAT_DOC = "PC/{0}-{1}/";
        private const int DIGIT_DOCUMENTNO = 5;

        private const string FIELD_DATE = "Advance_x0020_Received_x0020_Dat";
        private const string FIELD_STATUS = "Status";
        private const string FIELD_PAIDTO = "Paid_x0020_To";
        private const string FIELD_PROFESSIONALID = "ProfessionalID";
        private const string FIELD_PROFESSIONAL_NAME = "ProfessionalID_x003a_Full_x0020_";
        private const string FIELD_VENDORID = "Vendor_x0020_ID";
        private const string FIELD_CURRENCY = "NewColumn1";
        private const string FIELD_AMOUNT = "Amount_x0020_Paid";
        private const string FIELD_AMOUNTPAID_WORD = "Amount_x0020_Paid_x0020_in_x0020";
        private const string FIELD_REASON = "Reason_x0020_of_x0020_Payment";
        private const string FIELD_FUND = "Fund";
        private const string FIELD_WBS_ID = "WBS_x0020_ID";
        private const string FIELD_WBS_GLNO = "WBS_x0020_ID_x003a_WBS_x0020_ID";
        private const string FIELD_WBS_DESC = "WBS_x0020_ID_x003a_WBS_x0020_Des";
        private const string FIELD_GL_ID = "GL_x0020_ID";
        private const string FIELD_GL_NO = "GL_x0020_ID_x003a_GL_x0020_No";
        private const string FIELD_GL_DESC = "GL_x0020_ID_x003a_GL_x0020_Descr";
        private const string FIELD_REMARKS = "Remarks";
        //private const string FIELD_VOUCHERNO = "Voucher_x0020_NO";
        private const string FIELD_VOUCHERNO = "Title";
        private const string FIELD_ID = "ID";
        private const string FIELD_PCID_DOCUMENTS = "Petty_x0020_Cash_x0020_Payment_x0020_Voucher";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public PettyCashPaymentVoucherVM GetPettyCashPaymentVoucher(int? ID)
        {
            var viewModel = new PettyCashPaymentVoucherVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(LISTNAME, ID, siteUrl);
                viewModel = ConvertToVM(siteUrl, listItem);
            }

            return viewModel;

        }

        public int Create(PettyCashPaymentVoucherVM viewModel)
        {
            var newItem = new Dictionary<string, object>();
            string documentNoFormat = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            newItem.Add(FIELD_DATE, viewModel.Date);
            newItem.Add(FIELD_STATUS, viewModel.Status.Value);
            newItem.Add(FIELD_PAIDTO, viewModel.PaidTo.Text);

            if (viewModel.Professional.Value.HasValue)
            {
                newItem.Add(FIELD_PROFESSIONALID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Professional.Value) });
            }

            if (viewModel.Vendor.Value.HasValue)
            {
                newItem.Add(FIELD_VENDORID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Vendor.Value) });
            }

            newItem.Add(FIELD_CURRENCY, viewModel.Currency.Value);
            newItem.Add(FIELD_AMOUNT, viewModel.Amount);
            newItem.Add(FIELD_AMOUNTPAID_WORD, viewModel.AmountPaidInWord);
            newItem.Add(FIELD_REASON, viewModel.ReasonOfPayment);
            newItem.Add(FIELD_FUND, viewModel.Fund);
            newItem.Add(FIELD_WBS_ID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBS.Value) });
            newItem.Add(FIELD_GL_ID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
            newItem.Add(FIELD_REMARKS, viewModel.Remarks);

            string docNO = DocumentNumbering.Create(siteUrl, documentNoFormat, DIGIT_DOCUMENTNO);
            newItem.Add(FIELD_VOUCHERNO, docNO);
            viewModel.TransactionNo = docNO;

            try
            {
                SPConnector.AddListItem(LISTNAME, newItem, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(LISTNAME, siteUrl);
        }

        public bool Update(PettyCashPaymentVoucherVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FIELD_DATE, viewModel.Date);
            updatedValue.Add(FIELD_STATUS, viewModel.Status.Value);
            updatedValue.Add(FIELD_PAIDTO, viewModel.PaidTo.Text);

            if (viewModel.Professional.Value.HasValue)
            {
                updatedValue.Add(FIELD_PROFESSIONALID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Professional.Value) });
                updatedValue.Add(FIELD_VENDORID, "");
            }


            if (viewModel.Vendor.Value.HasValue)
            {
                updatedValue.Add(FIELD_VENDORID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Vendor.Value) });
                updatedValue.Add(FIELD_PROFESSIONALID, "");
            }

            updatedValue.Add(FIELD_CURRENCY, viewModel.Currency.Value);
            updatedValue.Add(FIELD_AMOUNT, viewModel.Amount);
            updatedValue.Add(FIELD_AMOUNTPAID_WORD, viewModel.AmountPaidInWord);
            updatedValue.Add(FIELD_REASON, viewModel.ReasonOfPayment);
            updatedValue.Add(FIELD_FUND, viewModel.Fund);
            updatedValue.Add(FIELD_WBS_ID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBS.Value) });
            updatedValue.Add(FIELD_GL_ID, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
            updatedValue.Add(FIELD_REMARKS, viewModel.Remarks);

            try
            {
                SPConnector.UpdateListItem(LISTNAME, viewModel.ID, updatedValue, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return true;
        }

        public void CreatePettyCashAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
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

        public void EditPettyCashAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
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


        public static IEnumerable<PettyCashPaymentVoucherVM> GetPettyCashPaymentVouchers(string siteUrl)
        {
            var paymentVouchers = new List<PettyCashPaymentVoucherVM>();

            paymentVouchers.Add(new PettyCashPaymentVoucherVM() { ID = -1, Title = string.Empty });

            foreach (var item in SPConnector.GetList(LISTNAME, siteUrl, null))
            {
                paymentVouchers.Add(ConvertToVM(siteUrl, item));
            }

            return paymentVouchers;
        }


        public delegate PettyCashTransactionItem ConvertToVMDelegate(string siteUrl, ListItem listItem, Post sign);

        private static PettyCashPaymentVoucherVM ConvertToVMShort(string siteUrl, ListItem listItem, Post sign)
        {
            PettyCashPaymentVoucherVM viewModel = new PettyCashPaymentVoucherVM();

            int multiplier = sign == Post.DR ? 1 : -1;

            viewModel.ID = Convert.ToInt32(listItem[FIELD_ID]);
            viewModel.Date = Convert.ToDateTime(listItem[FIELD_DATE]);
            viewModel.TransactionNo = Convert.ToString(listItem[FIELD_VOUCHERNO]);
            viewModel.Amount = multiplier * Convert.ToDecimal(listItem[FIELD_AMOUNT]);

            return viewModel;
        }


        private static PettyCashPaymentVoucherVM ConvertToVM(string siteUrl, ListItem listItem)
        {

            PettyCashPaymentVoucherVM viewModel = ConvertToVMShort(siteUrl, listItem, Post.DR);

            viewModel.Status.Value = Convert.ToString(listItem[FIELD_STATUS]);
            viewModel.PaidTo.Value = Convert.ToString(listItem[FIELD_PAIDTO]);

            //TODO: the following line is troublesome please check if we need to check both values for != null
            if (viewModel.Professional != null && listItem[FIELD_PROFESSIONALID] != null)
            {
                viewModel.Professional.Value = (listItem[FIELD_PROFESSIONALID] as FieldLookupValue).LookupId;

                //TODO: the following line causes error
                //viewModel.Professional.Text = (listItem[FIELD_PROFESSIONAL_NAME] as FieldLookupValue).LookupValue;

            }

            if (viewModel.Vendor != null && listItem[FIELD_VENDORID] != null)
            {
                viewModel.Vendor.Value = (listItem[FIELD_VENDORID] as FieldLookupValue).LookupId;
            }

            viewModel.Currency.Value = Convert.ToString(listItem[FIELD_CURRENCY]);


            viewModel.AmountPaidInWord = Convert.ToString(listItem[FIELD_AMOUNTPAID_WORD]);
            viewModel.ReasonOfPayment = Convert.ToString(listItem[FIELD_REASON]);
            viewModel.Fund = Convert.ToString(listItem[FIELD_FUND]);

            viewModel.WBS.Value = (listItem[FIELD_WBS_ID] as FieldLookupValue).LookupId;
            viewModel.WBS.Text = string.Format("{0}-{1}", (listItem[FIELD_WBS_GLNO] as FieldLookupValue).LookupValue, (listItem[FIELD_WBS_DESC] as FieldLookupValue).LookupValue);

            viewModel.GL.Value = (listItem[FIELD_GL_ID] as FieldLookupValue).LookupId;
            viewModel.GL.Text = string.Format("{0}-{1}", (listItem[FIELD_GL_NO] as FieldLookupValue).LookupValue, (listItem[FIELD_GL_DESC] as FieldLookupValue).LookupValue);

            viewModel.Remarks = Convert.ToString(listItem[FIELD_REMARKS]);

            viewModel.DocumentUrl = GetDocumentUrl(siteUrl, viewModel.ID);

            return viewModel;
        }

        private static string GetDocumentUrl(string siteUrl, int? iD)
        {
            return string.Format(UrlResource.PettyCashPaymentVoucherDocumentByID, siteUrl, iD);
        }

        public static IEnumerable<PettyCashTransactionItem> GetPettyCashTransaction(string siteUrl, DateTime dateFrom, DateTime dateTo, Post sign)
        {
            return SharedService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, LISTNAME, FIELD_DATE, sign, ConvertToVMShort);
        }

    }
}
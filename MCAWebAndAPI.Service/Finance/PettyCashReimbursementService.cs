using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.PettyCashTransactionItem;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public class PettyCashReimbursementService : IPettyCashReimbursementService
    {
        /// <summary>
        ///     Wireframe FIN12: Petty Cash Reimbursement
        ///         Petty Cash Reimbursement is a transaction for the reimbursement of petty cash only when
        ///         user has not asked for any petty cash advance.
        ///
        ///         Through this feature, finance will create the reimbursement of petty cash which results in 
        ///         user needs to receive the reimbursement. 
        /// </summary>
        private const string FIELD_FORMAT_DOC = "RPC/{0}-{1}/";
        private const int DIGIT_DOCUMENTNO = 5;
        private const string PettyCashDocument_URL = "{0}/Petty%20Cash%20Reimbursement%20Documents/Forms/AllItems.aspx?FilterField1=Petty_x0020_Cash_x0020_Reimbursement&FilterValue1={1}";

        private const string ListName = "Petty Cash Reimbursement";
        private const string ListNameDocument = "Petty Cash Reimbursement Documents";
        private const string FieldNameDocument_PettyCash = "Petty_x0020_Cash_x0020_Reimbursement";

        private const string FieldName_Id = "ID";
        private const string FieldName_DocNo = "Title";
        private const string FieldName_Date = "Reimbursement_x0020_Date";
        private const string FieldName_PaidTo = "Paid_x0020_To";
        private const string FieldName_Professional = "NewColumn1";
        private const string FieldName_Vendor = "Vendor_x0020_ID";
        private const string FieldName_Driver = "Driver";

        private const string FieldName_Currency = "Currency";
        private const string FieldName_AmountLiquidated = "Amount_x0020_Liquidated";
        private const string FieldName_AmountReimbursed = "Amount_x0020_Reimbursed";
        private const string FieldName_WBS = "WBS_x0020_ID";
        private const string FieldName_WBSID = "WBS_x0020_ID_x003a_WBS_x0020_ID";
        private const string FieldName_WBSDesc = "WBS_x0020_ID_x003a_WBS_x0020_Des";
        private const string FieldName_GL = "GL_x0020_ID";
        private const string FieldName_GLNo = "GL_x0020_ID_x003a_GL_x0020_No";
        private const string FieldName_GLDesc = "GL_x0020_ID_x003a_GL_x0020_Descr";
        private const string FieldName_Remarks = "Remarks";
        private const string FieldName_Reason = "Reason_x0020_of_x0020_Payment";
        private const string FieldName_Fund = "Fund";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int? Create(PettyCashReimbursementVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
           {
               {FieldName_Date, viewModel.Date},
               {FieldName_PaidTo, viewModel.PaidTo.Value},
               //{FieldName_Professional, viewModel.Professional},
               {FieldName_Vendor, viewModel.Vendor==null ? 0 : viewModel.Vendor.Value},
               {FieldName_Driver, viewModel.Driver},
               {FieldName_Currency, viewModel.Currency.Value},
               {FieldName_Reason, viewModel.Reason},
               {FieldName_Fund, viewModel.Fund},
               {FieldName_WBS, viewModel.WBS.Value},
               {FieldName_GL, viewModel.GL.Value},
               {FieldName_AmountLiquidated, viewModel.Amount},
               {FieldName_AmountReimbursed, viewModel.AmountReimbursed},
               {FieldName_Remarks, viewModel.Remarks}
            };

            try
            {
                if (viewModel.Operation == Operations.c)
                {
                    var documentNoFormat = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";
                    viewModel.TransactionNo = DocumentNumbering.Create(siteUrl, documentNoFormat, DIGIT_DOCUMENTNO);
                    columnValues.Add(FieldName_DocNo, viewModel.TransactionNo);

                    SPConnector.AddListItem(ListName, columnValues, siteUrl);
                    result = SPConnector.GetLatestListItemID(ListName, siteUrl);
                }
                else if (viewModel.Operation == Operations.e)
                {
                    SPConnector.UpdateListItem(ListName, viewModel.ID, columnValues, siteUrl);
                    result = viewModel.ID;
                }


            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public async Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            CreatePettyCashReimbursementAttachment(siteUrl, ID, attachment);
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public PettyCashReimbursementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new PettyCashReimbursementVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToVM(siteUrl, listItem);
                viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public PettyCashReimbursementVM GetPettyCashReimbursement(int? ID = null)
        {
            var viewModel = new PettyCashReimbursementVM();

            if (ID != null)
            {
                var list = SPConnector.GetListItem(ListName, ID, siteUrl);
                viewModel = ConvertToVM(siteUrl, list);
            }

            return viewModel;
        }

        public delegate PettyCashTransactionItem ConvertToVMDelegate(string siteUrl, ListItem listItem, Post sign);

        private static PettyCashReimbursementVM ConvertToVMShort(string siteUrl, ListItem listItem, Post sign)
        {
            PettyCashReimbursementVM viewModel = new PettyCashReimbursementVM();

            int multiplier = sign == Post.DR ? 1 : -1;

            viewModel.ID = Convert.ToInt32(listItem[FieldName_Id]);
            viewModel.Date = Convert.ToDateTime(listItem[FieldName_Date]);
            viewModel.TransactionNo = Convert.ToString(listItem[FieldName_DocNo]);
            viewModel.Amount = multiplier * Convert.ToDecimal(listItem[FieldName_AmountLiquidated]);

            return viewModel;
        }

        private static PettyCashReimbursementVM ConvertToVM(string siteUrl, ListItem listItem)
        {
            PettyCashReimbursementVM viewModel = new PettyCashReimbursementVM();


            viewModel.DocNo = Convert.ToString(listItem[FieldName_DocNo]);
            viewModel.PaidTo.Value = Convert.ToString(listItem[FieldName_PaidTo]);

            //TODO: the following line causes error
            //  but currently there is nothing you can do 
            //  we are waiting for eCEOs to fix Professional Master table
            //viewModel.Professional.Value = Convert.ToInt32((listItem[FieldName_Professional] as FieldLookupValue).LookupId.ToString());

            viewModel.Vendor.Value = listItem[FieldName_Vendor] == null ? 0 : Convert.ToInt32((listItem[FieldName_Vendor] as FieldLookupValue).LookupId.ToString());
            viewModel.Driver = Convert.ToString(listItem[FieldName_Driver]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Reason = Convert.ToString(listItem[FieldName_Reason]);
            viewModel.AmountReimbursed = Convert.ToDecimal(listItem[FieldName_AmountReimbursed]);
            viewModel.WBS.Value = Convert.ToInt32((listItem[FieldName_WBS] as FieldLookupValue).LookupId.ToString());

            //TODO: the following line causes error
            //  'listItem[FieldName_WBSID]' threw an exception of type 'Microsoft.SharePoint.Client.PropertyOrFieldNotInitializedException'
            //  biasanya karena salah tulis field name
            //  please fix this ASAP
            //viewModel.WBSDescription = string.Format("{0} - {1}", (listItem[FieldName_WBSID] as FieldLookupValue).LookupValue.ToString(), (listItem[FieldName_WBSDesc] as FieldLookupValue).LookupValue.ToString());

            viewModel.GL.Value = Convert.ToInt32((listItem[FieldName_GL] as FieldLookupValue).LookupId.ToString());

            //TODO: the following line causes error
            //  'listItem[FieldName_WBSID]' threw an exception of type 'Microsoft.SharePoint.Client.PropertyOrFieldNotInitializedException'
            //  biasanya karena salah tulis field name
            //  please fix this ASAP
            //viewModel.GLDescription = string.Format("{0} - {1}", (listItem[FieldName_GLNo] as FieldLookupValue).LookupValue.ToString(), (listItem[FieldName_GLDesc] as FieldLookupValue).LookupValue.ToString());

            viewModel.Remarks = Convert.ToString(listItem[FieldName_Remarks]);

            return viewModel;
        }

        private static void CreatePettyCashReimbursementAttachment(string siteUrl, int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(FieldNameDocument_PettyCash, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });
                        try
                        {
                            SPConnector.UploadDocument(ListNameDocument, updateValue, doc.FileName, doc.InputStream, siteUrl);
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

        public static IEnumerable<PettyCashTransactionItem> GetPettyCashTransaction(string siteUrl, DateTime dateFrom, DateTime dateTo, Post sign)
        {
            return SharedService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, ListName, FieldName_Date, sign, ConvertToVMShort);
        }

        private string GetDocumentUrl(int? ID)
        {
            return string.Format(PettyCashDocument_URL, siteUrl, ID);
        }
    }
}
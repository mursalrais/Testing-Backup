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

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// FIN14: Petty Cash Replenishment
    /// </summary>

    public class PettyCashReplenishmentService : IPettyCashReplenishmentService
    {
        private const string ListName = "Petty Cash Replenishment";
        private const string ListName_Document = "Petty Cash Replenishment Documents";

        private const string FieldName_Id = "ID";
        private const string FieldName_Date = "Date";
        private const string FieldName_Currency = "Currency";
        private const string FieldName_Amount = "Amount";
        private const string FieldName_DocNo = "Title";
        private const string FieldName_PettyCashReplenishID = "Petty_x0020_Cash_x0020_Replenishment";
        private const string FieldName_Remarks = "Remarks";

        private const string FINPettyCashReplenishmentDocumentByID = "{0}/Petty%20Cash%20Replenishment%20Documents/Forms/AllItems.aspx#InplviewHash5093bda1-84bf-4cad-8652-286653d6a83f=FilterField1%3Dpsa%255Fx003a%255FID-FilterValue1%3D{1}";

        string siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int Save(PettyCashReplenishmentVM viewModel)
        {
            var willCreate = viewModel.ID == null;
            var updatedValue = new Dictionary<string, object>();
            DateTime today = DateTime.Now;

            updatedValue.Add(FieldName_Date, viewModel.Date);
            updatedValue.Add(FieldName_Currency, viewModel.Currency.Value);
            updatedValue.Add(FieldName_Amount, viewModel.Amount);
            updatedValue.Add(FieldName_Remarks, viewModel.Remarks);

            try
            {
                if (willCreate)
                {
                    viewModel.TransactionNo = DocumentNumbering.Create(siteUrl, string.Format("RPPC/{0}-{1}/", DateTimeExtensions.GetMonthInRoman(today), today.ToString("yy")) + "{0}", 5);
                    updatedValue.Add(FieldName_DocNo, viewModel.TransactionNo);

                    SPConnector.AddListItem(ListName, updatedValue, siteUrl);
                }
                else
                    SPConnector.UpdateListItem(ListName, viewModel.ID, updatedValue, siteUrl);
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


            return SPConnector.GetLatestListItemID(ListName, siteUrl);
        }


        public async Task SaveAttachmentAsync(int? ID, string reference, IEnumerable<HttpPostedFileBase> documents)
        {
            SaveAttachment(ID, reference, documents);
        }

        private void SaveAttachment(int? ID, string reference, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(FieldName_PettyCashReplenishID, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });

                        try
                        {
                            SPConnector.UploadDocument(ListName_Document, updateValue, doc.FileName, doc.InputStream, siteUrl);
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

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public PettyCashReplenishmentVM Get(int? ID)
        {
            var viewModel = new PettyCashReplenishmentVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(ListName, ID, siteUrl);
                viewModel = ConvertToVM(siteUrl, listItem);
            }

            return viewModel;

        }

        public delegate PettyCashTransactionItem ConvertToVMDelegate(string siteUrl, ListItem listItem, Post sign);

        private static PettyCashReplenishmentVM ConvertToVMSort(string siteUrl, ListItem listItem, Post sign)
        {
            PettyCashReplenishmentVM viewModel = new PettyCashReplenishmentVM();

            int multiplier = sign == Post.DR ? 1 : -1;

            viewModel.ID = Convert.ToInt32(listItem[FieldName_Id]);
            viewModel.Date = Convert.ToDateTime(listItem[FieldName_Date]);
            viewModel.TransactionNo = Convert.ToString(listItem[FieldName_DocNo]);
            viewModel.Amount = multiplier * Convert.ToDecimal(listItem[FieldName_Amount]);

            return viewModel;
        }

        private static PettyCashReplenishmentVM ConvertToVM(string siteUrl, ListItem listItem)
        {
            PettyCashReplenishmentVM viewModel = new PettyCashReplenishmentVM();

            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Remarks = Convert.ToString(listItem[FieldName_Remarks]);
            viewModel.DocumentUrl = GetDocumentUrl(siteUrl, viewModel.ID);

            return viewModel;
        }

        public static IEnumerable<PettyCashTransactionItem> GetPettyCashTransaction(string siteUrl, DateTime dateFrom, DateTime dateTo, Post sign)
        {
            return SharedService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, ListName, FieldName_Date, sign, ConvertToVMSort);
        }

        private static string GetDocumentUrl(string siteUrl, int? iD)
        {
            return string.Format(FINPettyCashReplenishmentDocumentByID, siteUrl, iD);
        }

    }
}
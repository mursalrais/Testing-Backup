using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.SharePoint.Client;

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

        private const string ListName = "Petty Cash Reimbursement";

        private const string FieldName_Id = "ID";
        private const string FieldName_DocNo = "Title";
        private const string FieldName_Date = "Reimbursement_x0020_Date";
        private const string FieldName_PaidTo = "Paid_x0020_To";
        private const string FieldName_Professional = "NewColumn1";
        private const string FieldName_Vendor = "Vendor_x0020_ID";
        private const string FieldName_Driver = "Driver";

        private const string FieldName_Currency = "Currency";
        private const string FieldName_AmountLiquidated = "Amount_x0020_Liquidated";
        
        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int? Create(PettyCashReimbursementVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
           {
               {FieldName_Date, viewModel.Date},
               {FieldName_PaidTo, viewModel.PaidTo},
               {FieldName_Professional, viewModel.Professional},
               {FieldName_Vendor, viewModel.Vendor},
               {FieldName_Driver, viewModel.Driver}
            };

            try
            {
                SPConnector.AddListItem(ListName, columnValues, siteUrl);
                result = SPConnector.GetLatestListItemID(ListName, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public delegate PettyCashTransactionItem ConvertToVMDelegate(string siteUrl, ListItem listItem);

        private static PettyCashReimbursementVM ConvertToVM(string siteUrl, ListItem listItem)
        {
            PettyCashReimbursementVM viewModel = new PettyCashReimbursementVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldName_Id]);
            viewModel.DocNo = Convert.ToString(listItem[FieldName_DocNo]);
            viewModel.Date = Convert.ToDateTime(listItem[FieldName_Date]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Amount = Convert.ToDecimal(listItem[FieldName_AmountLiquidated]);

            return viewModel;
        }

        public static IEnumerable<PettyCashTransactionItem> GetPettyCashTransaction(string siteUrl, DateTime dateFrom, DateTime dateTo)
        {
            return SharedService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, ListName, FieldName_Date, ConvertToVM);
        }
    }
}

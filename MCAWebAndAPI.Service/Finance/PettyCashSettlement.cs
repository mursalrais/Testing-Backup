using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class PettyCashSettlement : IPettyCashSettlement
    {
        public  const string ListName = "Petty Cash Settlement";

        private const string FieldName_Date = "Date";
        private const string FieldName_PettyCashVoucherNo = "Petty Cash Voucher No";
        private const string FieldName_AmountLiquidated = "Amount Liquidated";
        private const string FieldName_AmountReimbursedOrReturned = "Amount Reimbursed/Returned";
        private const string FieldName_Remarks = "Remarks";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int? Create(PettyCashSettlementVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
           {
                {FieldName_Date, viewModel.Date},
                {FieldName_PettyCashVoucherNo, viewModel.PettyCasVoucher},
                {FieldName_AmountLiquidated, viewModel.AmountLiquidated},
                {FieldName_AmountReimbursedOrReturned, viewModel.AmountReimbursedOrReturned},
                { FieldName_Remarks, viewModel.Remarks}
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


    }
}

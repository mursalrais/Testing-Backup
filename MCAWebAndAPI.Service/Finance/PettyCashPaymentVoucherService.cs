using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class PettyCashPaymentVoucherService:IPettyCashPaymentVoucherService
    {
        private const string ListName = "Petty Cash Reimbursement";

        private const string FieldName_Date = "Date";
        private const string FieldName_PaidTo = "Paid To";
        private const string FieldName_Professional = "Professional";
        private const string FieldName_Vendor = "Vendor";
        private const string FieldName_Driver = "Driver";

        private string _siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public PettyCashPaymentVoucherVM GetPettyCashPaymentVoucher(int? ID)
        {
            var viewModel = new PettyCashPaymentVoucherVM();

            if (ID != null)
            {
                //var listItem = SPConnector.GetListItem(REQUISITION_SITE_LIST, ID, _siteUrl);
                //viewModel = ConvertToRequisitionNoteVM(listItem);
            }

            return viewModel;

        }

    }
}

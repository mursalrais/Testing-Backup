using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public static class Shared
    {
        public enum Operations { c, e, v }  //Create, Edit, View

        public const string Fund = "3000";

        public const string ErrorDevInvalidState = "DevError: Editing or Viewing without providing a valid id";

        public const string PettyCashTranscationType_PettyCashVoucher = "Petty Cash Voucher";
        public const string PettyCashTranscationType_PettyCashSettlementr = "Petty Cash Settlement";
        public const string PettyCashTranscationType_PettyCashReimbursement = "Petty Cash Reimbursement";
        public const string PettyCashTranscationType_PettyCashReplenishment = "Petty Cash Replenishment";

        public static Operations GetOperation(string op)
        {
            return string.IsNullOrEmpty(op) ? Operations.v : (Operations)Enum.Parse(typeof(Operations), op);
        }
    }
}

using MCAWebAndAPI.Model.Common;
using System;

namespace MCAWebAndAPI.Model.ViewModel.Form.Common
{
    public class PendingApprovalItemVM : Item
    {
        public string TransactionName { get; set; }

        public string Level { get; set; }

        public string Requestor { get; set; }

        public DateTime DateOfRequest { get; set; } = DateTime.Now;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class PettyCashSettlementVM : Item
    {

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM PettyCasVoucher { get; set; }

        public string AdvaceReceivedDate { get; }

        public string Status { get; }

        public string PaidTo { get; }

        public string Currency { get; }

        public string AmountPaid { get; }

        public string AmountPaidInWords { get; }

        public string ReasonOfPayment { get; }

        public string Fund { get; } = Shared.Fund;

        public string WBS { get; }

        public string GL { get; }

        [Required]
        public decimal AmountLiquidated { get; set; }

        [Required]
        public decimal AmountReimbursedOrReturned { get; set; }

        [Required]
        public string Remarks { get; set; }

        [DisplayName("Attachment")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}
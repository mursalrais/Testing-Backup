using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class SCASettlementItemVM : Item
    {    /// <summary>
         /// Wirefram FIN07: SCA Settlement
         /// </summary>

        [UIHint("Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReceiptDate { get; set; }
        public string ReceiptNo { get; set; }
        public string Payee { get; set; }
        public string DescriptionOfExpense { get; set; }

        [UIHint("AjaxComboBox")]
        [Required]
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();

        [UIHint("AjaxComboBox")]
        [Required]
        public AjaxComboBoxVM GL { get; set; } = new AjaxComboBoxVM();

        public decimal Amount { get; set; }


        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}

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
    public class RequisitionNoteVM : Item
    {
        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM Category { get; set; } = new ComboBoxVM();
     
        [UIHint("Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM EventBudgetNo { get; set; } = new AjaxComboBoxVM();

        [UIHint("ComboBox")]
        public ProjectComboBoxVM Project { get;} = new ProjectComboBoxVM();
       

        public string Fund { get; } = Shared.Fund;

        [UIHint("ComboBox")]
        [DisplayName("Currency")]
        [Required]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        public decimal Total { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        public IEnumerable<RequisitionNoteItemVM> ItemDetails { get; set; } = new List<RequisitionNoteItemVM>();

        public string Editor { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string UserEmail { get; set; }

        public string TransactionStatus { get; set; }
    }
}

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
    public class EventBudgetVM : Item
    {
        private const string FieldDefaultValue_Fund = "3000";

        private ProjectComboBoxVM project;
        private ComboBoxVM activity;

        [Required]
        [DisplayName("Event Name")]
        public string EventName { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        [UIHint("Currency")]
        public decimal Rate { get; set; }

        [Required]
        [DisplayName("Attachment")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        [Required]
        [UIHint("Currency")]
        public string Fund { get; set; } = FieldDefaultValue_Fund;

        [Required]
        [UIHint("Total Direct Payment (IDR)")]
        public decimal TotalDirectPayment { get; set; }

        [Required]
        [DisplayName("Total SCA (IDR)")]
        public decimal TotalSCA { get; set; }

        [Required]
        [DisplayName("Total (IDR)")]
        public decimal TotalIDR { get; set; }

        [Required]
        [DisplayName("Total (USD)")]
        public decimal TotalUSD { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date (from)")]
        public DateTime DateFrom { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date (to)")]
        public DateTime DateTo { get; set; } = DateTime.Now;

        [Required]
        [UIHint("ComboBox")]
        public ProjectComboBoxVM Project { get; set; } = new ProjectComboBoxVM();

        [Required]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM Activity { get; set; } = new AjaxCascadeComboBoxVM();

   
         public IEnumerable<EventBudgetItemVM> ItemDetails = new List<EventBudgetItemVM>();

        public string No { get; set; }

    }
}

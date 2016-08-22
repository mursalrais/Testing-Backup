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
    /// <summary>
    /// Wireframe FIN06: SCA Voucher
    ///     i.e.: Special Cash Advance Voucher
    /// </summary>

    public class SCAVoucherVM:Item
    {
        private const string locked = "Locked";
        private const string unlocked = "Unlocked";

        public enum ActionType { edit, approve };

        public IEnumerable<SCAVoucherItemsVM> SCAVoucherItems { get; set; } = new List<SCAVoucherItemsVM>();

        public IEnumerable<EventBudgetItemVM> EventBudgetItems { get; set; } = new List<EventBudgetItemVM>();

        [DisplayName("SCA No.")]
        public string SCAVoucherNo { get; set; }

        [Required]
        [DisplayName("Date")]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime SCAVoucherDate { get; set; } = DateTime.Now;

        [Required]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM SDO { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "ComboBox",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc"
        };

        public int SDOID { get; set; }

        public string SDOName { get; set; }

        public string Position { get; set; }

        public int EventBudgetID { get; set; }

        public string EventBudgetNo { get; set; }

        [Required]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM EventBudget { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "ComboBox",
            ActionName = "GetEventBudgets",
            ValueField = "Value",
            TextField = "Text",
            OnSelectEventName = "OnSelectEventBudgetNo"
        };

        //[Required]
        //public string Currency { get; set; }
        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [UIHint("TextArea")]
        public string TotalAmountInWord { get; set; }

        [Required]
        public string Purpose { get; set; }

        [Required]
        public string Project { get; set; }

        public int ActivityID { get; set; }

        [Required]
        public string ActivityName { get; set; }

        [Required]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM SubActivity { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetSubActivitiesByEventBudgetID",
            ControllerName = "ComboBox",
            ValueField = "Value",
            TextField = "Text",
            Cascade = "EventBudget_Value",
            Filter = "filterEventBudgetNo"
        };

        public int SubActivityID { get; set; }

        public string SubActivityName { get; set; }

        [Required]
        public decimal Fund { get; set; }

        public string RefferenceNo { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [Required]
        [UIHint("ComboBox")]
        public ComboBoxVM TransactionStatus { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                unlocked,
                locked
            },
            Value=unlocked
        };

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        public string Action { get; set; }
    }
}
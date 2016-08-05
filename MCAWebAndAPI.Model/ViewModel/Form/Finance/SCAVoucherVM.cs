using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class SCAVoucherVM
    {
        public IEnumerable<SCAVoucherItemsVM> SCAVoucherItems { get; set; } = new List<SCAVoucherItemsVM>();

        public int ID { get; set; }

        [DisplayName("SCA No.")]
        public string SCAVoucherNo { get; set; }

        [UIHint("Date")]
        [DisplayName("Date")]
        public DateTime SCAVoucherDate { get; set; } = DateTime.Now;

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM SDO { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "ComboBox",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc"
        };

        public string SDOName { get; set; }

        public string Position { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM EventBudgetNo { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "ComboBox",
            ActionName = "GetEventBudget",
            ValueField = "Value",
            TextField = "Text",
            OnSelectEventName = "OnSelectEventBudgetNo"
        };
        
        public string Currency { get; set; }

        public decimal TotalAmount { get; set; }

        public string TotalAmountInWord { get; set; }

        public string Purpose { get; set; }

        public string Project { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Activity { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "ComboBox",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc"
        };

        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM SubActivity { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetSubActivityByEventBudgetID",
            ControllerName = "ComboBox",
            ValueField = "Value",
            TextField = "Text",
            Cascade = "EventBudgetNo_Value",
            Filter = "filterEventBudgetNo"
        };

        public decimal Fund { get; set; }

        public string RefferenceNo { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}

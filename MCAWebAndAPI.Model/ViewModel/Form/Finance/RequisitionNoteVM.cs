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
        private const string CATEGORY_EVENT = "Event";
        private const string CATEGORY_NON_EVENT = "Non-event";
        private const string PROJECT_GREEN_PROSPERITY = "Green Prosperity";
        private const string PROJECT_PROCUREMENT = "Procurement Modernization";
        private const string PROJECT_HEALTH = "Health and Nutrition";
        private const string PROJECT_MONITORING = "Monitoring and Evaluation";
        private const string PROJECT_ADMINISTRATION = "Program Administration and Control";

        private ComboBoxVM _category;
        private ComboBoxVM _project;
       
        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM Category
        {
            get
            {
                if (_category == null)
                    _category = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            CATEGORY_EVENT,
                            CATEGORY_NON_EVENT
                        },
                        Value = CATEGORY_EVENT,
                        OnSelectEventName= "onSelectCategory"
                    };
                return _category;
            }
            set
            {
                _category = value;
            }
        }
        
        [UIHint("Date")]
        [Required]
        public DateTime? Date { get; set; } = DateTime.Now;

        [UIHint("AjaxComboBox")]
        public AjaxCascadeComboBoxVM EventBudgetNo { get; set; } = new AjaxCascadeComboBoxVM
        {
            ControllerName = "FINEventBudget",
            ActionName = "GetEventBudgetHeader",
            ValueField = "ID",
            TextField = "Title",
            OnSelectEventName = "onSelectEventBudgetNo"

        };

        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM Project
        {
            get
            {
                if (_project == null)
                    _project = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                           PROJECT_GREEN_PROSPERITY,
                           PROJECT_HEALTH,
                           PROJECT_PROCUREMENT,
                           PROJECT_MONITORING,
                           PROJECT_ADMINISTRATION
                        }
                    };
                return _project;
            }
            set
            {
                _project = value;
            }
        }

        [UIHint("Currency")]
        public decimal Fund { get; set; } = 3000;

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
    }
}

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
        private DateTime _dateFrom, _dateTo;
        private ComboBoxVM _project;
        private ComboBoxVM activity;

        [Required]
        [DisplayName("Event Name")]
        public string EventName { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        [UIHint("Currency")]
        public decimal Rate { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        [Required]
        [UIHint("Currency")]
        public string Fund { get; set; }

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
        [DisplayName("Date (from)")]
        public DateTime DateFrom
        {
            get
            {
                if (_dateFrom == null)
                    _dateFrom = new DateTime();
                return _dateFrom;
            }

            set
            {
                _dateFrom = value;
            }
        }

        [Required]
        [DisplayName("Date (to)")]
        public DateTime DateTo
        {
            get
            {
                if (_dateTo == null)
                    _dateTo = new DateTime();
                return _dateTo;
            }

            set
            {
                _dateTo = value;
            }
        }

        [Required]
        [UIHint("ComboBox")]
        public ComboBoxVM Project
        {
            get
            {
                if (_project == null)
                    _project = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Green Prosperity",
                            "Procurement Modernization",
                            "Health and Nutrition"
                        }
                    };
                return _project;
            }
            set
            {
                _project = value;
            }
        }

        [Required]
        [UIHint("ComboBox")]
        public ComboBoxVM Activity
        {
            get
            {
                if (this.activity == null)
                    this.activity = new ComboBoxVM();
                return this.activity;
            }

            set
            {
                this.activity = value;
            }
        }

        public IEnumerable<EventBudgetItemVM> ItemDetails = new List<EventBudgetItemVM>();
 
    }
}

using MCAWebAndAPI.Model.ViewModel.Control;
using System.Collections.Generic;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class EventBudgetItemVM
    {
        private ComboBoxVM _category;

        public string Description { get; set; }

        public int Quantity { get; set; }

        [DisplayName("UoM (Qty)")]
        public string UoMQty { get; set; }

        public int Frequency { get; set; }
        
        [DisplayName("UoM (Freq)")]
        public string UoMFreq { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? DirectPayment { get; set; }

        [DisplayName("SCA")]
        public decimal? SCA { get; set; }

        [DisplayName("Amount (per item)")]
        public decimal? AmountPerItem { get; set; }

        public string Remarks { get; set; }

        public ComboBoxVM Category
        {
            get
            {
                if(_category == null)
                {
                    _category = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Category 1", 
                            "Category 2"
                        }
                    };
                }
                return _category;
            }

            set
            {
                _category = value;
            }
        }
    }
}
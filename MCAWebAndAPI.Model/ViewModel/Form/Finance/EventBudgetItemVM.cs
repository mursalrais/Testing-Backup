using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class EventBudgetItemVM : Item
    {
        public string TypeOfExpense { get; set; }

        public string Description { get; set; }


        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();


        public static AjaxComboBoxVM GetWBSDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }


        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM GL { get; set; } = new AjaxComboBoxVM();


        public static AjaxComboBoxVM GetGLDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }


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
    }
}
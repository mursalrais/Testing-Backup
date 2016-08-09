using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class RequisitionNoteItemVM: Item
    {
        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM Activity { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetActivityDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM() { Text = string.Empty };
            }
            else
            {
                return model;
            }
        }
       

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();

      
        public static AjaxComboBoxVM GetWBSDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM() { Text = string.Empty};
            }
            else
            {
                return model;
            }
        }

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM GL { get; set; } = new AjaxComboBoxVM();

       
        public static AjaxComboBoxVM GetGLDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM() { Text = string.Empty };
            }
            else
            {
                return model;
            }
        }

        public string Specification { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal Total { get; set; }

        public int Frequency { get; set; } = 1;
        public bool IsFromEventBudget { get; set; } = false;
    }
}

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
        public AjaxComboBoxVM Activity { get; set; } = new AjaxComboBoxVM();

       
        public static AjaxComboBoxVM GetActivityDefaultValue(AjaxComboBoxVM model = null)
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

        public string Specification { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }

    }
}

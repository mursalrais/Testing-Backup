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
  public  class PettyCashReimbursementVM
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [Required]
        [UIHint("ComboBox")]
        public PaidToComboboxVM  PaidTo { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Professional { get; set; } 

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Vendor { get; set; } 
        //    = new AjaxCascadeComboBoxVM
        //{
        //    ControllerName = "xxxx",
        //    ActionName = "xxxxx",
        //    ValueField = "ID",
        //    TextField = "Title",
        //    OnSelectEventName = "onSelectEventBudgetNo"
        //};

        public string Driver { get; set; }


    }
}

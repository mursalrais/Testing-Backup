﻿using System;
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
        /// <summary>
        ///     Wireframe FIN12: Petty Cash Reimbursement
        ///         Petty Cash Reimbursement is a transaction for the reimbursement of petty cash only when
        ///         user has not asked for any petty cash advance.
        ///
        ///         Through this feature, finance will create the reimbursement of petty cash which results in 
        ///         user needs to receive the reimbursement. 
        /// </summary>

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

        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        [DisplayName("Reason of Payment")]
        public string Reason { get; set; }

        public string Fund { get; } = Shared.Fund;

        [Required]
        [DisplayName("WBS")]
        [UIHint("ComboBox")]
        public ComboBoxVM WBS { get; set; } = new ComboBoxVM();


    }
}

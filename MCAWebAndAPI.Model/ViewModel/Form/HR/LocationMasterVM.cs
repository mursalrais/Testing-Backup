using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class LocationMasterVM
    {
        public AjaxComboBoxVM MyProperty { get; set; } = new AjaxComboBoxVM();

        public string Office { get; set; }

        public int Floor { get; set; }

        public string Room { get; set; }
    }
}

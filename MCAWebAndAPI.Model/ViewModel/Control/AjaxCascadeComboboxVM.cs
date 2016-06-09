using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class AjaxCascadeComboBoxVM : AjaxComboBoxVM
    {
        public string Cascade { get; set; }

        public string Filter { get; set; }

    }
}

using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class TimesheetVM : Item
    {
 

        [UIHint("Date")]
        public DateTime? Period { get; set; } = DateTime.Today;


        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ProfessionalName { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };

        [UIHint("Int32")]
        [Required(ErrorMessage = "Professional ID Field Is Required")]
        [DisplayName("Professional ID")]
       

       public string UserLogin { get; set; }

        public string Name { get; set; }

        public int? ProfessionalID { get; set; }

        public int? LocationID { get; set; }

        public string LocationName { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Location { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetProvinces",
            ValueField = "ID",
            TextField = "Title",
            OnSelectEventName = "OnSelectLocationName"
        };


        [UIHint("Date")]
        public DateTime? From { get; set; } = DateTime.Today;

        [UIHint("Date")]
        public DateTime? To { get; set; } = DateTime.Today.AddDays(1);

        [DisplayName("Is Full Day?")]
        [UIHint("Boolean")]
        public bool IsFullDay { get; set; }

        public string ProjectUnit { get; set; }

        public string UserPermission { get; set; }

        public string Approver { get; set; }

        public string ApprovalLevel { get; set; }

        public string TimesheetStatus { get; set; }

        public string ApproverPosition { get; set; }

        public IEnumerable<TimesheetDetailVM> TimesheetDetails { get; set; }

    }
}

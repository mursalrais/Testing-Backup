using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class PositionsMaster
    {
        public int ID { get; set; }

        [DisplayName("Position Name")]
        public string Title { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Position Manpower Requisition Approver 1")]
        public ComboBoxVM PositionManpowerRequisitionApprover1 { get; set; } = new ComboBoxVM { Choices = new string[] { "DED", "ED" } };

        [UIHint("ComboBox")]
        [DisplayName("Position Status")]
        public ComboBoxVM positionStatus { get; set; } = new ComboBoxVM { Choices = new string[] { "Active", "Inactive" } };


        [UIHint("ComboBox")]
        [DisplayName("Position Manpower Requisition Approver 2")]
        public ComboBoxVM positionManpowerRequisitionApprover2 { get; set; } = new ComboBoxVM { Choices = new string[] { "DED", "ED" } };

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Is Key Position?")]
        public ComboBoxVM isKeyPosition { get; set; } = new ComboBoxVM { Choices = new string[] { "Yes", "No" } };
    }
}

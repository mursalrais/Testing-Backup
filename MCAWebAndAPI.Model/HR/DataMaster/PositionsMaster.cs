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
        [DisplayName("Position Status")]
        public ComboBoxVM PositionStatus
        {
            get
            {
                return positionStatus;
            }

            set
            {
                positionStatus = value;
            }
        }

        [UIHint("ComboBox")]
        [DisplayName("Position Manpower Requisition Approver 1")]
        public ComboBoxVM PositionManpowerRequisitionApprover1
        {
            get
            {
                return positionManpowerRequisitionApprover1;
            }

            set
            {
                positionManpowerRequisitionApprover1 = value;
            }
        }

        [UIHint("ComboBox")]
        [DisplayName("Position Manpower Requisition Approver 2")]
        public ComboBoxVM PositionManpowerRequisitionApprover2
        {
            get
            {
                return positionManpowerRequisitionApprover2;
            }

            set
            {
                positionManpowerRequisitionApprover2 = value;
            }
        }

        private ComboBoxVM positionStatus = new ComboBoxVM { Choices = new string[] { "Active", "Inactive" } };

        private ComboBoxVM positionManpowerRequisitionApprover1 = new ComboBoxVM { Choices = new string[] { "DED", "ED" } };

        private ComboBoxVM positionManpowerRequisitionApprover2 = new ComboBoxVM { Choices = new string[] { "DED", "ED" } };

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Is Key Position?")]
        public ComboBoxVM IsKeyPosition
        {
            get
            {
                return isKeyPosition;
            }

            set
            {
                isKeyPosition = value;
            }
        }

        private ComboBoxVM isKeyPosition = new ComboBoxVM { Choices = new string[] { "Yes", "No" } };
    }
}

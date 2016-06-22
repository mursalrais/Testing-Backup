using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class PositionMaster : Item
    {
        /// <summary>
        /// Title
        /// </summary> 
        [DisplayName("Position Name")]
        public string PositionName { get; set; }

        /// <summary>
        /// positionstatus
        /// </summary>
        public string PositionStatus { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        //[UIHint("ComboBox")]
        //[DisplayName("Is Key Position?")]
        //public ComboBoxVM isKeyPosition { get; set; } = new ComboBoxVM { Choices = new string[] { "Yes", "No" } };
        public string IsKeyPosition { get; set; }

        /// <summary>
        /// projectunit
        /// </summary>
        public string Unit { get; set; }

    }
}

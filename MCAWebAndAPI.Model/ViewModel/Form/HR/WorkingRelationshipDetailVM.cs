using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingRelationshipDetailVM : Item
    {
        [UIHint("InGridComboBox_Position")]
        public InGridComboBoxVM Position { get; set; } = new InGridComboBoxVM();        

        [UIHint("InGridComboBox_Relationship")]
        public InGridComboBoxVM Relationship { get; set; } = new InGridComboBoxVM();

        [UIHint("InGridComboBox_Frequency")]
        public InGridComboBoxVM Frequency { get; set; } = new InGridComboBoxVM();

    }
}
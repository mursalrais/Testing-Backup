using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingRelationshipDetailVM : Item
    {
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Position { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositions",
            ValueField = "ID",
            TextField = "Title"
        };

        [UIHint("ComboBox")]
        public ComboBoxVM Relationship { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "KTP",
                "KITAS",
                "Passport",
                "SIM"
            },
            DefaultValue = "KTP",
            OnSelectEventName = "onSelectRelationship"
        };

        [UIHint("ComboBox")]
        public ComboBoxVM Frequency { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "KTP",
                "KITAS",
                "Passport",
                "SIM"
            },
            DefaultValue = "KTP",
            OnSelectEventName = "onSelectFrequency"
        };        

    }
}
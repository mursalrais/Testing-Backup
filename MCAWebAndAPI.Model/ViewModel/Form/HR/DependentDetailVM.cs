using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class DependentDetailVM : Item
    {
        public string Name { get; set; }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Relationship { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetRelationshipOptions()
        {
            return new InGridComboBoxVM[]
            {
                new InGridComboBoxVM
                {
                    CategoryID = 1,
                    CategoryName = "Child"
                },
                new InGridComboBoxVM
                {
                    CategoryID = 2,
                    CategoryName = "Spouse"
                }
            };
        }

        public static InGridComboBoxVM GetRelationshipDefaultValue()
        {
            return new InGridComboBoxVM
            {
                CategoryID = 1,
                CategoryName = "Child"
            };
        }

        public string PlaceOfBirth { get; set; }

        [UIHint("Date")]
        public DateTime? DateOfBirth { get; set; }

        public string InsuranceNumber { get; set; }

        [UIHint("TextArea")]
        public string Remark { get; set; }
    }
}
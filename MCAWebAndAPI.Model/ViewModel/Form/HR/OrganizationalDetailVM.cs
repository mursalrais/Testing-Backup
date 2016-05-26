using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class OrganizationalDetailVM : Item
    {
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Project { get; set; } = new InGridComboBoxVM(); 

        public static IEnumerable<InGridComboBoxVM> GetProjectOptions()
        {
            return new InGridComboBoxVM[]
            {
                new InGridComboBoxVM
                {
                    CategoryID = 1,
                    CategoryName = "Green Prosperity"
                },
                new InGridComboBoxVM
                {
                    CategoryID = 2,
                    CategoryName = "Health and Nutrition"
                },
                new InGridComboBoxVM
                {
                    CategoryID = 3,
                    CategoryName = "Procurement Modernization"
                }
            };
        }

        public static InGridComboBoxVM GetProjectDefaultValue()
        {
            return new InGridComboBoxVM
            {
                CategoryID = 1,
                CategoryName = "Green Prosperity"
            };
        }

        public static InGridComboBoxVM GetProfessionalStatusDefaultValue()
        {
            return new InGridComboBoxVM
            {
                CategoryID = 1,
                CategoryName = "Permanent"
            };
        }

        public static IEnumerable<InGridComboBoxVM> GetProfessionalStatusOptions()
        {
            return new InGridComboBoxVM[]
            {
                new InGridComboBoxVM
                {
                    CategoryID = 1,
                    CategoryName = "Permanent"
                },
                new InGridComboBoxVM
                {
                    CategoryID = 2,
                    CategoryName = "Contract"
                }
            };
        }

        public string Position { get; set; }

        public string Level { get; set; }

        public string PSANumber { get; set; }

        [UIHint("Date")]
        public DateTime? StartDate { get; set; } = DateTime.UtcNow;

        [UIHint("Date")]
        public DateTime? LastWorkingDay { get; set; } = DateTime.UtcNow;

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ProfessionalStatus { get; set; } = new InGridComboBoxVM();
    }
}
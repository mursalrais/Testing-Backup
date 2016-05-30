﻿using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingRelationshipDetailVM : Item
    {
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Position { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetPositionOptions()
        {
            var index = 0;
            var options = new string[] {
                "LGL",
                "Ops - HR",
                "CC - ESP",
                "HN",
                "Ops - P",
                "ME",
                "PM",
                "GP",
                "Ops - IT",
                "Ops",
                "CC - E",
                "Ops - F",
                "CC",
                "EO",
                "COM",
                "CC - SGA",
                "RI",
                "HN - NST" };

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetPositionDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetPositionOptions();
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Relationship { get; set; } = new InGridComboBoxVM();
        public static IEnumerable<InGridComboBoxVM> GetRelationshipOptions()
        {
            var index = 0;
            var options = new string[] {
                "Reporting",
                "Coordination",
                "Liason",
                "Supervision"
                 };

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetRelationshipDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetRelationshipOptions();
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Frequency { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetFrequencyOptions()
        {
            var index = 0;
            var options = new string[] {
                "Daily",
                "Regularly",
                "When Necessary"


                 };

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetFrequencyDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetFrequencyOptions();
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

    }
}
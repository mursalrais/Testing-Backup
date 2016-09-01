using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class DayOffNextBalanceVM : Item
    {
        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("Day-Off Type")]
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM DayOffType { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetDayOffTypeOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Annual Day-Off",
                "Sick Day-Off",
                "Special Day-Off",
                "Unpaid Day-Off",
                "Compensatory Time",
                "Maternity",
                "Miscarriage",
                "Paternity",
                "Marriage of the Professional",
                "Marriage of the Professional's Children",
                "Circumcision of the Professional's Sons",
                "Baptism of the Professional's Children",
                "Death of the Professional’s dependent (i.e. spouse or children) or parent or parent in-laws",
                "Death of a member of the Professional’s household other than the Professional’s dependent or parent",
                "Professional’s separation date is on or after 19 of the month",
                "Unscheduled closing of MCA-Indonesia office(s)",
                "Voting Day",
                "Service as  a court witness",
                "Other"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static InGridComboBoxVM GetDayOffTypeDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetDayOffTypeOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        /// <summary>
        /// uom
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Unit { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetUnitOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Days",
                "Months"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static InGridComboBoxVM GetUnitDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetUnitOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        /// <summary>
        /// entitlement
        /// </summary>
        [UIHint("Int32")]
        public int Entitlement { get; set; }

        /// <summary>
        /// dayoffbrought
        /// </summary>
        [UIHint("Int32")]
        public int DayOffBrought { get; set; }

        /// <summary>
        /// eduction
        /// </summary>
        [UIHint("Int32")]
        public int Deduction { get; set; }

        /// <summary>
        /// entitlementtotal
        /// </summary>
        [UIHint("Int32")]
        public int Total { get; set; }

        /// <summary>
        /// statusdraft
        /// </summary>
        [UIHint("Int32")]
        public int Draft { get; set; }

        /// <summary>
        /// statuspendingapproval
        /// </summary>
        [UIHint("Int32")]
        public int PendingApproval { get; set; }

        /// <summary>
        /// statusapproved
        /// </summary>
        [UIHint("Int32")]
        public int Approved { get; set; }

        /// <summary>
        /// statusrejected
        /// </summary>
        [UIHint("Int32")]
        public int Rejected { get; set; }

        /// <summary>
        /// balance
        /// </summary>
        [UIHint("Double")]
        public double Balance { get; set; }

        [UIHint("Date")]
        public DateTime? FirstDate { get; set; }

        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// dayoffbrought
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM DayOffBroughtChoices { get; set; } = new InGridComboBoxVM();

        /// <summary>
        /// peryear
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM PerYear { get; set; } = new InGridComboBoxVM();

        /// <summary>
        /// eligibledayoff
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM EligibleDayOff { get; set; } = new InGridComboBoxVM();

        /// <summary>
        /// othercategory
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM OtherCategory { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetAllOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Yes",
                "No"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static InGridComboBoxVM GetDayOffBroughtDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetAllOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }
    }
}

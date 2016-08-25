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
    public class DayOffRequestDetailVM : DayOffRequestVM
    {
        [UIHint("InGridComboBox")]
        [DisplayName("Full/Half?")]
        public InGridComboBoxVM FullHalf { get; set; } = new InGridComboBoxVM();


        public static IEnumerable<InGridComboBoxVM> GetFullHalfOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Full Day",
                "Half Day"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static InGridComboBoxVM GetFullHalfDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetFullHalfOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        /// <summary>
        /// masterdayofftype
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        [DisplayName("Day-Off Type")]
        public AjaxComboBoxVM MasterDayOffType { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetMasterDayOffTypeDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }

        /// <summary>
        /// requeststartdate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Start Date")]
        public DateTime? RequestStartDate { get; set; }

        /// <summary>
        /// requestenddate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? RequestEndDate { get; set; }

        /// <summary>
        /// totaldays
        /// </summary>
        public int TotalDays { get; set; }

        /// <summary>
        /// returntowork
        /// </summary>
        public DateTime? ReturnWork { get; set; }

        /// <summary>
        /// remarks
        /// </summary>
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// dayoffrequest
        /// </summary>
        public int? DayOffRequestID { get; set; }

        /// <summary>
        /// approvalstatus
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ApprovalStatus { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetApprovalStatusOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Pending Of Approval",
                "Approved"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static InGridComboBoxVM GetApprovalStatusDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetApprovalStatusOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

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
    }
}

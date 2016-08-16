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
    public class DayOffRequestDetailVM : DayOffBalanceVM
    {
        [UIHint("InGridComboBox")]
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
            var options = GetUnitOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        /// <summary>
        /// masterdayofftype
        /// </summary>
        public AjaxComboBoxVM MasterDayOffType { get; set; }

        /// <summary>
        /// requeststartdate
        /// </summary>
        public DateTime? RequestStartDate { get; set; }

        /// <summary>
        /// requestenddate
        /// </summary>
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
            var options = GetUnitOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }
    }
}

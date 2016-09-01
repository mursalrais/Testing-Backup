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
    public class DayOffRequestDetailDisplayVM : DayOffRequestVM
    {
        [DisplayName("Full/Half?")]
        public string FullHalf { get; set; }

        /// <summary>
        /// requeststartdate
        /// </summary>
        //[UIHint("Date")]
        [DisplayName("Start Date")]
        [Required]
        public string RequestStartDate { get; set; }

        /// <summary>
        /// requestenddate
        /// </summary>
        //[UIHint("Date")]
        [DisplayName("End Date")]
        [Required]
        public string RequestEndDate { get; set; }

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
        [Required]
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
        [Required]
        public string DayOffType { get; set; }
        
    }
}

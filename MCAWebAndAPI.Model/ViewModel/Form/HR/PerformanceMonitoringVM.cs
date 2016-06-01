using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PerformanceMonitoringVM : Item
    {
        public IEnumerable<PerformanceMonitoringDetailVM> PerformancePlanMonitoringDetail { get; set; } = new List<PerformanceMonitoringDetailVM>();

        public string IntiationDate { get; set; }

        /// <summary>
        /// period
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// maxdateforpendingapproval1
        /// </summary>
        [UIHint("Date")]
        public DateTime? MaxDateForPending1 { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// maxdateforpendingapproval2
        /// </summary>
        [UIHint("Date")]
        public DateTime? MaxDateForPending2 { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// maxdateforapproved
        /// </summary>
        [UIHint("Date")]
        public DateTime? MaxDateForApprove { get; set; } = DateTime.UtcNow;
    }
}

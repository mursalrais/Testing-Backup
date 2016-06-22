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
        [UIHint("Date")]
        public DateTime? IntiationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// period
        /// </summary>
        /// 
        [UIHint("Int32")]
        public int? Period { get; set; }

        /// <summary>
        /// maxdateforpendingapproval1
        /// </summary>
        [UIHint("Date")]
        public DateTime? LatestCreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// maxdateforpendingapproval2
        /// </summary>
        [UIHint("Date")]
        public DateTime? LatestDateApproval1 { get; set; } = DateTime.UtcNow;

        [UIHint("Date")]
        public DateTime? LatestDateApproval2 { get; set; } = DateTime.UtcNow;

        public string Status { get; set; }

        public IEnumerable<PerformanceMonitoringDetailVM> PerformanceMonitoringDetails { get; set; } = new List<PerformanceMonitoringDetailVM>();

    }
}

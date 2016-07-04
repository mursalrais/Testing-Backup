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
    public class PerformanceEvaluationDetailVM : Item
    {
        public string EmployeeName { get; set; }

        /// <summary>
        /// ppstatus
        /// </summary>
        public string PlanStatus { get; set; }

        public string PlanIndicator { get; set; }

        public string Email { get; set; }
    }
}

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
    public class PerformancePlanVM : Item
    {
        /// <summary>
        /// professional
        /// </summary>
        public string Name { get; set; }

        public int? NameID { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public string PositionAndDepartement { get; set; }

        public int? PositionAndDepartementID { get; set; }

        /// <summary>
        /// performanceplan
        /// </summary>
        public string PerformancePeriod { get; set; }

        public int? PerformancePeriodID { get; set; }

        public int DatetimeCaml { get; set; } = DateTime.UtcNow.Year;
    }
}

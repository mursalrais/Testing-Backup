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
    public class PerformanceEvaluationVM : Item
    {
        public IEnumerable<FactorEvaluatedDetailVM> FactorEvaluatedDetail { get; set; } = new List<FactorEvaluatedDetailVM>();
        public IEnumerable<OutputDetailVM> OutputDetail { get; set; } = new List<OutputDetailVM>();

        /// <summary>
        /// professionalname
        /// </summary>
        public string Name { get; set; }

        public string TitleAndDepartement { get; set; }

        /// <summary>
        /// pppperiod
        /// </summary>
        public string PerformancePeriod { get; set; }
    }
}

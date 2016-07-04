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
        [UIHint("Date")]
        public DateTime? IntiationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// period
        /// </summary>
        /// 

        private static string[] GetPeriodChoices()
        {
            List<string> PeriodChoices = new List<string>();
            int YearNow = DateTime.Now.Year;            
            for (int i = 0; i < 10; i++)
            {
                PeriodChoices.Add(YearNow.ToString());
                YearNow++;
            }
            return PeriodChoices.ToArray();

        }

        [UIHint("ComboBox")]
        public ComboBoxVM Period { get; set; } = new ComboBoxVM
        {
            Choices = GetPeriodChoices(),
            Value = DateTime.Now.Year.ToString()

        };

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

        public IEnumerable<PerformanceEvaluationDetailVM> PerformanceEvaluationDetails { get; set; } = new List<PerformanceEvaluationDetailVM>();

    }
}

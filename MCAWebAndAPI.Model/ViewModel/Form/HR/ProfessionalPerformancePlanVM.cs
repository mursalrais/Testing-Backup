using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ProfessionalPerformancePlanVM : PerformancePlanVM
    {
        public IEnumerable<ProjectOrUnitGoalsDetailVM> ProjectOrUnitGoalsDetails { get; set; } = new List<ProjectOrUnitGoalsDetailVM>();

        /// <summary>
        /// majorstrength
        /// </summary>
        [UIHint("TextArea")]
        public string MajorStrength { get; set; }

        /// <summary>
        /// performancearea
        /// </summary>
        [UIHint("TextArea")]
        public string PerformanceArea { get; set; }

        /// <summary>
        /// recommendedactivities	
        /// </summary>
        [UIHint("TextArea")]
        public string RecommendedActivities { get; set; }

        [UIHint("Int32")]
        public int? ProfessionalID { get; set; }

        public string Position { get; set; }
    }
}

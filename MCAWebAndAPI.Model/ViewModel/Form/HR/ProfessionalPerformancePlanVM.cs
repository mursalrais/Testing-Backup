using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("1.Briefly describe major strengths of the professional as they relate to the assigned job duties and responsibilities.")]
        [UIHint("TextArea")]
        public string MajorStrength { get; set; }

        /// <summary>
        /// performancearea
        /// </summary>
        [DisplayName("2. Briefly describe those performance areas that could be strengthened or improved.")]
        [UIHint("TextArea")]
        public string PerformanceArea { get; set; }

        /// <summary>
        /// recommendedactivities	
        /// </summary>
        [DisplayName("3. If applicable, indicate recommended activities to improve professional's performance.")]
        [UIHint("TextArea")]
        public string RecommendedActivities { get; set; }

        [UIHint("Int32")]
        public int? ProfessionalID { get; set; }

        /// <summary>
        /// position	
        /// </summary>
        public string Position { get; set; }

        public string ProfessionalEmail { get; set; }

        public string ProfessionalEmailMessage { get; set; }

        /// <summary>
        /// pppstatus
        /// </summary>
        public string StatusForm { get; set; }

        public string Requestor { get; set; }

        /// <summary>
        /// statusworkflow
        /// </summary>
        public string StatusWorkflow { get; set; }

    }
}

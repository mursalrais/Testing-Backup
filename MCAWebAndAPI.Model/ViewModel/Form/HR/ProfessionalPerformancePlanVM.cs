using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
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
        public IEnumerable<WorkflowItemVM> WorkflowItems { get; set; } = new List<WorkflowItemVM>();

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
        /// Position	
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// pppstatus
        /// </summary>
        public string StatusForm { get; set; }

        public string Requestor { get; set; }

        public string TypeForm { get; set; }

        public string Approver1 { get; set; }

        public string Approver2 { get; set; }

        public int ApproverCount { get; set; }
    }
}

using MCAWebAndAPI.Model.ViewModel.Form.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ProfessionalPerformanceEvaluationVM : PerformancePlanVM
    {
        public IEnumerable<ProfessionalPerformanceEvaluationDetailVM> ProfessionalPerformanceEvaluationDetails { get; set; } = new List<ProfessionalPerformanceEvaluationDetailVM>();
        public IEnumerable<WorkflowItemVM> WorkflowItems { get; set; } = new List<WorkflowItemVM>();

        public int? ProfessionalID { get; set; }

        public string StatusForm { get; set; }

        public string Requestor { get; set; }

        [UIHint("Decimal")]
        [Range(0, 5, ErrorMessage = "Only 0-5")]
        public decimal OverallTotalScore { get; set; }

        public string TypeForm { get; set; }

        public string Approver1 { get; set; }

        public string Approver2 { get; set; }

        public int ApproverCount { get; set; }
    }
}

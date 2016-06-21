using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class InterviewDetailVM : Item
    {
        [UIHint("Date")]
        public DateTime? Date { get; set; } = DateTime.UtcNow;

        public string InterviewPanel { get; set; }

        public string InterviewSummary { get; set; }

        public string Result { get; set; }
    }
}

using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class InterviewDetailVM : Item
    {
        [UIHint("Date")]
        [DisplayName("Date")]
        public string Date { get; set; }

        [DisplayName("Date")]
        public string DateString { get; set; }

        public string InterviewPanel { get; set; }

        [UIHint("TextArea")]
        public string InterviewSummary { get; set; }

        public string Result { get; set; }
    }
}

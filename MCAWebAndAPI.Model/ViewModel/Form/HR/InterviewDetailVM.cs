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
    public class InterviewDetailVM : Item
    {
        [UIHint("Date")]
        public DateTime? Date { get; set; } = DateTime.UtcNow;

        public string InterviewPanel { get; set; }

        public string InterviewSummary { get; set; }

        public string Result { get; set; }
    }
}

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
    public class InterviewerResultVM : ApplicationShortlistVM  
    {
        public IEnumerable<ShortlistedDetailVM> ShortlistedDetail { get; set; } = new List<ShortlistedDetailVM>();
        public IEnumerable<InterviewDetailVM> InterviewDetail { get; set; } = new List<InterviewDetailVM>();

        [UIHint("ComboBox")]
        public ComboBoxVM Result { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]
            {
                "Recommended",
                "Not Recommended",
                "For Other Position"
            }
        };

        [UIHint("ComboBox")]
        public ComboBoxVM RecommendedForPosition { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]
            {
"Recommended",
"Not Recommended",
"For Other Position",
"Pending MCC Approval",
"Rejected by MCC",
"On Board",
"Decline to Join"
            }
        };

        public Boolean NeedNextInterviewer { get; set; } = new Boolean();

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();
    }
}

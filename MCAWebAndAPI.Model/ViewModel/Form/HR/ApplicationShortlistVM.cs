using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ApplicationShortlistVM : Item
    {
        public IEnumerable<ShortlistDetailVM> ShortlistDetails { get; set; } = new List<ShortlistDetailVM>();

        public IEnumerable<InterviewDetailVM> InterviewlistDetails { get; set; } = new List<InterviewDetailVM>();

        /// <summary>
        /// position
        /// </summary>
        [Required]
        public string SendTo { get; set; }

        public string EmailFrom { get; set; }

        /// <summary>
        /// position
        /// </summary>
        public string InterviewerPanel { get; set; }

        public string InterviewSummary { get; set; }

        public string Result { get; set; }

        public Boolean SendToCandidate { get; set; }

        [UIHint("TextArea")]
        public string EmailMessage { get; set; }

        [UIHint("TextArea")]
        public string Message { get; set; }

        public string Candidate { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }


        /// <summary>
        /// Result Interview
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Result")]
        public ComboBoxVM GetResultOptions { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Recomended",
                "Not Recomended",
                "For Other Position",
                "Pending MCC Approval",
                "Rejected by MCC",
                "On Board",
                "Decline to Join"
            },
        };

        /// <summary>
        /// positionrequested
        /// </summary>
        [DisplayName("PositionID")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM PositionActiv { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("Position")]
        [Required]
        public string Position { get; set; }

        /// <summary>
        /// positionrequested
        /// </summary>
        [DisplayName("Other Position")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM OtherPosition { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositions",
            ValueField = "ID",
            TextField = "PositionName"
        };

        /// <summary>
        /// positionrequested
        /// </summary>
        [DisplayName("Choose Position")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ActivePosition { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositions",
            ValueField = "ID",
            TextField = "PositionName"
        };

        [DisplayName("Attach Document")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> AttDocuments { get; set; } = new List<HttpPostedFileBase>();

        /// <summary>
        /// InterviewerDate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Interview Date")]
        public DateTime? InterviewerDate { get; set; } = DateTime.Now;

        //[UIHint("Date")]
        //public string InterviewerDate { get; set; } = DateTime.UtcNow.ToShortDateString();

        [UIHint("Date")]
        public string Time { get; set; } = DateTime.UtcNow.ToShortTimeString();

        [UIHint("ComboBox")]
        [DisplayName("Result")]
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
            },
            OnSelectEventName = "onPositionChange"
        };

        
    public Boolean NeedNextInterviewer { get; set; } = new Boolean();

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();
    }
}

using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ApplicationShortlistVM : Item
    {
        public IEnumerable<ShortlistDetailVM> ShortlistDetails { get; set; } = new List<ShortlistDetailVM>();

        public IEnumerable<InterviewDetailVM> InterviewlistDetails { get; set; } = new List<InterviewDetailVM>();

        public string SiteUrl { get; set; }

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

        public string InterviewerUrl { get; set; }

        [UIHint("TextArea")]
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

        public string useraccess { get; set; }

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

        public int? ManPos { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("Position")]
        [Required]
        public string PositionName { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("Position")]
        [Required]
        public string Position { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("File on Attachment")]
        public string Attachmentname { get; set; }

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
            ControllerName = "HRShortlist",
            ActionName = "GetPosition",
            ValueField = "ID",
            TextField = "PositionName",
            OnSelectEventName = "OnChangeActivePosition",

        };

        [DisplayName("Attach Document")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> AttDocuments { get; set; } = new List<HttpPostedFileBase>();

        /// <summary>
        /// InterviewerDate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Interview Date")]
        public DateTime? InterviewerDate { get; set; }

        /// <summary>
        /// InterviewTime
        /// </summary>
        /// 
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "Interview Time")]
        public DateTime? InterviewerTime { get; set; }

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

        [UIHint("ComboBox")]
        [DisplayName("Result")]
        public ComboBoxVM InterviewResultOption { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]
           {
                "Recommended",
                "Not Recommended",
                "For Other Position",
           },
            OnSelectEventName = "onResultOptionChange"
        };

        public static AjaxComboBoxVM GetPositionDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }
   
        [UIHint("ComboBox")]
        [DisplayName("Need Next Interviewer")]
        public ComboBoxVM NeedNextInterviewer { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]
            {
                "",
                "Yes",
                "No"
            },
            //OnSelectEventName = "onPositionChange"
        };

        [UIHint("MultiFileUploader")]
        [DisplayName("Documents (Max. 2MB)")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();
    }
}

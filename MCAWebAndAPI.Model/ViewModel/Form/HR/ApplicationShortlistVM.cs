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
    public class ApplicationShortlistVM : Item
    {
        public IEnumerable<ShortlistDetailVM> ShortlistDetails { get; set; } = new List<ShortlistDetailVM>();

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

        public Boolean SendToCandidate { get; set; }

        [UIHint("TextArea")]
        public string EmailMessage { get; set; }

        [UIHint("TextArea")]
        public string Message { get; set; }

        public string Candidate { get; set; }

        public string Position { get; set; }

        /// <summary>
        /// InterviewerDate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("ID Card Expiry")]
        public DateTime? InterviewerDate { get; set; } = DateTime.Now;

        //[UIHint("Date")]
        //public string InterviewerDate { get; set; } = DateTime.UtcNow.ToShortDateString();

        [UIHint("Date")]
        public string Time { get; set; } = DateTime.UtcNow.ToShortTimeString();
    }
}

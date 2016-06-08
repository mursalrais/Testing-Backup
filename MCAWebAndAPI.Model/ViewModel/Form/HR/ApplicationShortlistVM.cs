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
        public IEnumerable<ShortlistDetailVM> ShortlistDetail { get; set; } = new List<ShortlistDetailVM>();

        /// <summary>
        /// Title
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Position { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetPositions",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "PositionName"
            
        };

        public string SendTo { get; set; }

        public string InterviewerPanel { get; set; }

        [UIHint("TextArea")]
        public string EmailMessage { get; set; }

        public string Candidate { get; set; }

        [UIHint("Date")]
        public string InterviewerDate { get; set; } = DateTime.UtcNow.ToShortDateString();

        [UIHint("Date")]
        public string Time { get; set; } = DateTime.UtcNow.ToShortTimeString();
    }
}

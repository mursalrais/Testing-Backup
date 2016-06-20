using System;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class InterviewlistDetailVM : Item
    {
        /// <summary>
        /// dateofbirth
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Date of Birth")]
        public DateTime? Date { get; set; } = DateTime.Now.AddYears(-28);

        public string Interviewpanel { get; set; }

        public string Interviewsum { get; set; }

        public string Result { get; set; }
    }
}

using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class EducationDetailVM : Item
    {
        public string University { get; set; }

        public string Subject { get; set; }

        [DisplayName("Year of Graduation")]
        [UIHint("Date")]
        public DateTime? YearOfGraduation { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }

    }
}
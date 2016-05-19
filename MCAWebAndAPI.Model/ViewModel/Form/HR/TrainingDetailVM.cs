using System;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class TrainingDetailVM
    {
        public string Institution { get; set; }

        public string Subject { get; set; }

        [UIHint("Date")]
        public DateTime? Year { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }


    }
}
using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class TrainingDetailVM : Item
    {
        public string Institution { get; set; }

        public string Subject { get; set; }

        DateTime? _year = DateTime.Now;
        
        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [UIHint("Month")]
        public DateTime? Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
            }
        }
    }
}
using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class TrainingDetailVM : Item
    {
        /// <summary>
        /// traininginstitution
        /// </summary>
        public string Institution { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Subject { get; set; }

        [DisplayName("Training Year")]
        public string StrTrainingYear { get; set; }

        /// <summary>
        /// trainingyear
        /// </summary>
        DateTime? _year = DateTime.Now;

        /// <summary>
        /// trainingremarks
        /// </summary>
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
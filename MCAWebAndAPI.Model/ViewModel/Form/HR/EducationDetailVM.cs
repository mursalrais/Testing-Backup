using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class EducationDetailVM : Item
    {
        /// <summary>
        /// university
        /// </summary>
        public string University { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// yearofgraduation
        /// </summary>
        [DisplayName("Graduation")]
        public string StrYearOfGraduations { get; set; }

        /// <summary>
        /// yearofgraduation
        /// </summary>
        [DisplayName("Graduation")]
        [UIHint("Month")]
        public DateTime? YearOfGraduation { get; set; } = DateTime.Now;

        /// <summary>
        /// remarks
        /// </summary>
        [UIHint("TextArea")]
        [DisplayName("Degree")]
        public string Remarks { get; set; }

    }
}
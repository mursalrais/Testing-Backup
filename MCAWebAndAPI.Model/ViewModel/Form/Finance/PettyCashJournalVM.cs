using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class PettyCashJournalVM : Item
    {
        /// <summary>
        /// Wireframe FIN13: Petty Cash Journal
        /// </summary>

        [DataType(DataType.Date)]
        [DisplayName("Date (from)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateFrom { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [DisplayName("Date (to)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateTo { get; set; } = DateTime.Today;

        [UIHint("Decimal")]
        [Required]
        [DisplayName("Total amount")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal TotalAmount { get; set; } = 0;
    }
}

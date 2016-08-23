using System;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Form.Finance;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public class PettyCashStatementVM : PettyCashTransactionItem
    {
        [UIHint("Date (from)")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; } = DateTime.Now;

        [UIHint("Date (to)")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; } = DateTime.Now;

        [UIHint("Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTransaction { get; set; } = DateTime.Now;
        
    }
}

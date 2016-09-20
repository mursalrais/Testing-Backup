using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public class PettyCashStatementVM : PettyCashTransactionItem
    {

        [Required]
        [DisplayName("Date (from)")]
        [UIHint("Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; } = DateTime.Now;


        [Required]
        [DisplayName("Date (to)")]
        [UIHint("Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; } = DateTime.Now;

        [Required]
        [UIHint("Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTransaction { get; set; } = DateTime.Now;

    }
}

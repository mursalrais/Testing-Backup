using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class PettyCashJournalItemVM : Item
    {
        /// <summary>
        /// Wireframe FIN13: Petty Cash Journal
        /// </summary>

        [Required]
        public string PCVNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date{ get; set; } = DateTime.Today;

        [Required]
        public string Payee { get; set; }

        [Required]
        [DisplayName("Description of expenses")]
        public string DescOfExpenses { get; set; }

        public string Fund { get; } = Shared.Fund;

        public string WBS { get; set; }

        public string GL { get; set; }

        [Required]
        [UIHint("Decimal")]
        [DisplayName("Total amount")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; } = 0;

        public int PettyCashJournalID { get; set; }
    }
}
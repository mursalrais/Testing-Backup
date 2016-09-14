using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public class PettyCashTransactionItem : Item
    {
        public enum Post { DR, CR }

        /// <summary>
        /// Transcation Date
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; } = DateTime.Today;

        public string TransactionType { get; set; }

        /// <summary>
        /// Transaction number or the document number
        /// </summary>
        public string TransactionNo { get; set; }

        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        [UIHint("Decimal")]
        public decimal? Amount { get; set; } = 0;

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal Balance { get; set; } = 0;

        public string WBSName { get; set; }

        public string WBSDescription { get; set; }

        public string GLName { get; set; }

        public string Payee { get; set; }

        public string DescOfExpenses { get; set; }
    }
}
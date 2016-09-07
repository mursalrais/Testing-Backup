using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// Wireframe FIN07: SCA Settlement 
    /// </summary>

    public class SCASettlementVM : Item
    {
        public string DocNo { get; set; }

        [Required]
        [UIHint("AjaxComboBox")]
        [DisplayName("SCA Voucher No")]
        public AjaxComboBoxVM SCAVoucher { get; set; } = new AjaxComboBoxVM();

        public string Description { get; set; }

        public string Fund { get; set; } = Shared.Fund;

        [Required]
        [UIHint("ComboBox")]
        [DisplayName("Type of Settlement")]
        public ComboBoxVM TypeOfSettlement { get; set; } = new ComboBoxVM();

        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        [DisplayName("Special Cash Advance")]
        public decimal SpecialCashAdvanceAmount { get; set; }

        [Required]
        [DisplayName("Total Expense")]
        public decimal TotalExpense { get; set; }

        [Required]
        [DisplayName("Received from/to")]
        public decimal ReceivedFromTo { get; set; }
        
        public IEnumerable<SCASettlementItemVM> ItemDetails { get; set; } = new List<SCASettlementItemVM>();

        public Operations Operation { get; set; }
    }
}

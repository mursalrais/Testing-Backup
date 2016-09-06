using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class SCAReimbursementVM: Item
    {
        /// <summary>
        /// this is equal to Title in Sharepointlist
        /// </summary>
        public string DocNo { get; set; }

        [Required]
        [UIHint("AjaxComboBox")]
        [DisplayName("Event Budget No.")]
        public AjaxComboBoxVM EventBudget { get; set; } = new AjaxComboBoxVM();


        public string Description { get; set; }

        public string Fund { get; set; } = Shared.Fund;

        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        [DisplayName("Total Amount Reimbursed")]
        public decimal Amount { get; set; }

        public IEnumerable<SCAReimbursementItemVM> ItemDetails { get; set; } = new List<SCAReimbursementItemVM>();

        public Operations Operation { get; set; }
    }
}

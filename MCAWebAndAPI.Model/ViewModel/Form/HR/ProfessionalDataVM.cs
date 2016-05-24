using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ProfessionalDataVM : ApplicationDataVM
    {
        public string EmergencyNumber { get; set; }

        public string OfficePhone { get; set; }

        [UIHint("EmailAddress")]
        public string OfficeEmail { get; set; }

        public IEnumerable<OrganizationalDetailVM> OrganizationalDetails { get; set; } = new List<OrganizationalDetailVM>();

        [RegularExpression("([0-9]+)")]
        public string Extension { get; set; }

        public IEnumerable<DependentDetailVM> DependentDetails = new List<DependentDetailVM>();
       
        [DisplayName("Account Name")]
        public string AccountNameForHI { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumberForHI { get; set; }

        [DisplayName("Bank Name")]
        public string BankNameForHI { get; set; }

        [DisplayName("Branch Office")]
        public string BranchOfficeForHI { get; set; }
        
        [DisplayName("RI")]
        public string VendorAccountNumberRIForHI { get; set; }

        [DisplayName("RJ")]
        public string VendorAccountNumberRJForHI { get; set; }

        [DisplayName("HI")]
        public string VendorAccountNumberRGForHI { get; set; }

        [DisplayName("MA")]
        public string VendorAccountNumberMAForHI { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Currency")]
        public CurrencyComboBoxVM CurrencyForHI { get; set; } = new CurrencyComboBoxVM();

        [DisplayName("Account Name")]
        public string AccountNameForSP { get; set; }

        [DisplayName("Bank Name")]
        public string BankNameForSP { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumberForSP { get; set; }

        [DisplayName("Branch Office")]
        public string BranchOfficeForSP { get; set; }

        [DisplayName("Account Name")]
        public string AccountNameForPayroll { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumberForPayroll { get; set; }

        [DisplayName("Bank Name")]
        public string BankNameForPayroll { get; set; }

        [DisplayName("Branch Office")]
        public string BranchOfficeForPayroll { get; set; }

        [DisplayName("Tax ID")]
        public string TaxIDForPayroll { get; set; }

        [DisplayName("NIK")]
        public string NIK { get; set; }

        [DisplayName("Name in Tax")]
        public string NameInTaxForPayroll { get; set; }

        [DisplayName("Bank Swift Code")]
        public string BankSwiftCodeForPayroll { get; set; }

        [DisplayName("Tax ID Address")]
        [UIHint("TextArea")]
        public string TaxIDAddress { get; set; }

        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM CurrencyForPayroll { get; set; } = new CurrencyComboBoxVM();

        [UIHint("Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDateForHI { get; set; } = DateTime.Now;


        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? EndDateForHI { get; set; } = DateTime.Now;

        [UIHint("Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDateForSP { get; set; } = DateTime.Now;

        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? EndDateForSP { get; set; } = DateTime.Now;
        [UIHint("ComboBox")]
        [DisplayName("Tax Status")]
        public ComboBoxVM TaxStatusForPayroll { get; set; } = new ComboBoxVM();

        [UIHint("ComboBox")]
        [DisplayName("Currency")]
        public CurrencyComboBoxVM CurrencyForSP { get; set; } = new CurrencyComboBoxVM();

    }
}

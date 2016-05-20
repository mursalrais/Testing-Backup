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

        public IEnumerable<OrganizationalDetailVM> OrganizationalDetails
        {
            get
            {
                return _organizationalDetails;
            }
            set
            {
                _organizationalDetails = value;
            }
        }

   

        [RegularExpression("([0-9]+)")]
        public string Extension { get; set; }
        

        public IEnumerable<DependentDetailVM> DependentDetails
        {
            get
            {
                return _dependentDetails;
            }
            set
            {
                _dependentDetails = value;
            }
        }

        IEnumerable<OrganizationalDetailVM> _organizationalDetails = new List<OrganizationalDetailVM>();
        IEnumerable<DependentDetailVM> _dependentDetails = new List<DependentDetailVM>();

        [DisplayName("Account Name")]
        public string AccountNameForHI { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumberForHI { get; set; }

        [DisplayName("Bank Name")]
        public string BankNameForHI { get; set; }

        [DisplayName("Branch Office")]
        public string BranchOfficeForHI { get; set; }

        DateTime? _effectiveDateForHI = DateTime.Now;

        DateTime? _endDateForHI = DateTime.Now;

        CurrencyComboBoxVM _currencyForHI = new CurrencyComboBoxVM();

        public string VendorAccountNumberRIForHI { get; set; }

        public string VendorAccountNumberRJForHI { get; set; }

        public string VendorAccountNumberRGForHI { get; set; }

        public string VendorAccountNumberMAForHI { get; set; }

        public string AccountNameForSP { get; set; }

        public string BankNameForSP { get; set; }

        public string AccountNumberForSP { get; set; }

        public string BranchOfficeForSP { get; set; }

        DateTime? _effectiveDateForSP = DateTime.Now;

        DateTime? _endDateForSP = DateTime.Now;

        CurrencyComboBoxVM _currencyForSP = new CurrencyComboBoxVM();

        public string AccountNameForPayroll { get; set; }

        public string AccountNumberForPayroll { get; set; }

        public string BankNameForPayroll { get; set; }

        public string BranchOfficeForPayroll { get; set; }

        ComboBoxVM _taxStatusForPayroll = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Status A",
                "Status B"
            },
            DefaultValue = "Status A"
        };

        public string TaxIDForPayroll { get; set; }

        public string NIK { get; set; }

        public string NameInTaxForPayroll { get; set; }

        public string BankSwiftCodeForPayroll { get; set; }

        public string TaxIDAddress { get; set; }


        [UIHint("Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDateForHI
        {
            get
            {
                return _effectiveDateForHI;
            }

            set
            {
                _effectiveDateForHI = value;
            }
        }

        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? EndDateForHI
        {
            get
            {
                return _endDateForHI;
            }

            set
            {
                _endDateForHI = value;
            }
        }

        [UIHint("Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDateForSP
        {
            get
            {
                return _effectiveDateForSP;
            }

            set
            {
                _effectiveDateForSP = value;
            }
        }

        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? EndDateForSP
        {
            get
            {
                return _endDateForSP;
            }
            set
            {
                _endDateForSP = value;
            }
        }

        [UIHint("ComboBox")]
        [DisplayName("Tax Status")]
        public ComboBoxVM TaxStatusForPayroll
        {
            get
            {
                return _taxStatusForPayroll;
            }
            set
            {
                _taxStatusForPayroll = value;
            }
        }

        [UIHint("ComboBox")]
        [DisplayName("Currency")]
        public CurrencyComboBoxVM CurrencyForSP
        {
            get
            {
                return _currencyForSP;
            }
            set
            {
                _currencyForSP = value;
            }
        }

    }
}

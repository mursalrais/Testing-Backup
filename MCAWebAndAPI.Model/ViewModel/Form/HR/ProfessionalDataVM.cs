using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ProfessionalDataVM : ApplicationDataVM
    {
        [UIHint("ComboBox")]
        [DisplayName("Project Unit")]
        [Required]
        public ComboBoxVM DivisionProjectUnit { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]
            {
                "Communications & Outreach Unit",
                "Community-Based Health & Nutrition Project",
                "Cross-Cutting Sector",
                "Economic Analysis Unit",
                "Environment & Social Performance Unit",
                "Executive Office",
                "Finance Unit",
                "Green Prosperity Project",
                "Human Resources Unit",
                "Information Technology Unit",
                "Legal Unit",
                "Monitoring & Evaluation Unit",
                "Office Management",
                "Operations Support Div.",
                "Procurement Modernization Project",
                "Procurement Unit",
                "Program Div.",
                "Risk & Audit Unit",
                "Social & Gender Assessment Unit"
            }
        };

        /// <summary>
        /// Position
        /// </summary>
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM CurrentPosition { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetPositionsManpower",
            ControllerName = "HRDataMaster",
            TextField = "PositionName",
            ValueField = "ID",
            Filter = "filterLevel",
            Cascade = "DivisionProjectUnit_Value"
        };

        /// <summary>
        /// Professional_x0020_Status
        /// </summary>
        [UIHint("ComboBox")]
        public ComboBoxVM ProfessionalStatus { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Active",
                "Non Active"
            }
        };


        /// <summary>
        /// Join_x0020_Date
        /// </summary>
        [UIHint("Date")]
        public DateTime? JoinDate { get; set; } = DateTime.Now;

        /// <summary>
        /// datavalidationstatus
        /// </summary>
        public string ValidationStatus { get; set; } =
            Workflow.GetProfessionalValidationStatus(Workflow.ProfessionalValidationStatus.VALIDATED);

        public string ValidationAction { get; set; }

        /// <summary>
        /// emergencynumber
        /// </summary>
        public string EmergencyNumber { get; set; }

        /// <summary>
        /// officephone
        /// </summary>
        public string OfficePhone { get; set; }

        /// <summary>
        /// officeemail
        /// </summary>
        /// 
        [Required]
        [UIHint("EmailAddress")]
        public string OfficeEmail { get; set; }

        public IEnumerable<DependentDetailVM> DependentDetails { get; set; } = new List<DependentDetailVM>();

        public IEnumerable<OrganizationalDetailVM> OrganizationalDetails { get; set; } = new List<OrganizationalDetailVM>();

        /// <summary>
        /// Extension
        /// </summary>
        [RegularExpression("([0-9]+)")]
        public string Extension { get; set; }

        /// <summary>
        /// hiaccountname
        /// </summary>
        [DisplayName("Account Name")]
        public string AccountNameForHI { get; set; }

        /// <summary>
        /// hiaccountnr
        /// </summary>
        [DisplayName("Account Number")]
        public string AccountNumberForHI { get; set; }

        /// <summary>
        /// hibankname
        /// </summary>
        [DisplayName("Bank Name")]
        public string BankNameForHI { get; set; }

        /// <summary>
        /// hibankbranchoffice
        /// </summary>
        [DisplayName("Branch Office")]
        public string BranchOfficeForHI { get; set; }

        /// <summary>
        /// hiriaccountnr
        /// </summary>
        [DisplayName("RI")]
        public string VendorAccountNumberRIForHI { get; set; }

        /// <summary>
        /// hirjaccountnr
        /// </summary>
        [DisplayName("RJ")]
        public string VendorAccountNumberRJForHI { get; set; }

        /// <summary>
        /// hirgaccountnr
        /// </summary>
        [DisplayName("RG")]
        public string VendorAccountNumberRGForHI { get; set; }

        /// <summary>
        /// himaaccountnr
        /// </summary>
        [DisplayName("MA")]
        public string VendorAccountNumberMAForHI { get; set; }

        /// <summary>
        /// hicurrency
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Currency")]
        public CurrencyComboBoxVM CurrencyForHI { get; set; } = new CurrencyComboBoxVM();

        /// <summary>
        /// spaccountname
        /// </summary>
        [DisplayName("Account Name")]
        public string AccountNameForSP { get; set; }

        /// <summary>
        /// spbankname
        /// </summary>
        [DisplayName("Bank Name")]
        public string BankNameForSP { get; set; }

        /// <summary>
        /// spaccountnr
        /// </summary>
        [DisplayName("Account Number")]
        public string AccountNumberForSP { get; set; }

        /// <summary>
        /// spbranchoffice
        /// </summary>
        [DisplayName("Branch Office")]
        public string BranchOfficeForSP { get; set; }

        /// <summary>
        /// payrollaccountname
        /// </summary>
        [DisplayName("Account Name")]
        public string AccountNameForPayroll { get; set; }

        /// <summary>
        /// payrollaccountnr
        /// </summary>
        [DisplayName("Account Number")]
        public string AccountNumberForPayroll { get; set; }

        /// <summary>
        /// payrollbankname
        /// </summary>
        [DisplayName("Bank Name")]
        public string BankNameForPayroll { get; set; }

        /// <summary>
        /// payrollbranchoffice
        /// </summary>
        [DisplayName("Branch Office")]
        public string BranchOfficeForPayroll { get; set; }

        /// <summary>
        /// payrollbankswiftcode
        /// </summary>
        [DisplayName("Bank Swift Code")]
        public string BankSwiftCodeForPayroll { get; set; }

        /// <summary>
        /// payrollcurrency
        /// </summary>
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM CurrencyForPayroll { get; set; } = new CurrencyComboBoxVM();

        /// <summary>
        /// hieffectivedate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDateForHI { get; set; } = DateTime.Now;

        /// <summary>
        /// hienddate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? EndDateForHI { get; set; } = DateTime.Now;

        /// <summary>
        /// speffectivedate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDateForSP { get; set; } = DateTime.Now;

        /// <summary>
        /// spenddate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("End Date")]
        public DateTime? EndDateForSP { get; set; } = DateTime.Now;

        /// <summary>
        /// payrolltaxstatus
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Tax Status")]
        public ComboBoxVM TaxStatusForPayroll { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "TK0","TK1","TK2","TK3","K0","K1","K2","K3"
            }
        };

        /// <summary>
        /// spcurrency
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Currency")]
        public CurrencyComboBoxVM CurrencyForSP { get; set; } = new CurrencyComboBoxVM();

    }
}

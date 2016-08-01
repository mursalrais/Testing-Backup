using System;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PayrollDetailVM
    {
        [DisplayName("Professional ID")]
        public string ProfessionalID { get; set; }

        [DisplayName("Full Name")]
        public string Name { get; set; }

        [DisplayName("Project/Unit")]
        public string ProjectUnit { get; set; }

        [DisplayName("Position")]
        public string Position { get; set; }

        [DisplayName("Join Date")]
        public DateTime JoinDate { get; set; }

        [DisplayName("Date of New PSA ")]
        public DateTime DateOfNewPSA { get; set; }

        [DisplayName("End of Contract")]
        public DateTime EndOfContract { get; set; }

        [DisplayName("Last Working Day")]
        public DateTime LastWorkingDay { get; set; }

        [DisplayName("STATUS")]
        public DateTime Status { get; set; }

        [DisplayName("Monthly Fee")]
        public double MonthlyFee { get; set; }

        [DisplayName("Currency")]
        public string Currency { get; set; }

        [DisplayName("Unpaid Day-Off")]
        public double UnpaidDayOff { get; set; }

        [DisplayName("Base")]
        public double Base { get; set; }

        [DisplayName("Adjustment")]
        public double Adjustment { get; set; }

        [DisplayName("Spot Award")]
        public double SpotAward { get; set; }

        [DisplayName("Retention Award")]
        public double RetentionAward { get; set; }

        [DisplayName("Overtime")]
        public double Overtime { get; set; }

        [DisplayName("13th Months")]
        public double ThirteenthMonth { get; set; }

        [DisplayName("Day-Off Balance")]
        public double DayOffBalance { get; set; }

        [DisplayName("Payment ")]
        public double Payment { get; set; }

        [DisplayName("Deduction")]
        public double Deduction{ get; set; }

        [DisplayName("Take Home Pay")]
        public double TakeHomePay { get; set; }

        [DisplayName("Saving Fund")]
        public double SavingFund { get; set; }

        [DisplayName("Bank Account Name ")]
        public string BankAccountName { get; set; }

        [DisplayName("Currency")]
        public string BankAccountCurrency { get; set; }

        [DisplayName("Bank Account Number ")]
        public string BankAccountNumber { get; set; }

        [DisplayName("Bank Name")]
        public string BankName { get; set; }

        [DisplayName("Branch Office")]
        public string BankBranchOffice { get; set; }
    }
}

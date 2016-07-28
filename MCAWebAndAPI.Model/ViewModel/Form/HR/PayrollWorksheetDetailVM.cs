using System;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PayrollWorksheetDetailVM
    {
        [DisplayName("Payroll Date")]
        public DateTime PayrollDate { get; set; }

        [DisplayName("Last 13th Months Date ")]
        public DateTime Last13thMonthDate { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }


        [DisplayName("Project/Unit")]
        public string ProjectUnit { get; set; }


        [DisplayName("Position")]
        public string Position { get; set; }

        [DisplayName("Join Date")]
        public DateTime JoinDate { get; set; }

        [DisplayName("Date of New PSA")]
        public DateTime DateOfNewPSA { get; set; }

        [DisplayName("Last Working Date")]
        public DateTime LastWorkingDate { get; set; }

        [DisplayName("PSA Number")]
        public string PSANumber { get; set; }

        [DisplayName("Date of New Fee")]
        public DateTime DateOfNewFee { get; set; }

        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }


        [DisplayName("Monthly Fee Master")]
        public double MonthlyFeeMaster { get; set; }

        [DisplayName("Total Working Days")]
        public int TotalWorkingDays { get; set; }

        [DisplayName("Propotional Monthly Fee")]
        public double PropotionalMonthlyFee { get; set; }


        [DisplayName("Remarks")]
        public string Remarks { get; set; }


        [DisplayName("Days Request Unpaid")]
        public int DaysRequestUnpaid { get; set; }


        [DisplayName("Unpaid Day-Off")]
        public double UnpaidDayOff { get; set; }


        [DisplayName("Base")]
        public double Base { get; set; }

        [DisplayName("Adjustment")]
        public double Adjustment { get; set; }

        [DisplayName("Spot Award")]
        public double SpotAward { get; set; }

        [DisplayName("Retention Payment")]
        public double RetentionPayment { get; set; }

        [DisplayName("Overtime")]
        public double Overtime { get; set; }

        [DisplayName("Last Working Date PSA Terakhir berada di dalam periode payroll (Join Date > Date 13th Months Terakhir)")]
        public double LastWorkingDatePSAGreaterThan { get; set; }

        [DisplayName("Last Working Date PSA Terakhir berada di dalam periode payroll (Join Date < Date 13th Months Terakhir)")]
        public double LastWorkingDatePSALessThan { get; set; }

        [DisplayName("13th Months Final")]
        public double ThirteenthMonthFinal { get; set; }

        [DisplayName("Balance (Tidak ada PSA lagi setelah Last Working Date)")]
        public double BalanceWithoutPSA { get; set; }

        [DisplayName("Balance (Jika Date of New PSA > Last Working Date PSA sebelumnya + 1) ")]
        public double BalanceWithPSA { get; set; }

        [DisplayName("Day-Off Balance")]
        public double DayOffBalance { get; set; }

        [DisplayName("Payment")]
        public double Payment { get; set; }

        [DisplayName("Deduction")]
        public double Deduction { get; set; }

        [DisplayName("Take Home Pay")]
        public double TakeHomePay { get; set; }

        [DisplayName("Saving Fund")]
        public double SavingFund { get; set; }

        [DisplayName("Bank Account Name")]
        public string BankAccountName { get; set; }

        [DisplayName("Currency")]
        public string Currency { get; set; }

        [DisplayName("Bank Account Number")]
        public string BankAccountNumber { get; set; }

        [DisplayName("Bank Name")]
        public string BankName { get; set; }

        [DisplayName("Branch Office")]
        public string BankBranchOffice { get; set; }
        
    }
}

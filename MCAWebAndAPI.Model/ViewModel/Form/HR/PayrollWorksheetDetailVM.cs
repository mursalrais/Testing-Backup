using System;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    /// <summary>
    /// ViewModel used to export Timesheet calculation result as an excel file
    /// </summary>
    public class PayrollWorksheetDetailVM : Item
    {
        public int ProfessionalID { get; set; }

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

        /// <summary>
        /// Monthly Fee Master / Total Working Days
        /// </summary>
        [DisplayName("Propotional Monthly Fee")]
        public double PropotionalMonthlyFee
        {
            get
            {
                return (MonthlyFeeMaster / TotalWorkingDays).ConvertInfinityOrNanToZero();
            }
        }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        
        [DisplayName("Days Request Unpaid")]
        public int DaysRequestUnpaid { get; set; }

        /// <summary>
        /// Propotional Monthly Fee * Days Request Unpaid
        /// </summary>
        [DisplayName("Unpaid Day-Off")]
        public double UnpaidDayOff
        {
            get
            {
                return (PropotionalMonthlyFee / DaysRequestUnpaid).ConvertInfinityOrNanToZero();
            }
        }

        /// <summary>
        /// Propotional Monthly Fee - Unpaid Day-Off
        /// </summary>
        [DisplayName("Base")]
        public double Base
        {
            get
            {
                return (PropotionalMonthlyFee - UnpaidDayOff).ConvertInfinityOrNanToZero();
            }
        }

        [DisplayName("Adjustment")]
        public double Adjustment { get; set; }

        [DisplayName("Spot Award")]
        public double SpotAward { get; set; }

        [DisplayName("Retention Payment")]
        public double RetentionPayment { get; set; }

        [DisplayName("Overtime")]
        public double Overtime { get; set; }

        /// <summary>
        /// ((Last Working Date-Join Date)+1)/(365*Monthly Fee Master)
        /// </summary>
        [DisplayName("Last Working Date PSA Terakhir berada di dalam periode payroll (Join Date > Date 13th Months Terakhir)")]
        public double LastWorkingDatePSAGreaterThan
        {
            get
            {
                return ((LastWorkingDate.Subtract(JoinDate).Days) + 1) / (365 * MonthlyFeeMaster).ConvertInfinityOrNanToZero();
            }
        }

        /// <summary>
        /// ((Last Working Date-Last 13th Months Date)+1)/(365*Monthly Fee Master)
        /// </summary>
        [DisplayName("Last Working Date PSA Terakhir berada di dalam periode payroll (Join Date < Date 13th Months Terakhir)")]
        public double LastWorkingDatePSALessThan
        {
            get
            {
                return ((LastWorkingDate.Subtract(Last13thMonthDate).Days) + 1) / (365 * MonthlyFeeMaster).ConvertInfinityOrNanToZero();
            }
        }

        /// <summary>
        /// LastWorkingDatePSAGreaterThan + LastWorkingDatePSALessThan
        /// </summary>
        [DisplayName("13th Months Final")]
        public double ThirteenthMonthFinal
        {
            get
            {
                return (LastWorkingDatePSAGreaterThan + LastWorkingDatePSALessThan).ConvertInfinityOrNanToZero();
            }
        }

        [DisplayName("Balance (Tidak ada PSA lagi setelah Last Working Date)")]
        public double BalanceWithoutPSA { get; set; }

        [DisplayName("Balance (Jika Date of New PSA > Last Working Date PSA sebelumnya + 1) ")]
        public double BalanceWithPSA { get; set; }

        /// <summary>
        /// (BalanceWithoutPSA + BalanceWithPSA) * Propotional Monthly Fee
        /// </summary>
        [DisplayName("Day-Off Balance")]
        public double DayOffBalance
        {
            get
            {
                return ((BalanceWithoutPSA + BalanceWithPSA) * PropotionalMonthlyFee).ConvertInfinityOrNanToZero();
            }
        }

        /// <summary>
        /// Base + Adjustment + Spot Award + Retention Payment + Overtime + 13th Months Final + Day-Off Balance
        /// </summary>
        [DisplayName("Payment")]
        public double Payment
        {
            get
            {
                return (Base + Adjustment + SpotAward + RetentionPayment + Overtime + ThirteenthMonthFinal + DayOffBalance).ConvertInfinityOrNanToZero();
            }
        }

        [DisplayName("Deduction")]
        public double Deduction { get; set; }

        /// <summary>
        /// Payment + Deduction
        /// </summary>
        [DisplayName("Take Home Pay")]
        public double TakeHomePay
        {
            get
            {
                return (Payment + Deduction).ConvertInfinityOrNanToZero();
            }
        }

        [DisplayName("Saving Fund")]
        public double SavingFund
        {
            get
            {
                return (5 / 100 * Base).ConvertInfinityOrNanToZero();
            }
        }

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

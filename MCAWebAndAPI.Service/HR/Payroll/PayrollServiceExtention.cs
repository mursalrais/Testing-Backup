using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.HR.Leave;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    /// <summary>
    /// Implementation extension for HR Payroll Service
    /// </summary>
    public static class PayrollServiceExtention
    {
        private static DateTime Last13MonthDate_HardCoded = new DateTime(2016, 7, 7);
        private static string _siteUrl;

        /// <summary>
        /// In-Memory List to cut network round-trip time
        /// </summary>
        private static IEnumerable<ProfessionalMaster> _allProfessionals;
        private static IEnumerable<PSAMaster> _allValidPSAs;
        private static IEnumerable<MonthlyFeeMaster> _allProfessionalMonthlyFees;
        private static IEnumerable<DayOffRequest> _allProfessionalDayOffRequests;
        private static IEnumerable<EventCalendar> _allHolidaysAndPublicHolidays;
        private static IEnumerable<AdjustmentMaster> _allAdjustments;

        private static int[] _professionalIDs;
        private static DateTime[] _dateRanges;

        /// <summary>
        /// Services from all depependent transactions
        /// </summary>
        private static IPSAManagementService _psaService;
        private static IDataMasterService _dataMasterService;
        private static IPayrollService _payrollService;
        private static ICalendarService _calendarService;
        private static IDayOffService _dayOffService;

        public static void SetSiteUrl(this List<PayrollWorksheetDetailVM> payrollWorksheet, string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);

            _psaService = new PSAManagementService();
            _dataMasterService = new DataMasterService();
            _payrollService = new PayrollService();
            _calendarService = new CalendarService();
            _dayOffService = new DayOffService();

            _psaService.SetSiteUrl(_siteUrl);
            _dataMasterService.SetSiteUrl(_siteUrl);
            _psaService.SetSiteUrl(_siteUrl);
            _calendarService.SetSiteUrl(_siteUrl);
            _dayOffService.SetSiteUrl(_siteUrl);
        }

        /// <summary>
        /// To retrived all required master data in order to minimise network round-trip time
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        public static async Task PopulateAllProfessionals(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            _allProfessionals = _allProfessionals ?? _dataMasterService.GetProfessionals();
        }

        /// <summary>
        /// To retrived all required master data in order to minimise network round-trip time
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="startDatePeriod"></param>
        /// <returns></returns>
        public static async Task PopulateAllValidPSAs(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime startDatePeriod)
        {
            _allValidPSAs = _psaService.GetPSAs(startDatePeriod);
        }

        /// <summary>
        /// To retrived all required master data in order to minimise network round-trip time
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="startDatePeriod"></param>
        /// <returns></returns>
        public static async Task PopulateALLAdjustment(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<int> professionalIDs)
        {
            _allAdjustments = _dataMasterService.GetAdjustemnt(professionalIDs.ToArray());
        }

        /// <summary>
        /// To retrived all required master data in order to minimise network round-trip time
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="professionalIDs"></param>
        /// <returns></returns>
        public static async Task PopulateAllProfessionalMonthlyFee(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<int> professionalIDs)
        {
            _allProfessionalMonthlyFees = _dataMasterService.GetMonthlyFees(professionalIDs.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="professionalIDs"></param>
        /// <returns></returns>
        public static async Task PopulateAllProfessionalDayOffRequests(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<int> professionalIDs)
        {
            _allProfessionalDayOffRequests = _dayOffService.GetDayOffRequests(professionalIDs.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="professionalIDs"></param>
        /// <returns></returns>
        public static async Task PopulateAllHolidaysAndPublicHolidays(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<DateTime> dateRange)
        {
            var listHolidaysAndPublicHolidays = new List<EventCalendar>();
            listHolidaysAndPublicHolidays.AddRange(_calendarService.GetHolidays(dateRange));
            listHolidaysAndPublicHolidays.AddRange(_calendarService.GetPublicHolidays(dateRange));

            _allHolidaysAndPublicHolidays = listHolidaysAndPublicHolidays;
        }

        /// <summary>
        /// To produce rows for all professionals who have valid PSA based on given period listed per day
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="dateRange">Start date until next date of period</param>
        /// <param name="professionalIDs">All professional IDs whose PSA is valid</param>
        /// <returns></returns>
        public static List<PayrollWorksheetDetailVM> PopulateRows(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<DateTime> dateRange, IEnumerable<int> professionalIDs)
        {
            foreach (var professionalID in professionalIDs)
            {
                foreach (var date in dateRange)
                {
                    payrollWorksheet.AddPayrollWorksheetDetailRow(date, professionalID);
                }
            }

            // Put the professional IDs and daterange in the static variables
            _professionalIDs = payrollWorksheet.Select(e => e.ProfessionalID).Distinct().ToArray();
            _dateRanges = dateRange.ToArray();
            
            return payrollWorksheet;
        }

        /// <summary>
        /// Populate worksheet row-wise
        /// PLEASE NOTE: Last13thMonth is hardcoded since it has not been implemented yet as per Aug, 10th 2016
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="date"></param>
        /// <param name="indexProfessional"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> AddPayrollWorksheetDetailRow(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime date, int indexProfessional)
        {
            payrollWorksheet.Add(new PayrollWorksheetDetailVM
            {
                PayrollDate = date,
                ProfessionalID = indexProfessional, 
                //TODO: To get this value from 13th Month Salary module
                Last13thMonthDate = Last13MonthDate_HardCoded
            });

            return payrollWorksheet;
        }

        /// <summary>
        /// Summarize Worksheet means to aggregate the columns grouped by Professional
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        public static List<PayrollWorksheetDetailVM> SummarizeData(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var summarizedPayrollWorksheets = new List<PayrollWorksheetDetailVM>();

            foreach (var professionalID in _professionalIDs)
            {
                var payrollSummaryRow = payrollWorksheet.GeneratePayrollSummaryRow(professionalID);
                summarizedPayrollWorksheets.Add(payrollSummaryRow);
            }

            // return the summarized version of the worksheet
            return summarizedPayrollWorksheets;
        }

        /// <summary>
        /// Populate worksheet row-wise
        /// PLEASE NOTE: Last13thMonth is hardcoded since it has not been implemented yet as per Aug, 10th 2016
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="date"></param>
        /// <param name="indexProfessional"></param>
        /// <returns></returns>
        private static PayrollWorksheetDetailVM GeneratePayrollSummaryRow(this List<PayrollWorksheetDetailVM> payrollWorksheet, int indexProfessional)
        {
            var payrollWorksheetVM = new PayrollWorksheetDetailVM();
            
            //TODO: To aggregate column from detail view to pivot view
            
            return payrollWorksheetVM;
        }

        /// <summary>
        /// Populate worksheet column-wise
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        public static async Task<List<PayrollWorksheetDetailVM>> PopulateColumns(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            payrollWorksheet.PopulateColumnsFromPSA();
            payrollWorksheet.PopulateColumnsFromMonthlyFee();
            payrollWorksheet.PopulateColumnsFromProfessional();
            payrollWorksheet.PopulateColumnsFromEventCalendar();
            payrollWorksheet.PopulateColumnsFromDayOff();
            payrollWorksheet.PopulateColumnsFromAdjustment();

            return payrollWorksheet;
        }

        /// <summary>
        /// Columns retrived from Day-Off Request: DaysRequestUnpaid
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromDayOff(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var rowIndex = 0;
            foreach (var professionalID in _professionalIDs)
            {
                foreach (var date in _dateRanges)
                {
                    payrollWorksheet[rowIndex++].DaysRequestUnpaid =
                        IsUnpaidDayOff(professionalID, date) ? 1 : 0;
                }
            }

            return payrollWorksheet;
        }

        private static bool IsUnpaidDayOff(int professionalID, DateTime date)
        {
            var dayOffRequestData = _allProfessionalDayOffRequests.FirstOrDefault(e => e.ProfessionalID == professionalID
                && e.StartDate <= date && date <= e.EndDate && e.DayOffType == "Unpaid Day-Off");

            return dayOffRequestData != null;
        }

        /// <summary>
        /// Columns retrived from Professional Master: Name, ProjectUnit, Position, BankAccountName, BankAccountNumber, Currency
        /// BankName, and BankBranchOffice
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromProfessional(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                var professionalData = _allProfessionals.FirstOrDefault(m => m.ID == _professionalIDs[indexProfessional]);

                if (professionalData == null)
                    continue;

                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var rowIndex = indexProfessional * _dateRanges.Length + indexDate;
                    payrollWorksheet[rowIndex].Name = professionalData.Name;
                    payrollWorksheet[rowIndex].ProjectUnit = professionalData.Project_Unit;
                    payrollWorksheet[rowIndex].Position = professionalData.Position;
                    payrollWorksheet[rowIndex].BankAccountName = professionalData.BankAccountName;
                    payrollWorksheet[rowIndex].BankAccountNumber = professionalData.BankAccountNumber;
                    payrollWorksheet[rowIndex].Currency = professionalData.Currency;
                    payrollWorksheet[rowIndex].BankName = professionalData.BankName;
                    payrollWorksheet[rowIndex].BankBranchOffice = professionalData.BankBranchOffice;
                }
            }

            return payrollWorksheet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromPSA(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var psaData = _allValidPSAs.FirstOrDefault(e =>
                        e.ProfessionalID == _professionalIDs[indexProfessional] + string.Empty &&
                        IsInScopePSA(_dateRanges[indexDate], e));

                    if (psaData == null)
                        continue;

                    var rowIndex = indexProfessional * _dateRanges.Length + indexDate;

                    payrollWorksheet[rowIndex].JoinDate = psaData.JoinDate;
                    payrollWorksheet[rowIndex].DateOfNewPSA = psaData.DateOfNewPSA;
                    payrollWorksheet[rowIndex].PSANumber = psaData.PSANumber;
                    payrollWorksheet[rowIndex].LastWorkingDate = psaData.LastWorkingDate;
                }
            }

            return payrollWorksheet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="psaMaster"></param>
        /// <returns></returns>
        private static bool IsInScopePSA(DateTime date, PSAMaster psaMaster)
        {
            var psaDateOfNewPSA = psaMaster.DateOfNewPSA;
            var psaExpiryDate = psaMaster.PSAExpiryDate;

            return psaDateOfNewPSA <= date && date <= psaExpiryDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromMonthlyFee(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var rowIndex = indexProfessional * _dateRanges.Length + indexDate;

                    // Skip for condition without valid PSA
                    if (string.IsNullOrEmpty(payrollWorksheet[rowIndex].PSANumber))
                    {
                        payrollWorksheet[rowIndex].Remarks = "No PSA";
                        continue;
                    }

                    // Skip for public holidays, sunday, and saturday
                    if (_allHolidaysAndPublicHolidays.FirstOrDefault(e => 
                      e.Date.Day == _dateRanges[indexDate].Day &&
                      e.Date.Month == _dateRanges[indexDate].Month &&
                      e.Date.Year == _dateRanges[indexDate].Year) != null)
                    {
                        payrollWorksheet[rowIndex].Remarks = "Holiday";
                        continue;
                    }
                     
                    
                    var monthlyFeeData = _allProfessionalMonthlyFees
                        .FirstOrDefault(e => e.ProfessionalID == _professionalIDs[indexProfessional]
                        && IsInScopeMonthlyFee(_dateRanges[indexDate], e));

                    if (monthlyFeeData == null)
                        continue;
                    
                    payrollWorksheet[rowIndex].DateOfNewFee = monthlyFeeData.DateOfNewFee;
                    payrollWorksheet[rowIndex].EndDate = monthlyFeeData.EndDate;
                    payrollWorksheet[rowIndex].MonthlyFeeMaster = monthlyFeeData.MonthlyFee;
                }
            }

            return payrollWorksheet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromEventCalendar(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var totalWorkingDays = _calendarService.GetTotalWorkingDays(_dateRanges);
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var rowIndex = indexProfessional * _dateRanges.Length + indexDate;
                    payrollWorksheet[rowIndex].TotalWorkingDays = totalWorkingDays;
                }
            }

            return payrollWorksheet;
        }

        private static bool IsInScopeMonthlyFee(DateTime date, MonthlyFeeMaster monthlyFees)
        {
            return monthlyFees.DateOfNewFee <= date && date <= monthlyFees.EndDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetValidProfessionalIDs(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime startTime)
        {
            var startTimeUniversalString = startTime.ToUniversalTime().ToString("o");
            var caml = @"<View><Query><Where><Geq><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>"
              + startTimeUniversalString
              + @"</Value></Geq></Where><OrderBy><FieldRef Name='renewalnumber' Ascending='False' /></OrderBy></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='professional' /></ViewFields> </View>";

            var ids = new List<int>();
            foreach (var item in SPConnector.GetList("PSA", _siteUrl, caml))
            {
                var id = (int)FormatUtil.ConvertLookupToID(item, "professional");
                if (!ids.Contains(id))
                    ids.Add(id);
            }
            
            return ids.OrderBy(e=> e).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromAdjustment(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var rowIndex = 0;
            foreach (var professionalID in _professionalIDs)
            {
                foreach (var date in _dateRanges)
                {
                    payrollWorksheet[rowIndex++].Adjustment =
                        IsAdjustment(professionalID, date) ? 1 : 0;

                    payrollWorksheet[rowIndex++].SpotAward =
                        IsSpotAward(professionalID, date) ? 1 : 0;

                    payrollWorksheet[rowIndex++].RetentionPayment =
                        IsRetention(professionalID, date) ? 1 : 0;

                    payrollWorksheet[rowIndex++].Overtime =
                        IsOvertime(professionalID, date) ? 1 : 0;

                    payrollWorksheet[rowIndex++].Deduction =
                        IsDeduction(professionalID, date) ? 1 : 0;
                }
            }

            return payrollWorksheet;
        }

        private static bool IsAdjustment(int professionalID, DateTime date)
        {
            var adjustmentData = _allAdjustments.FirstOrDefault(e => e.ProfessionalID == professionalID
                && e.AdjustmentPeriod <= date && date <= e.AdjustmentPeriod && e.AdjustmentType == "Adjustment");

            return adjustmentData != null;
        }

        private static bool IsSpotAward(int professionalID, DateTime date)
        {
            var adjustmentData = _allAdjustments.FirstOrDefault(e => e.ProfessionalID == professionalID
                && e.AdjustmentPeriod <= date && date <= e.AdjustmentPeriod && e.AdjustmentType == "Spot Award");

            return adjustmentData != null;
        }

        private static bool IsRetention(int professionalID, DateTime date)
        {
            var adjustmentData = _allAdjustments.FirstOrDefault(e => e.ProfessionalID == professionalID
                && e.AdjustmentPeriod <= date && date <= e.AdjustmentPeriod && e.AdjustmentType == "Retention Payment");

            return adjustmentData != null;
        }

        private static bool IsOvertime(int professionalID, DateTime date)
        {
            var adjustmentData = _allAdjustments.FirstOrDefault(e => e.ProfessionalID == professionalID
                && e.AdjustmentPeriod <= date && date <= e.AdjustmentPeriod && e.AdjustmentType == "Overtime");

            return adjustmentData != null;
        }

        private static bool IsDeduction(int professionalID, DateTime date)
        {
            var adjustmentData = _allAdjustments.FirstOrDefault(e => e.ProfessionalID == professionalID
                && e.AdjustmentPeriod <= date && date <= e.AdjustmentPeriod && e.AdjustmentType == "Deduction");

            return adjustmentData != null;
        }
    }
}

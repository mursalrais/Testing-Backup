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
    public static class PayrollHelper
    {

        private static string _siteUrl;
        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PROF_LIST_NAME = "Professional Master";
        const string SP_MON_FEE_DETAIL_LIST_NAME = "Monthly Fee Detail";

        private static IEnumerable<ProfessionalMaster> _allProfessionals;
        private static IEnumerable<PSAMaster> _allValidPSAs;
        private static IEnumerable<MonthlyFeeMaster> _allProfessionalMonthlyFees;

        private static IPSAManagementService _psaService;
        private static IDataMasterService _dataMasterService;
        private static IPayrollService _payrollService;
        private static ICalendarService _calendarService;
        private static IDayOffService _dayOffService;

        private static int[] _professionalIDs;
        private static DateTime[] _dateRanges;

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
        /// To retrived all required master data in order to minimise network round trip time
        /// </summary>
        /// <param name="startDatePeriod"></param>
        /// <returns></returns>
        public static async Task PopulateAllProfessionals(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            _allProfessionals = _allProfessionals ?? _dataMasterService.GetProfessionals();
        }

        public static async Task PopulateAllValidPSAs(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime startDatePeriod)
        {
            _allValidPSAs = _psaService.GetPSAs(startDatePeriod);
        }

        public static async Task PopulateAllProfessionalMonthlyFee(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<int> professionalIDs)
        {
            _allProfessionalMonthlyFees = _dataMasterService.GetMonthlyFees(professionalIDs.ToArray());
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
            _professionalIDs = payrollWorksheet.Select(e => e.ProfessionalID).ToArray();
            _dateRanges = dateRange.ToArray();


            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> AddPayrollWorksheetDetailRow(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime date, int indexProfessional)
        {
            payrollWorksheet.Add(new PayrollWorksheetDetailVM
            {
                PayrollDate = date,
                ProfessionalID = indexProfessional
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
                var payrollRow = payrollWorksheet.FirstOrDefault(e => e.ProfessionalID == professionalID);

                summarizedPayrollWorksheets.Add(new PayrollWorksheetDetailVM
                {
                    ProfessionalID = payrollRow.ProfessionalID,
                    PayrollDate = payrollRow.PayrollDate
                    //TODO: To aggregate each columns
                });
            }
            // return the summarized version of the worksheet
            return summarizedPayrollWorksheets;
        }

        public static async Task<List<PayrollWorksheetDetailVM>> PopulateColumns(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            //Populate columns
            payrollWorksheet.PopulateColumnsFromPSA();
            payrollWorksheet.PopulateColumnsFromMonthlyFee();
            payrollWorksheet.PopulateColumnsFromProfessional();
            payrollWorksheet.PopulateColumnsFromEventCalendar();
            payrollWorksheet.PopulateColumnsFromDayOff();

            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromDayOff(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var rowIndex = 0;
            foreach (var professionalID in _professionalIDs)
            {
                foreach (var date in _dateRanges)
                {
                    payrollWorksheet[rowIndex++].DaysRequestUnpaid =
                        _dayOffService.IsUnpaidDayOff(professionalID, date, _dateRanges) ? 1 : 0;
                }
            }return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromProfessional(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                var professionalData = _allProfessionals.FirstOrDefault(m => m.ID == indexProfessional);
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var rowIndex = indexProfessional * indexDate + indexDate;
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

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromPSA(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var psaData = _allValidPSAs.FirstOrDefault(e =>
                        e.ProfessionalID == indexProfessional + string.Empty &&
                        IsInScopePSA(_dateRanges[indexDate], e));

                    if (psaData == null)
                        continue;

                    var rowIndex = indexProfessional * indexDate + indexDate;
                    payrollWorksheet[rowIndex].JoinDate = psaData.JoinDate;
                    payrollWorksheet[rowIndex].DateOfNewPSA = psaData.DateOfNewPSA;
                    payrollWorksheet[rowIndex].PSANumber = psaData.PSANumber;
                }
            }

            return payrollWorksheet;
        }

        private static bool IsInScopePSA(DateTime date, PSAMaster psaMaster)
        {
            var psaDateOfNewPSA = psaMaster.DateOfNewPSA;
            var psaExpiryDate = psaMaster.PSAExpiryDate;

            return psaDateOfNewPSA <= date && date <= psaExpiryDate;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromMonthlyFee(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var monthlyFeeData = _allProfessionalMonthlyFees
                        .FirstOrDefault(e => e.ProfessionalID == indexProfessional &&
                        IsInScopeMonthlyFee(_dateRanges[indexDate], e));

                    if (monthlyFeeData == null)
                        continue;

                    var rowIndex = indexProfessional * indexDate + indexDate;
                    payrollWorksheet[rowIndex].DateOfNewFee = monthlyFeeData.DateOfNewFee;
                    payrollWorksheet[rowIndex].EndDate = monthlyFeeData.EndDate;
                    payrollWorksheet[rowIndex].MonthlyFeeMaster = monthlyFeeData.MonthlyFee;
                }
            }

            return payrollWorksheet;
        }
        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromEventCalendar(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var totalWorkingDays = _calendarService.GetTotalWorkingDays(_dateRanges);
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {
                    var rowIndex = indexProfessional * indexDate + indexDate;
                    payrollWorksheet[rowIndex].TotalWorkingDays = totalWorkingDays;
                }
            }

            return payrollWorksheet;
        }

        private static bool IsInScopeMonthlyFee(DateTime date, MonthlyFeeMaster monthlyFees)
        {
            return monthlyFees.DateOfNewFee <= date && date <= monthlyFees.EndDate;
        }

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
            return ids;
        }
    }
}

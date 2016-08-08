using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Common;
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

        private static IPSAManagementService _psaService;
        private static IDataMasterService _dataMasterService;
        private static IPayrollService _payrollService;

        private static int[] _professionalIDs;
        private static DateTime[] _dateRanges;

        public static void SetSiteUrl(this List<PayrollWorksheetDetailVM> payrollWorksheet, string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);

            _psaService = new PSAManagementService();
            _dataMasterService = new DataMasterService();
            _payrollService = new PayrollService();

            _psaService.SetSiteUrl(_siteUrl);
            _dataMasterService.SetSiteUrl(_siteUrl);
            _psaService.SetSiteUrl(_siteUrl);
        }

        /// <summary>
        /// To retrived all required master data in order to minimise network round trip time
        /// </summary>
        /// <param name="startDatePeriod"></param>
        /// <returns></returns>
        public static async Task PopulateRequiredMasterData(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime startDatePeriod)
        {
            _allProfessionals = _allProfessionals ?? _dataMasterService.GetProfessionals();
            _allValidPSAs = _psaService.GetPSAs(startDatePeriod);
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
                });
            }
            // return the summarized version of the worksheet
            return summarizedPayrollWorksheets;
        }

        public static async Task<List<PayrollWorksheetDetailVM>> PopulateColumns(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            // get StartDate from the first row of the worksheet
            var startDate = _dateRanges[0];
            
            //Retrieve Professional based on given Professional IDs
            var getProfessionalTask = GetProfessionals(_professionalIDs);

            //Retrive PSA based on given Professional IDs
            var getPSATask = GetPSAs(_professionalIDs);

            //Retrieve MonthlyFeeDetail based on given Professional IDs
            var getMonthlyFeeTask = GetMonthlyFees(_professionalIDs);

            //Populate columns
            payrollWorksheet.PopulateColumnsFromProfessional(await getProfessionalTask);
            payrollWorksheet.PopulateColumnsFromPSA(await getPSATask);
            payrollWorksheet.PopulateColumnsFromMonthlyFee(await getMonthlyFeeTask);

            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromPSA(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<PSAMaster> psas)
        {
            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromProfessional(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<ProfessionalMaster> professionals)
        {
            for (int indexProfessional = 0; indexProfessional < _professionalIDs.Length; indexProfessional++)
            {
                for (int indexDate = 0; indexDate < _dateRanges.Length; indexDate++)
                {

                }
            }


            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromMonthlyFee(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<MonthlyFeeDetailVM> monthlyFees)
        {
            return payrollWorksheet;
        }

        private static async Task<IEnumerable<PSAMaster>> GetPSAs(IEnumerable<int> ids)
        {
            var results = await Task.WhenAll(ids.Select(i => GetPSA(i)));
            return results.OrderBy(e => e.ID);
        }

        private static async Task<IEnumerable<ProfessionalMaster>> GetProfessionals(IEnumerable<int> ids)
        {
            var results = await Task.WhenAll(ids.Select(i => GetProfessional(i)));
            return results.OrderBy(e => e.ID);
        }

        private static async Task<IEnumerable<MonthlyFeeDetailVM>> GetMonthlyFees(IEnumerable<int> ids)
        {
            var results = await Task.WhenAll(ids.Select(i => GetMonthlyFee(i)));
            //ID refers to Professional ID
            return results.OrderBy(e => e.ID);
        }

        /// <summary>
        /// Get latest PSA by professional ID
        /// </summary>
        /// <param name="id">Professional ID</param>
        /// <returns></returns>
        private static async Task<PSAMaster> GetPSA(int id)
        {
            var psa = _allValidPSAs.FirstOrDefault(e => e.ProfessionalID == id + string.Empty);
            return psa;
        }

        private static async Task<ProfessionalMaster> GetProfessional(int id)
        {
            var professional = _allProfessionals.FirstOrDefault(m => m.ID == id);
            return professional;
        }

        //TODO: To create method in PayRoll to return professional monthly fee
        private static async Task<MonthlyFeeDetailVM> GetMonthlyFee(int id)
        {
            //var monthlyFee = _payrollService.Ge
            return new MonthlyFeeDetailVM();
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

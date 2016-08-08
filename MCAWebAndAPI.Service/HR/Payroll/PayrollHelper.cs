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
        private static string[] _names = new string[] {
                "Joko Widodo",
                "Joko Santoso",
                "Joko Taroeb",
                "Joko Tingkir"
            };

        private static string[] _positions = new string[] {
                "Walikota",
                "Bupati",
                "Adipati",
                "Presiden"
            };

        private static string[] _units = new string[] {
                "Solo",
                "Magetan",
                "Tegal",
                "Zimbabwe"
            };

        private static double[] _monthlyFees = new double[]
        {
            12000000d,
            32000000d,
            25000000d,
            54000000d
        };


        private static string _siteUrl;
        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PROF_LIST_NAME = "Professional Master";
        const string SP_MON_FEE_DETAIL_LIST_NAME = "Monthly Fee Detail";

        private static IEnumerable<ProfessionalMaster> _allProfessionals;
        private static IEnumerable<PSAMaster> _allValidPSAs;

        private static IPSAManagementService _psaService;
        private static IHRDataMasterService _dataMasterService;
        private static IHRPayrollServices _payrollService;

        public static void SetSiteUrl(this List<PayrollWorksheetDetailVM> payrollWorksheet, string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);

            _psaService = new PSAManagementService();
            _dataMasterService = new HRDataMasterService();
            _payrollService = new HRPayrollServices();
        }

        /// <summary>
        /// To produce correct number of rows for all professionals whose have valid PSA based on given period 
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
        /// Please note that this is only a dummy logic
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <param name="dateRange"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<PayrollWorksheetDetailVM> PopulateColumns_Dummy(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<DateTime> dateRange, IEnumerable<int> ids)
        {
            var index = 0;
            for (int indexName = 0; indexName < _names.Length; indexName++)
            {
                var dateLength = dateRange.Count();
                for (int indexDate = 0; indexDate < dateLength; indexDate++, index++)
                {
                    payrollWorksheet[index].Name = _names[indexName];
                    payrollWorksheet[index].MonthlyFeeMaster = 3 * 100000;
                    payrollWorksheet[index].TotalWorkingDays = 21;
                    payrollWorksheet[index].DaysRequestUnpaid = (dateRange.ToArray()[indexDate].Day * (indexName + 1)) % 21 == 0 ? 1 : 0;
                    payrollWorksheet[index].LastWorkingDate = DateTime.Today.AddDays((10 + indexName) % 30);
                    payrollWorksheet[index].JoinDate = new DateTime(2016, 1, (indexName + 1) % 30);
                    payrollWorksheet[index].Last13thMonthDate = new DateTime(2016, 7, 11);
                }
            }
            return payrollWorksheet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollWorksheet"></param>
        /// <returns></returns>
        public static List<PayrollWorksheetDetailVM> SummarizeData(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var summarizedPayrollWorksheets = new List<PayrollWorksheetDetailVM>();
            var professionalIDs = payrollWorksheet.Select(m => (int)m.ID).Distinct();

            foreach (var professionalID in professionalIDs)
            {
                var payrollRow = payrollWorksheet.FirstOrDefault(e => e.ProfessionalID == professionalID);

                summarizedPayrollWorksheets.Add(new PayrollWorksheetDetailVM
                {
                    ProfessionalID = payrollRow.ProfessionalID,
                    PayrollDate = payrollRow.PayrollDate,
                    
                });
            }

            return payrollWorksheet;
        }

        public static async Task<List<PayrollWorksheetDetailVM>> PopulateColumns(this List<PayrollWorksheetDetailVM> payrollWorksheet)
        {
            var startDate = payrollWorksheet[0].PayrollDate;

            //Get Valid Professional IDs
            var ids = payrollWorksheet.Select(e => e.ProfessionalID);

            //Retrive PSA based on given Professional IDs
            Task<IEnumerable<PSAManagementVM>> getPSATask = GetPSAs(ids);

            //Retrieve Professional based on given Professional IDs
            Task<IEnumerable<ProfessionalMaster>> getProfessionalTask = GetProfessionals(ids);

            //Retrieve MonthlyFeeDetail based on given Professional IDs
            Task<IEnumerable<MonthlyFeeDetailVM>> getMonthlyFeeTask = GetMonthlyFees(ids);

            // Populate columns
            payrollWorksheet.PopulateColumnsFromPSA(await getPSATask);
            payrollWorksheet.PopulateColumnsFromProfessional(await getProfessionalTask);
            payrollWorksheet.PopulateColumnsFromMonthlyFee(await getMonthlyFeeTask);

            await Task.Delay(10000000);

            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromPSA(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<PSAManagementVM> psas)
        {
            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromProfessional(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<ProfessionalMaster> professionals)
        {
            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromMonthlyFee(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<MonthlyFeeDetailVM> monthlyFees)
        {
            return payrollWorksheet;
        }

        private static async Task<IEnumerable<PSAManagementVM>> GetPSAs(IEnumerable<int> ids)
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
        private static async Task<PSAManagementVM> GetPSA(int id)
        {
            var caml = "";
            var psa = new PSAManagementVM();

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, caml))
            {
                psa = new PSAManagementVM
                {
                    ID = id // Please note that 
                };
                break;
            }

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

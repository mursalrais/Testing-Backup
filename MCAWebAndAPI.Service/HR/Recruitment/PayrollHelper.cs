using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public static class PayrollHelper
    {
        private static string[] _names = new string[] {
                "Joko Widodo",
                "Joko Santoso",
                "Joko Taroeb",
                "Joko Tingkir"
            };

        public static List<PayrollWorksheetDetailVM> PopulateRows(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<DateTime> dateRange, IEnumerable<int> professionalIDs)
        {
           
            var index = 0;
            foreach (var profID in _names)
            {
                foreach (var item in dateRange)
                {
                    payrollWorksheet.Add(new PayrollWorksheetDetailVM
                    {
                        PayrollDate = item,
                        ProfessionalID = index
                    });
                }
                index++;
            }
            return payrollWorksheet;
        }

        public static List<PayrollWorksheetDetailVM> PopulateColumns_Dummy(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<DateTime> dateRange, IEnumerable<int> ids)
        {
            var index = 0;
            foreach (var profID in _names)
            {
                foreach (var date in dateRange)
                {
                    
                }
            }
            return payrollWorksheet;
        }

        public static async Task<List<PayrollWorksheetDetailVM>> PopulateColumns(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<int> ids, string siteURL)
        {
            var startDate = payrollWorksheet[0].PayrollDate;

            //TODO: Get All PSA having Last Working Date >= Start Date
            var caml = @"<View><Query><Where><Geq><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>"
              + startDate.ToUniversalTime().ToString("o")
              + @"</Value></Geq></Where></Query></View>";

            Task<IEnumerable<PSAManagementVM>> getPSATask = GetPSAs(ids);
            Task<IEnumerable<ProfessionalDataVM>> getProfessionalTask = GetProfessionals(ids);
            Task<IEnumerable<MonthlyFeeDetailVM>> getMonthlyFeeTask = GetMonthlyFees(ids);

            Task allTask = Task.WhenAll(getPSATask, getProfessionalTask, getMonthlyFeeTask);
            await allTask;

            payrollWorksheet.PopulateColumnsFromPSA(await getPSATask);

            var professionals = await getProfessionalTask;
            var monthlyFees = await getMonthlyFeeTask;


            return payrollWorksheet;
        }

        private static List<PayrollWorksheetDetailVM> PopulateColumnsFromPSA(this List<PayrollWorksheetDetailVM> payrollWorksheet, IEnumerable<PSAManagementVM> psas)
        {

            return payrollWorksheet;
        }

        private static async Task<IEnumerable<PSAManagementVM>> GetPSAs(IEnumerable<int> ids)
        {
            return await Task.WhenAll(ids.Select(i => GetPSA(i)));
        }

        private static async Task<IEnumerable<ProfessionalDataVM>> GetProfessionals(IEnumerable<int> ids)
        {
            var results = await Task.WhenAll(ids.Select(i => GetProfessional(i)));
            return results.OrderBy(e => e.ID);
        }

        private static async Task<IEnumerable<MonthlyFeeDetailVM>> GetMonthlyFees(IEnumerable<int> ids)
        {
            var results = await Task.WhenAll(ids.Select(i => GetMonthlyFee(i)));
            return results.OrderBy(e => e.ID);
        }

        private static async Task<PSAManagementVM> GetPSA(int id)
        {
            return new PSAManagementVM();
        }

        private static async Task<ProfessionalDataVM> GetProfessional(int id)
        {
            return new ProfessionalDataVM();
        }

        private static async Task<MonthlyFeeDetailVM> GetMonthlyFee(int id)
        {
            return new MonthlyFeeDetailVM();
        }

        public static IEnumerable<int> GetValidProfessionalIDs(this List<PayrollWorksheetDetailVM> payrollWorksheet, DateTime startTime, string siteURL)
        {
            var caml = @"<View><Query><Where><Geq><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>"
              + startTime.ToUniversalTime().ToString("o")
              + @"</Value></Geq></Where><OrderBy><FieldRef Name='renewalnumber' Ascending='False' /></OrderBy></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='professional' /></ViewFields> </View>";

            var ids = new List<int>();
            foreach (var item in SPConnector.GetList("PSA", siteURL, caml))
            {
                var id = (int)FormatUtil.ConvertLookupToID(item, "professional");
                if (!ids.Contains(id))
                    ids.Add(id);
            }
            return ids;
        }
    }
}

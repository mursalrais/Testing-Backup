using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstPayrollDay(this DateTime startDate)
        {
            var month = startDate.Month;
            var year = startDate.Year;

            return new DateTime(year, month, 20);
        }

        public static DateTime GetLastPayrollDay(this DateTime startDate)
        {
            var month = startDate.Month;
            var year = startDate.Year;

            if (month == 12)
                return new DateTime(year + 1, 1, 19);
            return new DateTime(year, month + 1, 19);
        }

        public static DateTime GetSameDayInNextMonth(this DateTime startDate)
        {
            var day = startDate.Day;
            var month = startDate.Month;
            var year = startDate.Year;

            if (month == 12)
                return new DateTime(year + 1, 1, day);
            return new DateTime(year, month + 1, day);
        }

        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static bool ContainsSameDay(this List<DateTime> ranges, DateTime dateTarget)
        {
            foreach (var item in ranges)
            {
                if (item.Day == dateTarget.Day && item.Month == dateTarget.Month && item.Year == dateTarget.Year)
                    return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;

namespace MCAWebAndAPI.Model.Common
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstPayrollDay(this DateTime startDate)
        {
            var firstDate = startDate;
            if (startDate.Day <= 20)
                firstDate = startDate.AddMonths(-1);

            var month = firstDate.Month;
            var year = firstDate.Year;

            return new DateTime(year, month, 20);
        }

        public static DateTime GetLastPayrollDay(this DateTime startDate)
        {
            var finishDate = startDate;
            if (startDate.Day >= 20)
                finishDate = startDate.AddMonths(1);

            var month = finishDate.Month;
            var year = finishDate.Year;

            return new DateTime(year, month, 19);
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

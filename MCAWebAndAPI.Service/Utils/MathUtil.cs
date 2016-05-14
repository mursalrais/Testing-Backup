using System;

namespace MCAWebAndAPI.Service.Utils
{
    public class MathUtil
    {
        public static bool CompareDouble(double value1, double value2)
        {
            // 100 % = 1
            // 0.1% = .0001
            var difference = .0001;
            return Math.Abs(value1 - value2) <= difference;
        }
        

        public static DateTime ConvertToDateWithoutTime(DateTime dateTime) {
            var day = dateTime.Day;
            var month = dateTime.Month;
            var year = dateTime.Year;

            return new DateTime(year, month, day);
        }
        
        public static DateTime ConvertToDateWithoutTime(string dateTimeString)
        {
            var splittedString = dateTimeString.Split('/');
            var day = Convert.ToInt32(splittedString[0]);
            var month = Convert.ToInt32(splittedString[1]);
            var year = Convert.ToInt32(splittedString[2]);

            return new DateTime(year, month, day);
        }

        public static double CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            double iWeek, iDays, isDays, ieDays;
            int NoOfDayWeek = 5;
            //* Find the number of weeks between the dates. Subtract 1 */
            // since we do not want to count the current week. * /
            iWeek = DateDiff("ww", startDate, endDate) - 1;
            iDays = iWeek * NoOfDayWeek;
            //

            //-- If Saturday, Sunday is holiday
            if (startDate.DayOfWeek == DayOfWeek.Saturday)
                isDays = 7 - (int)startDate.DayOfWeek;
            else
                isDays = 7 - (int)startDate.DayOfWeek - 1;

            //-- Calculate the days in the last week. These are not included in the
            //-- week calculation. Since we are starting with the end date, we only
            //-- remove the Sunday (datepart=1) from the number of days. If the end
            //-- date is Saturday, correct for this.
            if (endDate.DayOfWeek == DayOfWeek.Saturday)
                ieDays = (int)endDate.DayOfWeek - 2;
            else
                ieDays = (int)endDate.DayOfWeek - 1;

            //-- Sum everything together.
            iDays = iDays + isDays + ieDays;
            return iDays;
        }

        /// <summary>
        /// Calculate weeks between starting date and ending date
        /// </summary>
        /// <param name="stdate"></param>
        /// <param name="eddate"></param>
        /// <returns></returns>

        static int GetWeeks(DateTime stdate, DateTime eddate)
        {

            TimeSpan t = eddate - stdate;
            int iDays;

            if (t.Days < 7)
            {
                if (stdate.DayOfWeek > eddate.DayOfWeek)
                    return 1; //It is accross the week 
                else
                    return 0; // same week
            }
            else
            {
                iDays = t.Days - 7 + (int)stdate.DayOfWeek;
                int i = 0;
                int k = 0;

                for (i = 1; k < iDays; i++)
                {
                    k += 7;
                }

                if (i > 1 && eddate.DayOfWeek != DayOfWeek.Sunday) i -= 1;
                return i;
            }
        }


        /// <summary>
        /// Mimic the Implementation of DateDiff function of VB.Net.
        /// Note : Number of Year/Month is calculated
        ///        as how many times you have crossed the boundry.
        /// e.g. if you say starting date is 29/01/2005
        ///        and 01/02/2005 the year will be 0,month will be 1.
        /// 

        /// </summary>
        /// <param name="datePart">specifies on which part 
        ///   of the date to calculate the difference </param>
        /// <param name="startDate">Datetime object containing
        ///   the beginning date for the calculation</param>
        /// <param name="endDate">Datetime object containing
        ///   the ending date for the calculation</param>
        /// <returns></returns>

        static double DateDiff(string datePart,
                      DateTime startDate, DateTime endDate)
        {

            //Get the difference in terms of TimeSpan
            TimeSpan T;

            T = endDate - startDate;

            //Get the difference in terms of Month and Year.
            int sMonth, eMonth, sYear, eYear;
            sMonth = startDate.Month;
            eMonth = endDate.Month;
            sYear = startDate.Year;
            eYear = endDate.Year;
            double Months, Years = 0;
            Months = eMonth - sMonth;
            Years = eYear - sYear;
            Months = Months + (Years * 12);

            switch (datePart.ToUpper())
            {
                case "WW":
                case "DW":
                    return (double)GetWeeks(startDate, endDate);
                case "MM":
                    return Months;
                case "YY":
                case "YYYY":
                    return Years;
                case "QQ":
                case "QQQQ":
                    //Difference in Terms of Quater
                    return Math.Ceiling((double)T.Days / 90.0);
                case "MI":
                case "N":
                    return T.TotalMinutes;
                case "HH":
                    return T.TotalHours;
                case "SS":
                    return T.TotalSeconds;
                case "MS":
                    return T.TotalMilliseconds;
                case "DD":
                default:
                    return T.Days;
            }
        }

    }
}

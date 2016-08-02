using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Service.HR.Timesheet
{
    public class TimesheetService : ITimesheetService
    {
        string _siteUrl;
        const string TYPE_PUB_HOLIDAY = "Public Holiday";
        const string TYPE_HOLIDAY = "Holiday";
        const string TYPE_DAYOFF = "Day-Off";
        const string TYPE_COMP_LEAVE = "Compensatory Leave";

        public TimesheetVM GetTimesheet(string userlogin, DateTime period)
        {
            return new TimesheetVM
            {
                UserLogin = userlogin,
                Name = GetFullName(userlogin),
                Period = period,
                ProjectUnit = GetProjectUnitName(userlogin),
                TimesheetDetails = GetTimesheetDetails(userlogin, period)
            };
        }

        private string GetProjectUnitName(string userlogin)
        {
            //TODO: To get from SP
            return "Informaton Technology";
        }

        private string GetFullName(string userlogin)
        {
            //TODO: To get from SP
            return "Joko Prasetyo";
        }

        public IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period)
        {
            var startDate = period.GetFirstPayrollDay();
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            var timesheetDetails = new List<TimesheetDetailVM>();

            foreach (var item in dateRange)
            {
                if (IsPublicHoliday_Dummy(item))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Status = TYPE_PUB_HOLIDAY,
                        Type = TYPE_PUB_HOLIDAY
                    });
                }
                else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Status = TYPE_HOLIDAY,
                        Type = TYPE_HOLIDAY
                    });
                else if (IsDayOff_Dummy(item))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Status = TYPE_DAYOFF,
                        Type = TYPE_DAYOFF
                    });
                }else if (IsCompLeave_Dummy(item))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Status = TYPE_COMP_LEAVE,
                        Type = TYPE_COMP_LEAVE
                    });
                }
            }

            return timesheetDetails;
        }

        private bool IsCompLeave_Dummy(DateTime item)
        {
            // random way to populate
            return (item.Day % 17 == 0);
        }

        private bool IsDayOff_Dummy(DateTime item)
        {
            // random way to populate
            return (item.Day % 15 == 0);
        }

        private bool IsPublicHoliday_Dummy(DateTime date)
        {
            // random way to populate
            return (date.Day % 13 == 0);
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public IEnumerable<TimesheetDetailVM> AppendWorkingDays(IEnumerable<TimesheetDetailVM> currentDays, DateTime from, DateTime to, bool isFullDay, string location = null)
        {
            var dateRange = from.EachDay(to);
            var existingDays = currentDays.Select(e => (DateTime)e.Date).ToList();
            var allDays = currentDays.ToList();
            
            foreach (var workingDay in dateRange)
            {
                if(!existingDays.ContainsSameDay(workingDay))
                {
                    allDays.Add(new TimesheetDetailVM
                    {
                        Date = workingDay,
                        FullHalf = isFullDay ? 1.0d : 0.5d,
                        Location = location ?? string.Empty,
                    });
                }
            }

            return allDays;
            
        }
    }
}

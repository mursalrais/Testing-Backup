using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.HR.Common;
using Microsoft.SharePoint.Client;
using NLog;
using View = System.Web.UI.WebControls.View;

namespace MCAWebAndAPI.Service.HR.Timesheet
{
    public class TimesheetService : ITimesheetService
    {
        string _siteUrl;
        const string TYPE_PUB_HOLIDAY = "Public Holiday";
        const string TYPE_HOLIDAY = "Holiday";
        const string TYPE_DAYOFF = "Day-Off";
        const string TYPE_COMP_LEAVE = "Compensatory Leave";
        const string LIST_PUB_HOLIDAY = "Event Calendar";
        const string LIST_DAY_OFF = "Day-Off Request";
        const string LIST_DAY_OFF_DETAIL = "Day-Off Request Detail";
        const string LIST_COMPEN = "Compensatory Request";
        const string LIST_COMPEN_DETAIL = "Compensatory Request Detail";

        const string LIST_TIME = "Timesheet";
        const string LIST_TIME_DETAIL = "Timesheet Detail";
        //Compensatory Request
        private IDataMasterService _dataService;
        private IProfessionalService _professionalService;
        static Logger logger = LogManager.GetCurrentClassLogger();
        public TimesheetService()
        {
            _dataService = new DataMasterService();
            _professionalService = new ProfessionalService();
        }

        public TimesheetVM GetTimesheet(string userlogin, DateTime period)
        {

            var viewModel = new TimesheetVM
            {
                ProfessionalID= GetProfessionalID(userlogin),
                UserLogin = userlogin,
                Name = GetFullName(userlogin),
                Period = period,
                ProjectUnit = GetProjectUnitName(userlogin),
                TimesheetDetails = GetTimesheetDetails(userlogin, period)
            };
            viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
            viewModel.ProfessionalName.Text = viewModel.Name;
            viewModel.UserPermission  = viewModel.ProjectUnit == "Human Resources Unit" ? "HR" : "Professional";

            return viewModel;
        }

        private string GetProjectUnitName(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
            return professionalData.Project_Unit;
        }

        private int? GetProfessionalID(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
            return professionalData.ID;
        }

        private string GetFullName(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
            return professionalData.Name;
        }

        private static List<DateTime> getPublicHoliday(string strUrl)
        {
            var listItem = SPConnector.GetList(LIST_PUB_HOLIDAY, strUrl);
            List<DateTime> lstpublicHoliday = new List<DateTime>();
            foreach (var item in listItem)
            {
                lstpublicHoliday.Add(Convert.ToDateTime(item["EventDate"]).ToLocalTime());
            }

            return lstpublicHoliday;
 
        }

        private static List<DateTime> getCompensatory(string strUrl, string strName)
        {
            List<DateTime> lstCompensatory = new List<DateTime>();
            var caml = @"<View><Query><Where><And><Eq><FieldRef Name='professional' />
                        <Value Type='Lookup'>" + strName +"</Value></Eq>" +
                       "<Eq><FieldRef Name='crstatus' /><Value Type='Text'>Approved</Value></Eq>" +
                       "</And></Where></Query></View>";
            var listMaster = SPConnector.GetList(LIST_COMPEN, strUrl, caml);
           
            foreach (var item in listMaster)
            {
                caml = @"<View><Query><Where><Eq><FieldRef Name='compensatoryrequest' />
                        <Value Type='Lookup'>" + Convert.ToString(item["ID"]) +
                        "</Value></Eq></Where></Query></View>";
                var listDetail = SPConnector.GetList(LIST_COMPEN_DETAIL, strUrl, caml);

                foreach (var itemDetail in listDetail)
                {
                    var startdate = Convert.ToDateTime(itemDetail["compensatorystarttime"]).ToLocalTime();
                    var finishdate = Convert.ToDateTime(itemDetail["compensatoryendtime"]).ToLocalTime();
                    var dateRange = startdate.EachDay(finishdate);
                    lstCompensatory.AddRange(dateRange);
                }

            }
            return lstCompensatory;
        }

        private static List<DateTime> getUserDayOFF(string strUrl,string strName)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + strName +
               "</Value></Eq></Where></Query></View>";
            var listMaster = SPConnector.GetList(LIST_DAY_OFF, strUrl, caml);
            List<DateTime> lstDayOff = new List<DateTime>();
            foreach (var item in listMaster)
            {
                caml = @"<View><Query><Where><And><Eq><FieldRef Name='dayoffrequest' />
                        <Value Type='Lookup'>" + Convert.ToString(item["ID"] ) +
                        "</Value></Eq>" +
                        "<Eq><FieldRef Name='approvalstatus' />" +
                        "<Value Type='Text'>Approved</Value></Eq></And></Where></Query></View>";
                var listDetail = SPConnector.GetList(LIST_DAY_OFF_DETAIL, strUrl, caml);

                foreach (var itemDetail in listDetail)
                {
                    var startdate = Convert.ToDateTime(itemDetail["requeststartdate"]).ToLocalTime();
                    var finishdate = Convert.ToDateTime(itemDetail["requestenddate"]).ToLocalTime();
                    var dateRange = startdate.EachDay(finishdate);
                    lstDayOff.AddRange(dateRange);
                }
             
            }
            return lstDayOff;
        }


        public IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period)
        {
            var startDate = period;
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            var listHoliday = getPublicHoliday( _siteUrl);
            var listDayOff = getUserDayOFF(_siteUrl,GetFullName(userlogin));
            var listCompen = getCompensatory(_siteUrl, GetFullName(userlogin));

            var timesheetDetails = new List<TimesheetDetailVM>();

            foreach (var item in dateRange)
            {
                if (IsDate(item, listHoliday))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_PUB_HOLIDAY,
                        Type = TYPE_PUB_HOLIDAY
                    });
                }
                else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_HOLIDAY,
                        Type = TYPE_HOLIDAY
                    });
                else if (IsDate(item, listDayOff))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_DAYOFF,
                        Type = TYPE_DAYOFF
                    });
                }else if (IsDate(item, listCompen))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_COMP_LEAVE,
                        Type = TYPE_COMP_LEAVE
                    });
                }
            }

            return timesheetDetails;
        }

        //private bool IsCompLeave(DateTime item)
        //{
        //    //TODO: To get from SP list
        //    return (item.Day % 17 == 0);
        //}

        //private bool IsDayOff(DateTime date, List<DateTime> lstRange)
        //{
        //    var bcek = false;
        //    if (lstRange == null) return false;
        //    else
        //    {
        //        if (lstRange.Any(item => date.ToString("yy-MM-dd") == item.ToString("yy-MM-dd")))
        //        {
        //            return true;
        //        }
        //    }

        //    return bcek;
        //}

        private bool IsDate(DateTime date,List<DateTime> lstRange )
        {
            var bcek = false;
            if (lstRange == null) return false;
            else
            {
                if (lstRange.Any(item => date.ToString("yy-MM-dd") == item.ToString("yy-MM-dd")))
                {
                    return true;
                }
            }

            return bcek;
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
            _dataService.SetSiteUrl(_siteUrl);
            _professionalService.SetSiteUrl(_siteUrl);
        }

        public IEnumerable<TimesheetDetailVM> AppendWorkingDays(IEnumerable<TimesheetDetailVM> currentDays, DateTime from,
            DateTime to, bool isFullDay, string location = null, int? locationid = null)
        {
            var dateRange = from.EachDay(to);
            var existingDays = currentDays.Select(e => (DateTime)e.Date).ToList();
            var allDays = currentDays.ToList();

            foreach (var workingDay in dateRange)
            {
                if (!existingDays.ContainsSameDay(workingDay))
                {
                    if (workingDay.DayOfWeek != DayOfWeek.Saturday && workingDay.DayOfWeek != DayOfWeek.Sunday)
                    {
                        allDays.Add(new TimesheetDetailVM
                        {
                            Date = workingDay,
                            FullHalf = isFullDay ? 1.0d : 0.5d,
                            Location = location ?? string.Empty,
                            LocationID = locationid ?? null
                        });
                    }

                }
            }

            return allDays;
        }

        public void CreateTimesheetDetails(int? headerId, 
            IEnumerable<TimesheetDetailVM> timesheetDetails)
        {
            foreach (var viewModel in timesheetDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(LIST_TIME_DETAIL, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>
                {
                    {"timesheet", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                   {"timesheetdetaildate", viewModel.Date},
                    {"FullHalf", viewModel.FullHalf}
                };

                if (!string.IsNullOrEmpty(viewModel.Location))
                {
                    updatedValue.Add("location", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.LocationID) });
                    updatedValue.Add("Title", "Working Days");
                }
                else
                {
                    updatedValue.Add("Title", viewModel.Type);
                }

                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(LIST_TIME_DETAIL, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(LIST_TIME_DETAIL, updatedValue, _siteUrl);



                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    //throw new Exception(ErrorResource.SPInsertError);
                    throw new Exception(e.Message);
                }
            }
        }


        public int CreateHeader(TimesheetVM header)
        {
            int ID = 0;
            var columnValues = new Dictionary<string, object>
           {
               {"timesheetstatus", header.TimesheetStatus}
           };

            var strPeriod = Convert.ToDateTime(header.Period).ToString("MM") + "-" + Convert.ToDateTime(header.Period).ToString("yyyy");

            columnValues.Add("Title", strPeriod);

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
                header.ProfessionalID = Convert.ToInt32(header.ProfessionalName.Value);
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }

            columnValues.Add("visibleto", SPConnector.GetUser(header.UserLogin, _siteUrl, "hr"));

            try
            {
                SPConnector.AddListItem(LIST_TIME, columnValues, _siteUrl);
                ID = SPConnector.GetLatestListItemID(LIST_TIME, _siteUrl);
              

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return ID;
        }
    }
}

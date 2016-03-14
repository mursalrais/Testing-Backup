using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class CalendarController : Controller
    {
        ICalendarService calendarService;

        public CalendarController()
        {
            calendarService = new CalendarService();
        }


        public ActionResult GetEvents(string siteUrl = null) {

            calendarService.SetSiteUrl(siteUrl);
            var data = calendarService.GetEvents();

            return this.Jsonp(data);
        }


    }
}
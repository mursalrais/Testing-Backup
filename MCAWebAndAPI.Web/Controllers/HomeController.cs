using MCAWebAndAPI.Service.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ScheduleTracking()
        {
            return View();
        }

        public ActionResult ProjectHealthByActivity()
        {
            return View();
        }

        [HttpGet]
        public FileResult Print()
        {
            var result = PDFConverter.Instance.Convert("Kompas", "http://www.kompas.com/");
            return File(result, "application/pdf");
        }
    }
}
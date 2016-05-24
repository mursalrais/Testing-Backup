using MCAWebAndAPI.Service.Converter;
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
            var result = PDFConverter.Instance.ConvertFromURL("Kompas", "http://www.kompas.com/");
            return File(result, "application/pdf");
        }
    }
}
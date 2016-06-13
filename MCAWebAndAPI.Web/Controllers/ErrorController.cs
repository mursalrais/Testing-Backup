using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string errorMessage = null)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
    }
}
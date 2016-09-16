using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string errorMessage = null, string errorMessageDetail = null)
        {
            ViewBag.ErrorMessage = errorMessage;
            ViewBag.ErrorMessageDetail = errorMessageDetail;

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class SuccessController : Controller
    {
        public ActionResult Index(string successMessage = null, string previousUrl = null)
        {
            ViewBag.SuccessMessage = successMessage;
            ViewBag.PreviousUrl = previousUrl;
            return View();
        }

        public ActionResult ErrorMessage(string eMessage = null, string previousUrl = null)
        {
            ViewBag.SuccessMessage = eMessage;
            ViewBag.PreviousUrl = previousUrl;
            return View();
        }

        
    }
}
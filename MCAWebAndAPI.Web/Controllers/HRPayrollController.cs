using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Payroll;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRPayrollController : Controller
    {
        IHRPayrollServices _hRPayrollService;

        public HRPayrollController()
        {
            _hRPayrollService = new HRPayrollServices();
        }

        public ActionResult CreateMonthlyFee(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRPayrollService.GetPopulatedModel();

            // Modify ViewModel based on spesific case, e.g., Asset Transfer is one of conditions in Asset transaction

            // Return to the name of the view and parse the model
            return View("CreateMonthlyFee", viewModel);
        }

        [HttpPost]
        public JsonResult SubmitMonthlyFee(MonthlyFeeVM viewModel)
        {
            _hRPayrollService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);

            // Get Header ID after inster to SharePoint
            var headerID = _hRPayrollService.CreateHeader(viewModel.Header);

            // Check whether error is found
            if (!ModelState.IsValid)
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "Error" }
                };
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            return Json(new
            {
                result = "Success",
                urlToRedirect =
string.Format("{0}/{1}", siteUrl, UrlResource.MonthlyFee)

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditMonthlyFee(int ID, string siteUrl = null)
        {
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRPayrollService.GetHeader(ID);

            return View(viewModel);
        }

        [HttpPost]

        public JsonResult UpdateMonthlyFee(MonthlyFeeVM _data, string site)
        {
            _hRPayrollService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            _hRPayrollService.UpdateHeader(_data);

            if (!ModelState.IsValid)
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "Error" }
                };
            }

            //add to database

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            return Json(new
            {
                result = "Success",
                urlToRedirect =
string.Format("{0}/{1}", siteUrl, UrlResource.MonthlyFee)

            }, JsonRequestBehavior.AllowGet);
        }


        // GET: HRMonthly
        public ActionResult Index()
        {
            return View();
        }
    }
}
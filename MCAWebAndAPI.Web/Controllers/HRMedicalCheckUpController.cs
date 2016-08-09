using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.InsuranceClaim;
using MCAWebAndAPI.Service.HR.MedicalCheckUp;
using MCAWebAndAPI.Service.Resources;
//using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class HRMedicalCheckUpController : Controller
    {

        readonly IMedicalCheckUpService _service;

        public HRMedicalCheckUpController()
        {
            _service = new MedicalCheckUpService();
        }


        public ActionResult CreateMedical(string siteUrl = null, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModel(useremail);

            return View("CreateMedical", viewModel);

        }

        public ActionResult EditMedical(string siteUrl, int? id, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetMedical(id,useremail);
            return View(viewModel);
        }

        public ActionResult DisplayMedical(string siteUrl, int? id, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetMedical(id, useremail);
            viewModel.URL = siteUrl;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SubmitMedical(FormCollection form, MedicalCheckUpVM viewModel)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

          
            try
            {
             _service.CreateMedical(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }


           
            var strPages = viewModel.UserPermission == "HR" ? "/sitePages/HRMedicalView.aspx" : "/sitePages/ProfessionalMedicalView.aspx";

            return RedirectToAction("Redirect", "HRMedicalCheckUp", new { siteUrl = siteUrl + strPages });

        }

        public ActionResult UpdateMedical(FormCollection form, MedicalCheckUpVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            _service.UpdateMedical(viewModel);

            var strPages = viewModel.UserPermission == "HR" ? "/sitePages/HRMedicalView.aspx" : "/sitePages/ProfessionalMedicalView.aspx";

            return RedirectToAction("Redirect", "HRMedicalCheckUp", new { siteUrl = siteUrl + strPages });
        }


        public ActionResult Redirect(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = new MedicalCheckUpVM {URL = siteUrl ?? ConfigResource.DefaultHRSiteUrl};
            return View("Redirect", viewModel);

        }


        private IEnumerable<MedicalCheckUpVM> GetFromExistingSessionMedical(string strYear, string strEmail)
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["MedicalMaster"] as IEnumerable<MedicalCheckUpVM>;
            var dependents = sessionVariable ?? _service.GetMedicalByYear(strYear, strEmail);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["MedicalMaster"] = dependents;
            return dependents;
        }


        public JsonResult GetMedicalYear(string year)
        {
            _service.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var medical = GetFromExistingSessionMedical(year.ToString(), "44");
            return Json(medical.Select(e =>
              new
              {
                  e.ID

              }),
              JsonRequestBehavior.AllowGet);
        }

    }
}
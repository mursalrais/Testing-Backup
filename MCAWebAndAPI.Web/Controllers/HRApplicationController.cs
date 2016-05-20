using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRApplicationController : Controller
    {

        IHRApplicationService _service;

        public HRApplicationController()
        {
            _service = new HRApplicationService();
        }


        [HttpPost]
        public ActionResult CreateApplicationData(ApplicationDataVM viewModel)
        {

            var headerID = _service.CreateApplicationData(viewModel);

            _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            _service.CreateWorkingExperienceDetails(headerID, viewModel.WorkingExperienceDetails);
            _service.CreateProfessionalDocuments(headerID, viewModel.Documents);

            return Json(new { status = true, urlToRedirect = "/hr" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateApplicationData(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _service.GetBlankApplicationDataForm();
            return View(viewModel);
        }
    }
}
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
        public ActionResult CreateApplicationData(FormCollection form, ApplicationDataVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var headerID = _service.CreateApplicationData(viewModel);

            viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            _service.CreateEducationDetails(headerID, viewModel.EducationDetails);

            viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);

            viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
            _service.CreateWorkingExperienceDetails(headerID, viewModel.WorkingExperienceDetails);

            _service.CreateProfessionalDocuments(headerID, viewModel.Documents);

            return Json(new { status = true, urlToRedirect = siteUrl + ConfigResource.UrlApplication }, 
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<WorkingExperienceDetailVM> BindWorkingExperienceDetails(FormCollection form, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails)
        {
            var array = workingExperienceDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].From = BinderHelper.BindDateInGrid("WorkingExperienceDetails",
                    i, "From", form);
                array[i].To = BinderHelper.BindDateInGrid("WorkingExperienceDetails",
                    i, "To", form);
            }

            return array;
        }

        private IEnumerable<TrainingDetailVM> BindTrainingDetails(FormCollection form, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            var array = trainingDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Year = BinderHelper.BindDateInGrid("TrainingDetails",
                    i, "Year", form);
            }

            return array;
        }

        private IEnumerable<EducationDetailVM> BindEducationDetails(FormCollection form, 
            IEnumerable<EducationDetailVM> educationDetails)
        {
            var array = educationDetails.ToArray();
            for (int i = 0; i< array.Length; i++)
            {
                array[i].YearOfGraduation = BinderHelper.BindDateInGrid("EducationDetails",
                    i, "YearOfGraduation", form);
            }

            return array;
        }

        public ActionResult CreateApplicationData(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetBlankApplicationDataForm();
            return View(viewModel);
        }
    }
}
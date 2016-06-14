using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [HandleError]
    public class HRProfessionalController : Controller
    {
        IHRDataMasterService _service;

        public HRProfessionalController()
        {
            _service = new HRDataMasterService();
        }

        public ActionResult EditProfessional(string siteUrl = null, int? ID = null)
        {
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetProfessionalData(ID);
            ViewBag.IsHRView = true;
            return View(viewModel);
        }

        public ActionResult EditCurrentProfessional(string siteUrl = null, string username = null)
        {
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetProfessionalData(username);

            if (viewModel == null)
                return RedirectToAction("Index", "Error", new { errorMessage = 
                    string.Format(MessageResource.ErrorProfessionalNotFound, username)});

            ViewBag.IsHRView = false;
            return View("EditProfessional", viewModel);
        }

        [HttpPost]
        public ActionResult EditProfessional(FormCollection form, ProfessionalDataVM viewModel)
        {
            if(!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.EditProfessionalData(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.OrganizationalDetails = BindOrganizationalDetails(form, viewModel.OrganizationalDetails);
                _service.CreateOrganizationalDetails(headerID, viewModel.OrganizationalDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
                _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
                _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _service.CreateDependentDetails(headerID, viewModel.DependentDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                switch (viewModel.ValidationAction)
                {
                    case "ask-hr-to-validate-action":
                        _service.SendEmailValidation(
                            "randi.prayengki@eceos.com",
                            string.Format(EmailResource.ProfessionalEmailValidation,
                            string.Format(UrlResource.ProfessionalDisplayByID, siteUrl, headerID)));
                        break;
                    case "approve-action":
                        _service.SendEmailValidation(
                            "mariani.yosefi@eceos.com",
                            string.Format(EmailResource.ProfessionalEmailValidationResponse), isApproved: true);
                        break;
                    case "reject-action":
                        _service.SendEmailValidation(
                            "mariani.yosefi@eceos.com",
                            string.Format(EmailResource.ProfessionalEmailValidationResponse), isApproved: false);
                        break;
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
          
            return JsonHelper.GenerateJsonSuccessResponse(UrlResource.Professional);
        }

        IEnumerable<OrganizationalDetailVM> BindOrganizationalDetails(FormCollection form, IEnumerable<OrganizationalDetailVM> organizationalDetails)
        {
            var array = organizationalDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].LastWorkingDay = BindHelper.BindDateInGrid("OrganizationalDetails",
                    i, "LastWorkingDay", form);
            }
            return array;
        }

        IEnumerable<TrainingDetailVM> BindTrainingDetails(FormCollection form, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            var array = trainingDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Year = BindHelper.BindDateInGrid("TrainingDetails",
                    i, "Year", form);
            }
            return array;
        }

        private IEnumerable<EducationDetailVM> BindEducationDetails(FormCollection form,
            IEnumerable<EducationDetailVM> educationDetails)
        {
            var array = educationDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].YearOfGraduation = BindHelper.BindDateInGrid("EducationDetails",
                    i, "YearOfGraduation", form);
            }
            return array;
        }
    }
}
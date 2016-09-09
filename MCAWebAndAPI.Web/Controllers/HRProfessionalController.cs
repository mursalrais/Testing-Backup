using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [HandleError]
    public class HRProfessionalController : Controller
    {
        IDataMasterService _dataMasterService;
        IProfessionalService _professionalService;

        public HRProfessionalController()
        {
            _dataMasterService = new DataMasterService();
            _professionalService = new ProfessionalService();
        }

        private void SetSiteUrl(string siteUrl)
        {
            _dataMasterService.SetSiteUrl(siteUrl);
            _professionalService.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);
        }

        public async Task<ActionResult> EditProfessional(string siteUrl = null, int? ID = null)
        {
            SetSiteUrl(siteUrl);
            var viewModel = await _professionalService.GetProfessionalDataAsync(ID);
            ViewBag.IsHRView = true;
            return View(viewModel);
        }

        public ActionResult EditCurrentProfessional(string siteUrl = null, string username = null)
        {
            // MANDATORY: Set Site URL
            SetSiteUrl(siteUrl);

            var viewModel = _professionalService.GetProfessionalData(username);
            if (viewModel == null)
                return RedirectToAction("Index", "Error", new { errorMessage = 
                    string.Format(MessageResource.ErrorProfessionalNotFound, username)});

            ViewBag.IsHRView = false;
            return View("EditProfessional", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditProfessional(FormCollection form, ProfessionalDataVM viewModel)
        {            
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            SetSiteUrl(siteUrl);

            int? headerID = null;
            try
            {
                headerID = _professionalService.EditProfessionalData(viewModel);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }

            viewModel.OrganizationalDetails = BindOrganizationalDetails(form, viewModel.OrganizationalDetails);
            Task createOrganizationalDetailsTask = _professionalService.CreateOrganizationalDetailsAsync(headerID, viewModel.OrganizationalDetails);
            viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            Task createEducationDetailsTask = _professionalService.CreateEducationDetailsAsync(headerID, viewModel.EducationDetails);
            viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            Task createTrainingDetailsTask = _professionalService.CreateTrainingDetailsAsync(headerID, viewModel.TrainingDetails);
            viewModel.DependentDetails = BindDependentDetails(form,viewModel.DependentDetails);
            Task createDependentDetailsTask = _professionalService.CreateDependentDetailsAsync(headerID, viewModel.DependentDetails);

            Task allTask = Task.WhenAll(createOrganizationalDetailsTask, createEducationDetailsTask, createTrainingDetailsTask, createDependentDetailsTask);
            await allTask;
            try
            {
                // TODO: To change email address based on position. The email must be the HR personnel's email.
                switch (viewModel.ValidationAction)
                {
                    case "ask-hr-to-validate-action":
                        List<string> EmailsHR = _professionalService.GetEmailHR();
                        _professionalService.SetValidationStatus(headerID, Workflow.ProfessionalValidationStatus.NEED_VALIDATION);
                        foreach (var item in EmailsHR)
                        {
                            if (!(string.IsNullOrEmpty(item)))
                            {
                                _professionalService.SendEmailValidation(item,
                            string.Format(EmailResource.ProfessionalEmailValidation,
                            string.Format(UrlResource.ProfessionalDisplayByID, siteUrl, headerID)));
                            }
                            
                        }                        
                        break;

                    // Suppose mariani.yosefi is the applicant
                    // TODO: Please update the email to be retrived from SP List
                    case "approve-action":
                        _professionalService.SetValidationStatus(headerID, Workflow.ProfessionalValidationStatus.VALIDATED);
                        _professionalService.SendEmailValidation(
                            viewModel.OfficeEmail,
                            string.Format(EmailResource.ProfessionalEmailValidationResponse));
                        break;
                    case "reject-action":
                        _professionalService.SetValidationStatus(headerID, Workflow.ProfessionalValidationStatus.REJECTED);
                        _professionalService.SendEmailValidation(
                            viewModel.OfficeEmail,
                            string.Format(EmailResource.ProfessionalEmailValidationResponse));
                        break;
                }
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }
            if (viewModel.ValidationStatus == "Need HR to Validate")
            {
                return JsonHelper.GenerateJsonSuccessResponse(siteUrl + "/" + UrlResource.ProfessionalMaster);
            }
            else
            {
                return JsonHelper.GenerateJsonSuccessResponse(siteUrl + "/" + UrlResource.ProfessionalData);
            }
            
        }

        private IEnumerable<DependentDetailVM> BindDependentDetails(FormCollection form, IEnumerable<DependentDetailVM> dependentDetails)
        {
            var array = dependentDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].DateOfBirthGrid = BindHelper.BindDateInGridProfessional("DependentDetails",
                    i, "DateOfBirthGrid", form);
            }
            return array;
        }

        IEnumerable<OrganizationalDetailVM> BindOrganizationalDetails(FormCollection form, IEnumerable<OrganizationalDetailVM> organizationalDetails)
        {
            var array = organizationalDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].LastWorkingDay = BindHelper.BindDateInGridProfessional("OrganizationalDetails",
                    i, "LastWorkingDay", form);
                array[i].StartDate = BindHelper.BindDateInGridProfessional("OrganizationalDetails",
                    i, "StartDate", form);
            }
            return array;
        }

        IEnumerable<TrainingDetailVM> BindTrainingDetails(FormCollection form, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            var array = trainingDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Year = BindHelper.BindDateInGridProfessional("TrainingDetails",
                    i, "Year", form);
            }
            return array;
        }

        IEnumerable<EducationDetailVM> BindEducationDetails(FormCollection form,
            IEnumerable<EducationDetailVM> educationDetails)
        {
            var array = educationDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].YearOfGraduation = BindHelper.BindDateInGridProfessional("EducationDetails",
                    i, "YearOfGraduation", form);
            }
            return array;
        }
    }
}
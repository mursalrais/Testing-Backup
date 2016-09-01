using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRApplicationController : Controller
    {
        readonly IApplicationService _service;
        public HRApplicationController()
        {
            _service = new ApplicationService();
        }
        public ActionResult GetIDCardType(string nationality)
        {
            string[] result = { };
            Dictionary<int, string> choice = _service.GetIDCardType();

            if (string.Compare(nationality, "Indonesia", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Json(choice.Where(e => e.Value == "e-KTP" || e.Value == "KTP").Select(
                    f => new {
                        Value = f.Key,
                        Text = f.Value
                    }
                ), JsonRequestBehavior.AllowGet);
            }
            return Json(choice.Where(e => e.Value == "KITAS" || e.Value == "Passport").Select(
                   f => new {
                       Value = f.Key,
                       Text = f.Value
                   }
               ), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProfessionalData(FormCollection form, ApplicationDataVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateProfessionalData(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            Task createEducationDetailsTask = _service.CreateEducationDetailsAsync(headerID, viewModel.EducationDetails);

            viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            Task createTrainingDetailsTask = _service.CreateTrainingDetailsAsync(headerID, viewModel.TrainingDetails);
            Task allTasks = Task.WhenAll(createEducationDetailsTask, createEducationDetailsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }

        public ActionResult ListVacantPositions(string siteUrl)
        {
            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.siteUrl = siteUrl ?? ConfigResource.DefaultHRSiteUrl;

            var viewModel = _service.GetVacantPositions();
            return View(viewModel);
        }

        public async Task<ActionResult> DisplayApplicationData(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = await _service.GetApplicationAsync(ID);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateApplicationData(FormCollection form, ApplicationDataVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultHRSiteUrl;
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateApplication(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            Task createEducationDetailsTask = _service.CreateEducationDetailsAsync(headerID, viewModel.EducationDetails);
            viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            Task createTrainingDetailsTask = _service.CreateTrainingDetailsAsync(headerID, viewModel.TrainingDetails);
            viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
            Task createWorkingExperienceDetailsTask = _service.CreateWorkingExperienceDetailsAsync(headerID, viewModel.WorkingExperienceDetails);
            Task createApplicationDocumentTask = _service.CreateApplicationDocumentAsync(headerID, viewModel.Documents);
            //Task sendTask = EmailUtil.SendAsync(viewModel.EmailAddresOne, "Application Submission Confirmation",
            //     EmailResource.ApplicationSubmissionNotification);
            Task allTasks = Task.WhenAll(createEducationDetailsTask, createTrainingDetailsTask,
                createWorkingExperienceDetailsTask, createApplicationDocumentTask);

            _service.SendMail(viewModel.EmailAddresOne, string.Format("{0} at MCA-Indonesia", viewModel.Position), string.Format(EmailResource.ApplicationData, viewModel.FirstMiddleName, viewModel.Position));
            
            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index",
                "Success",
                new
                {
                    successMessage =
                string.Format(MessageResource.SuccessCreateApplicationData, viewModel.FirstMiddleName)
                });
        }

        [HttpPost]
        public ActionResult SetStatusApplicationData(ApplicationDataVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultHRSiteUrl;
            _service.SetSiteUrl(siteUrl);

            try
            {
                _service.SetApplicationStatus(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            var status = viewModel.WorkflowStatusOptions.Value;
            var applicationOwner = string.Format("{0} {1}", viewModel.FirstMiddleName, viewModel.LastName);

            var message = string.Format(MessageResource.SuccessUpdateApplicationStatus, applicationOwner, status);
            var prevUrl = string.Format("{0}/{1}", siteUrl, UrlResource.ApplicationData);
            return RedirectToAction("Index", "Success", new { successMessage = message, previousUrl = prevUrl });
        }

        [HttpPost]
        public ActionResult PrintApplicationData(FormCollection form, ApplicationDataVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultHRSiteUrl;
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            string nationalityName = _service.GetNationality(Convert.ToInt32(viewModel.Nationality.Value));
            viewModel.NationalityName = nationalityName;

            viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
            ViewData.Model = AdjustViewModel(viewModel);

            const string RelativePath = "~/Views/HRApplication/PrintApplicationData.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.FirstMiddleName + "-" + viewModel.Position + "-" + "MCA-Indonesia.pdf";
            byte[] pdfBuf = null;
            string content;

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();

                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    RedirectToAction("Index", "Error");
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
        }

        private object AdjustViewModel(ApplicationDataVM viewModel)
        {
            var iDCardID = viewModel.IDCardType.Value;
            viewModel.IDCardType.Text = _service.GetIDCardType()[(int)iDCardID];

            return viewModel;
        }

        private IEnumerable<WorkingExperienceDetailVM> BindWorkingExperienceDetails(FormCollection form, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails)
        {
            var array = workingExperienceDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].From = BindHelper.BindDateInGrid("WorkingExperienceDetails",
                    i, "From", form);
                array[i].To = BindHelper.BindDateInGrid("WorkingExperienceDetails",
                    i, "To", form);
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

        IEnumerable<EducationDetailVM> BindEducationDetails(FormCollection form,
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

        public ActionResult CreateApplicationData(string siteUrl = null, int? ID = null, string position = null)
        {
            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);
            
            var viewModel = _service.GetApplication(null);
            viewModel.Position = position;
            viewModel.ManpowerRequisitionID = ID;

            return View(viewModel);
        }
    }
}
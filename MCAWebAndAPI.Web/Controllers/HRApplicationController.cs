using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
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
        [JsonHandleError]
        public ActionResult CreateApplicationData(FormCollection form, ApplicationDataVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new {  errorMessage = BindHelper.GetErrorMessages(ModelState.Values) },
                    JsonRequestBehavior.AllowGet);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateApplicationData(viewModel);
            }
            catch (Exception e)
            {
                return Json(new {errorMessage = e.Message },
                   JsonRequestBehavior.AllowGet);
            }

            try
            {
                viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
                _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            }
            catch (Exception e)
            {
                return Json(new { errorMessage = e.Message },
                   JsonRequestBehavior.AllowGet);
            }

            try
            {
                viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
                _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            }
            catch(Exception e)
            {
                return Json(new { errorMessage = e.Message },
                  JsonRequestBehavior.AllowGet);
            }

            try
            {
                viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
                _service.CreateWorkingExperienceDetails(headerID, viewModel.WorkingExperienceDetails);
            }catch(Exception e)
            {
                return Json(new { errorMessage = e.Message },
                 JsonRequestBehavior.AllowGet);
            }

            try
            {
                _service.CreateProfessionalDocuments(headerID, viewModel.Documents);
            }
            catch (Exception e)
            {
                return Json(new { errorMessage = e.Message },
                JsonRequestBehavior.AllowGet);
            }

            // Use this if only not embedded in SharePoint page
            return Json(new {
                successMessage = string.Format(MessageResource.SuccessCreateApplicationData, viewModel.FirstMiddleName) },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JsonHandleError]
        public FileResult PrintApplicationData(FormCollection form, ApplicationDataVM viewModel)
        { 
            viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);

            const string relativePath = "~/Views/HRApplication/PrintApplicationData.cshtml";
            string content;
            
            var view = ViewEngines.Engines.FindView(ControllerContext, relativePath, null);
            ViewData.Model = viewModel;
            
            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();
                byte[] pdfBuf = PDFConverter.Instance.ConvertFromHTML(viewModel.FirstMiddleName + "_Application.pdf", content);
                return File(pdfBuf, "application/pdf");
            }
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

        private IEnumerable<TrainingDetailVM> BindTrainingDetails(FormCollection form, IEnumerable<TrainingDetailVM> trainingDetails)
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
            for (int i = 0; i< array.Length; i++)
            {
                array[i].YearOfGraduation = BindHelper.BindDateInGrid("EducationDetails",
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
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRManpowerRequisitionController : Controller
    {
        IHRManpowerRequisitionService _service;

        public HRManpowerRequisitionController()
        {
            _service = new HRManpowerRequisitionService();
        }

        public ActionResult DisplayManpowerRequisition(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetManpowerRequisition(ID);
            return View(viewModel);
        }

        //[HttpPost]
        //public ActionResult CreateManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        RedirectToAction("Index", "Error");
        //    }

            //var siteUrl = SessionManager.Get<string>("SiteUrl");
            //_service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            //int? headerID = null;
            //try
            //{
            //    headerID = _service.CreateManpowerRequisition(viewModel);
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error");
            //}

            //try
            //{
            //    viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
            //    _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error");
            //}

            //try
            //{
            //    viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
            //    _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error");
            //}

            //try
            //{
            //    viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
            //    _service.CreateWorkingExperienceDetails(headerID, viewModel.WorkingExperienceDetails);
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error");
            //}

            //try
            //{
            //    _service.CreateProfessionalDocuments(headerID, viewModel.Documents);
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error");
            //}

        //    return RedirectToAction("Index",
        //        "Success",
        //        new { successMessage = string.Format(MessageResource.SuccessCreateManpowerRequisition, viewModel.FirstMiddleName) });
        //}

        //[HttpPost]
        //public ActionResult PrintManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        //{
        //    viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
        //    viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
        //    viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);

        //    const string relativePath = "~/Views/HRManpowerRequisition/PrintManpowerRequisition.cshtml";
        //    string content;

        //    var view = ViewEngines.Engines.FindView(ControllerContext, relativePath, null);
        //    ViewData.Model = viewModel;
        //    var fileName = viewModel.FirstMiddleName + "_ManpowerRequisition.pdf";

        //    using (var writer = new StringWriter())
        //    {
        //        var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
        //        view.View.Render(context, writer);
        //        writer.Flush();
        //        content = writer.ToString();

        //        // Get PDF Bytes
        //        byte[] pdfBuf = null;
        //        try
        //        {
        //            pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
        //        }
        //        catch (Exception e)
        //        {
        //            ErrorSignal.FromCurrentContext().Raise(e);
        //            RedirectToAction("Index", "Error");
        //        }

        //        if (pdfBuf == null)
        //            return HttpNotFound();
        //        return File(pdfBuf, "ManpowerRequisition/pdf");
        //    }
        //}

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

        
        public ActionResult CreateManpowerRequisition(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetManpowerRequisition(null);
            return View(viewModel);
        }
    }
}
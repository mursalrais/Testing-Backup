﻿using System;
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
using MCAWebAndAPI.Service.Resources;
//using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class HRInsuranceClaimController : Controller
    {

        readonly IInsuranceClaimService _service;

        public HRInsuranceClaimController()
        {
            _service = new InsuranceClaimService();
        }

      
             public ActionResult CreateInsuranceClaim(string siteUrl = null, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModel(useremail);

            return View("CreateInsuranceClaim", viewModel);

        }

        public ActionResult Redirect(string siteUrl = null)
        {
           _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModel();
            viewModel.URL = siteUrl;
            return View("Redirect", viewModel);

        }


        [HttpPost]
        public ActionResult SubmitInsuranceClaim(FormCollection form, InsuranceClaimVM viewModel)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerId;
            try
            {
                  headerId = _service.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }



            try
            {
                viewModel.ClaimComponentDetails = BindClaimComponentDetails(form, viewModel.ClaimComponentDetails);
                _service.CreateClaimComponentDetails(headerId, viewModel.ClaimComponentDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            var strPages = viewModel.UserPermission == "HR" ? "/sitePages/hrInsuranceView.aspx" : "/sitePages/ProfessionalClaim.aspx";

             return RedirectToAction("Redirect","HRInsuranceClaim", new { siteUrl = siteUrl + strPages });

        }


        IEnumerable<ClaimComponentDetailVM> BindClaimComponentDetails(FormCollection form, 
            IEnumerable<ClaimComponentDetailVM> claimComponentDetails)
        {
            var array = claimComponentDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].ReceiptDate = BindHelper.BindDateInGrid("ClaimComponentDetails",
                    i, "ReceiptDate", form);
            }
            return array;
        }


        //IEnumerable<ClaimPaymentDetailVM> BindClaimPaymentDetails(FormCollection form,
        //   IEnumerable<ClaimPaymentDetailVM> medicalClaimDetails)
        //{
        //    var array = medicalClaimDetails.ToArray();

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        array[i].ReceiptDate = BindHelper.BindDateInGrid("ClaimPaymentDetails",
        //            i, "ReceiptDate", form);
        //    }
        //    return array;
        //}


        public ActionResult EditInsuranceClaim(string siteUrl ,int ? id , string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetInsuranceHeader(id, useremail);

            return View(viewModel);
        }

        public ActionResult DisplayInsuranceClaim(string siteUrl, int? id, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetInsuranceHeader(id, useremail);

            return View(viewModel);
        }


        public ActionResult UpdateInsuranceClaim(FormCollection form, InsuranceClaimVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            _service.UpdateHeader(viewModel);


            try
            {
                viewModel.ClaimComponentDetails = BindClaimComponentDetails(form, viewModel.ClaimComponentDetails);
                _service.CreateClaimComponentDetails(viewModel.ID, viewModel.ClaimComponentDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            var strPages = viewModel.UserPermission == "HR" ? "/sitePages/hrInsuranceView.aspx" : "/sitePages/ProfessionalClaim.aspx";
            return RedirectToAction("Redirect", "HRInsuranceClaim", new { siteUrl = siteUrl + strPages });
            // return JsonHelper.GenerateJsonSuccessResponse(siteUrl + strPages);
        }

     


      
  
        public ActionResult ViewClaim(string siteUrl = null, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.getViewProfessionalClaimDefault(useremail);

            return View(viewModel);

        }

        public ActionResult ViewClaimAll(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.getViewProfessionalClaimDefault();

            return View(viewModel);

        }

        public ActionResult ViewClaimOutStanding(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.getViewProfessionalClaimDefault();

            return View(viewModel);

        }

        public ActionResult ReadProfessional([DataSourceRequest] DataSourceRequest request, string useremail = null)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            // _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            // SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewProfessionalClaim(useremail);
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult ReadClaimAll([DataSourceRequest] DataSourceRequest request)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewClaimHR("All items");
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult ReadClaimOutStanding([DataSourceRequest] DataSourceRequest request)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewClaimHR("Outstanding Claim");
            return Json(data.ToDataSourceResult(request));
        }


        public JsonResult DeleteClaimId(int id, string siteUrl = null)
        {
           // var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            _service.DeleteClaim(id);
           

            return null;
        }


        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getComponentAXAdetails();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult SubmitAxa(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModelAXA(true);

            return View(viewModel);

        }

        public ActionResult ReadExportExcel([DataSourceRequest] DataSourceRequest request)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewAXA();
            return Json(data.ToDataSourceResult(request));
        }


        public ActionResult ExportToExcel(InsuranceClaimAXAVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //var siteUrl = SessionManager.Get<string>("SiteUrl");
            //_service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            _service.CreateAxa(viewModel);
            viewModel = _service.GetPopulatedModelAXA(false);
            return View(viewModel);
        }

    


        public ActionResult PrintToPDF(string siteUrl = null)
        {
           // var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModelAXA(false);
            viewModel.dtDetails = _service.getViewAXA();

            return View("PrintToPDF", viewModel);
        }

        public ActionResult PrintToPDF(InsuranceClaimAXAVM viewModel)
        //public ActionResult PrintToPDF()
        {


            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            _service.CreateAxa(viewModel);
            viewModel = _service.GetPopulatedModelAXA(false);

            const string RelativePath = "~/Views/HRInsuranceClaim/PrintToPDF.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = "SubmissionAXA.pdf";
            byte[] pdfBuf = null;
            string content;
            // ControllerContext context = new ControllerContext();
            ControllerContext.Controller.ViewData.Model = viewModel;
            ViewData = ControllerContext.Controller.ViewData;
            TempData = ControllerContext.Controller.TempData;

            using (var writer = new StringWriter())
            {
                //var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(contextviewContext, writer);
                writer.Flush();
                content = writer.ToString();

                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTMLLandscape(fileName, content);
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


        [HttpPost]
        public ActionResult RedirectPrintToPDF(FormCollection form, InsuranceClaimAXAVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            //_service.CreateAxa(viewModel);

            viewModel.dtDetails = _service.getViewAXA();


            const string RelativePath = "~/Views/HRInsuranceClaim/PrintToPDF.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = "_Application.pdf";
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
            // return RedirectToAction("PrintToPDF", "HRInsuranceClaim");
        }

        public ActionResult ReadPrintToPDF([DataSourceRequest] DataSourceRequest request)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl);
            //_service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            //SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewAXA();
            return Json(data.ToDataSourceResult(request));
        }

     

        //[HttpPost]
        //public ActionResult PrintApplicationData(FormCollection form, ApplicationDataVM viewModel)
        //{
        //    viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
        //    viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
        //    viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
        //    ViewData.Model = AdjustViewModel(viewModel);

        //    const string RelativePath = "~/Views/HRApplication/PrintApplicationData.cshtml";
        //    var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
        //    var fileName = viewModel.FirstMiddleName + "_Application.pdf";
        //    byte[] pdfBuf = null;
        //    string content;

        //    using (var writer = new StringWriter())
        //    {
        //        var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
        //        view.View.Render(context, writer);
        //        writer.Flush();
        //        content = writer.ToString();

        //        // Get PDF Bytes
        //        try
        //        {
        //            pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
        //        }
        //        catch (Exception e)
        //        {
        //            ErrorSignal.FromCurrentContext().Raise(e);
        //            RedirectToAction("Index", "Error");
        //        }
        //    }
        //    if (pdfBuf == null)
        //        return HttpNotFound();
        //    return File(pdfBuf, "application/pdf");
        //}

    }
}
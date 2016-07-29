using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
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

        public ActionResult SubmitAxa(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModelAXA();

            return View(viewModel);
           
        }


        public ActionResult ExportToExcel(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModelAXA();
            //Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            return View(viewModel);

        }


        [HttpPost]
        public ActionResult ExportAxa()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
          
            return RedirectToAction("SubmitAxa", "HRInsuranceClaim");
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

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.InsuranceClaim);

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

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.InsuranceClaim);
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl",  ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getComponentAXAdetails();
            return Json(data.ToDataSourceResult(request));
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
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewProfessionalClaim(useremail);
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult ReadClaimAll([DataSourceRequest] DataSourceRequest request)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewClaimHR("All items");
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult ReadClaimOutStanding([DataSourceRequest] DataSourceRequest request)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            DataTable data = _service.getViewClaimHR("Outstanding Claim");
            return Json(data.ToDataSourceResult(request));
        }


        public JsonResult DeleteClaimId(int id, string siteUrl = null)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);
            _service.DeleteClaim(id);
           

            return null;
        }

    }
}
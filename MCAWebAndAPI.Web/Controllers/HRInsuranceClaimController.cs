using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.InsuranceClaim;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRInsuranceClaimController : Controller
    {

        readonly IInsuranceClaimService _service;

        public HRInsuranceClaimController()
        {
            _service = new InsuranceClaimService();
        }

        //public ActionResult CreateInsuranceClaim(string siteUrl = null, string useremail = "junindar@gmail.com")
             public ActionResult CreateInsuranceClaim(string siteUrl = null, string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModel(useremail);

            return View("CreateInsuranceClaim", viewModel);

        }

        [HttpPost]
        public ActionResult SubmitInsuranceClaim(FormCollection form, InsuranceClaimVM viewModel)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                  headerID = _service.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }



            try
            {
                viewModel.ClaimComponentDetails = BindClaimComponentDetails(form, viewModel.ClaimComponentDetails);
                _service.CreateClaimComponentDetails(headerID, viewModel.ClaimComponentDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

          

            //send email to professional
            //try
            //{
            //    if (emailTo == "" || emailTo == null)
            //    {
            //        emailTo = "anugerahseptian@gmail.com";
            //    }
            //    EmailUtil.Send(emailTo, "Notify to initiate Performance Plan", string.Format(emailMessage, UrlResource.ProfessionalPerformancePlan));

            //}
            //catch (Exception e)
            //{
            //    logger.Error(e);
            //    throw e;
            //}

            //try
            //{
            //    viewModel.ClaimPaymentDetails = BindClaimPaymentDetails(form, viewModel.ClaimPaymentDetails);
            //    _service.CreateClaimPaymentDetails(headerID, viewModel.ClaimPaymentDetails);
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.InsuranceClaim);
           // return JsonHelper.GenerateJsonSuccessResponse(siteUrl);
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


        IEnumerable<ClaimPaymentDetailVM> BindClaimPaymentDetails(FormCollection form,
           IEnumerable<ClaimPaymentDetailVM> medicalClaimDetails)
        {
            var array = medicalClaimDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].ReceiptDate = BindHelper.BindDateInGrid("ClaimPaymentDetails",
                    i, "ReceiptDate", form);
            }
            return array;
        }


        public ActionResult EditInsuranceClaim(string siteUrl ,int ? ID , string useremail = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetInsuranceHeader(ID, useremail);

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

            //try
            //{
            //    viewModel.ClaimPaymentDetails = BindClaimPaymentDetails(form, viewModel.ClaimPaymentDetails);
            //    _service.CreateClaimPaymentDetails(viewModel.ID, viewModel.ClaimPaymentDetails);
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}



            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.InsuranceClaim);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Destroy([DataSourceRequest] DataSourceRequest request, ClaimComponentDetailVM component)
        {
            return Json(new[] { component }.ToDataSourceResult(request, ModelState));
        }


    }
}
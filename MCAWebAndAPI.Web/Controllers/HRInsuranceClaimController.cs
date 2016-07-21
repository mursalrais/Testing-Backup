using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Elmah;
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

        public ActionResult CreateInsuranceClaim(string siteUrl = null, int? ID = null)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetPopulatedModel();

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

            try
            {
                viewModel.ClaimPaymentDetails = BindClaimPaymentDetails(form, viewModel.ClaimPaymentDetails);
                _service.CreateClaimPaymentDetails(headerID, viewModel.ClaimPaymentDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

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

    }
}
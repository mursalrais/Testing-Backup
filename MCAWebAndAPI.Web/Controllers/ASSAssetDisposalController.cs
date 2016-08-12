using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetDisposalController : Controller
    {
        IAssetDisposalService assetDisposalService;

        public ASSAssetDisposalController()
        {
            assetDisposalService = new AssetDisposalService();
        }

        // GET: ASSAssetDisposal
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateAssetDisposal(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            assetDisposalService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            var viewModel = assetDisposalService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("CreateAssetDisposal", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitAssetDisposal(FormCollection form, AssetDisposalVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetDisposalService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = assetDisposalService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.Professional);
            //try
            //{
            //    viewModel.AssetTransferDetail = BindMonthlyFeeDetailDetails(form, viewModel.MonthlyFeeDetails);
            //    _hRPayrollService.CreateMonthlyFeeDetails(headerID, viewModel.MonthlyFeeDetails);
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}            
        }
    }
}
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Payroll;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Resources;
using System.Net;
using System;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRFeeSlipController : Controller
    {
        IFeeSlipService _hRFeeSlipService;

        public HRFeeSlipController()
        {
            _hRFeeSlipService = new FeeSlipService();
        }

        public ActionResult CreateFeeSlip(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _hRFeeSlipService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRFeeSlipService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("FeeSlip", viewModel);
        }

        [HttpPost]
        public ActionResult CreateFeeSlip(FormCollection form, FeeSlipVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRFeeSlipService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _hRFeeSlipService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _hRFeeSlipService.CreateFeeSlipDetails(headerID, viewModel.FeeSlipDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }
    }
}
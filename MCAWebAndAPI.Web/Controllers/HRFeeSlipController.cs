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
            viewModel.ID = 100;
          //  SessionManager.Set("ProfessionalFeeSlip", viewModel.FeeSlipDetails);

            // Return to the name of the view and parse the model
            return View(viewModel);
        }

        IEnumerable<FeeSlipDetailVM> BindClaimfeeDetails(FormCollection form,
         IEnumerable<FeeSlipDetailVM> feeDetails)
        {
            var array = feeDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].ProfessionalID = BindHelper.BindIntInGrid("gridDataView",
                    i, "ProfessionalID", form);
            }
            return array;
        }

        [HttpPost]
        public ActionResult PrintFeeSlip(FormCollection form, FeeSlipVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRFeeSlipService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);


            if (!viewModel.FeeSlipDetails.Any())
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Empty");
            }
            else
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(viewModel.FeeSlipDetails.Count().ToString());
            }

            var strPages = "";//viewModel.UserPermission == "HR" ? "/sitePages/hrInsuranceView.aspx" : "/sitePages/ProfessionalClaim.aspx";
            return RedirectToAction("Redirect", "HRInsuranceClaim", new { siteUrl = siteUrl + strPages });
            //int? headerID = null;
            //try
            //{
            //    //   headerID = _hRFeeSlipService.CreateHeader(viewModel);

            //    viewModel.FeeSlipDetails = BindClaimfeeDetails(form, viewModel.FeeSlipDetails);

            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}

            //try
            //{
            //   // _hRFeeSlipService.CreateFeeSlipDetails(headerID, viewModel.FeeSlipDetails);
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}

            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        //public JsonResult GridProfessional_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    // Get from existing session variable or create new if doesn't exist
        //    if (SessionManager.Get<IEnumerable<FeeSlipDetailVM>>("ProfessionalFeeSlip") == null) return null;
        //    var items = SessionManager.Get<IEnumerable<FeeSlipDetailVM>>("ProfessionalFeeSlip");

        //    // Convert to Kendo DataSource
        //    DataSourceResult result = items.ToDataSourceResult(request);

        //    // Convert to Json
        //    var json = Json(result, JsonRequestBehavior.AllowGet);
        //    json.MaxJsonLength = int.MaxValue;
        //    return json;
        //    // return null;

        //    //// Get from existing session variable or create new if doesn't exist
        //    //if (SessionManager.Get<FeeSlipVM>("FeeSlipModel") == null) return null;
        //    //var items = SessionManager.Get<FeeSlipVM>("FeeSlipModel");
        //    //var dtProfessional = items.dtDetails;
        //    //if (dtProfessional == null || dtProfessional.Rows.Count == 0) return null;

        //    //// Convert to Kendo DataSource
        //    //DataSourceResult result = dtProfessional.ToDataSourceResult(request);

        //    //// Convert to Json
        //    //var json = Json(result, JsonRequestBehavior.AllowGet);
        //    //json.MaxJsonLength = int.MaxValue;
        //    //return json;
        //    //// return null;
        //}

    }
}
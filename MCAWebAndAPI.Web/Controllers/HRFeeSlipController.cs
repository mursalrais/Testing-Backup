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
using System.IO;
using Elmah;
using MCAWebAndAPI.Service.Converter;

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
            return View(viewModel);
        }

       
        [HttpPost]
        public ActionResult PrintFeeSlip(FormCollection form, FeeSlipVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRFeeSlipService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);


            // return null;

             string RelativePath = "~/Views/HRFeeSlip/PrintFeeSlip.cshtml";
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);

            viewModel = _hRFeeSlipService.GetModelPrint(viewModel);


            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName ="FeeSlip.pdf";
            byte[] pdfBuf = null;
            string content;

            ControllerContext.Controller.ViewData.Model = viewModel;
            ViewData = ControllerContext.Controller.ViewData;
            TempData = ControllerContext.Controller.TempData;

            using (var writer = new StringWriter())
            {
                var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(contextviewContext, writer);
                writer.Flush();
                content = writer.ToString();
                content = content.Replace("{XIMGPATHX}", domain);

                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTMLFeeSLip(fileName, content);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return JsonHelper.GenerateJsonErrorResponse(e);
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");


            //if (viewModel.FeeSlipDetails.All(c => c.Intchecklist != 1))
            //{
            //    Response.TrySkipIisCustomErrors = true;
            //    Response.StatusCode = (int) HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse("Please select the professional first");
            //}
            //else
            //{
            //    Response.TrySkipIisCustomErrors = true;
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(viewModel.FeeSlipDetails.Count().ToString());
            //}

            //var strPages = "";//viewModel.UserPermission == "HR" ? "/sitePages/hrInsuranceView.aspx" : "/sitePages/ProfessionalClaim.aspx";
            //return RedirectToAction("Redirect", "HRInsuranceClaim", new { siteUrl = siteUrl + strPages });
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
            return null;
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
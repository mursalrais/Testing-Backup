﻿using MCAWebAndAPI.Model.ViewModel.Form.Asset;
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

        public ActionResult Index(string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            assetDisposalService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            String url = (siteUrl ?? ConfigResource.DefaultBOSiteUrl) + UrlResource.AssetDisposal;

            return Content("<script>window.top.location.href = '" + url + "';</script>");
        }

        public ActionResult Edit(int ID, string SiteUrl)
        {
            assetDisposalService.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = assetDisposalService.GetHeader(ID,SiteUrl);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = assetDisposalService.GetDetails(headerID);
                viewModel.Details = viewdetails;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return View(viewModel);
        }

        public ActionResult View(int ID, string SiteUrl)
        {
            assetDisposalService.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = assetDisposalService.GetHeader(ID, SiteUrl);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = assetDisposalService.GetDetails(headerID);
                viewModel.Details = viewdetails;
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Get Data...s");
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SubmitAssetDisposal(FormCollection form, AssetDisposalVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetDisposalService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //if (viewModel.filename == "" || viewModel.filename == null)
            //{
            //    Response.TrySkipIisCustomErrors = true;
            //    Response.TrySkipIisCustomErrors = true;
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest; 
            //    return JsonHelper.GenerateJsonErrorResponse("Please Attach the memo for disposal");
            //}

            int? headerID = null;
            try
            {
                headerID = assetDisposalService.CreateHeader(viewModel);
                if (headerID == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Have To Attach File to Change Completion Status into Complete");
                    //var mesej = "Have To Attach File to Change Completion Status into Complete";
                    //return Json(mesej, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                assetDisposalService.CreateDetails(headerID, viewModel.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index");
           // return Redirect(siteUrl + UrlResource.AssetDisposal);
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

        [HttpPost]
        public ActionResult Update(AssetDisposalVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetDisposalService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if ((_data.attach == null && _data.filename == null) || (_data.attach == null && _data.filename == ""))
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Have To Attach File to Change Completion Status into Complete");
            }

            try
            {
                var update = assetDisposalService.UpdateHeader(_data);
                // if (update == false)
                //{
                //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    return JsonHelper.GenerateJsonErrorResponse("Have To Attach File to Change Completion Status into Complete");
                //}
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            { 
                //update items
                assetDisposalService.UpdateDetails(_data.ID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index");
        }

        //IEnumerable<AssetDisposalDetailVM> BindMonthlyFeeDetailDetails(FormCollection form, IEnumerable<AssetDisposalDetailVM> monthlyFeeDetails)
        //{
        //    var array = monthlyFeeDetails.ToArray();
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        array[i].AssetSubAsset= BindHelper.BindDateInGrid("Details",
        //            i, "AssetSubAsset", form);
        //    }
        //    return array;
        //}

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetDisposalService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.AssetSubAsset.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssetAcquisitionItemVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            //var sessionVariable = System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] as IEnumerable<AssetAcquisitionItemVM>;
            var positions = assetDisposalService.GetAssetSubAsset();

            //if (sessionVariable == null) // If no session variable is found
            //    System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] = positions;
            return positions;
        }
    }
}
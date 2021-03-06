﻿using Kendo.Mvc.Extensions;
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
using MCAWebAndAPI.Service.Converter;
using Elmah;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]

    public class HRAdjustmentController : Controller
    {
        IAdjustmentService _service;

        public HRAdjustmentController()
        {
            _service = new AdjustmentService();
        }

        public ActionResult InputAdjustmentData (string siteurl = null)
        {
            var viewmodel = new AdjustmentDataVM();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            return View(viewmodel); 
        }

        public ActionResult ViewAdjustmentData(string siteurl = null, int? ID = null)
        {
            var viewmodel = new AdjustmentDataVM();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            viewmodel = _service.GetPeriod(ID);

            return View(viewmodel);
        }
        public ActionResult EditAdjustmentData(string siteurl = null, int? ID = null)
        {
            var viewmodel = new AdjustmentDataVM();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            viewmodel = _service.GetPeriod(ID);

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateAdjustmentData(FormCollection form, AdjustmentDataVM viewModel)
        { 
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            string period = Convert.ToString(viewModel.periodDate) ;
            bool checkadjust;

            checkadjust = _service.CheckRequest(viewModel);

            if (checkadjust == true)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
               // return JsonHelper.GenerateJsonErrorResponse("professional with same adjustment type already add in same period..!!");
                return JsonHelper.GenerateJsonErrorResponse("Professional cannot have two same types of adjustment within the same period");
            }

            try
            {
                _service.CreateAdjustmentData(period, viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AdjustmentList);

        }

        [HttpPost]
        public ActionResult EditAdjustmentData(FormCollection form, AdjustmentDataVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            string period = Convert.ToString(viewModel.periodDate);
            bool checkadjust;

            checkadjust = _service.CheckEditRequest(viewModel);

            if (checkadjust == true)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //return JsonHelper.GenerateJsonErrorResponse("professional with same adjustment type already add in same period..!!");
                return JsonHelper.GenerateJsonErrorResponse("Professional cannot have two same types of adjustment within the same period");
            }

            try
            {
                _service.CreateAdjustmentData(period, viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AdjustmentList);

        }

        public async Task<ActionResult> GetAdjustmentDetails(string period)
        {
            var viewmodel = new AdjustmentDataVM();

            if (period == null)
                return PartialView("_InputAdjustmentDetails", viewmodel.AdjustmentDetails);

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            viewmodel = _service.GetAjusmentData(period);

            return PartialView("_InputAdjustmentDetails", viewmodel.AdjustmentDetails);
        }

        public async Task<ActionResult> GetViewAdjustmentDetails(string period)
        {
            var viewmodel = new AdjustmentDataVM();

            if (period == null)
                return PartialView("_ViewAdjustmentDetails", viewmodel.AdjustmentDetails);

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            viewmodel = _service.GetAjusmentData(period);

            return PartialView("_ViewAdjustmentDetails", viewmodel.AdjustmentDetails);
        }

        public JsonResult GetAdjusmentGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = AdjustmentDetailsVM.getAjusmentTypeOptions();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayMethodGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = AdjustmentDetailsVM.getPayTypeOptions();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrencyGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = AdjustmentDetailsVM.getCurrencyOptions();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        //private IEnumerable<AdjustmentDetailsVM> BindAdjustmentlistDateTime(FormCollection form, IEnumerable<AdjustmentDetailsVM> adjustDetails)
        //{
        //    var array = adjustDetails.ToArray();
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        array[i].period = BindHelper.BindDateInGrid("AdjustmentDetails",
        //            i, "CmpDate", form);

        //    }
        //    return array;
        //}
    }
}
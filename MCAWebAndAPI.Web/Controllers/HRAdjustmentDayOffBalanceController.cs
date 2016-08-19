using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Service.HR.AdjustmentDayOffBalance;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using Elmah;
using MCAWebAndAPI.Service.Resources;
using System.Net;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]

    public class HRAdjustmentDayOffBalanceController : Controller
    {
        IAdjustmentDayOffBalanceService adjustmentDayOffBalanceService;
        string _siteUrl;

        public HRAdjustmentDayOffBalanceController()
        {
            adjustmentDayOffBalanceService = new AdjustmentDayOffBalanceService();

        }

        public ActionResult CreateAdjustmentDayOffBalance(int?ID, string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            adjustmentDayOffBalanceService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = adjustmentDayOffBalanceService.GetAdjustmentDayOffBalance(ID);

            if(viewModel.ID == null)
            {
                return View("CreateAdjustmentDayOffBalance", viewModel);
            }
            else
            {
                return View("EditAdjustmentDayOffBalance", viewModel);
            }
        }

        public JsonResult GetLastBalanceAnnualDayOff(int? ID, string dayOffType)
        {
            adjustmentDayOffBalanceService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var lastBalanceAnnualDayOff = adjustmentDayOffBalanceService.GetLastBalanceAnnualDayOff(ID, dayOffType);
            return Json(lastBalanceAnnualDayOff, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastBalanceSpecialDayOff(int? ID, string dayOffType)
        {
            adjustmentDayOffBalanceService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var lastBalanceSpecialDayOff = adjustmentDayOffBalanceService.GetLastBalanceSpecialDayOff(ID, dayOffType);
            return Json(lastBalanceSpecialDayOff, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastBalancePaternity(int? ID, string dayOffType)
        {
            adjustmentDayOffBalanceService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var lastBalancePaternity = adjustmentDayOffBalanceService.GetLastBalancePaternity(ID, dayOffType);
            return Json(lastBalancePaternity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastBalanceCompensatory(int? ID, string dayOffType)
        {
            adjustmentDayOffBalanceService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var lastBalanceCompensatory = adjustmentDayOffBalanceService.GetLastBalanceCompensatory(ID, dayOffType);
            return Json(lastBalanceCompensatory, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateAdjustmentDayOffBalance(FormCollection form, AdjustmentDayOffBalanceVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            adjustmentDayOffBalanceService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? adjustmentDayOffBalanceID = null;

            try
            {
                adjustmentDayOffBalanceID = adjustmentDayOffBalanceService.CreateAdjustmentDayOffBalance(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            if(Convert.ToString(viewModel.DayOffType.Value) == "Annual Day-Off")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdateAnnualDayOffBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }
            if (Convert.ToString(viewModel.DayOffType.Value) == "Special Day-Off")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdateSpecialDayOffBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }
            if (Convert.ToString(viewModel.DayOffType.Value) == "Day-off due to Compensatory time")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdateCompensatoryBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }
            if (Convert.ToString(viewModel.DayOffType.Value) == "Paternity")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdatePaternityBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }

            //return RedirectToAction("Index",
            //    "Success",
            //    new { successMessage = string.Format(MessageResource.SuccessCreateAdjustmentDayOffBalance) });

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AdjustmentDayOffBalanceList);

        }

        [HttpPost]
        public ActionResult UpdateAdjustmentDayOffBalance(FormCollection form, AdjustmentDayOffBalanceVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            adjustmentDayOffBalanceService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            try
            {
                adjustmentDayOffBalanceService.UpdateAdjustmentDayOffBalance(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            if (Convert.ToString(viewModel.DayOffType.Value) == "Annual Day-Off")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdateAnnualDayOffBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }
            if (Convert.ToString(viewModel.DayOffType.Value) == "Special Day-Off")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdateSpecialDayOffBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }
            if (Convert.ToString(viewModel.DayOffType.Value) == "Day-off due to Compensatory time")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdateCompensatoryBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }
            if (Convert.ToString(viewModel.DayOffType.Value) == "Paternity")
            {
                try
                {
                    adjustmentDayOffBalanceService.UpdatePaternityBalance(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error");
                }
            }

            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessCreateAdjustmentDayOffBalance) });
        }
    }
}
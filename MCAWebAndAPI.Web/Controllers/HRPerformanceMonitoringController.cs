using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRPerformanceMonitoringController : Controller
    {
        IPerformanceMonitoringService _service; 
        public HRPerformanceMonitoringController()
        {
            _service = new PerformanceMonitoringService();
        }

        // GET: HRPerformanceMonitoring
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerformanceMonitoring(int? ID = null,string type = null,string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            
            var viewModel = _service.GetPerformanceMonitoring(ID);
            ViewBag.Type = type;

            return View("PerformanceMonitoring", viewModel);
        }
        [HttpPost]
        public ActionResult PerformanceMonitoring(FormCollection form, PerformanceMonitoringVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? ID = viewModel.ID;
            //if id == 0 / null then create else update
            if (ID == null)
            {                
                try
                {
                    ID = _service.CreatePerformanceMonitoring(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
                }

                try
                {
                    _service.CreatePerformanceMonitoringDetails(ID,EmailResource.PerformancePlan);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
                }                
            }
            else
            {
                //update
                try
                {
                    _service.UpdatePerformanceMonitoring(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
                }
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.PerformancePlan);
        }

    }
}
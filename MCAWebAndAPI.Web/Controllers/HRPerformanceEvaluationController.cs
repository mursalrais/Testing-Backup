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
    public class HRPerformanceEvaluationController : Controller
    {
        IPerformanceEvaluationService _service; 
        public HRPerformanceEvaluationController()
        {
            _service = new PerformanceEvaluationService();
        }

        // GET: HRPerformanceEvaluation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerformanceEvaluation(int? ID = null,string type = null,string siteUrl = null)
        {



            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            
            var viewModel = _service.GetPerformanceEvaluation(ID);
            ViewBag.Type = type;
            viewModel.EditType = type;
            return View("PerformanceEvaluation", viewModel);
        }
        [HttpPost]
        public ActionResult PerformanceEvaluation(FormCollection form, PerformanceEvaluationVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? ID = viewModel.ID;
            //if id == 0 / null then create else update
            if (ID == null)
            {                
                try
                {
                    ID = _service.CreatePerformanceEvaluation(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
                }

                try
                {
                    _service.CreatePerformanceEvaluationDetails(ID,EmailResource.PerformanceEvaluation);
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
                    _service.UpdatePerformanceEvaluation(viewModel);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
                }
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.PerformanceEvaluation);
        }

    }
}
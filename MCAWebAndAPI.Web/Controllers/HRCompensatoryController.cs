using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRCompensatoryController : Controller
    {
        IHRCompensatoryService _service;

        public HRCompensatoryController()
        {
            _service = new HRCompensatoryService();
        }

        public ActionResult InputCompensatoryUser(string siteurl = null, int? iD = null)
        {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
            }
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetComplistbyCmpid(iD);

            viewmodel.cmpID = iD;
            //viewmodel.ID = id;
            return View(viewmodel);
        }
         
        public ActionResult InputCompensatoryHR(string siteurl = null, int? iD = null)
        {
            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            else
            {
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
                SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }

            var viewmodel = _service.GetComplistbyProfid(iD);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        public ActionResult CompensatorylistUser(string siteurl = null, int? iD = null)
        {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetComplistbyCmpid(iD);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        public ActionResult CompensatorylistHR(string siteurl = null, int? ID = null)
        {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetComplistActive();

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateCompensatoryData(FormCollection form, CompensatoryVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var testget = form[""];

            int? cmpID = viewModel.cmpID;

            try
            {
                viewModel.CompensatoryDetails = BindCompensatorylistDetails(form, viewModel.CompensatoryDetails);
                _service.CreateCompensatoryData(cmpID, viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index",
               "Success",
               new
               {
                   errorMessage = string.Format(MessageResource.SuccessCreateCompensatoryData, viewModel.cmpName)
               });
        }

        private IEnumerable<CompensatoryDetailVM> BindCompensatorylistDetails(FormCollection form, IEnumerable<CompensatoryDetailVM> compDetails)
        {
            var array = compDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].CmpDate = BindHelper.BindDateInGrid("CompensatoryDetails",
                    i, "CmpDate", form);

                array[i].StartTime = array[i].CmpDate + BindHelper.BindTimeInGrid("CompensatoryDetails",
                    i, "StartTime", form);

                array[i].FinishTime = array[i].CmpDate + BindHelper.BindTimeInGrid("CompensatoryDetails",
                    i, "FinishTime", form);
            }
            return array;
        }

        public JsonResult GetStatusGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = ShortlistDetailVM.GetStatusOptions();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

    }
}
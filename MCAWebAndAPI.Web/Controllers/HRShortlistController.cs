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
    public class HRShortlistController : Controller
    {
        IHRShortlistService _service;


        public HRShortlistController()
        {
            _service = new HRShortlistService();
        }

        public ActionResult ShortlistData(string siteurl = null, int? position = null, string username = null, string useraccess = null, params int[] positionid)
        {



            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlist(position, username, useraccess);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult UpdateShortlistData(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var testget = form[""];

            int? headerID = null;

            try
            {
                viewModel.ShortlistDetails = BindShortlistDetails(form, viewModel.ShortlistDetails);
                _service.UpdateShortlistDataDetail(headerID, viewModel.ShortlistDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            _service.SendEmailValidation(viewModel.SendTo, EmailResource.EmailShortlistData);

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }

        public ActionResult ShortlistSendInvite(string siteurl = null, int? ID = null)
        {



            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlistSend(ID);
            //viewmodel.SendTo = "";
            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateSendInvite(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                viewModel.ShortlistDetails = BindShortlistDetails(form, viewModel.ShortlistDetails);
                _service.CreateShorlistSendintv(headerID, viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }

        public ActionResult ShortlistIntvinvite(string siteurl = null, int? position = null, string username = null, string useraccess = null)
        {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlist(position, username, useraccess);
            //viewmodel.SendTo = "";

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateIntvinvite(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                viewModel.ShortlistDetails = BindShortlistDetails(form, viewModel.ShortlistDetails);
                _service.CreateShortlistInviteIntv(headerID, viewModel, EmailResource.EmailInterviewResult);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }

        private IEnumerable<ShortlistDetailVM> BindShortlistDetails(FormCollection form, IEnumerable<ShortlistDetailVM> shortDetails)
        {
            var array = shortDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].GetStat = BindHelper.BindStringInGrid("ShortlistDetails",
                    i, "Status", form);

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


        public JsonResult GetPosition()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = _service.GetPositions();

            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.IsKeyPosition,
                    Desc = string.Format("{0}", e.PositionName)
                }),
                JsonRequestBehavior.AllowGet);
        }
    }
}
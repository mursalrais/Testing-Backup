using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Resources;
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

        public ActionResult ShortlistData(string siteurl = null, string position = null, string username = null, string useraccess = null)
        {
            // clear existing session variables if any
            SessionManager.RemoveAll();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlist(position, username, useraccess);
            //viewmodel.SendTo = "";
            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ShortlistData(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;

            try
            {
                viewModel.ShortlistDetails = BindShortlistDetails(form, viewModel.ShortlistDetails);
                _service.CreateShortlistDataDetail(headerID, viewModel.ShortlistDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            SPConnector.SendEmail(viewModel.SendTo, "Data list candidate has been send", "Interview Invitation", siteUrl);

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }

        public ActionResult ShortlistSendInvite(string siteurl = null, int? ID = null)
        {
            // clear existing session variables if any
            SessionManager.RemoveAll();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlistSend(ID);
            //viewmodel.SendTo = "";
            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ShortlistSendInvite(FormCollection form, ApplicationShortlistVM viewModel)
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

        public ActionResult ShortlistIntvinvite(string siteurl = null, string position = null, string username = null, string useraccess = null)
        {
            // clear existing session variables if any
            SessionManager.RemoveAll();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlist(position, username, useraccess);
            //viewmodel.SendTo = "";

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ShortlistIntvinvite(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                viewModel.ShortlistDetails = BindShortlistDetails(form, viewModel.ShortlistDetails);
                _service.CreateShortlistInviteIntv(headerID, viewModel);
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
            return array;
        }
    }
}
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
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

        public ActionResult ShortlistData(string siteurl = null, int? id = null, string position = null)
        {
            // clear existing session variables if any
            SessionManager.RemoveAll();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlist(null);
            //viewmodel.SendTo = "";
            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult EditShortlistData(FormCollection form, ShortlistDetailVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.EditShortlistData(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(UrlResource.Professional);
        }

        [HttpPost]
        public ActionResult ShortlistData(FormCollection form, ApplicationShortlistVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                viewModel.ShortlistDetail = BindShortlistDetails(form, viewModel.ShortlistDetail);
                _service.CreateShortlistDataDetail(headerID, viewModel.ShortlistDetail);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            
            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }


        private IEnumerable<ShortlistDetailVM> BindShortlistDetails(FormCollection form,
           IEnumerable<ShortlistDetailVM> shortDetails)
        {
            var array = shortDetails.ToArray();
            return array;
        }
    }
}
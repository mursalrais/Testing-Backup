using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Utils;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRInterviewlistController : Controller
    {
        IHRInterviewService _service;

        public HRInterviewlistController()
        {
            _service = new HRInterviewService();
        }

        public ActionResult InterviewlistData(string siteurl = null, int? position = null, string username = null, string useraccess = null)
        {
            //mandatory: set site url
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

            var viewmodel = _service.GetInterviewlist(position, username, useraccess);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        public ActionResult NextInterviewlist(string siteurl = null, int? position = null, string username = null, string useraccess = null)
        {
            //mandatory: set site url
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

            var viewmodel = _service.GetInterviewlist(position, username, useraccess);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        public ActionResult InterviewPanellistData(string siteurl = null, int? position = null, string username = null, string useraccess = null)
        {
            //mandatory: set site url
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

            var viewmodel = _service.GetInterviewlist(position, username, useraccess);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateInterviewlistData(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var testget = form[""];

            int? headerID = viewModel.ID;

            try
            {
                _service.CreateInterviewDataDetail(headerID, viewModel);
                Task CreateManpowerRequisitionDocumentsTask = _service.CreateInterviewDocumentsSync(headerID, viewModel.Documents);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("InputInterviewResult",
               "HRInterviewlist",
               new
               {
                   siteurl = siteUrl,
                   ID = viewModel.ID
               });
        }

        [HttpPost]
        public ActionResult SendMailCandidate(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            try
            {
                char[] delimiterChars = { ' ', ',', ';' };

                string[] words = viewModel.InterviewerPanel.Split(delimiterChars);

                foreach (string mail in words)
                {
                    if (mail != "")
                    {
                        string link = string.Format(UrlResource.InterviewPanelList, siteUrl, viewModel.Position);

                        string mailbody = string.Format(EmailResource.EmailInterviewToInterviewPanel, viewModel.EmailMessage, link);

                        EmailUtil.Send(mail, "Next Process Interview for position " + viewModel.PositionName + " (based on respective position)", mailbody);
                    }
                }

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
                   errorMessage = string.Format(MessageResource.SuccessCreateApplicationData, viewModel.Candidate)
               });
        }

        public ActionResult InputInterviewResult(string siteurl = null, int? ID = null)
        {
            //mandatory: get site url
            _service.SetSiteUrl(siteurl ?? SessionManager.Get<string>("SiteUrl"));
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetResultlistInterview(ID);
            viewmodel.SiteUrl = siteurl;
            //viewmodel.ID = id;

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult InputResultInterview(FormCollection form, ApplicationShortlistVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = viewModel.ID;
            try
            {
                viewModel.ShortlistDetails = BindShortlistDetails(form, viewModel.ShortlistDetails);
                _service.CreateInputIntvResult(headerID, viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("InputInterviewResult",
               "HRInterviewlist",
               new
               {
                   siteurl = siteUrl,
                   ID = headerID
               });
        }

        public ActionResult InputInterviewResultDetail(string siteurl = null, int? ID = null)
        {

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetResultlistInterview(ID); 
            //viewmodel.SendTo = "";
            //viewmodel.ID = id;
            return View(viewmodel);
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
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

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
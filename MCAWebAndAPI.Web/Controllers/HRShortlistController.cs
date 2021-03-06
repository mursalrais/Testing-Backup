﻿using Elmah;
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

        //Display Shortlist Step 1 by HR & Requestor
        public ActionResult ShortlistData(string siteurl = null, int? position = null, string username = null, string useraccess = null)
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

            var viewmodel = _service.GetShortlist(position, username, useraccess);
            //viewmodel.ID = id;
            return View(viewmodel);
        }

        //Update Shortlist Step 1 by HR & Requestor
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

            char[] delimiterChars = { ' ', ',', ';' };

            string[] words = viewModel.SendTo.Split(delimiterChars);

            //send mail by HR
            if (viewModel.useraccess == "HR")
            {
                string bodymailHR = string.Format(EmailResource.EmailShortlistToRequestor, _service.GetMailUrl(Convert.ToInt32(viewModel.Position), viewModel.useraccess), viewModel.PositionName);
                List<string> lstEmail = new List<string>();

                foreach (string mail in words)
                {
                    if (mail != "")
                    {
                        lstEmail.Add(mail);
                    }
                }
                _service.SendEmailValidation(lstEmail, viewModel.PositionName, bodymailHR);
            }
            //send mail by Requestor
            else if (viewModel.useraccess == "REQ")
            {
                string bodymailREQ = string.Format(EmailResource.EmailShortlistToHR, _service.GetMailUrl(Convert.ToInt32(viewModel.Position), viewModel.useraccess), viewModel.PositionName);
                List<string> lstEmail = new List<string>();

                foreach (string mail in words)
                {
                    if (mail != "")
                    {
                        lstEmail.Add(mail);
                    }
                }
                _service.SendEmailValidation(lstEmail, viewModel.PositionName, bodymailREQ);
            }

            return RedirectToAction("Index",
                "Success",
                new
                {
                    errorMessage =
                string.Format(MessageResource.SuccessCreateApplicationData, viewModel.Position)
                });
        }

        //Send invitation to candidate
        public ActionResult ShortlistSendInvite(string siteurl = null, int? ID = null)
        {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlistSend(ID);
            //viewmodel.ID = id;
            return View(viewmodel);
        }

        //Update data Send invitation to candidate
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

            var dateintv = viewModel.InterviewerDate.Value.ToString("yyyy-MM-dd");

            var timeintv = viewModel.InterviewerTime.Value.ToString("HH:mm");

            string bodymailREQ = string.Format(EmailResource.EmailShortlistToCandidate, viewModel.Message);

            List<string> lstEmail = new List<string> { viewModel.SendTo };

            _service.SendEmailValidation(lstEmail, viewModel.PositionName, bodymailREQ);

            return RedirectToAction("Index",
               "Success",
               new
               {
                   errorMessage =
               string.Format(MessageResource.SuccessCreateApplicationData, viewModel.Candidate)
               });
        }

        //Shortlist Step 3 by HR
        public ActionResult ShortlistIntvinvite(string siteurl = null, int? position = null, string username = null, string useraccess = null)
         {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetShortlist(position, username, useraccess);
            //viewmodel.SendTo = "";

            return View(viewmodel);
        }

        public ActionResult ShortlistInterviewPanel(string siteurl = null, int? position = null, string username = null, string useraccess = null)
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
                _service.CreateShortlistInviteIntv(headerID, viewModel);

                char[] delimiterChars = { ' ', ',', ';' };

                string[] words = viewModel.InterviewerPanel.Split(delimiterChars);

                foreach (string mail in words)
                {
                    string linkmail = string.Format(UrlResource.ShortlistInterviewPanel, siteUrl, viewModel.Position);

                    string bodymail = string.Format(EmailResource.EmailShortlistToInterviewPanel, linkmail, viewModel.EmailMessage);

                    if (mail != "")
                    {
                        EmailUtil.Send(mail, "Interview Invitation for Position " + viewModel.PositionName , bodymail);
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
                   errorMessage =
               string.Format(MessageResource.SuccessCreateApplicationData, viewModel.Position)
               });
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

        private IEnumerable<PositionMaster> GetShortlistPositionExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ShortlistPositionActive"] as IEnumerable<PositionMaster>;
            var shortlistpositionactive = sessionVariable ?? _service.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ShortlistPositionActive"] = shortlistpositionactive;
            return shortlistpositionactive;
        }

        public JsonResult GetPosition()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = GetShortlistPositionExistingSession();

            positions = positions.OrderBy(x => x.ID);

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
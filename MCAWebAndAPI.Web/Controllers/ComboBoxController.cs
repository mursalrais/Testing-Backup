﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Shared;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

using FinService = MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ComboBoxController : Controller
    {
        private IComboBoxService service;

        public ComboBoxController()
        {
            service = new ComboBoxService();
        }

        public JsonResult GetProfessionals()
        {
            return new HRDataMasterController().GetProfessionals();
        }

        public JsonResult GetProfessionalsActive()
        {
            return new HRDataMasterController().GetProfessionalsActive();
        }

        public JsonResult GetEventBudgetsDirectPayment()
        {
            return GetEventBudgets(EventBudgetService.GetChoice.DirectPayment);
        }

        public JsonResult GetEventBudgetsSCA()
        {
            return GetEventBudgets(EventBudgetService.GetChoice.SCA);
        }

        public JsonResult GetEventBudgets(EventBudgetService.GetChoice choice)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var result = EventBudgetService.GetAllAjaxComboBoxVMs(choice, siteUrl).ToList();
            result.Insert(0, new AjaxComboBoxVM() { Value = 0, Text = "" });

            return Json(result.Select(e =>
                new
                {
                    Value = e.Value,
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivities()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var activities = Service.Common.ActivityService.GetAll(siteUrl);

            return Json(activities.Select(e => new
            {
                e.ID,
                e.Title,
                Project = e.Project.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivitiesByProject(string projectValue)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var activities = Service.Common.ActivityService.GetAllByProject(siteUrl, projectValue);

            return Json(activities.Select(e => new
            {
                Value = e.ID,
                Text = e.Title,
                Project = e.Project
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubActivitiesByEventBudgetID(int? eventBudgetId)
        {
            int activityID = 0;
            List<AjaxComboBoxVM> result = new List<AjaxComboBoxVM>();

            ISCAVoucherService _scaVoucherService = new SCAVoucherService(ConfigResource.DefaultBOSiteUrl);
            service.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            if (eventBudgetId.HasValue && eventBudgetId.Value > 0)
            {
                activityID = _scaVoucherService.GetActivityIDByEventBudgetID(Convert.ToInt32(eventBudgetId));
                result = service.GetSubActivities(activityID).ToList();
            }

            return Json(result.Select(e =>
                new
                {
                    Value = e.Value,
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendors()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var vendors = Service.Common.VendorService.GetAll(siteUrl, false);

            return Json(vendors.Select(e => new
            {
                e.ID,
                e.Title,
                Desc = e.ID == -1 ? string.Empty : string.Format("{0} - {1}", e.Title, e.Name)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGLMasters()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var glMasters = FinService.SharedService.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Title) ? string.Empty : string.Format("{0} - {1}", e.Title, e.GLDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlaces()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var placeService = new PlaceService();

            var list = placeService.GetAllListItems(siteUrl);

            return Json(list.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Name) ? string.Empty : e.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProvinces()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            var provinceService = new ProvinceService();

            var list = provinceService.GetAllListItems(siteUrl);

            return Json(list.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Name) ? string.Empty : e.Name
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using FinService = MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ComboBoxController : Controller
    {
        private const string SITE_URL = "SiteUrl";
        IComboBoxService _comboBoxService;

        public ComboBoxController()
        {
            _comboBoxService = new ComboBoxService();
        }

        public JsonResult GetProfessionals()
        {
            _comboBoxService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var result = _comboBoxService.GetProfessionals().ToList();
            //result.Insert(0, new Model.HR.DataMaster.ProfessionalMaster() { ID = 0, Name = "",Position="" });

            return Json(result.Select(e =>
                new {
                    e.ID,
                    e.Name,
                    e.FirstMiddleName,
                    e.Position,
                    e.Status,
                    e.OfficeEmail,
                    e.Project_Unit,
                    Desc = string.Format("{0} - {1}", e.Name, e.Position)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventBudget()
        {
            _comboBoxService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var result = _comboBoxService.GetEventBudget().ToList();
            result.Insert(0, new Model.ViewModel.Control.AjaxComboBoxVM() { Value = 0, Text = "" });

            return Json(result.Select(e =>
                new
                {
                    Value = e.Value,
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubActivityByEventBudgetID(int? eventBudgetId)
        {
            int activityID = 0;
            List<AjaxComboBoxVM> result = new List<AjaxComboBoxVM>();

            ISCAVoucherService _scaVoucherService = new SCAVoucherService();
            _scaVoucherService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            _comboBoxService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            if (eventBudgetId.HasValue && eventBudgetId.Value > 0)
            {
                activityID = _scaVoucherService.GetActivityIDByEventBudgetID(Convert.ToInt32(eventBudgetId));
                result = _comboBoxService.GetSubActivity(activityID).ToList();
            }

            return Json(result.Select(e =>
                new
                {
                    Value = e.Value,
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendor()
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;

            var vendors = FinService.Shared.GetVendorMaster(siteUrl);

            return Json(vendors.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfessional()
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            
            var vendors = FinService.Shared.GetProfessionalMaster(siteUrl);

            return Json(vendors.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;

            var glMasters = FinService.Shared.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Title) ? string.Empty : string.Format("{0} - {1}",e.Title, e.GLDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWBSMaster()
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;

            var wbsMasters = FinService.Shared.GetWBSMaster(siteUrl);

            return Json(wbsMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = (e.Title + "-" + e.WBSDescription)
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
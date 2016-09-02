using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Service.ProjectManagement.Common;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMWBSController : Controller
    {
        IProjectHierarchyService _service;

        public COMWBSController()
        {
            _service = new ProjectHierarchyService();
        }


        public JsonResult GetWBS()
        {
            _service.SetSiteUrl(ConfigResource.DefaultProgramSiteUrl);
            var data = GetWBSMappingFromExistingSession();

            return Json(data.Select(e =>
                new
                {
                    e.WBSID,
                    e.WBSDescription,
                    e.Activity,
                    e.SubActivity,
                    e.Project
                }
            ), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetWBSMappings(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.GetAllWBSMappings();
            return this.Jsonp(data);
        }

        public ActionResult UpdateWBSMapping(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.UpdateWBSMapping();
            return this.Jsonp(data);
        }


        private IEnumerable<WBSMapping> GetWBSMappingFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["WBSMapping"] as IEnumerable<WBSMapping>;
            var wbsMapping = sessionVariable ?? _service.GetWBSMappingsInProgram();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["WBSMapping"] = wbsMapping;
            return wbsMapping;
        }

    }
}
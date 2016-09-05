using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Service.ProjectManagement.Schedule.Common;
using MCAWebAndAPI.Web.Helpers;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMWBSController : Controller
    {
        public JsonResult GetAllAsJsonResult(string siteUrl = null)
        {
            var data = GetWBSMappingFromExistingSession(siteUrl);

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
            var data = WBSMasterService.GetAllWBSMappings(siteUrl);
            return this.Jsonp(data);
        }

        public ActionResult UpdateWBSMapping(string siteUrl = null)
        {
            var data = WBSMasterService.UpdateWBSMapping(siteUrl);
            return this.Jsonp(data);
        }


        private IEnumerable<WBSMapping> GetWBSMappingFromExistingSession(string siteUrl)
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["WBSMapping"] as IEnumerable<WBSMapping>;
            var wbsMapping = sessionVariable ?? WBSMasterService.GetWBSMappingsInProgram(siteUrl);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["WBSMapping"] = wbsMapping;
            return wbsMapping;
        }

    }
}
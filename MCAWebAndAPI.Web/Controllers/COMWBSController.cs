using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMWBSController : Controller
    {
        public const string ControllerName = "COMWBS";
        public const string GetAllByActivityAsJsonResult_MethodName = "GetAllByActivityAsJsonResult";

        private static string siteUrl = ConfigResource.DefaultProgramSiteUrl;

        public JsonResult GetAllAsJsonResult()
        {
            var data = GetWBSMappingFromExistingSession();

            return Json(data.Select(e =>
                new
                {
                    e.ID,
                    e.WBSID,
                    e.WBSDescription,
                    e.Activity,
                    e.SubActivity,
                    e.Project,
                    Long = string.Format("{0} - {1}", e.WBSID, e.WBSDescription)
                }
            ), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWBSMappings()
        {
            var data = WBSMasterService.GetAllWBSMappings(siteUrl);
            return this.Jsonp(data);
        }

        public JsonResult GetAllByActivityAsJsonResult(string activity = null)
        {
            JsonResult result;

            IEnumerable<WBSMapping> wbsMasters = GetWBSMappingFromExistingSession();

            if (string.IsNullOrEmpty(activity))
            {
                result = Json(wbsMasters.Select(e => new
                {
                    Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                    Text = (e.WBSID + "-" + e.WBSDescription)
                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                result= Json(wbsMasters.Where(w => w.Activity == activity).Select(e => new
                {
                    Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                    Text = (e.WBSID + "-" + e.WBSDescription)
                }), JsonRequestBehavior.AllowGet);
            }


            return result;
        }

        public IEnumerable<WBSMapping> GetAllByActivity(string activity = null)
        {
            IEnumerable<WBSMapping> wbsMappings = GetWBSMappingFromExistingSession();

            return wbsMappings.Where(w => w.Activity == activity);
        }
        
        public static WBSMapping GetWBSMappings(int? ID=null)
        {
            var result = new WBSMapping();
            if (ID != null)
            {
                result = WBSMasterService.GetWBSMappingsInProgram(siteUrl, ID);
            }

            return result;
        }

        private static IEnumerable<WBSMapping> GetWBSMappingFromExistingSession()
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
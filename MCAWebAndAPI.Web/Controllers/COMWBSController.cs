using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMWBSController : Controller
    {
        public const string ControllerName = "COMWBS";

        public const string MethodName_GetAllByActivityAsJsonResult = "GetAllByActivityAsJsonResult";
        public const string MethodName_GetAllAsJsonResult = "GetAllAsJsonResult";

        public const string FieldName_Value = "Value";
        public const string FieldName_Text = "Text";
        public const string FieldName_ID = "ID";
        public const string FieldName_Long = "Long";

        private static string siteUrl = ConfigResource.DefaultProgramSiteUrl;

        public JsonResult GetAllAsJsonResult()
        {
            var data = GetAllCached();

            return Json(data.Select(e =>
                new
                {
                    Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                    Text = (e.Title + "-" + e.WBSDescription),
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

        public IEnumerable<WBS> GetAll()
        {
            return WBSService.GetAll(siteUrl);
        }

        public JsonResult GetAllByActivityAsJsonResult(string activity = null)
        {
            JsonResult result;

            IEnumerable<WBS> wbsMasters = GetAllCached();

            if (string.IsNullOrEmpty(activity))
            {
                result = Json(wbsMasters.Select(e => new
                {
                    Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                    Text = e.WBSID + "-" + e.WBSDescription
                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                result = Json(wbsMasters.Where(w => w.Activity == activity).Select(e => new
                {
                    Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                    Text = e.WBSID + "-" + e.WBSDescription
                }), JsonRequestBehavior.AllowGet);
            }

            return result;
        }

        public static IEnumerable<WBS> GetAllByActivity(string activity = null)
        {
            IEnumerable<WBS> wbsMappings = GetAllCached();

            return wbsMappings.Where(w => w.Activity == activity);
        }

        public static WBS Get(int? id = null)
        {
            var result = new WBS();

            if (id != null)
            {
                result = GetAllCached().SingleOrDefault(w => w.ID == id);
            }

            return result;
        }

        private static IEnumerable<WBS> GetAllCached()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["WBSMapping"] as IEnumerable<WBS>;
            var wbsMapping = sessionVariable ?? WBSService.GetAll(siteUrl);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["WBSMapping"] = wbsMapping;

            return wbsMapping;
        }
    }
}
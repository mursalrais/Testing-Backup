using System.Web.Mvc;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Resources;
using System.Linq;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMMGLController : Controller
    {
        private static string siteUrl = ConfigResource.DefaultBOSiteUrl;

        public JsonResult GetAllAsJsonResult()
        {
            var data = GetFromExistingSession();

            return Json(data.Select(e =>
                new
                {
                    e.ID,
                    e.GLNo,
                    e.GLDescription,
                    Long = string.Format("{0} - {1}", e.GLNo, e.GLDescription)
                }
            ), JsonRequestBehavior.AllowGet);
        }

        private static IEnumerable<GLMasterVM> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["GLMaster"] as IEnumerable<GLMasterVM>;
            var glMasters = sessionVariable ?? GLMasterService.GetAll(siteUrl);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["GLMaster"] = glMasters;

            return glMasters;
        }

    }
}
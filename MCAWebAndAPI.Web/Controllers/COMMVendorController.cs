using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using MCAWebAndAPI.Service.Shared;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMMVendorController : Controller
    {
        private static string siteUrl = ConfigResource.DefaultBOSiteUrl;

        public JsonResult GetAllAsJsonResult()
        {
            var data = GetFromExistingSession();

            return Json(data.Select(e =>
                new
                {
                    e.ID,
                    e.VendorId,
                    e.Name,
                    Long = string.Format("{0} - {1}", e.VendorId, e.Name)
                }
            ), JsonRequestBehavior.AllowGet);
        }

        public static IEnumerable<VendorVM> GetAll()
        {
            return GetFromExistingSession();
        }

        private static IEnumerable<VendorVM> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Vendor"] as IEnumerable<VendorVM>;
            var glMasters = sessionVariable ?? VendorService.GetVendorMaster(siteUrl);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["GLMaster"] = glMasters;

            return glMasters;
        }
    }
}
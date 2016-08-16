using System;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class VendorController : Controller
    {
        private const string SessionSiteUrl = "SiteUrl";

        public JsonResult GetVendor()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;

            SessionManager.Set(SessionSiteUrl, siteUrl);

            var vendors = Service.Shared.VendorService.GetVendorMaster(siteUrl);

            return Json(vendors.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.VendorId + " - " + e.Name
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINComboboxController : Controller
    {
        private const string SiteUrl = "SiteUrl";


        public FINComboboxController()
        {
           
        }

        public JsonResult GetSCAVouchers(string siteUrl)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            ISCAVoucherService service = new SCAVoucherService(siteUrl);

            var result = service.GetAllAjaxComboBoxVM().ToList();
            result.Insert(0, new AjaxComboBoxVM() { Value = 0, Text = "" });

            return Json(result.Select(e =>
                new
                {
                    Value = e.Value,
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }
    }
}
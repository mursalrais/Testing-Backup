using System;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Web.Controllers
{
    public class SharedFinanceController: Controller
    {
        public const string PrintImageLogo = "{0}/img/logo.png";

        public JsonResult GetGLMaster(string siteUrl)
        {
            var glMasters = SharedService.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Title) ? string.Empty : e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public string GetImageLogoPrint(bool isHttps, string domain)
        {
            string url = string.Empty;
            if (isHttps)
            {
                url = "https://";
            }
            else
            {
                url = "http://";
            }

            return string.Format(PrintImageLogo, url + domain);
        }
    }
}
using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using MCAWebAndAPI.Web.Helpers;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class RIAController : Controller
    {
        IRIAService riaService;

        public RIAController()
        {
            riaService = new RIAService();
        }

        // GET: RIA
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetOverallRIAChart(string siteUrl = null) {

            riaService.SetSiteUrl(siteUrl);
            var data = riaService.GetOverallRIAChart();

            return this.Jsonp(data);
            
        }

        public ActionResult GetRIAResourceChart(string riaType, string siteUrl = null)
        {
            riaService.SetSiteUrl(siteUrl);
            riaType = riaType.Replace("\"", "");
            riaType = riaType.Replace("\'", "");

            var data = riaService.GetRIAResourceChart(riaType);

            return this.Jsonp(data);
        }

        public ActionResult GetRIAStatusChart(string riaType, string siteUrl = null)
        {
            riaService.SetSiteUrl(siteUrl);
            riaType = riaType.Replace("\"", "");
            riaType = riaType.Replace("\'", "");

            var data = riaService.GetRIAStatusChart(riaType);

            return this.Jsonp(data);
        }

        public ActionResult GetIssuesAgeingChart(string siteUrl = null)
        {
            riaService.SetSiteUrl(siteUrl);

            var data = riaService.GetIssuesAgeingChart();

            return this.Jsonp(data);
        }
    }
}
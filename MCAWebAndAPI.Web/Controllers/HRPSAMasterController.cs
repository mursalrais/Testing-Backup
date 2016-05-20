using MCAWebAndAPI.Service.HR.Common;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRPSAMasterController : Controller
    {

        IPSAMasterService _dataPSAMasterService;

        public HRPSAMasterController()
        {
            _dataPSAMasterService = new PSAMasterService();
        }

        public JsonResult GetPsa(string id)
        {
            _dataPSAMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var professionals = GetFromExistingSession();
            return Json(professionals.Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.ID,
                        e.JoinDate,
                        e.DateOfNewPSA,
                        e.PsaExpiryDate,
                        e.ProjectOrUnit
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PSAMaster> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PSA"] as IEnumerable<PSAMaster>;
            var psa = sessionVariable ?? _dataPSAMasterService.GetPSAs();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PSA"] = psa;
            return psa;
        }
    }
}
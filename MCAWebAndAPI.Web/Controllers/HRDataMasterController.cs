using MCAWebAndAPI.Service.HR.Common;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Web.Helpers;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRDataMasterController : Controller
    {

        IHRDataMasterService _dataMasterService;

        public HRDataMasterController()
        {
            _dataMasterService = new HRDataMasterService();
        }

        public JsonResult GetProfessionalMonthlyFees()
        {
            _dataMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var professionalmonthlyfee = GetFromProfessionalMonthlyFeesExistingSession();

            return Json(professionalmonthlyfee.Select(e =>
                new {
                    e.ID,
                    e.Name,
                    e.Status,
                    Desc = string.Format("{0} - {1}", e.Name, e.Status)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfessionalMonthlyFeesEdit()
        {
            //TODO: Ask whether it is from BO or from HR
            _dataMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var professionalmonthlyfee = GetFromProfessionalMonthlyFeesEditExistingSession();

            return Json(professionalmonthlyfee.Select(e =>
                new {
                    e.ID,
                    e.Name,
                    e.Status,
                    Desc = string.Format("{0} - {1}", e.Name, e.Status)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfessionals()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var professionals = GetFromExistingSession();

            return Json(professionals.Select(e => 
                new {
                    e.ID,
                    e.Name, 
                    e.Position,
                    e.Status,
                    e.Project_Unit,
                    Desc = string.Format("{0} - {1}", e.Name, e.Position) }),
                JsonRequestBehavior.AllowGet);
        }

        /*
        public JsonResult GetProfessionalPositionProject(int id)
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var professionals = GetFromExistingSession();
            return Json(professionals.Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.ID,
                        e.Name,
                        e.Position,
                        e.Status
                    }
                ), JsonRequestBehavior.AllowGet);
        }
        */

        public JsonResult GetProfessional(int id)
        {
            _dataMasterService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var professionals = GetFromExistingSession();
            return Json(professionals.Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.ID,
                        e.Name,
                        e.Position,
                        e.Status,
                        e.Project_Unit
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositions()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.Title,
                    e.positionStatus,
                    e.PositionManpowerRequisitionApprover1,
                    e.positionManpowerRequisitionApprover2,
                    e.Remarks,
                    e.isKeyPosition,
                    Desc = string.Format("{0}", e.Title)
                }),
                JsonRequestBehavior.AllowGet);
        }


        private IEnumerable<ProfessionalMaster> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionals = sessionVariable ?? _dataMasterService.GetProfessionals();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionals;
            return professionals;
        }

        /*
        private IEnumerable<ProfessionalMaster> GetFromProfessionalPositionProjectExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionals = sessionVariable ?? _dataMasterService.GetProfessionalPositionProject();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionals;
            return professionals;
        }
        */

        private IEnumerable<ProfessionalMaster> GetFromProfessionalMonthlyFeesEditExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionals = sessionVariable ?? _dataMasterService.GetProfessionalMonthlyFeesEdit();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionals;
            return professionals;
        }

        private IEnumerable<ProfessionalMaster> GetFromProfessionalMonthlyFeesExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionalmonthlyfees = sessionVariable ?? _dataMasterService.GetProfessionalMonthlyFees();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionalmonthlyfees;
            return professionalmonthlyfees;
        }

        private IEnumerable<PositionsMaster> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PositionsMaster"] as IEnumerable<PositionsMaster>;
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PositionsMaster"] = positions;
            return positions;
        }

    }
}
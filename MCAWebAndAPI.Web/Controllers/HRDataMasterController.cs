using MCAWebAndAPI.Service.HR.Common;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Web.Helpers;
using System;

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
            _dataMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var professionalmonthlyfee = GetFromProfessionalMonthlyFeesEditExistingSession();
            return Json(professionalmonthlyfee.Select(e =>
                new
                {
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
            professionals = professionals.OrderBy(e => e.FirstMiddleName);

            return Json(professionals.Select(e => 
                new {
                    e.ID,
                    e.Name, 
                    e.FirstMiddleName,
                    e.Position,
                    e.Status,
                    e.OfficeEmail,
                    e.Project_Unit,
                    Desc = string.Format("{0}", e.Name),
                    Desc1 = string.Format("{0} - {1}", e.Name, e.Position) }),
                JsonRequestBehavior.AllowGet);
        }

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
                        e.FirstMiddleName,
                        e.Position,
                        e.Status,
                        e.Project_Unit,
                        e.PositionId, 
                        e.PSANumber,
                        e.JoinDate,
                        e.OfficeEmail, 
                        e.PersonalMail,
                        e.JoinDateTemp,
                        e.InsuranceAccountNumber
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectUnits()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionsExistingSession();
            positions = positions.GroupBy(e => e.ProjectUnit).Select(y => y.First());
            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.IsKeyPosition,
                    e.ProjectUnit,
                    Desc = string.Format("{0}", e.PositionName)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositions()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionsExistingSession();
            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.IsKeyPosition,
                    e.ProjectUnit,
                    Desc = string.Format("{0}", e.PositionName)
                }),
                JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = (2 * 3600))]
        public JsonResult GetPositionsManpower(string Level = null)
        {
            _dataMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var positions = _dataMasterService.GetPositionsManpower(Level);
            positions = positions.OrderBy(e => e.PositionName);

            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.IsKeyPosition,
                    Desc = string.Format("{0}", e.PositionName)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosition(int id)
        {
            _dataMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultHRSiteUrl);
            var position = _dataMasterService.GetPosition(id);
            return Json(new {
                position.ID, 
                position.PositionName
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKeyPosition(int id)
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetKeyPositionsExistingSession();

            return Json(positions.Where(e => e.ID == id).Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.IsKeyPosition
                    //Desc = string.Format("{0}", e.Title)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositionsGrid()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionsExistingSession();
            positions = positions.OrderBy(e => e.ProjectUnit);
            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.ID),
                    Text = e.ProjectUnit+" - "+ e.PositionName
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfessionalsGrid()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var professional = GetFromExistingSession();

            return Json(professional.Select(e =>
                new {
                    Value = Convert.ToString(e.ID),
                    Text = e.Name
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

        private IEnumerable<ProfessionalMaster> GetFromProfessionalMonthlyFeesExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMasterMonthlyFees"] as IEnumerable<ProfessionalMaster>;
            var professionalmonthlyfees = sessionVariable ?? _dataMasterService.GetProfessionalMonthlyFees();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMasterMonthlyFees"] = professionalmonthlyfees;
            return professionalmonthlyfees;
        }

        private IEnumerable<ProfessionalMaster> GetFromProfessionalMonthlyFeesEditExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMasterMonthlyFeesEdit"] as IEnumerable<ProfessionalMaster>;
            var professionalmonthlyfees = sessionVariable ?? _dataMasterService.GetProfessionalMonthlyFeesEdit();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMasterMonthlyFeesEdit"] = professionalmonthlyfees;
            return professionalmonthlyfees;
        }

        private IEnumerable<PositionMaster> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PositionMaster"] as IEnumerable<PositionMaster>;
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PositionMaster"] = positions;
            return positions;
        }

        private IEnumerable<PositionMaster> GetFromPositionsManpowerExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PositionMaster"] as IEnumerable<PositionMaster>;
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PositionMaster"] = positions;
            return positions;
        }

        private IEnumerable<PositionMaster> GetKeyPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PositionMaster"] as IEnumerable<PositionMaster>;
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PositionMaster"] = positions;
            return positions;
        }


        private IEnumerable<DependentMaster> GetFromExistingSessionDependent()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["DependentMaster"] as IEnumerable<DependentMaster>;
            var dependents = sessionVariable ?? _dataMasterService.GetDependents();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["DependentMaster"] = dependents;
            return dependents;
        }

        private IEnumerable<DependentMaster> GetFromExistingSessionDependentForInsurance()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["DependentMaster"] as IEnumerable<DependentMaster>;
            var dependents = sessionVariable ?? _dataMasterService.GetDependentsForInsurance();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["DependentMaster"] = dependents;
            return dependents;
        }

        public JsonResult GetDependants()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var dependents = GetFromExistingSessionDependent();

            return Json(dependents.Select(e =>
                new
                {
                    e.ID,
                    e.Name,
                    e.InsuranceNumber,
                    e.OrganizationInsurance,
                    Desc = string.Format("{0}", e.Name)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependantsForInsurance()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var dependents = GetFromExistingSessionDependentForInsurance();

            return Json(dependents.Select(e =>
                new
                {
                    e.ID,
                    e.Name,
                    e.InsuranceNumber,
                    e.OrganizationInsurance,
                    Desc = string.Format("{0}", e.Name)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependant(int id)
        {
            _dataMasterService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);
            var dependents = GetFromExistingSessionDependent();
            return Json(dependents.Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.ID,
                        e.Name,
                        e.InsuranceNumber,
                        e.OrganizationInsurance
                    }
                ), JsonRequestBehavior.AllowGet);
        }

    }
}
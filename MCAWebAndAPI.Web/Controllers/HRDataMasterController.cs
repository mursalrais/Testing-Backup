﻿using MCAWebAndAPI.Service.HR.Common;
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
                    Desc = string.Format("{0}", e.Name) }),
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
                        e.Position,
                        e.Status,
                        e.Project_Unit,
                        e.PositionId
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositions()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var positions = GetFromPositionExistingSession();

            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.Unit,
                    e.IsKeyPosition,
                    Desc = string.Format("{0}", e.PositionName)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosition(int id)
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var position = GetFromPositionExistingSession().FirstOrDefault(e => e.ID == id);
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
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositionsGrid()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionExistingSession();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.ID),
                    Text = e.PositionName
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
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionalmonthlyfees = sessionVariable ?? _dataMasterService.GetProfessionalMonthlyFees();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionalmonthlyfees;
            return professionalmonthlyfees;
        }

        private IEnumerable<PositionMaster> GetFromPositionExistingSession()
        {
            //Get existing session variable
            var sessionVariable = SessionManager.Get<IEnumerable<PositionMaster>>("PositionMaster");
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                SessionManager.Set<IEnumerable<PositionMaster>>("PositionMaster", positions);
            return positions;
        }

        private IEnumerable<PositionMaster> GetKeyPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PositionsMaster"] as IEnumerable<PositionMaster>;
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PositionsMaster"] = positions;
            return positions;
        }

    }
}
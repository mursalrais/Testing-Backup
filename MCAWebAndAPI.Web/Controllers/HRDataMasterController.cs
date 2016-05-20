﻿using MCAWebAndAPI.Service.HR.Common;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRDataMasterController : Controller
    {

        IDataMasterService _dataMasterService;

        public HRDataMasterController()
        {
            _dataMasterService = new DataMasterService();
        }

        public JsonResult GetProfessionals()
        {
            //TODO: Ask whether it is from BO or from HR
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var professionals = GetFromExistingSession();

            return Json(professionals.Select(e => 
                new {
                    e.ID,
                    e.Name, 
                    e.Position,
                    e.ContactNo, 
                    e.ProjectUnit,
                    Desc = string.Format("{0} - {1}", e.Name, e.Position) }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfessional(int id)
        {
            var professionals = GetFromExistingSession();
            return Json(professionals.Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.ID,
                        e.Name,
                        e.Position,
                        e.ContactNo,
                        e.ProjectUnit
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositions()
        {
            //TODO: Ask whether it is from BO or from HR
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.Title,
                    e.PositionStatus,
                    e.PositionManpowerRequisitionApprover1,
                    e.PositionManpowerRequisitionApprover2,
                    e.Remarks,
                    e.IsKeyPosition,
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
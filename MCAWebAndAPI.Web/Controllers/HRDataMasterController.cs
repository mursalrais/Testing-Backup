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

        IHRDataMasterService _dataMasterService;

        public HRDataMasterController()
        {
            _dataMasterService = new HRDataMasterService();
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

        private IEnumerable<ProfessionalMaster> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionals = sessionVariable ?? _dataMasterService.GetProfessionals();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionals;
            return professionals;
        }

    }
}
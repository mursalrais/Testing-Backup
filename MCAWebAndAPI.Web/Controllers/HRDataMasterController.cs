using MCAWebAndAPI.Service.HR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            var professionals = _dataMasterService.GetProfessionals();

            return Json(professionals.Select(e => 
                new {
                    ProfessionalID = e.ID,
                    ProfessionalDesc = string.Format("{0} - {1}", e.Name, e.Position) }),
                JsonRequestBehavior.AllowGet);
        }

    }
}
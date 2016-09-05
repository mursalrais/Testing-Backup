using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class COMProfessionalController : Controller
    {

        //TODO: the following field name constants should be maintained in a central place
        private const string FieldName_OfficeEmail = "officeemail";

        private static IDataMasterService service;

        public COMProfessionalController()
        {
            service = new DataMasterService();
        }

        public static IEnumerable<ProfessionalMaster> GetAll(string siteUrl = null)
        {
            return GetFromExistingSession(siteUrl);
        }

        public static ProfessionalMaster Get(string siteUrl = "", int? id = 0)
        {
            if ((int)id.Value == 0)
            {
                throw new InvalidOperationException("You should specify a valid parameter: id");
            }

            ProfessionalMaster result;

            IEnumerable<ProfessionalMaster> professionals = GetAll(siteUrl);

            result = professionals.FirstOrDefault(prof => prof.ID == id);

            return result;
        }

        public ProfessionalMaster GetFirstOrDefaultByOfficeEmail(string siteUrl = "", string officeEmail = "")
        {
            if (string.IsNullOrEmpty(officeEmail))
            {
                throw new InvalidOperationException("You should specify a valid parameter: officeEmail");
            }

            ProfessionalMaster result;

            IEnumerable<ProfessionalMaster> professionals = GetAll(siteUrl);

            //TODO: may need to check email address for uniqueness since the field is not marked as unique
            result = professionals.FirstOrDefault(prof => prof.OfficeEmail == officeEmail);

            return result;
        }

        public JsonResult GetAllJsonResult(string siteUrl = null)
        {
            service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var professionals = GetFromExistingSession();
            professionals = professionals.OrderBy(e => e.FirstMiddleName);

            return GetJsonResult(professionals);
        }

        public JsonResult GetJsonResult(string siteUrl = "", int? id = 0)
        {
            IEnumerable<ProfessionalMaster> professional = new List<ProfessionalMaster>() { Get(siteUrl, id) };

            return GetJsonResult(professional);
        }

        private JsonResult GetJsonResult(IEnumerable<ProfessionalMaster> professional)
        {
            var  result= Json(professional.Select(e =>
             new
             {
                 e.ID,
                 e.Name,
                 e.FirstMiddleName,
                 e.Position,
                 e.Status,
                 e.OfficeEmail,
                 e.Project_Unit,

                 Desc = string.Format("{0}", e.Name),
                 Desc1 = string.Format("{0} - {1}", e.Name, e.Position),
                 Desc2 = string.Format("{0}", e.FirstMiddleName)
             }), JsonRequestBehavior.AllowGet);

            return result;
        }

        private static IEnumerable<ProfessionalMaster> GetFromExistingSession(string siteUrl = null)
        {

            IEnumerable<ProfessionalMaster> professionals;

            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;

            if (sessionVariable == null)
            {
                siteUrl = string.IsNullOrEmpty(siteUrl) ? ConfigResource.DefaultHRSiteUrl : siteUrl;

                service.SetSiteUrl(siteUrl);
                SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

                professionals = service.GetProfessionals();
            }
            else
            {
                professionals = sessionVariable;
            }

            if (sessionVariable == null)
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionals;

            return professionals;
        }

    }
}
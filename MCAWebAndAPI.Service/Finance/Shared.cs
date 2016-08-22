using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public static class Shared
    {
        private const string GLMASTER_SITE_LIST = "GL Master";
        private const string WBSMASTER_SITE_LIST = "WBS Master";
        private const string SUBACTIVITY_SITE_LIST = "Sub Activity";

        //TODO: this sounds fishy - check with HR
        private const string PROFESSIONAL_SITE_LIST = "Back Office Professional Master";

        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";

        private const string FIELD_GL_DESCRIPTION = "yyxi";
        private const string FIELD_WBS_DESCRIPTION = "WBSDesc";

        private const string ACTIVITYID_SUBACTIVITY = "Activity_x003a_ID";
        private const string WBS_SUBACTIVITY_ID = "Sub_x0020_Activity_x003a_ID";

        public static IEnumerable<GLMasterVM> GetGLMaster(string siteUrl)
        {
            var glMasters = new List<GLMasterVM>();

            foreach (var item in SPConnector.GetList(GLMASTER_SITE_LIST, siteUrl, null))
            {
                glMasters.Add(ConvertToGLMasterModel(item));
            }

            return glMasters;
        }

        public static IEnumerable<WBSMasterVM> GetWBSMaster(string siteUrl, string activityValue=null)
        {
            string caml = null;
            if (!string.IsNullOrWhiteSpace(activityValue))
            {
                var camlGetSubactivity = @"<View><Query><Where><Eq><FieldRef Name='" + ACTIVITYID_SUBACTIVITY + "' /><Value Type='Lookup'>" +
                    activityValue + "</Value></Eq></Where></Query></View>";

                string valuesText = string.Empty;
                foreach (var item in SPConnector.GetList(SUBACTIVITY_SITE_LIST, siteUrl, camlGetSubactivity))
                {
                    valuesText += "<Value Type='Lookup'>" + Convert.ToString(item[FIELD_ID]) + "</Value>";
                }

                var camlGetWbs = @"<View><Query><Where><In><FieldRef Name='" + WBS_SUBACTIVITY_ID + "' /><Values>" +
                    valuesText + "</Values></In></Where></Query></View>";
            }


            var wbsMasters = new List<WBSMasterVM>();

            foreach (var item in SPConnector.GetList(WBSMASTER_SITE_LIST, siteUrl, caml))
            {
                wbsMasters.Add(ConvertToWBSMasterModel(item));
            }

            return wbsMasters;
        }
   

        public static IEnumerable<ProfessionalVM> GetProfessionalMaster(string siteUrl)
        {
            var professionals = new List<ProfessionalVM>();

            professionals.Add(new ProfessionalVM() { ID = -1, Title = string.Empty });

            foreach (var item in SPConnector.GetList(PROFESSIONAL_SITE_LIST, siteUrl, null))
            {
                professionals.Add(ConvertToProfessionalModel(item));
            }

            return professionals;
        }

        private static ProfessionalVM ConvertToProfessionalModel(ListItem item)
        {
            return new ProfessionalVM
            {
                ID = Convert.ToInt32(item[FIELD_ID]),
                Title = Convert.ToString(item[FIELD_TITLE]),


            };
        }

        private static GLMasterVM ConvertToGLMasterModel(ListItem item)
        {
            return new GLMasterVM
            {
                ID = Convert.ToInt32(item[FIELD_ID]),
                Title = Convert.ToString(item[FIELD_TITLE]),

                GLDescription = Convert.ToString(item[FIELD_GL_DESCRIPTION]),
            };
        }

        private static WBSMasterVM ConvertToWBSMasterModel(ListItem item)
        {
            return new WBSMasterVM
            {
                ID = Convert.ToInt32(item[FIELD_ID]),
                Title = Convert.ToString(item[FIELD_TITLE]),
                WBSDescription = Convert.ToString(item[FIELD_WBS_DESCRIPTION])
            };
        }


        


    }
}

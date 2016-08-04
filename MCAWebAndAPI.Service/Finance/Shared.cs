using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Finance
{
    public static class Shared
    {
        private const string GLMASTER_SITE_LIST = "GL Master";
        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";
        private const string FIELD_GL_DESCRIPTION = "yyxi";

        public static IEnumerable<GLMasterVM> GetGLMaster(string siteUrl)
        {
            var glMasters = new List<GLMasterVM>();

            foreach (var item in SPConnector.GetList(GLMASTER_SITE_LIST, siteUrl, null))
            {
                glMasters.Add(ConvertToGLMasterModel(item));
            }

            return glMasters;
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
    }
}

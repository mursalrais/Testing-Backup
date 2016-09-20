using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class GLMasterService: IGLMasterService
    {
        private const string  ListName = "GL Master";

        private const string FieldName_ID = "ID";
        private const string FieldName_No = "Title";
        private const string FieldName_Description = "yyxi";
    
        public static IEnumerable<GLMasterVM> GetAll(string siteUrl)
        {
            var items = new List<GLMasterVM>();

            foreach (var item in SPConnector.GetList(ListName, siteUrl))
            {
                items.Add(ConvertToVM( item));
            }

            return items;
        }

        public static GLMasterVM Get(string siteUrl, int id)
        {
            var items = new List<GLMasterVM>();

            var item = SPConnector.GetListItem(ListName, id, siteUrl);

            return ConvertToVM(item);
        }

        private static GLMasterVM ConvertToVM(ListItem item)
        {
            return new GLMasterVM
            {
                ID = Convert.ToInt32(item[FieldName_ID]),
                GLNo = Convert.ToString(item[FieldName_No]),
                GLDescription = Convert.ToString(item[FieldName_Description]),
            };
        }


    }
}

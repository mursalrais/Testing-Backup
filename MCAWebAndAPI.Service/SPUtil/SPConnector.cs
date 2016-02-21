using System.Linq;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System;
using System.Linq.Expressions;

namespace MCAWebAndAPI.Service.SPUtil
{
    public class SPConnector
    {
        static string CurUrl = "https://eceos2.sharepoint.com/sites/mca-dev";
        static string UserName =  "sp.services@eceos.com";
        static string Password = "Raja0432";
       

        public static ListItemCollection GetList(string listName, string caml = null)
        {
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                var SPList = context.Web.Lists.GetByTitle(listName);
                var camlQuery = caml == null ?
                    CamlQuery.CreateAllItemsQuery() :  
                    new CamlQuery {
                        ViewXml = caml
                    };

                ListItemCollection SPListItems = SPList.GetItems(camlQuery);

                context.Load(SPListItems);
                context.ExecuteQuery();

                return SPListItems;
            }
        }



    }
}

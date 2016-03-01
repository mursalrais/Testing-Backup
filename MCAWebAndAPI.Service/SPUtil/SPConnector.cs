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

        public static ListItem GetListItem(string listName, int listItemID)
        {
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                var SPListItem = context.Web.Lists.GetByTitle(listName).GetItemById(listItemID);
                context.Load(SPListItem);
                context.ExecuteQuery();

                return SPListItem;

            }
        }


        public static void UpdateListItem(string listName, ListItem listItem)
        {
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);
                ListItem SPListItem = SPList.GetItemById(listItem.Id);
                context.Load(SPListItem);
                context.ExecuteQuery();

                // Get current editor
                var currentEditor = SPListItem["Editor"];

                // Set listitem value to parsed listitem
                SPListItem = listItem;

                // Update columns remotely
                SPListItem.Update();
                context.ExecuteQuery();

                // Set editor not to be SP Service
                SPListItem["Editor"] = currentEditor;
                SPListItem.Update();
                context.ExecuteQuery();
            }
        }

        public static void UpdateListItem(string listName, int listItemID, Dictionary<string, object> updatedValues)
        {
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);
                ListItem SPListItem = SPList.GetItemById(listItemID);
                context.Load(SPListItem);
                context.ExecuteQuery();

                // Get current editor
                var currentEditor = SPListItem["Editor"];

                // Set listitem value to parsed listitem
                foreach (var key in updatedValues.Keys)
                {
                    SPListItem[key] = updatedValues[key];
                }
                
                // Update columns remotely
                SPListItem.Update();
                context.ExecuteQuery();

                // Set editor not to be SP Service
                SPListItem["Editor"] = currentEditor;
                SPListItem.Update();
                context.ExecuteQuery();
            }
        }



    }
}

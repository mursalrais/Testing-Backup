using System.Linq;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Collections.Generic;
using System;
using Microsoft.SharePoint.Client.Utilities;

namespace MCAWebAndAPI.Service.Utils
{
    public class SPConnector
    {
        static string CurUrl = "https://eceos2.sharepoint.com/sites/mca-dev/dev";
        static string UserName =  "sp.services@eceos.com";
        static string Password = "Raja0432";

        private static void MapCredential(string url)
        {
            if (url == null || url.Contains("eceos"))
            {
                UserName = "sp.services@eceos.com";
                Password = "Raja0432";
            }

            if (url.Contains("mcai"))
            {
                UserName = "admin.sharepoint@mca-indonesia.go.id";
                Password = "admin123$";
            }
        }

        internal static int GetInsertedItemID(string listName, string siteUrl = null, string caml = null)
        {
            string camlViewXml = caml ?? @"<View>  
            <Query> 
               <OrderBy><FieldRef Name='ID' Ascending='FALSE' /><FieldRef Name='ID' Ascending='FALSE' /></OrderBy> 
            </Query> 
                <ViewFields><FieldRef Name='ID' /></ViewFields> 
            <RowLimit>1</RowLimit> 
            </View>";

            var result = 1;
            var list = GetList(listName, siteUrl, camlViewXml);
            foreach(var item in list)
            {
                result = Convert.ToInt32(item["ID"]);
            }

            return result;
        }

        public static ListItemCollection GetList(string listName, string siteUrl = null, string caml = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
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

                var SPListItems = SPList.GetItems(camlQuery);
                context.Load(SPListItems);
                context.ExecuteQuery();

                return SPListItems;
            }
        }
        
        public static ListItem GetListItem(string listName, int listItemID, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
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

        public static void UpdateListItem(string listName, int listItemID, Dictionary<string, object> updatedValues, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="columnValues"></param>
        /// <param name="siteUrl"></param>
        public static void AddListItem(string listName, Dictionary<string, object> columnValues, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                List spList = context.Web.Lists.GetByTitle(listName);
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = spList.AddItem(itemCreateInfo);
                foreach (var key in columnValues.Keys)
                {
                    newItem[key] = columnValues[key];
                }
                newItem.Update();
                context.ExecuteQuery();
            }
        }

        public static string[] GetChoiceFieldValues(string listName, string fieldName, string siteUrl = null)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl ?? CurUrl))
            {

                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                clientContext.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                Web web = clientContext.Web;
                var list = web.Lists.GetByTitle(listName);
                clientContext.Load(list, d => d.Title);
                clientContext.ExecuteQuery();

                var field = clientContext.CastTo<FieldChoice>(list.Fields.GetByInternalNameOrTitle(fieldName));
                clientContext.Load(field, f => f.Choices);
                clientContext.ExecuteQuery();

                return field.Choices;
            }
        }

        public static bool sendEmail( string email, string content, string subject,string siteUrl= null)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                clientContext.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                EmailProperties properties = new EmailProperties();
                properties.To = new string[] { email };
                properties.Subject = subject;
                properties.Body = content;

                Utility.SendEmail(clientContext, properties);

                clientContext.ExecuteQuery();
            }
            return true;
        }
    }
}

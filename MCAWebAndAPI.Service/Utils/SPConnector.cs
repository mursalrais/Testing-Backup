using System.Linq;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Collections.Generic;
using System;
using Microsoft.SharePoint.Client.Utilities;
using System.IO;

namespace MCAWebAndAPI.Service.Utils
{
    public class SPConnector
    {
        static string CurUrl = "";
        static string UserName =  "";
        static string Password = "";

        private static void MapCredential(string url)
        {
            if (url.Contains("eceos"))
            {
                UserName = "sp.services@eceos.com";
                Password = "Raja0432";
            }
            else if (url.Contains("mcai"))
            {
                UserName = "admin.sharepoint@mca-indonesia.go.id";
                Password = "admin123$";
            }
        }

        internal static int GetLatestListItemID(string listName, string siteUrl = null, string caml = null)
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
                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                return SPListItems;
            }
        }

        public static ListItem GetListItem(string listName, int? listItemID, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);
                
                // Get one listitem
                var SPListItem = context.Web.Lists.GetByTitle(listName).GetItemById((int)listItemID);
                context.Load(SPListItem);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                return SPListItem;
            }
        }

        public static void UpdateListItem(string listName, int? listItemID, Dictionary<string, object> updatedValues, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);
                ListItem SPListItem = SPList.GetItemById(listItemID + string.Empty);
                context.Load(SPListItem);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }

                // Get current editor
                var currentEditor = SPListItem["Editor"];

                // Set listitem value to parsed listitem
                foreach (var key in updatedValues.Keys)
                {
                    SPListItem[key] = updatedValues[key];
                }
                
                // Update columns remotely
                SPListItem.Update();

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }

                // Set editor not to be SP Service
                SPListItem["Editor"] = currentEditor;
                SPListItem.Update();

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void UpdateListItemNoVersionConflict(string listName, int? listItemID, Dictionary<string, object> updatedValues, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);
                ListItem SPListItem = SPList.GetItemById(listItemID + string.Empty);
                context.Load(SPListItem);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }

                // Get current editor
                var currentEditor = SPListItem["Editor"];
               
                // Set listitem value to parsed listitem
                foreach (var key in updatedValues.Keys)
                {
                    SPListItem[key] = updatedValues[key];
                }
                SPListItem["Editor"] = currentEditor;
                // Update columns remotely
                SPListItem.Update();

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }

                // Set editor not to be SP Service
                //SPListItem["Editor"] = currentEditor;
                //SPListItem.Update();

                //try
                //{
                //    context.ExecuteQuery();
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}
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
                
                try
                {
                    newItem.Update();
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void UploadDocument(string listName, Dictionary<string, object> columnValues, string docName, Stream fileStream, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                using (var fs = fileStream)
                {
                    List spList = context.Web.Lists.GetByTitle(listName);
                    context.Load(spList.RootFolder);
                    context.ExecuteQuery();

                    // Upload the file first
                    var fi = new FileInfo(docName);
                    var fileUrl = string.Format("{0}/{1}", spList.RootFolder.ServerRelativeUrl, fi.Name);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, fileUrl, fs, true);

                    // Get uploaded file
                    Microsoft.SharePoint.Client.File fileUploaded = spList.RootFolder.Files.GetByUrl(fileUrl);
                    context.Load(fileUploaded);

                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    // Get list item from File
                    ListItem newItem = fileUploaded.ListItemAllFields;
                    context.Load(newItem);

                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    // Modify column
                    foreach (var key in columnValues.Keys)
                    {
                        newItem[key] = columnValues[key];
                    }
                    newItem.Update();

                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        public static void UploadDocument(string listName, string docName, Stream fileStream, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                using (var fs = fileStream)
                {
                    List spList = context.Web.Lists.GetByTitle(listName);
                    context.Load(spList.RootFolder);

                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    var fi = new FileInfo(docName);
                    var fileUrl = string.Format("{0}/{1}", spList.RootFolder.ServerRelativeUrl, fi.Name);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, fileUrl, fs, true);
                }
            }
        }

        public static string[] GetChoiceFieldValues(string listName, string fieldName, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {

                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                Web web = context.Web;
                var list = web.Lists.GetByTitle(listName);
                context.Load(list, d => d.Title);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }

                var field = context.CastTo<FieldChoice>(list.Fields.GetByInternalNameOrTitle(fieldName));
                context.Load(field, f => f.Choices);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }

                return field.Choices;
            }
        }
        

        public static bool SendEmail( string email, string content, string subject, string siteUrl= null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                EmailProperties properties = new EmailProperties();
                properties.To = new string[] { email };
                properties.Subject = subject;
                properties.Body = content;

                Utility.SendEmail(context, properties);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return true;
        }

        public static bool SendEmails(IEnumerable<string> emails, string content, string subject, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                EmailProperties properties = new EmailProperties();
                properties.To = emails;
                properties.Subject = subject;
                properties.Body = content;

                Utility.SendEmail(context, properties);

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return true;
        }

        public static void DeleteListItem(string listName, int? listItemID, string siteUrl)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);
                ListItem SPListItem = SPList.GetItemById(listItemID + string.Empty);

                SPListItem.DeleteObject();

                try
                {
                    context.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static FieldUserValue GetUser(string useremail, string siteUrl,string strwebname)
        {
          
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                clientContext.Credentials = new SharePointOnlineCredentials(UserName, secureString);
                Web communitySite = clientContext.Site.OpenWeb(strwebname);
                clientContext.Load(communitySite);
                clientContext.ExecuteQuery();

                User newUser = communitySite.EnsureUser("i:0#.f|membership|" + useremail);
                clientContext.Load(newUser);
                clientContext.ExecuteQuery();

                FieldUserValue userValue = new FieldUserValue();
                userValue.LookupId = newUser.Id;
                return userValue;

            }
        }
    }
}

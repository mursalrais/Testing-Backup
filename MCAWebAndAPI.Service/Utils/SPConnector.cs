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
        static string CurUrl = "https://eceos2.sharepoint.com/sites/mca-dev/hr";
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

        internal static int GetRenewalNumber(string listname, int? professionalID, string siteUrl = null, string caml = null)
        {
            string camlViewXml = caml ??  @"<View><ViewFields>
                            < FieldRef Name = 'renewalnumber' />
                        </ViewFields>
                        <OrderBy>
                            <FieldRef Name = 'ID' Ascending = 'FALSE' />
                        </OrderBy>
                        <Where>
                            <Eq>
                                <FieldRef Name = 'professional_x003a_ID'/>
                                <Value Type = 'Lookup'>" + professionalID + @"</Value>
                            </Eq>
                        </Where>
                        <QueryOptions>
                            <RowLimit> 1 </RowLimit>
                        </QueryOptions >
                        </View>";

            /*
            string camlViewXml = caml ?? @"<View>  
            <Query> 
               <OrderBy><FieldRef Name='ID' Ascending='FALSE' /><FieldRef Name='ID' Ascending='FALSE' /></OrderBy> 
            </Query> 
                <ViewFields><FieldRef Name='ID' /></ViewFields> 
            <RowLimit>1</RowLimit> 
            </View>";
            */

            var result = 1;
            var list = GetList(listname, siteUrl, camlViewXml);
            foreach (var item in list)
            {
                result = Convert.ToInt32(item["renewalnumber"]);
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
                context.ExecuteQuery();
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
                    context.ExecuteQuery();

                    // Get list item from File
                    ListItem newItem = fileUploaded.ListItemAllFields;
                    context.Load(newItem);
                    context.ExecuteQuery();

                    // Modify column
                    foreach (var key in columnValues.Keys)
                    {
                        newItem[key] = columnValues[key];
                    }
                    newItem.Update();
                    context.ExecuteQuery();
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
                    context.ExecuteQuery();

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
                context.ExecuteQuery();

                var field = context.CastTo<FieldChoice>(list.Fields.GetByInternalNameOrTitle(fieldName));
                context.Load(field, f => f.Choices);
                context.ExecuteQuery();

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

                context.ExecuteQuery();
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

                context.ExecuteQuery();
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
                context.ExecuteQuery();
            }
        }
    }
}

using System.Linq;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Collections.Generic;
using System;
using Microsoft.SharePoint.Client.Utilities;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.Utils
{
    public class SPConnector
    {
        static string CurUrl = "";
        static string UserName = "";
        static string Password = "";

        public static Task ExecuteQueryAsync(ClientContext clientContext)
        {
            try
            {
                return Task.Factory.StartNew(() =>
                {
                    clientContext.ExecuteQuery();
                });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

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
            foreach (var item in list)
            {
                result = Convert.ToInt32(item["ID"]);
            }

            return result;
        }

        internal static int GetLatestListItemIdbyGuid(string listName, string siteUrl = null, string strGuid = null, string caml = null)
        {
            string camlViewXml = string.Format("<Query>" +
                                               "<Where>" +
                                               "<Eq><FieldRef Name ='TimesheetGUID'/>" +
                                               "<Value Type ='Text'>{0}</Value>" +
                                               "</Eq>" +
                                               "</Where>" +
                                               "<OrderBy>" +
                                               "<FieldRef Name ='ID' Ascending ='False'/>" +
                                               "</OrderBy>" +
                                               "</Query>" +
                                               "<ViewFields>" +
                                               "<FieldRef Name ='ID'/>" +
                                               "</ViewFields><RowLimit>1</RowLimit>", strGuid);


            var result = 1;
            var list = GetList(listName, siteUrl, camlViewXml);
            foreach (var item in list)
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
                        new CamlQuery
                        {
                            ViewXml = caml
                        };


                camlQuery.DatesInUtc = false;

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
            string caml = @"<View><Query><Where>
                        <Eq><FieldRef Name='ID' />
                        <Value Type='Number'>" + listItemID + "</Value></Eq>" +
                        "</Where></Query>" +
                        "<RowLimit>1</RowLimit> </View>";
            //MapCredential(siteUrl);
            //using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            //{
            //    SecureString secureString = new SecureString();
            //    Password.ToList().ForEach(secureString.AppendChar);
            //    context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

            //    // Get one listitem
            //    var SPListItem = context.Web.Lists.GetByTitle(listName).GetItemById((int)listItemID);
            //    context.Load(SPListItem);

            //    try
            //    {
            //        context.ExecuteQuery();
            //    }
            //    catch (Exception e)
            //    {
            //        throw e;
            //    }
            //    return SPListItem;
            //}

            var SPListItem = GetList(listName, siteUrl, caml).FirstOrDefault();
            return SPListItem;
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

        public static void UpdateSingleListItemAsync(string listName, int? listItemID, Dictionary<string, object> updatedValues, string siteUrl = null)
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


                // Set listitem value to parsed listitem
                foreach (var key in updatedValues.Keys)
                {
                    SPListItem[key] = updatedValues[key];
                }

                SPListItem.Update();

                try
                {
                    ExecuteQueryAsync(context);
                }
                catch (Exception e)
                {
                    throw e;
                }


            }
        }

        public static void UpdateMultipleListItemAsync(string listName, Dictionary<string,
            Dictionary<string, object>> updatedValues, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);


                foreach (var key in updatedValues.Keys)
                {
                    if (key.IndexOf(";Edit", StringComparison.Ordinal) <= 0) continue;
                    var id = key.Split(Convert.ToChar(";"))[0];
                    ListItem SPListItem = SPList.GetItemById(id + string.Empty);
                    context.Load(SPListItem);

                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    var detailvalue = updatedValues[key];
                    foreach (var keyvalue in detailvalue.Keys)
                    {
                        SPListItem[keyvalue] = detailvalue[keyvalue];
                    }

                    SPListItem.Update();

                    try
                    {
                        ExecuteQueryAsync(context);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }


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
                //var currentEditor = SPListItem["Editor"];

                // Set listitem value to parsed listitem
                foreach (var key in updatedValues.Keys)
                {
                    SPListItem[key] = updatedValues[key];
                }

                //SPListItem["Editor"] = currentEditor;
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

        public static void AddListItemAsync(string listName, Dictionary<string,
            Dictionary<string, object>> columnValues, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                List spList = context.Web.Lists.GetByTitle(listName);
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();


                foreach (var key in columnValues.Keys)
                {
                    if (key.IndexOf(";Add", StringComparison.Ordinal) <= 0) continue;
                    ListItem newItem = spList.AddItem(itemCreateInfo);
                    var detailvalue = columnValues[key];

                    foreach (var keyvalue in detailvalue.Keys)
                    {
                        newItem[keyvalue] = detailvalue[keyvalue];
                    }

                    try
                    {
                        newItem.Update();
                        ExecuteQueryAsync(context);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

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

        public static bool SendEmail(string email, string content, string subject, string siteUrl = null)
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

        public static void DeleteSingleListItemAsync(string listName, int? listItemID, string siteUrl)
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
                    // context.ExecuteQuery();
                    ExecuteQueryAsync(context);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void DeleteMultipleListItemAsync(string listName, List<string> itemsDelete, string siteUrl)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                // Get one listitem
                List SPList = context.Web.Lists.GetByTitle(listName);

                foreach (var item in itemsDelete)
                {
                    ListItem SPListItem = SPList.GetItemById(item);

                    SPListItem.DeleteObject();

                    try
                    {
                        ExecuteQueryAsync(context);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }


            }
        }


        public static FieldUserValue GetUser(string useremail, string siteUrl, string strwebname)
        {

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                clientContext.Credentials = new SharePointOnlineCredentials(UserName, secureString);
                Web communitySite = clientContext.Site.OpenWeb("hr");
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

        public static void AttachFile(string listName, int listItemID, HttpPostedFileBase file, string siteUrl = null)
        {
            MapCredential(siteUrl);
            ClientContext clientContext = new ClientContext(siteUrl);

            SecureString secureString = new SecureString();
            Password.ToList().ForEach(secureString.AppendChar);
            clientContext.Credentials = new SharePointOnlineCredentials(UserName, secureString);

            List oList = clientContext.Web.Lists.GetByTitle(listName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.GetItemById(listItemID);

            clientContext.ExecuteQuery();

            byte[] contents = new byte[Convert.ToInt32(file.ContentLength)];
            Stream fStream = file.InputStream;
            fStream.Read(contents, 0, Convert.ToInt32(file.ContentLength));
            fStream.Close();
            MemoryStream mStream = new MemoryStream(contents);
            AttachmentCreationInformation aci = new AttachmentCreationInformation();
            aci.ContentStream = mStream;
            aci.FileName = file.FileName;
            Attachment attachment = oListItem.AttachmentFiles.Add(aci);
            clientContext.Load(attachment);
            clientContext.ExecuteQuery();
            clientContext.Dispose();

        }

        public static string GetAttachFileName(string listName, int? ID, string siteUrl = null)
        {
            MapCredential(siteUrl);
            using (ClientContext context = new ClientContext(siteUrl ?? CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                var list = context.Web.Lists.GetByTitle(listName);
                var filename = "";
                context.Load(list);
                context.Load(list.RootFolder);
                context.Load(list.RootFolder.Folders);
                context.Load(list.RootFolder.Files);
                context.ExecuteQuery();
                Folder fol = context.Web.GetFolderByServerRelativeUrl(list.RootFolder.ServerRelativeUrl + "/Attachments/" + ID);
                FileCollection files = fol.Files;
                context.Load(files, fs => fs.Include(f => f.ServerRelativeUrl, f => f.Name, f => f.ServerRelativeUrl));

                try
                {
                    context.ExecuteQuery();
                    foreach (Microsoft.SharePoint.Client.File f in files)
                    {
                        filename = f.Name;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                return filename;
            }
        }
    }
}

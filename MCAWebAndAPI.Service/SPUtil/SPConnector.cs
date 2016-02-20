using System.Linq;
using Microsoft.SharePoint.Client;
using System.Security;

namespace MCAWebAndAPI.Service.SPUtil
{
    public class SPConnector
    {
        static string CurUrl = "https://eceos2.sharepoint.com/sites/mca-dev";
        static string UserName =  "sp.services@eceos.com";
        static string Password = "Raja0432";

        public static ListItemCollection GetListById(string guid)
        {
            using (var context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                Web site = context.Web;
                context.Load(site);
                context.ExecuteQuery();

                List byId = context.Web.Lists.GetById(new System.Guid(guid));
                CamlQuery query = CamlQuery.CreateAllItemsQuery();
                Microsoft.SharePoint.Client.ListItemCollection clientObject = byId.GetItems(query);
                context.Load<Microsoft.SharePoint.Client.ListItemCollection>(clientObject);
                context.ExecuteQuery();
                return clientObject;
            }
        }
        

        public static ListItemCollection GetListByName(string listName, string caml = null)
        {
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);

                var site = context.Web;
                context.Load(site);
                context.ExecuteQuery();

                var byTitle = context.Web.Lists.GetByTitle(listName);
                var camlQuery = caml == null ?
                    CamlQuery.CreateAllItemsQuery() :  
                    new CamlQuery {
                        ViewXml = caml
                    };

                ListItemCollection clientObject = byTitle.GetItems(camlQuery);

                context.Load(clientObject);
                context.ExecuteQuery();
                return clientObject;
            }
        }



    }
}

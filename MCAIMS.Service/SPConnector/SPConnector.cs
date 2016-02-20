using System.Linq;
using Microsoft.SharePoint.Client;
using System.Security;

namespace SPReportGenerator.Service.SPConnector
{
    internal class SPConnector
    {
        static string CurUrl = "https://eceos2.sharepoint.com/sites/mca-dev";
        static string UserName =  "sp.services@eceos.com";
        static string Password = "Raja0432";

        public static ListItemCollection GetList(string listName)
        {
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                Password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(UserName, secureString);
                Site site = context.Site;
                context.Load(site);
                context.ExecuteQuery();

                List byTitle = context.Web.Lists.GetByTitle(listName);
                CamlQuery query = CamlQuery.CreateAllItemsQuery();
                ListItemCollection clientObject = byTitle.GetItems(query);

                context.Load(clientObject);
                context.ExecuteQuery();
                return clientObject;
            }
        }
    }
}

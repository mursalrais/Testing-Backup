using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using System.Security;

namespace MCAIMS
{

    public partial class _Default : Page
    {
        string CurUrl = "";
        int width = 0;
        int height = 0;

        private string userName = "sp.services@eceos.com";
        private string password = "Raja0432";

        private string checkNull(object item)
        {
            try
            {
                return item.ToString();
            }
            catch
            {
                return "";
            }
        }

        private DateTime? checkNullDateTime(object item)
        {
            try
            {
                return new DateTime?(Convert.ToDateTime(item.ToString()));
            }
            catch
            {
                return null;
            }
        }


        Microsoft.SharePoint.Client.ListItemCollection getList(string listName)
        {
            CurUrl = "https://eceos2.sharepoint.com/sites/mca-dev";
            using (ClientContext context = new ClientContext(CurUrl))
            {
                SecureString secureString = new SecureString();
                password.ToList().ForEach(secureString.AppendChar);
                context.Credentials = new SharePointOnlineCredentials(userName, secureString);
                Site site = context.Site;
                context.Load(site);
                context.ExecuteQuery();

                List byTitle = context.Web.Lists.GetByTitle(listName);
                CamlQuery query = CamlQuery.CreateAllItemsQuery();
                Microsoft.SharePoint.Client.ListItemCollection clientObject = byTitle.GetItems(query);
                context.Load<Microsoft.SharePoint.Client.ListItemCollection>(clientObject);
                context.ExecuteQuery();
                return clientObject;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
    }
}

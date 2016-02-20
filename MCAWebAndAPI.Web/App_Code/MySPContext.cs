using System.Configuration;
using System.Linq;
using System.Security;
using Microsoft.SharePoint.Client;


    public static class MySPContext
    {

        public static ClientContext myContext(string strUrl)
        {
            ClientContext context = new ClientContext(strUrl);

            SecureString secureString = new SecureString();
            //ConfigurationManager.AppSettings["mypassword"].ToList().ForEach(secureString.AppendChar);
            //context.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings["myusername"], secureString);

            "Raja0432".ToList().ForEach(secureString.AppendChar);
            context.Credentials = new SharePointOnlineCredentials("sp.services@eceos.com", secureString);
           

            return context;
        }

    }


using MCAWebAndAPI.Web.Helpers;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MCAWebAndAPI.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            // Removing all the view engines
            ViewEngines.Engines.Clear();
            //Add Razor Engine (which we are using)
            ViewEngines.Engines.Add(new CsHtmlRazorViewEngine());
        }
        
    }
}
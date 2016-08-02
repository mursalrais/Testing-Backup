using System.Web.Optimization;
using System.Web.UI;

namespace MCAWebAndAPI.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-2.6.2.js"));

            bundles.Add(new ScriptBundle("~/bundles/bi")
                .Include("~/Scripts/bi-frame.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo")
                .Include(
                 "~/Scripts/kendo/2016.2.714/pake_deflate.min.js",
                 "~/Scripts/kendo/2016.2.714/jszip.min.js",
                 "~/Scripts/kendo/2016.2.714/kendo.all.min.js",
                 "~/Scripts/kendo/2016.2.714/kendo.aspnetmvc.min.js"));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });

            bundles.Add(new StyleBundle("~/Content/theme").Include(
               "~/Content/smartadmin-production-plugins.min.css",
               "~/Content/smartadmin-production.min.css",
               "~/Content/smartadmin-skins.min.css",
               "~/Content/custom.css"));

            bundles.Add(new StyleBundle("~/Content/bi").Include(
                "~/Content/bi.css"));

            bundles.Add(new StyleBundle("~/Content/kendo").Include(
                "~/Content/kendo/2016.2.714/kendo.common-fiori.min.css",
                "~/Content/kendo/2016.2.714/kendo.fiori.min.css",
                "~/Content/kendo/2016.2.714/kendo.dataviz.fiori.min.css"));

            bundles.IgnoreList.Ignore("*.unobtrusive-ajax.min.js", OptimizationMode.WhenEnabled);
            bundles.IgnoreList.Ignore("*.unobtrusive-ajax.min.js", OptimizationMode.WhenDisabled);
        }
    }
}
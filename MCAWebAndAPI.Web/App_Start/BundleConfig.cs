using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;

namespace MCAWebAndAPI.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Order is very important for these files to work, they have explicit dependencies
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/apps")
                .IncludeDirectory("~/Scripts/apps/common", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/libScripts")
                .Include(
                "~/Scripts/libs/jquery.min",
                "~/Scripts/libs/jszip.min.js",
                "~/Scripts/libs/pako_deflate.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bi")
                .Include("~/Scripts/bi-frame.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo")
                .Include("~/Scripts/kendo/2016.2.504/kendo.all.min.js",
                 "~/Scripts/kendo/2016.2.504/kendo.aspnetmvc.min.js",
                 "~/Scripts/kendo/2016.2.504/kendo.culture.id-ID.min.js"));

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
                "~/Content/kendo/2016.2.504/kendo.common-fiori.min.css",
                "~/Content/kendo/2016.2.504/kendo.fiori.min.css",
                "~/Content/kendo/2016.2.504/kendo.dataviz.fiori.min.css"));


        }
    }
}
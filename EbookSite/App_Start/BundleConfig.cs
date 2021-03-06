﻿using System.Web;
using System.Web.Optimization;

namespace EbookSite {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jHtmlArea").Include(
                "~/Scripts/jHtmlArea-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/jHtmlAreaCss").Include(
                "~/Content/jHtmlArea/jHtmlArea.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryunobtrusive").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                "~/Content/themes/base/all.css"));

            bundles.Add(new StyleBundle("~/Content/combo").Include(
                "~/Content/combo.css"));

            bundles.Add(new ScriptBundle("~/bundles/combo").Include(
                "~/Scripts/combo.js"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/Content/DataTables/css/jquery.dataTables.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new StyleBundle("~/Content/tagger").Include(
                "~/Content/jquery.tagger.css"));

            bundles.Add(new ScriptBundle("~/bundles/tagger").Include(
                "~/Scripts/jquery.tagger.js"));
        }
    }
}

using System.Web;
using System.Web.Optimization;

namespace prueba1
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Content/bootstrap.js",
                    "~/Content/assets/scripts/jquery.min.js",
                    "~/Content/assets/scripts/modernizr.min.js",
                    "~/Content/assets/plugin/bootstrap/js/bootstrap.min.js",
                    "~/Content/assets/plugin/mCustomScrollbar/jquery.mCustomScrollbar.concat.min.js",
                    "~/Content/assets/plugin/nprogress/nprogress.js",
                    "~/Content/assets/plugin/sweet-alert/sweetalert.min.js",
                    "~/Content/assets/plugin/waves/waves.min.js",
                    "~/Content/assets/plugin/fullscreen/jquery.fullscreen-min.js",
                    "~/Content/assets/scripts/main.min.js",
                    "~/Content/assets/color-switcher/color-switcher.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.css",
                  "~/Content/assets/styles/style.min.css",
                  "~/Content/assets/plugin/mCustomScrollbar/jquery.mCustomScrollbar.min.css",
                  "~/Content/assets/plugin/waves/waves.min.css",
                  "~/Content/assets/plugin/sweet-alert/sweetalert.css",
                  "~/Content/assets/color-switcher/color-switcher.min.css"));
        }
    }
}

using System.Web;
using System.Web.Optimization;

namespace FairyTales
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*",
                        "~/Scripts/search.js",
                        "~/Scripts/jquery-2.1.4.js",
                        "~/Scripts/jquery-1.11.0.min.js",
                        "~/Scripts/login.js",
                        "~/Scripts/navigation.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/css/css").Include(
                      "~/css/style.css"));
        }
    }
}

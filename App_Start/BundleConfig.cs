using System.Web;
using System.Web.Optimization;

namespace NiceHandles
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/summernote.min.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/select2.min.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/autoNumeric*",
                        "~/Scripts/nicehandle.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/moment.min.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datetimepicker.min.css",
                      "~/Content/datepicker3.css",
                      "~/FontAwesome/css/all.min.css",
                      "~/Content/select2/select2.min.css",
                      "~/Content/summernote/summernote.css",
                      "~/Content/toastr/toastr.min.css",
                      "~/Content/site.css"
                      ));
        }
    }
}

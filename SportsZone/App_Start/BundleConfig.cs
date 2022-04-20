using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace SportsZone
{
    class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
    static class BundleExtentions
    {
        public static Bundle NonOrdering(this Bundle bundle)
        {
            bundle.Orderer = new NonOrderingBundleOrderer();
            return bundle;
        }
    }
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //js bundle
            bundles.Add(new ScriptBundle("~/bundles/js").NonOrdering()
            .Include(
                "~/themes/default/js/vendor/jquery.js",
                "~/themes/default/js/vendor/bootstrap.min.js",
                "~/themes/default/js/gmap3.min.js",
                "~/themes/default/js/bigslide.js",
                "~/themes/default/js/slick.js",
                "~/themes/default/js/waterwheelCarousel.js",
                "~/themes/default/js/contact-form.js",
                "~/themes/default/js/countTo.js",
                "~/themes/default/js/datepicker.js",
                "~/themes/default/js/rating-star.js",
                "~/themes/default/js/range-slider.js",
                "~/themes/default/js/spinner.js",
                "~/themes/default/js/parallax.js",
                "~/themes/default/js/countdown.js",
                "~/themes/default/js/appear.js",
                "~/themes/default/js/prettyPhoto.js",
                "~/themes/default/js/wow-min.js",
                "~/themes/default/js/main.js"
                ));
            // css bundle
            bundles.Add(new StyleBundle("~/bundles/css").NonOrdering()
                .Include(
                "~/themes/default/css/bootstrap/bootstrap.min.css",
                "~/themes/default/css/main.css",
                "~/themes/default/css/icomoon.css",
                "~/themes/default/css/animate.css",
                "~/themes/default/css/transition.css",
                "~/themes/default/css/font-awesome.min.css",
                "~/themes/default/style.css",
                "~/themes/default/css/color.css",
                "~/themes/default/css/responsive.css"
                ));
            // mod js bundle
            bundles.Add(new ScriptBundle("~/bundles/mod").NonOrdering()
                .Include(
                "~/themes/default/js/vendor/modernizr.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/dashboard/js").NonOrdering()
            .Include(
                    "~/themes/ud/js/jquery.min.js",
                    "~/themes/ud/js/bootstrap.bundle.min.js",
                    "~/themes/ud/js/metismenu.min.js",
                    "~/themes/ud/js/jquery.slimscroll.js",
                    "~/themes/ud/js/waves.min.js",
                    "~/themes/ud/plugins/apexchart/apexcharts.min.js",
                    "~/themes/ud/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js",
                    "~/themes/ud/plugins/morris/morris.min.js",
                    "~/themes/ud/plugins/raphael/raphael.min.js",
                    "~/themes/ud/pages/dashboard.init.js",
                    "~/themes/ud/js/app.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/dashboard/css").NonOrdering()
            .Include(
                "~/themes/ud/plugins/bootstrap-datepicker/css/bootstrap-datepicker.min.css",
                "~/themes/ud/plugins/morris/morris.css",
                "~/themes/ud/css/bootstrap.min.css",
                "~/themes/ud/css/metismenu.min.css",
                "~/themes/ud/css/icons.css",
                "~/themes/ud/css/style.css"
                ));
            //BundleTable.EnableOptimizations = true;
        }
    }
}

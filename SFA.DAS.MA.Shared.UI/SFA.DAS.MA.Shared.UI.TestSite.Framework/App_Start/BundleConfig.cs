using System.Web;
using System.Web.Optimization;

namespace SFA.DAS.MA.Shared.UI.TestSite.Framework
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/sfajs").Include(
          "~/dist/javascripts/jquery-1.11.0.min.js",
          "~/dist/javascripts/selection-buttons.js",
          "~/dist/javascripts/showhide-content.js",
          "~/dist/javascripts/stacker.js",
          "~/dist/javascripts/app.js",
          "~/dist/javascripts/cookiebanner.js"));

            bundles.Add(new ScriptBundle("~/bundles/apprentice").Include(
                      "~/dist/javascripts/apprentice/select2.min.js",
                      "~/dist/javascripts/apprentice/dropdown.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/characterLimitation").Include(
                    "~/dist/javascripts/character-limit.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/lengthLimitation").Include(
                    "~/dist/javascripts/length-limit.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/paymentOrder").Include(
                  "~/dist/javascripts/payment-order.js"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalcustom").Include(
                      "~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/dist/javascripts/sfa-modernizr.js"));

            // This is a temporary fix while the asset references in the EmployerCommitments site are sorted out. 05/10/2017
            bundles.Add(new ScriptBundle("~/bundles/lodash").Include(
                        "~/dist/javascripts/lodash.js"));

            bundles.Add(new StyleBundle("~/bundles/screenie6").Include("~/dist/css/screen-ie6.css"));
            bundles.Add(new StyleBundle("~/bundles/screenie7").Include("~/dist/css/screen-ie7.css"));
            bundles.Add(new StyleBundle("~/bundles/screenie8").Include("~/dist/css/screen-ie8.css"));
            bundles.Add(new StyleBundle("~/bundles/screen").Include("~/dist/css/screen.css"));
            bundles.Add(new StyleBundle("~/bundles/site").Include("~/dist/css/site.css"));
        }
    }
}

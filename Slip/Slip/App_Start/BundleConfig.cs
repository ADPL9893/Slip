using System.Web;
using System.Web.Optimization;

namespace Slip
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/bootstrapcss").Include(
                                                                            "~/Content/css/font-awesome.css",
                                                                            "~/Content/css/datepicker.css",
                                                                             "~/Content/css/AG_Style.css",
                                                                          "~/Content/css/jquery.dataTables.min.css",
                                                                           "~/Content/css/main.css",
                                                                            "~/Content/angular/ui-grid.css"

                                                                           ));

            var js = new ScriptBundle("~/Content/jQueryjs").Include(

              );
            bundles.Add(js);
            bundles.Add(new ScriptBundle("~/bundles/bootstrapJS").Include(
                 "~/Content/plugins/Noty/js/noty/packaged/jquery.noty.packaged.min.js",

                                                                          "~/Content/js/bootstrap.min.js",
                                                                          "~/Content/js/bootstrap.js",
                                                                           "~/Content/js/docs.min.js",
                                                                           "~/Content/js/ie10-viewport-bug-workaround.js"

                                                                          ));



            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                                                                         "~/Content/angular/angular.js",
                                                                          "~/Content/angular/angular-touch.js",
                                                                           "~/Content/angular/angular-animate.js",
                                                                           "~/Content/Angular/csv.js",//new
                                                                          "~/Content/Angular/pdfmake.js",
                                                                           "~/Content/Angular/vfs_fonts.js",

                                                                        "~/Content/angular/ui-grid.js",

                                                                        "~/Content/Angular/angularjs-dropdown-multiselect.js",
                                                                        "~/Content/Angular/autoFitColumns.min.js",
                                                                        "~/Content/js/jquery.auto-complete.min.js",
                                                                          "~/Content/js/ag-grid-enterpriseEN.js",
                                                                            "~/Content/Angular/ComonAngularGried.js"



                                                                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapplugin").Include(
                                                                              "~/Content/js/plugins/pace.min.js",
                                                                              "~/Content/js/plugins/bootstrap-datepicker.min.js",
                                                                              "~/Content/js/plugins/jquery.dataTables.min.js",
                                                                              "~/Content/js/plugins/dataTables.bootstrap.min.js",
                                                                              "~/Content/js/plugins/select2.min.js",
                                                                             "~/Content/js/plugins/jquery-ui.custom.min.js",
                                                                             "~/Content/js/plugins/sweetalert.min.js",
                                                                               "~/Content/Comman/CommonJsMessages.js",
                                                                                  "~/Content/js/ang.dblbx.js",
                                                                                "~/Content/js/jquery.dblbx.js"

                                                                             ));

            BundleTable.EnableOptimizations = true;
        }
    }
}

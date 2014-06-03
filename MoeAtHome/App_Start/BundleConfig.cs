using System.Web;
using System.Web.Optimization;

namespace MoeAtHome
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/semantic").Include(
                "~/Scripts/semantic.js",
                "~/Scripts/semantic-dropdown.js",
                "~/Scripts/waypoints.js",
                "~/Scripts/menu.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-route.js"));

            bundles.Add(new ScriptBundle("~/bundles/highlight").Include(
                        "~/Scripts/highlight.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app/userInfoModel.js",
                "~/Scripts/app/app.js",
                "~/Scripts/app/appDataCtrl.js",
                "~/Scripts/app/sidebarCtrl.js",
                "~/Scripts/app/homePageCtrl.js",
                "~/Scripts/app/viewBlogCtrl.js",
                "~/Scripts/app/loginCtrl.js",
                "~/Scripts/app/registerCtrl.js",
                "~/Scripts/app/postBlogCtrl.js"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include(
                "~/Scripts/angular-ui/tinymce.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/semantic.css",
                "~/Content/Site.css",
                "~/Content/highlight/xcode.css"));

            // 将 EnableOptimizations 设为 false 以进行调试。有关详细信息，
            // 请访问 http://go.microsoft.com/fwlink/?LinkId=301862
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}

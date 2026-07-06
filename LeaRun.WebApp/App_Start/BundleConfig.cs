using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace DNS.WebApp.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            //<!--框架必需start-->
            Bundle scriptBundle = new ScriptBundle("~/Content/Scripts/frameworkjquery").Include("~/Content/Scripts/jquery/jquery-1.8.2.min.js", "~/Content/Scripts/learunui-framework.js");
            Bundle styleBundle = new StyleBundle("~/Content/Styles/frameworkcss").Include("~/Content/Styles/learunui-framework.css");
            Bundle styleBundle1 = new StyleBundle("~/Content/Styles/startmenucss").Include("~/Content/Styles/learunui-startmenu.css");
            scriptBundle.Transforms.Add(new BundleTransform());
            styleBundle.Transforms.Add(new BundleTransform());
            styleBundle1.Transforms.Add(new BundleTransform());
            bundles.Add(scriptBundle);
            bundles.Add(styleBundle);
            bundles.Add(styleBundle1);
            //bundles.Add(new ScriptBundle("~/Content/Scripts/frameworkjquery").Include("~/Content/Scripts/jquery/jquery-1.8.2.min.js", "~/Content/Scripts/learunui-framework.js"));
            //bundles.Add(new StyleBundle("~/Content/Styles/frameworkcss").Include("~/Content/Styles/learunui-framework.css"));
            //bundles.Add((new StyleBundle("~/Content/Styles/startmenucss").Include("~/Content/Styles/learunui-startmenu.css")));
            bundles.Add(new StyleBundle("~/Content/Styles/accordioncss").Include("~/Content/Styles/learunui-accordion.css"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/Indexjquery").Include("~/Content/Scripts/Index.js"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/cookie/cookiejquery").Include("~/Content/Scripts/cookie/jquery.cookie.js"));
            bundles.Add(new StyleBundle("~/Content/Styles/accordionTreecss").Include("~/Content/Styles/learunui-accordionTree.css"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/less/lessjquery").Include("~/Content/Scripts/less/less-1.3.3.min.js"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/jquery/md5jquery").Include("~/Content/Scripts/jquery/jQuery.md5.js"));
            bundles.Add(new StyleBundle("~/Content/Styles/logincss").Include("~/Content/Styles/learunui-login.css"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/jqgrid/gridtreegridjquery").Include("~/Content/Scripts/jqgrid/grid.treegrid.js"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/jqprint/jqprintjquery").Include("~/Content/Scripts/jqprint/jquery.jqprint-0.3.js"));
            //<script src="~/Content/Scripts/jqprint/jquery.jqprint-0.3.js"></script>
            //<script src="~/Content/Scripts/jqgrid/grid.treegrid.js"></script>
            //<link href="~/Content/Styles/learunui-framework.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/jquery/jquery-1.8.2.min.js"></script>
            //<script src="~/Content/Scripts/learunui-framework.js"></script>
            //<link href="~/Content/Styles/learunui-startmenu.css" rel="stylesheet" />
            //<link href="~/Content/Styles/learunui-accordion.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/Index.js"></script>
            //<script src="~/Content/Scripts/cookie/jquery.cookie.js"></script>
            //<link href="~/Content/Styles/learunui-accordionTree.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/less/less-1.3.3.min.js"></script>
            //<script src="~/Content/Scripts/jquery/jQuery.md5.js"></script>
            //<link href="~/Content/Styles/learunui-login.css" rel="stylesheet" />
            //<!--框架必需end-->
            //<!--jqgrid表格组件start-->
            bundles.Add(new ScriptBundle("~/Content/Scripts/jqgrid/jqgridjquery").Include("~/Content/Scripts/jqgrid/jquery-ui-custom.min.js", "~/Content/Scripts/jqgrid/grid.locale-cn.js", "~/Content/Scripts/jqgrid/jqGrid.js"));
            bundles.Add(new StyleBundle("~/Content/Scripts/jqgrid/css/jqgridcss").Include("~/Content/Scripts/jqgrid/css/jqgrid.css"));
            //<script src="~/Content/Scripts/jqgrid/jquery-ui-custom.min.js"></script>
            //<script src="~/Content/Scripts/jqgrid/grid.locale-cn.js"></script>
            //<link href="~/Content/Scripts/jqgrid/css/jqgrid.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/jqgrid/jqGrid.js"></script>
            //<!--表格组件end-->
            //<!--树形组件start-->
            bundles.Add(new ScriptBundle("~/Content/Scripts/tree/treejquery").Include("~/Content/Scripts/tree/tree.js"));
            bundles.Add(new StyleBundle("~/Content/Scripts/tree/treecss").Include("~/Content/Scripts/tree/tree.css"));
            //<link href="~/Content/Scripts/tree/tree.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/tree/tree.js"></script>
            //<!--树形组件end-->
            //<!--布局组件start-->
            bundles.Add(new ScriptBundle("~/Content/Scripts/layout/layoutjquery").Include("~/Content/Scripts/layout/splitter.js"));
            //<script src="~/Content/Scripts/layout/splitter.js"></script>
            //<!--布局组件end-->
            //<!--日期组件start-->
            //bundles.Add(new ScriptBundle("~/Content/Scripts/datepicker/datepickerjquery").Include("~/Content/Scripts/datepicker/WdatePicker.js"));
            //<script src="~/Content/Scripts/datepicker/WdatePicker.js"></script>
            //<!--日期组件start-->
            //<!--表单验证组件start-->
            bundles.Add(new ScriptBundle("~/Content/Scripts/validator/validatorjquery").Include("~/Content/Scripts/validator/learunui-validator.js"));
            //<script src="~/Content/Scripts/validator/learunui-validator.js"></script>
            //<!--表单验证组件end-->

            //            <!--表单智能向导组件start-->
            //<link href="~/Content/Scripts/smartwizard/smart_wizard.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/smartwizard/jquery.smartWizard-2.0.min.js" charset="GBK"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/smartwizard/smartwizardjquery").Include("~/Content/Scripts/smartwizard/jquery.smartWizard-2.0.min.js"));
            bundles.Add(new StyleBundle("~/Content/Scripts/smartwizard/smartwizardcss").Include("~/Content/Scripts/smartwizard/smart_wizard.css"));
            //<!--表单智能向导组件end-->

            //            <!--代码高亮start-->
            //<link type="text/css" rel="stylesheet" href="~/Content/Scripts/SyntaxHighlighter/Styles/SyntaxHighlighter.css" />
            //<script src="~/Content/Scripts/SyntaxHighlighter/Scripts/shCore.js" type="text/javascript" charset="GBK"></script>
            //<script src="~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushCpp.js" type="text/javascript"></script>
            //<script src="~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushCSharp.js" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            //<script src="" type="text/javascript"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/SyntaxHighlighter/Scripts/SyntaxHighlighterjquery").Include("~/Content/Scripts/SyntaxHighlighter/Scripts/shCore.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushCpp.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushCSharp.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushCss.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushDelphi.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushJava.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushJScript.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushPhp.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushPython.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushRuby.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushSql.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushVb.js",
                "~/Content/Scripts/SyntaxHighlighter/Scripts/shBrushXml.js"));
            bundles.Add(new StyleBundle("~/Content/Scripts/SyntaxHighlighter/Styles/SyntaxHighlightercss").Include("~/Content/Scripts/SyntaxHighlighter/Styles/SyntaxHighlighter.css"));
            //<!--代码高亮end-->

            //barcode
            //<script src="~/Content/Scripts/barcode/jquery-barcode.min.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/barcode/barcodejquery").Include("~/Content/Scripts/barcode/jquery-barcode.min.js"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/barcode/barcodejquery1").Include("~/Content/Scripts/barcode/jquery-barcode.js"));
            //lodop
            //<script src="~/Content/Scripts/lodop/LodopFuncs.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/lodop/lodopjquery").Include("~/Content/Scripts/lodop/LodopFuncs.js"));


            //<link href="~/Content/Scripts/Workday/kalendar.css" rel="stylesheet" />
            //<script src="~/Content/Scripts/Workday/kalendar.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/Workday/kalendarjquery").Include("~/Content/Scripts/Workday/kalendar.js"));
            bundles.Add(new StyleBundle("~/Content/Scripts/Workday/kalendarcss").Include("~/Content/Scripts/Workday/kalendar.css"));

            bundles.Add(new ScriptBundle("~/Content/Scripts/Workday/kalendarjqueryyyyydd").Include("~/Content/Scripts/Workday/kalendaryyyydd.js"));
            bundles.Add(new ScriptBundle("~/Content/Scripts/Workday/kalendarjqueryyyyy").Include("~/Content/Scripts/Workday/kalendaryyyy.js"));
            //<!--引入弹窗组件start-->
            //<script src="~/Content/Scripts/lhgdialog/lhgdialog.min.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/lhgdialog/lhgdialogjquery").Include("~/Content/Scripts/lhgdialog/lhgdialog.min.js"));
            //<!--引入弹窗组件end-->
            //        <!--自定义滚动条组件start-->
            //<script src="~/Content/Scripts/scrollbar/scrollbar.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/scrollbar/scrollbarjquery").Include("~/Content/Scripts/scrollbar/scrollbar.js"));
            //<!--自定义滚动条组件end-->
            //        <!--第三方统计图start-->
            //<script src="~/Content/Scripts/highcharts/highcharts.js"></script>
            //<script src="~/Content/Scripts/highcharts/highcharts-more.js"></script>
            //<script src="~/Content/Scripts/highcharts/modules/exporting.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/highcharts/highchartsjquery").Include("~/Content/Scripts/highcharts/highcharts.js",
                "~/Content/Scripts/highcharts/highcharts-more.js",
                "~/Content/Scripts/highcharts/modules/exporting.js"));
            //<!--第三方统计图end-->

    //            <link href="~/Content/Scripts/IndexPagejs/css/smartmenu.css" type="text/css" rel="stylesheet" />
    //<link type="text/css" rel="stylesheet" href="~/Content/Scripts/IndexPagejs/css/shCore.css" />
    //<link type="text/css" rel="stylesheet" href="~/Content/Scripts/IndexPagejs/css/shThemeDefault.css" />

    //<script type="text/javascript" src="~/Content/Scripts/IndexPagejs/js/smartmenu.js"></script>
    //<script type="text/javascript" src="~/Content/Scripts/IndexPagejs/js/shCore.js"></script>
    //<script type="text/javascript" src="~/Content/Scripts/IndexPagejs/js/shBrushJScript.js"></script>
    //<script type="text/javascript" src="~/Content/Scripts/IndexPagejs/js/shBrushXml.js"></script>
            bundles.Add(new ScriptBundle("~/Content/Scripts/IndexPagejs/js/smartmenujquery").Include("~/Content/Scripts/IndexPagejs/js/smartmenu.js"
                , "~/Content/Scripts/IndexPagejs/js/smartmenu-min.js", "~/Content/Scripts/IndexPagejs/js/shCore.js"
                , "~/Content/Scripts/IndexPagejs/js/shBrushJScript.js"
                , "~/Content/Scripts/IndexPagejs/js/shBrushXml.js"));
            bundles.Add(new StyleBundle("~/Content/Scripts/IndexPagejs/css/smartmenucss").Include("~/Content/Scripts/IndexPagejs/css/smartmenu.css"
                , "~/Content/Scripts/IndexPagejs/css/shCore.css"
                , "~/Content/Scripts/IndexPagejs/css/shThemeDefault.css"));

            //BundleTable.EnableOptimizations = true;
             
        }

    }

    public class BundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            foreach (var file in response.Files)
            {
                using (var fileStream = File.OpenRead(HostingEnvironment.MapPath(file.IncludedVirtualPath)))
                {

                    /// 此处选择使用MD5计算方式
                    var algorithm = new MD5CryptoServiceProvider();
                    var hashBytes = algorithm.ComputeHash(fileStream);
                    var version = string.Join(null, hashBytes.Select(value => value.ToString("x2")));
                    //var version = HttpServerUtility.UrlTokenEncode(hashBytes);
                    file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, $"?v={version}");
                }
            }
        }
    }
    public class NoCacheTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            context.EnableOptimizations = false;
            context.UseServerCache = false;
            response.ContentType = "text/css";//text/css; charset=utf-8
            response.Cacheability = HttpCacheability.NoCache;
        }
    }
}
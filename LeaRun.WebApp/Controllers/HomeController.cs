using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Controllers
{
    public class HomeController : Controller
    {
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Export(string filename, string type, int width, string svg)
        {
            Exporter export = new Exporter(filename, type, width, svg);

            //// 1、文件名输出
            //FileStream fileWrite = new FileStream(@"d:\HighCharts导出图片.png", FileMode.Create, FileAccess.Write);
            //ExportFileInfo fi = export.WriteToStream(fileWrite);
            //string fileName = "导出的图片." + fi.Extension;
            //fileWrite.Close();
            //fileWrite.Dispose();
            //return File(@"d:\HighCharts导出图片.png", fi.ContentType, fileName);

            // 2、 字节输出
            ExportFileInfo fi;
            byte[] buffer = export.WriteToBuffer(out fi);
            string fileName = "导出的图片." + fi.Extension;
            return File(buffer, fi.ContentType, fileName);
        }

        public Base_ModuleBll base_modulebll = new Base_ModuleBll();
        Base_ModulePermissionBll base_modulepermissionbll = new Base_ModulePermissionBll();
        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Login");
        }
        /// <summary>
        /// 访问模块，写入系统菜单Id
        /// </summary>
        /// <param name="ModuleId">模块id</param>
        /// <param name="ModuleName">模块名称</param>
        /// <returns></returns>
        public ActionResult SetModuleId(string ModuleId, string ModuleName)
        {
            string _ModuleId = DESEncrypt.Encrypt(ModuleId);
            CookieHelper.WriteCookie("ModuleId", _ModuleId);
            if (!string.IsNullOrEmpty(ModuleName))
            {
                Base_SysLogBll.Instance.WriteLog(ModuleId, OperationType.Visit, "1", ModuleName);
            }
            return Content(_ModuleId);
        }
        /// <summary>
        /// 离开tab事件
        /// </summary>
        /// <param name="ModuleId">模块id</param>
        /// <param name="ModuleName">模块名称</param>
        /// <returns></returns>
        public ActionResult SetLeave(string ModuleId, string ModuleName)
        {
            Base_SysLogBll.Instance.WriteLog(ModuleId, OperationType.Leave, "1", ModuleName);
            return Content(ModuleId);
        }

        #region 后台首页-开始菜单
        /// <summary>
        /// 开始菜单UI
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult StartIndex()
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "（" + ManageProvider.Provider.Current().UserName + "）";
            return View();
        }
        /// <summary>
        /// 开始-欢迎首页
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult StartPanel()
        {
            return View();
        }
        /// <summary>
        /// 加载开始菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadStartMenu()
        {
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<Base_Module> list = base_modulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            return Content(list.ToJson().Replace("&nbsp;", ""));
        }
        #endregion

        #region 后台首页-手风琴菜单
        /// <summary>
        /// 手风琴UI
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult AccordionIndex()
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "（" + ManageProvider.Provider.Current().UserName + "）";
            return View();
        }
        /// <summary>
        /// 手风琴-欢迎首页
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult AccordionPage()
        {
            return View();
        }
        /// <summary>
        /// 加载手风琴菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadAccordionMenu()
        {
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<Base_Module> list = base_modulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            return Content(list.ToJson().Replace("&nbsp;", ""));
        }
        #endregion

        #region 后台首页-无限树菜单
        /// <summary>
        /// 无限树UI
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult TreeIndex()
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "（" + ManageProvider.Provider.Current().UserName + "）";
            string isrg = BaseCommon.IsRegistration();
            if (isrg != "Registration")
            {
                ViewBag.isrg = "测试版本截止时间：" + isrg + ",购买请联系：15012791005";
            }
            else
            {
                ViewBag.isrg = "已注册，技术支持/紧急电话：15012791005";
            }
            return View();
        }
        /// <summary>
        /// 无限树-欢迎首页
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult TreePage()
        {
            return View();
        }
        /// <summary>
        /// 加载无限树菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadTreeMenu(string ModuleId)
        {
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<Base_Module> list = base_modulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            List<TreeJsonEntity> TreeList = new List<TreeJsonEntity>();
            foreach (Base_Module item in list)
            {
                TreeJsonEntity tree = new TreeJsonEntity();
                bool hasChildren = false;
                List<Base_Module> childnode = list.FindAll(t => t.ParentId == item.ModuleId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                if (item.Category == "页面")
                {
                    tree.Attribute = "Location";
                    tree.AttributeValue = item.Location;
                }
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                tree.isexpand = false;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson(ModuleId));
        }
        #endregion

        #region 快捷方式设置
        /// <summary>
        /// 快捷方式设置
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Shortcuts()
        {
            return View();
        }
        /// <summary>
        /// 快捷方式 返回菜单模块树JSON
        /// </summary>
        /// <returns></returns>
        public ActionResult ShortcutsModuleTreeJson()
        {
            Base_ShortcutsBll base_shortcutsbll = new Base_ShortcutsBll();
            string UserId = ManageProvider.Provider.Current().UserId;
            List<Base_Module> ShortcutList = base_shortcutsbll.GetShortcutList(UserId);
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<Base_Module> list = base_modulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            List<TreeJsonEntity> TreeList = new List<TreeJsonEntity>();
            foreach (Base_Module item in list)
            {
                TreeJsonEntity tree = new TreeJsonEntity();
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                if (item.Category == "页面")
                {
                    tree.checkstate = ShortcutList.FindAll(t => t.ModuleId == item.ModuleId).Count == 0 ? 0 : 1;
                    //tree.checkstate = item["objectid"].ToString() != "" ? 1 : 0;
                    tree.showcheck = true;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = list.FindAll(t => t.ParentId == item.ModuleId).Count > 0 ? true : false;
                tree.parentId = item.ParentId;
                tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }
        /// <summary>
        /// 快捷方式列表返回JSON
        /// </summary>
        /// <returns></returns>
        public ActionResult ShortcutsListJson()
        {
            Base_ShortcutsBll base_shortcutsbll = new Base_ShortcutsBll();
            string UserId = ManageProvider.Provider.Current().UserId;
            List<Base_Module> ShortcutList = base_shortcutsbll.GetShortcutList(UserId);
            return Content(ShortcutList.ToJson());
        }
        /// <summary>
        /// 快捷方式设置 提交保存
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public ActionResult SubmitShortcuts(string ModuleId)
        {
            try
            {
                Base_ShortcutsBll base_shortcutsbll = new Base_ShortcutsBll();
                string UserId = ManageProvider.Provider.Current().UserId;
                int IsOk = base_shortcutsbll.SubmitForm(ModuleId, UserId);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "设置成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 技术支持
        /// <summary>
        /// 技术支持
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SupportPage()
        {
            return View();
        }
        #endregion

        #region 关于我们
        /// <summary>
        /// 关于我们
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult AboutPage()
        {
            return View();
        }
        #endregion

        #region 个性化皮肤设置
        /// <summary>
        /// 个性化皮肤设置
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SkinIndex()
        {
            return View();
        }
        [LoginAuthorize]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LEDIndex()
        {
            return View();
        }
        
        /// <summary>
        /// 切换主题
        /// </summary>
        /// <param name="UItheme"></param>
        /// <returns></returns>
        public ActionResult SwitchTheme(string UItheme)
        {
            CookieHelper.WriteCookie("UItheme", UItheme, 43200);
            return Content("1");
        }
        #endregion

        /// <summary>
        /// 系统首页统计 
        /// </summary>
        /// <returns></returns>
        public ActionResult TaskMessageChart()
        {
            try
            {
                //报警物料
                int A = DataFactory.Database().FindCountBySql(" SELECT COUNT(1) FROM dbo.BY_Inventory WHERE Weight <=2000 ");
                //进行中的报价单
                int B = DataFactory.Database().FindCountBySql("SELECT COUNT(1) FROM dbo.BY_Quotation WHERE BillStatus IN ( '待审核', '已审核', '已驳回', '已列印', '已回传' )  AND DeleteMark = 0 ");
                //订单
                int C = DataFactory.Database().FindCountBySql("SELECT COUNT(1) FROM dbo.BY_Order WHERE OrderStatus = '正常生产'  and DeleteMark=0 ");
                //订单
                int D = DataFactory.Database().FindCountBySql("SELECT COUNT(1) FROM dbo.BY_Order WHERE OrderStatus = '正常生产' and DeleteMark=0 ");
                //报价单累计
                int E = DataFactory.Database().FindCountBySql(" SELECT COUNT(1) FROM dbo.BY_Order WHERE OrderStatus = '已完工' and DeleteMark=0  ");

                List<WeekChart> by_quotationweekchart = DataFactory.Database().FindListBySql<WeekChart>(@" SELECT TOP 5
        DATEPART(wk, CreateDate) OrderWeek ,
        COUNT(1) cnt
FROM    dbo.BY_Quotation WHERE BillStatus<>'已取消' and DeleteMark = 0 
GROUP BY DATEPART(wk, CreateDate)
ORDER BY OrderWeek DESC");
                List<WeekChart> by_orderweekchart = DataFactory.Database().FindListBySql<WeekChart>(@" SELECT TOP 5
        DATEPART(wk, CreateDate) OrderWeek ,
        COUNT(1) cnt
FROM    dbo.BY_Order WHERE OrderStatus<>'已取消' and DeleteMark=0 
GROUP BY DATEPART(wk, CreateDate)
ORDER BY OrderWeek DESC");
                List<WeekChart> by_deliveryweekchart = DataFactory.Database().FindListBySql<WeekChart>(@" SELECT TOP 5
        DATEPART(wk, CreateDate) OrderWeek ,
        COUNT(1) cnt
FROM    dbo.BY_Delivery where  DeleteMark = 0 
GROUP BY DATEPART(wk, CreateDate)
ORDER BY OrderWeek DESC ");

                int Maxtemp = by_quotationweekchart[0].OrderWeek;
                if (by_orderweekchart[0].OrderWeek > Maxtemp)
                {
                    Maxtemp = by_orderweekchart[0].OrderWeek;
                }
                if (by_deliveryweekchart[0].OrderWeek > Maxtemp)
                {
                    Maxtemp = by_deliveryweekchart[0].OrderWeek;
                }
                int[] by_quotationweekchart2 = new int[5];
                int[] by_orderweekchart2 = new int[5];
                int[] by_deliveryweekchart2 = new int[5];
                string[] names = new string[5];
                Maxtemp = Maxtemp - 4;
                for (int i = 0; i < 5; i++)
                {
                    if (i == 4)
                    {
                        names[i] = "本周";
                    }
                    else
                    {
                        names[i] = Maxtemp + "周";
                    }

                    WeekChart weekchartQ = new WeekChart();
                    weekchartQ.OrderWeek = Maxtemp;
                    weekchartQ.cnt = 0;
                    foreach (WeekChart item in by_quotationweekchart)
                    {
                        if (item.OrderWeek == Maxtemp)
                        {
                            weekchartQ.cnt = item.cnt;
                        }
                    }
                    by_quotationweekchart2[i] = weekchartQ.cnt;
                    WeekChart weekchartO = new WeekChart();
                    weekchartO.OrderWeek = Maxtemp;
                    weekchartO.cnt = 0;
                    foreach (WeekChart item in by_orderweekchart)
                    {
                        if (item.OrderWeek == Maxtemp)
                        {
                            weekchartO.cnt = item.cnt;
                        }
                    }
                    by_orderweekchart2[i] = weekchartO.cnt;
                    WeekChart weekchartD = new WeekChart();
                    weekchartD.OrderWeek = Maxtemp;
                    weekchartD.cnt = 0;
                    foreach (WeekChart item in by_deliveryweekchart)
                    {
                        if (item.OrderWeek == Maxtemp)
                        {
                            weekchartD.cnt = item.cnt;
                        }
                    }
                    by_deliveryweekchart2[i] = weekchartD.cnt;
                    Maxtemp = Maxtemp + 1;
                }
                var JsonData = new
                {
                    A = A,
                    B = B,
                    C = C,
                    D = D,
                    E = E,
                    names = names,
                    weekchartQ = by_quotationweekchart2,
                    weekchartO = by_orderweekchart2,
                    weekchartD = by_deliveryweekchart2,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return Content(null);
                throw;
            }
            
        }
        /// <summary>
        /// 首页曲线图
        /// </summary>
        /// <returns></returns>
        //public DataSet GroupQuery()
        //{
            

//            strSql.Append(@"SELECT    ISNULL(SUM(CASE WHEN OrderWeek = '1' THEN cnt
//                            ELSE 0
//                       END), 0) cnt1 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '2' THEN cnt
//                            ELSE 0
//                       END), 0) cnt2 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '3' THEN cnt
//                            ELSE 0
//                       END), 0) cnt3 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '4' THEN cnt
//                            ELSE 0
//                       END), 0) cnt4 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '5' THEN cnt
//                            ELSE 0
//                       END), 0) cnt5
//  FROM      ( SELECT    DATEPART(wk, CreateDate) - DATEPART(wk,
//                                                            DATEADD(dd,
//                                                              -DAY(CreateDate),
//                                                              CreateDate)) + 1 AS OrderWeek ,
//                        COUNT(1) cnt
//              FROM      dbo.BY_Order
//              WHERE     DATEPART(month, CreateDate) = DATEPART(MONTH,
//                                                              GETDATE())
//              GROUP BY  DATEPART(wk, CreateDate) - DATEPART(wk,
//                                                            DATEADD(dd,
//                                                              -DAY(CreateDate),
//                                                              CreateDate)) + 1
//            ) A
//  --工单按周统计
//  SELECT    ISNULL(SUM(CASE WHEN OrderWeek = '1' THEN cnt
//                            ELSE 0
//                       END), 0) cnt1 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '2' THEN cnt
//                            ELSE 0
//                       END), 0) cnt2 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '3' THEN cnt
//                            ELSE 0
//                       END), 0) cnt3 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '4' THEN cnt
//                            ELSE 0
//                       END), 0) cnt4 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '5' THEN cnt
//                            ELSE 0
//                       END), 0) cnt5
//  FROM      ( SELECT    DATEPART(wk, CreateDate) - DATEPART(wk,
//                                                            DATEADD(dd,
//                                                              -DAY(CreateDate),
//                                                              CreateDate)) + 1 AS OrderWeek ,
//                        COUNT(1) cnt
//              FROM      dbo.BY_Quotation
//              WHERE     DATEPART(month, CreateDate) = DATEPART(MONTH,
//                                                              GETDATE())
//              GROUP BY  DATEPART(wk, CreateDate) - DATEPART(wk,
//                                                            DATEADD(dd,
//                                                              -DAY(CreateDate),
//                                                              CreateDate)) + 1
//            ) A
//  --报价单按周统计
//  SELECT    ISNULL(SUM(CASE WHEN OrderWeek = '1' THEN cnt
//                            ELSE 0
//                       END), 0) cnt1 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '2' THEN cnt
//                            ELSE 0
//                       END), 0) cnt2 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '3' THEN cnt
//                            ELSE 0
//                       END), 0) cnt3 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '4' THEN cnt
//                            ELSE 0
//                       END), 0) cnt4 ,
//            ISNULL(SUM(CASE WHEN OrderWeek = '5' THEN cnt
//                            ELSE 0
//                       END), 0) cnt5
//  FROM      ( SELECT    DATEPART(wk, CreateDate) - DATEPART(wk,
//                                                            DATEADD(dd,
//                                                              -DAY(CreateDate),
//                                                              CreateDate)) + 1 AS OrderWeek ,
//                        COUNT(1) cnt
//              FROM      dbo.BY_Delivery
//              WHERE     DATEPART(month, CreateDate) = DATEPART(MONTH,
//                                                              GETDATE())
//              GROUP BY  DATEPART(wk, CreateDate) - DATEPART(wk,
//                                                            DATEADD(dd,
//                                                              -DAY(CreateDate),
//                                                              CreateDate)) + 1
//            ) A --发运单按周统计");
        //    DataSet ds = DataFactory.Database().FindDataSetBySql(strSql.ToString());
        //    return ds;
        //}
        //#endregion

        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult test()
        //{
        //    return View();
        //}
    }
    public class WeekChart
    {
        public int OrderWeek { get; set; }
        public int cnt { get; set; }
    }
}

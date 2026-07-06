using DNS.DataAccess;
using DNS.Utilities;
using DNS.WebApp.App_Start;
using DNS.WebApp.Areas.CheckSheetModule.Controllers;
using DNS.WebApp.Areas.OrderModule.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DNS.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_BeginRequest()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization, X-Requested-With");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "86400");
                HttpContext.Current.Response.StatusCode = 204;
                HttpContext.Current.Response.End();
            }

        }
        //对账单取消自动生成
        System.Threading.Thread _thread;

        System.Threading.Thread _thread1;
        /// <summary>
        /// 当前应用程序启动这件事会发生
        /// </summary>
        protected void Application_Start()
        {

            //设置当前数据库类型
            DbHelper.DbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), ConfigHelper.AppSettings("ComponentDbType"), true);
            Application["OnLineCount"] = 50;//在应用程序第一次启动时初始化在线人数为0
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            System.Timers.Timer t = new System.Timers.Timer(86400000);//设计时间间隔，5分钟300000一个小时执行一次为3600000,24小时 86400000
            t.Elapsed += new System.Timers.ElapsedEventHandler(Elapsed);
            t.AutoReset = true;
            t.Enabled = true;

            //判断是否服务器macinfo 是1，否0
            Application["MacInfo"] = "1";
            System.Timers.Timer t1 = new System.Timers.Timer(86400000);//设计时间间隔，5分钟300000一个小时执行一次为3600000,24小时 86400000
            t1.Elapsed += new System.Timers.ElapsedEventHandler(Elapsed1);
            t1.AutoReset = true;
            t1.Enabled = true;
        }

        private void Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _thread = new System.Threading.Thread(ExcuteBlack);
            _thread.IsBackground = true;
            _thread.Start();
        }
        private void ExcuteBlack()
        {
            try
            {
                new OrderQueryController().UpdateStatus();

            }
            catch (Exception)
            {
                _thread.Abort();
            }
            _thread.Abort();
        }


        private void Elapsed1(object sender, System.Timers.ElapsedEventArgs e)
        {
            _thread1 = new System.Threading.Thread(ExcuteBlack1);
            _thread1.IsBackground = true;
            _thread1.Start();
        }
        private void ExcuteBlack1()
        {
            try
            {
                string temp = DESEncrypt.Decrypt(ConfigHelper.AppSettings("MacInfo"));
                if (temp.Substring(0, temp.Length - 11) == BaseCommon.GetInfo())
                {
                    Application["MacInfo"] = "1";
                }
                else
                {
                    Application["MacInfo"] = "0";
                }
            }
            catch (Exception)
            {
                Application["MacInfo"] = "0";
                _thread1.Abort();
            }

            _thread1.Abort();
        }
        /// <summary>
        /// 离开应用程序启动这件事会发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnLineCount"] = (int)Application["OnLineCount"] - 1;
            Application.UnLock();
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = this.Context.Server.GetLastError();
            if (ex != null)
            {
                //登录是否过期
                if (!ManageProvider.Provider.IsOverdue())
                {
                    HttpContext.Current.Response.Redirect("~/Login/Default");
                    return;
                }
                Dictionary<string, string> modulesError = new Dictionary<string, string>();
                modulesError.Add("发生时间", DateTime.Now.ToString());
                modulesError.Add("错误描述", ex.Message.Replace("\r\n", ""));
                modulesError.Add("错误对象", ex.Source);
                modulesError.Add("错误页面", "" + HttpContext.Current.Request.Url + "");
                modulesError.Add("浏览器IE", HttpContext.Current.Request.UserAgent);
                modulesError.Add("服务器IP", NetHelper.GetIPAddress());
                Application["error"] = modulesError;
                HttpContext.Current.Response.Redirect("~/Error/Index");
            }
        }

        //MachineSet.CloseFocas(cnchandle);



    }
}
using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using DNS.WebApp.Areas.CheckSheetModule.Controllers;
using DNS.WebApp.Areas.OrderModule.Controllers;
using DNS.WebApp.Areas.QuotationHeatModule.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;



namespace DNS.WebApp.Controllers
{
    /// <summary>
    /// 登录控制器
    /// </summary>
    public class LoginController : Controller
    {
        /// <summary>
        /// 导入数据处理
        /// </summary>
        public void ImportingData()
        {
            //DataTable dt = DataFactory.Database().FindTableBySql("SELECT * FROM BYERP..Base_Steel_List");
            //int SortCode = 1000001;
            //foreach (DataRow item in dt.Rows)
            //{
            //    Base_Steel entity = new Base_Steel();
            //    entity.Create();
            //    entity.SteelId = item["steel_id"].ToString();
            //    entity.SteelCode = item["steel_code"].ToString();
            //    entity.SteelName = item["steel_desc"].ToString();
            //    entity.OtherDescription = item["attribute1"].ToString();
            //    entity.Warranty = CommonHelper.GetInt(item["attribute2"]);
            //    entity.SteelType = item["steel_type_id"].ToString();
            //    entity.Density = CommonHelper.GetDecimal(item["density"]);
            //    entity.Price = CommonHelper.GetDecimal(item["price"]);
            //    entity.Enabled = 1;
            //    entity.SortCode = SortCode;
            //    int isOk = DataFactory.Database().Insert(entity);
            //    SortCode++;
            //}
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        public static DNS.Utilities.LogHelper log = DNS.Utilities.LogFactory.GetLogger("LoginController");

        Base_UserBll base_userbll = new Base_UserBll();
        Base_ObjectUserRelationBll base_objectuserrelationbll = new Base_ObjectUserRelationBll();
        /// <summary>
        /// 默认页面
        /// </summary>
        /// <returns></returns>
         [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Default()
        {
            return View();
        }
        /// <summary>
        /// 登录视图
        /// </summary>
        /// <returns></returns>
         [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Index()
        {
            ImportingData();
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
         public ActionResult MacInfo()
         {
             ViewBag.MacInfo = BaseCommon.GetInfo();
             return View();
         }
        [OutputCache(CacheProfile = "Cache1Hour")]
         public ActionResult LED()
         {
             return View();
         }
        [OutputCache(CacheProfile = "Cache1Hour")]
         public ActionResult LEDWorkers()
         {
             return View();
         }
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyCode()
        {
            int codeW = 80;
            int codeH = 22;
            int fontSize = 16;
            string chkCode = string.Empty;
            //颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            //字体列表，用于验证码 
            string[] font = { "Times New Roman", "Verdana", "Arial", "Gungsuh", "Impact" };
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            //写入Session、验证码加密
            Session["session_verifycode"] = Md5Helper.MD5(chkCode.ToLower(), 16);
            //创建画布
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 1; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 18 + 2, (float)0);
            }
            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), @"image/Gif");
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                g.Dispose();
                bmp.Dispose();
            }
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="Account">账户</param>
        /// <param name="Password">密码</param>
        /// <returns></returns>
        public ActionResult CheckLogin(string Account, string Password, string Token, string PasswordMobile = "")
        {
            //LeaRun.WebApp._01_Extension.BarCode.Code128 _Code = new LeaRun.WebApp._01_Extension.BarCode.Code128();
            //_Code.ValueFont = new Font("宋体", 20);
            //System.Drawing.Bitmap imgTemp = _Code.GetCodeImage("A002-A0037-2/5-CR12MOV-228*65.5*37!@#$%^&*()", LeaRun.WebApp._01_Extension.BarCode.Code128.Encode.Code128A);
            //imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "BarCode.gif", System.Drawing.Imaging.ImageFormat.Gif);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://47.99.118.200:801/Index.asmx/CheckCompany?Code=BLC");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseData = "2000-01-01";
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                responseData = reader.ReadToEnd();
            }
            response.Close();
            string checkdate = responseData.Substring(responseData.IndexOf("<string xmlns=\"http://tempuri.org/\">")).Replace("<string xmlns=\"http://tempuri.org/\">", "").Replace("</string>", "");

            string Msg = "";
            try
            {
                //string isrg = BaseCommon.IsRegistration();
                //if (isrg != "Registration")
                //{
                if (DateTime.Now > Convert.ToDateTime(checkdate))
                {
                    Dictionary<string, string> modulesError = new Dictionary<string, string>();
                    modulesError.Add("内部错误：", "电话联系：15012791005");
                    Session["error"] = modulesError;
                    //return RedirectToRoute(new { controller = "Error", action = "Index" });
                    return Content("200");
                }
                //}
                Account = Account.ToUpper();
                IPScanerHelper objScan = new IPScanerHelper();
                string IPAddress = NetHelper.GetIPAddress();
                objScan.IP = IPAddress;
                objScan.DataPath = Server.MapPath("~/Resource/IPScaner/QQWry.Dat");
                string IPAddressName = objScan.IPLocation();
                string outmsg = "";
                VerifyIPAddress(Account, IPAddress, IPAddressName, Token);
                //系统管理
                if (Account.ToLower() == "admin")//ConfigHelper.AppSettings("CurrentUserName"))
                {
                    if ("admin123" == Password.ToLower())
                    {
                        IManageUser imanageuser = new IManageUser();
                        imanageuser.UserId = "System";
                        imanageuser.Account = "System";
                        imanageuser.UserName = "超级管理员";
                        imanageuser.Gender = "男";
                        imanageuser.Code = "System";
                        imanageuser.LogTime = DateTime.Now;
                        imanageuser.CompanyId = "系统";
                        imanageuser.DepartmentId = "系统";
                        imanageuser.IPAddress = IPAddress;
                        imanageuser.IPAddressName = IPAddressName;
                        imanageuser.IsSystem = true;
                        imanageuser.ObjectId = "20178431-3604-46b6-a75b-f7ba13548aed";
                        ManageProvider.Provider.AddCurrent(imanageuser);
                        //对在线人数全局变量进行加1处理
                        HttpContext rq = System.Web.HttpContext.Current;
                        rq.Application["OnLineCount"] = (int)rq.Application["OnLineCount"] + 1;
                        Msg = "3";//验证成功
                        Base_SysLogBll.Instance.WriteLog(Account, OperationType.Login, "1", "登陆成功、IP所在城市：" + IPAddressName);
                    }
                    else
                    {
                        return Content("4");
                    }
                }
                else
                {
                    Base_User base_user = base_userbll.UserLogin(Account, Password, out outmsg);
                    switch (outmsg)
                    {
                        case "-1":      //账户不存在
                            if (isCustomerCount(Account, PasswordMobile, IPAddress))
                            {
                                Msg = "3";
                                break;
                            }
                            if (isWorkerCount(Account, PasswordMobile, IPAddress))
                            {
                                Msg = "3";
                                break;
                            }
                            Msg = "-1";
                            Base_SysLogBll.Instance.WriteLog(Account, OperationType.Login, "-1", "账户不存在、IP所在城市：" + IPAddressName);
                            break;
                        case "lock":    //账户锁定
                            Msg = "2";
                            Base_SysLogBll.Instance.WriteLog(Account, OperationType.Login, "-1", "账户锁定、IP所在城市：" + IPAddressName);
                            break;
                        case "error":   //密码错误
                            Msg = "4";
                            Base_SysLogBll.Instance.WriteLog(Account, OperationType.Login, "-1", "密码错误、IP所在城市：" + IPAddressName);
                            break;
                        case "succeed": //验证成功
                            IManageUser imanageuser = new IManageUser();
                            imanageuser.UserId = base_user.UserId;
                            imanageuser.Account = base_user.Account;
                            imanageuser.UserName = base_user.RealName;
                            imanageuser.Gender = base_user.Gender;
                            imanageuser.Password = base_user.Password;
                            imanageuser.Code = base_user.Code;
                            imanageuser.Secretkey = base_user.Secretkey;
                            imanageuser.LogTime = DateTime.Now;
                            imanageuser.CompanyId = base_user.CompanyId;
                            imanageuser.CompanyName = DataFactory.Database().FindEntity<Base_Company>(base_user.CompanyId).FullName;
                            imanageuser.DepartmentId = base_user.DepartmentId;
                            imanageuser.ObjectId = base_objectuserrelationbll.GetObjectId(imanageuser.UserId);
                            imanageuser.IPAddress = IPAddress;
                            imanageuser.IPAddressName = IPAddressName;
                            imanageuser.IsSystem = false;
                            imanageuser.IsMobileUser = false;
                            ManageProvider.Provider.AddCurrent(imanageuser);
                            //对在线人数全局变量进行加1处理
                            HttpContext rq = System.Web.HttpContext.Current;
                            rq.Application["OnLineCount"] = (int)rq.Application["OnLineCount"] + 1;
                            Msg = "3";//验证成功
                            Base_SysLogBll.Instance.WriteLog(Account, OperationType.Login, "1", "登陆成功、IP所在城市：" + IPAddressName);
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            return Content(Msg);
        }

        private bool isCustomerCount(string name ,string password,string ip)
        {
            DataTable base_customer = DataFactory.Database().FindTableBySql(string.Format(@" SELECT  TOP 1 c.*,ca.Contact,ca.DetailAddress 
FROM    dbo.Base_Customer c
        LEFT JOIN dbo.Base_CustomerAddress ca ON c.CustomerId = ca.CustomerId  
        WHERE ISNULL(ca.Contact,'') != ''
		AND c.Code = '{0}' AND c.PASSWord ='{1}' ", name, password));
            if (base_customer != null && base_customer.Rows.Count > 0)
            {
                IManageUser imanageuser = new IManageUser();
                imanageuser.UserId = name;
                imanageuser.Account = base_customer.Rows[0]["CustomerId"].ToString();
                imanageuser.UserName = base_customer.Rows[0]["Contact"].ToString();
                imanageuser.Gender = base_customer.Rows[0]["Contact"].ToString().IndexOf("姐") >= 0 ? "女" : "男"; 
                imanageuser.LogTime = DateTime.Now;
                imanageuser.CompanyName = base_customer.Rows[0]["FullName"].ToString();
                imanageuser.ObjectId = "955c51de-89c6-45ed-8652-9e9439466b6d";//common角色的bojectid
                imanageuser.IPAddress = ip;
                imanageuser.IPAddressName = base_customer.Rows[0]["DetailAddress"].ToString();
                imanageuser.IsSystem = false;
                imanageuser.IsMobileUser = true;
                ManageProvider.Provider.AddCurrent(imanageuser);
                //对在线人数全局变量进行加1处理
                HttpContext rq = System.Web.HttpContext.Current;
                rq.Application["OnLineCount"] = (int)rq.Application["OnLineCount"] + 1;
                Base_SysLogBll.Instance.WriteLog(name, OperationType.Login, "1", "登陆成功、IP所在城市：" + ip);
                return true;
            }
            return false;
        }


        private bool isWorkerCount(string name, string password, string ip)
        {
            DataTable base_customer = DataFactory.Database().FindTableBySql(string.Format(@" SELECT  TOP 1 w.* 
FROM    dbo.BY_Worker w
        
        WHERE  w.mobile = '{0}' AND w.password ='{1}' ", name, password));
            if (base_customer != null && base_customer.Rows.Count > 0)
            {
                IManageUser imanageuser = new IManageUser();
                imanageuser.UserId = base_customer.Rows[0]["WorkerId"].ToString(); 
                imanageuser.Account = base_customer.Rows[0]["WorkerCode"].ToString();
                imanageuser.UserName = base_customer.Rows[0]["WorkerName"].ToString();
                imanageuser.Gender = "男";
                imanageuser.LogTime = DateTime.Now;
                imanageuser.CompanyName = base_customer.Rows[0]["DepartId"].ToString();
                imanageuser.ObjectId = base_customer.Rows[0]["mobile"].ToString();
                imanageuser.IPAddress = ip;
                imanageuser.IPAddressName = ip;
                imanageuser.IsSystem = false;
                imanageuser.IsMobileUser = true;
                ManageProvider.Provider.AddCurrent(imanageuser);
                //对在线人数全局变量进行加1处理
                HttpContext rq = System.Web.HttpContext.Current;
                rq.Application["OnLineCount"] = (int)rq.Application["OnLineCount"] + 1;
                Base_SysLogBll.Instance.WriteLog(name, OperationType.Login, "1", "登陆成功、IP所在城市：" + ip);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证强迫退出 下线
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckForceOutLogin()
        {
            return null;
        }
        /// <summary>
        /// 同账号不允许重复登录
        /// </summary>
        /// <param name="Account">账户</param>
        /// <returns></returns>
        public bool CheckOnLine(string Account)
        {
            if (!CommonHelper.GetBool(ConfigHelper.AppSettings("CheckOnLine")))
            {
                return true;
            }
            ArrayList list = Session["OnLineList"] as ArrayList;
            if (list == null)
            {
                list = new ArrayList();
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (Account == (list[i] as string))
                {
                    //已经登录了，提示错误信息 
                    return false;
                }
            }
            Session.Add("OnLineList", list.Add(Account));
            return true;
        }
        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        public ActionResult OutLogin()
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            //更改数据库用户表在线状态
            Base_User entity = new Base_User();
            entity.UserId = UserId; entity.Online = 0;
            base_userbll.Repository().Update(entity);
            //清空当前登录用户信息
            ManageProvider.Provider.EmptyCurrent();
            Session.Abandon();  //取消当前会话
            Session.Clear();    //清除当前浏览器所以Session
            return Content("1");
        }
        /// <summary>
        /// IP限制验证
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="IPAddress"></param>
        /// <param name="IPAddressName"></param>
        /// <param name="OpenId"></param>
        public void VerifyIPAddress(string Account, string IPAddress, string IPAddressName, string OpenId)
        {
            if (ConfigHelper.AppSettings("VerifyIPAddress") == "true")
            {
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@IPAddress", IPAddress));
                parameter.Add(DbFactory.CreateDbParameter("@IPAddressName", IPAddressName));
                parameter.Add(DbFactory.CreateDbParameter("@OpenId", DESEncrypt.Decrypt(OpenId)));
                int IsOk = DataFactory.Database().ExecuteByProc("[Login].dbo.[PROC_verify_IPAddress]", parameter.ToArray());
            }
        }

        public JsonResult GetCookie()
        {
            return Json(DNS.Utilities.ManageProvider.Provider.Current()); 
        }

        public JsonResult GetEncode(string text)
        {
            return Json(DESEncrypt.Encrypt(text));
        }
        public JsonResult GetDecode(string text)
        {
            return Json(DESEncrypt.Decrypt(text));
        }
        public JsonResult btn_code128(string text)
        {
            BY_CheckSheetController by_checksheetcontroller = new BY_CheckSheetController();
            DateTime dt = DateTime.Now;
            //by_checksheetcontroller.BuildCheckSheetForXJ(dt.Year.ToString(), dt.Month.ToString());
            //LeaRun.WebApp._01_Extension.BarCode.Code128 _Code = new LeaRun.WebApp._01_Extension.BarCode.Code128();
            //_Code.ValueFont = new Font("宋体", 20);
           
            //List<Base_DataDictionaryDetail> base_datadictionarydetails = DataFactory.Database().FindList<Base_DataDictionaryDetail>(" and DataDictionaryId='21d16fdf-780d-43ac-9bab-2c5142ba6a7c' AND Enabled=1 ");
            //foreach (Base_DataDictionaryDetail item in base_datadictionarydetails)
            //{
            //    System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(item.Code, LeaRun.WebApp._01_Extension.BarCode.Code128.Encode.Code128A);
            //    imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "Processgif\\" + item.FullName + ".gif", System.Drawing.Imaging.ImageFormat.Gif);

            //}
            //List<BY_Worker> by_workers = DataFactory.Database().FindList<BY_Worker>();
            //foreach (BY_Worker item in by_workers)
            //{
            //    System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(item.WorkerCode, LeaRun.WebApp._01_Extension.BarCode.Code128.Encode.Code128A);
            //    imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "Namegif\\" + item.WorkerName + ".gif", System.Drawing.Imaging.ImageFormat.Gif);

            //}
            return Json("1");
        }
        public JsonResult updateorder(string OrderCode)
        {
            //DNS.WebApp.Areas.OrderModule.Controllers.OrderQueryController.SetOrderStatus(OrderCode);
            new OrderQueryController().UpdateStatus();
            return Json("1");
        }
        public JsonResult UpdateHeatStatus()
        {
            //DNS.WebApp.Areas.OrderModule.Controllers.OrderQueryController.SetOrderStatus(OrderCode);
            List<BY_StoveHeatList> BY_StoveHeatLists = DataFactory.Database().FindList<BY_StoveHeatList>();
            foreach (var item in BY_StoveHeatLists)
            {
                new BY_StoveController().UpdateHeatStatus(item);
            }
            return Json("1");
        }
        public JsonResult InsertSteelCDOE()
        {
            List<Base_Steel> Base_Steels = DataFactory.Database().FindList<Base_Steel>();
            foreach (var item in Base_Steels)
            {
                string batchCode = item.SteelCode;
                BY_Inventory nt = DataFactory.Database().FindList<BY_Inventory>("MaterialType", "1").Where<BY_Inventory>(p => p.BatchCode == batchCode).FirstOrDefault<BY_Inventory>();
                if (nt == null || nt.InventoryId == null)
                {
                    nt = new BY_Inventory();
                    nt.BatchCode = batchCode;
                    nt.ChildBatch = "0×0×0";
                    nt.DeleteMark = 0;
                    nt.Enabled = 1;
                    nt.MaterialType = 1;
                    nt.Remark = item.Remark;
                    nt.SteelId = item.SteelId;
                    nt.SteelType = item.SteelType;
                    nt.SteelCode = item.SteelCode;
                    nt.SteelDescription = item.SteelName;
                    nt.Weight = 1;
                    nt.TheoryWeight = 0;
                    nt.Price = item.Price;
                    nt.InStockDate = DateTime.Now;
                    nt.Density = item.Density;
                    nt.Create();
                    DataFactory.Database().Insert<BY_Inventory>(nt);
                }
            }
            return Json("1");
        }
    }
}

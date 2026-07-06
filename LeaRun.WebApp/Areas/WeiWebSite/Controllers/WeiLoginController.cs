using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.WeiWebSite.Controllers
{
    public class WeiLoginController : Controller
    {
        //
        // GET: /WeiWebSite/Login/

        #region 登录页面
        public ActionResult WeiLogin()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user != null)
            {
                return RedirectToAction("UserCenter", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region 首页
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult WeiIndex()
        {
            return View();
        }

        private bool isAdmin()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user != null)
            {
                if (string.IsNullOrEmpty(ManageProvider.Provider.Current().ObjectId) || ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取这个客户正在生产的订单数
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrdersIng()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            int count = 0;
            if (user != null)
            {
                string sql = @" SELECT   ISNULL(COUNT(1),0)
   FROM     dbo.BY_Quotation q
            LEFT JOIN dbo.BY_Order o ON q.QuotationId = o.QuotationId
   WHERE (ISNULL(o.OrderCode,'') ='' OR o.OrderStatus ='正常生产' OR o.OrderStatus ='已暂停'
        OR o.OrderStatus ='部分暂停' OR o.OrderStatus ='部分完工' OR o.OrderStatus ='部分发运')  AND q.DeleteMark = 0  and o.DeleteMark=0  ";
                if (!isAdmin())
                {
                    sql = sql + string.Format(" AND q.CustomerCode = '{0}'", ManageProvider.Provider.Current().UserId);
                }
                count = DataFactory.Database().FindCountBySql(sql);
            }
            return Content(new JsonMessage { Success = false, Code = "1", Message = count.ToString() }.ToString());
        }
        #endregion
        #region 仓库
        public ActionResult InstockIndex()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }

        public ActionResult InstockIndexWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region //下单
        public ActionResult QuotFrom()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QuoDetailCreate()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QuotCreate(string OrderType, int CustMaterial, int FreeBill, int IsEmergency, string QuotationId)
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            BY_Quotation by_quotation = new BY_Quotation();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                
                
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
                    by_quotation.Modify(QuotationId);
                    by_quotation.CustMaterial = CustMaterial;
                    by_quotation.FreeBill = FreeBill;
                    by_quotation.IsEmergency = IsEmergency;
                    by_quotation.IsMobileInput = 1;
                    DataFactory.Database().Update(by_quotation, isOpenTrans);
                }
                else
                {
                    Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                    Base_Customer base_customer = DataFactory.Database().FindEntityByWhere<Base_Customer>(" and Code = '" + ManageProvider.Provider.Current().UserId + "'");
                    Base_CustomerAddress base_customeraddress = new Base_CustomerAddress();
                    if (base_customer != null && base_customer.CustomerId != null)
                    {
                        CommonHelper.CopySameFieldsObject(base_customer, ref by_quotation);
                        base_customeraddress = DataFactory.Database().FindEntityByWhere<Base_CustomerAddress>(" and CustomerId = '" + base_customer.CustomerId + "'");
                        if (base_customeraddress != null && base_customeraddress.CustomerAddressId != null)
                        {
                            CommonHelper.CopySameFieldsObject(base_customeraddress, ref by_quotation);
                        }
                    }
                    else
                    {
                        return Content("-1");
                    }
                    by_quotation.Create();
                    by_quotation.CustomerCode = base_customer.Code;
                    by_quotation.CustomerName = base_customer.FullName;
                    by_quotation.ShortName = base_customer.ShortName;
                    by_quotation.ContactName = base_customeraddress.Contact;
                    by_quotation.ContactTel = base_customeraddress.Telephone;
                    by_quotation.ContactFax = base_customeraddress.Fax;
                    by_quotation.ContactMail = base_customeraddress.EmailAddress;
                    by_quotation.MobileCode = base_customeraddress.Mobile;
                    by_quotation.CustomerAddress = base_customeraddress.DetailAddress;
                    by_quotation.PaymentMode = base_customer.PaymentMode;
                    by_quotation.ShipMode = base_customer.TransitMode;
                    by_quotation.ShipGroup = "北仑机械锻造厂";
                    by_quotation.SalesMan = base_customer.Sellpeople;
                    by_quotation.SalesArea = base_customer.Sellarea;
                    by_quotation.ShipRoute = base_customeraddress.SendPath;
                    by_quotation.ShipAddress = base_customeraddress.AddressCode + " " + base_customeraddress.DetailAddress;
                    by_quotation.Enabled = 1;
                    by_quotation.ModifyDate = null;
                    by_quotation.ModifyUserId = " ";
                    by_quotation.ModifyUserName = " ";
                    by_quotation.CustMaterial = CustMaterial;
                    by_quotation.FreeBill = FreeBill;
                    by_quotation.IsEmergency = IsEmergency;
                    by_quotation.IsMobileInput = 1;
                    by_quotation.OrderType = OrderType;
                    by_quotation.LessRate = base_customer.DiscountRate == null ? 0 : base_customer.DiscountRate;
                    string ModuleId = "";
                    if (by_quotation.OrderType == "BC")
                    {
                        ModuleId = "BYC";
                        by_quotation.QuotationCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, ModuleId);
                    }
                    else if (by_quotation.OrderType == "BJ")
                    {
                        ModuleId = "BYJ";
                        by_quotation.QuotationCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, ModuleId);
                    }
                    DataFactory.Database().Insert(by_quotation, isOpenTrans);
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                    QuotationId = by_quotation.QuotationId;
                }
                database.Commit();
                var JsonData = new
                {
                    QuotationId = QuotationId
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content("-1");
            }
        }
        public ActionResult InputQuot()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult SetQuotDetail(string QuotationId)
        {
            try
            {
                BY_Quotation by_quotation = DataFactory.Database().FindEntityByWhere<BY_Quotation>(" and QuotationId = '" + QuotationId.Trim() + "'");
                //BY_Quotation by_quotation = DataFactory.Database().FindEntityByWhere<BY_Quotation>(" and CreateUserId = '" + ManageProvider.Provider.Current().UserId + "' and isMobileInput = 1");
                IList<BY_QuotationDetail> by_quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>(" and QuotationId = '" + QuotationId.Trim() + "'");
                if (by_quotation == null || by_quotation.QuotationId == null)
                {
                    return Content("-1");
                }
                var JsonData = new
                {
                    by_quotation = by_quotation,
                    by_quotationdetails = by_quotationdetails
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content("-1");
            }
        }
        public ActionResult SetQuot(string QuotationId)
        {
            try
            {
                BY_Quotation by_quotation = DataFactory.Database().FindEntityByWhere<BY_Quotation>(" and QuotationId = '" + QuotationId.Trim() + "'  AND DeleteMark = 0 ");
                if (by_quotation == null || by_quotation.QuotationId == null)
                {
                    return Content("-1");
                }
                return Content(by_quotation.ToJson());
            }
            catch (Exception ex)
            {
                return Content("-1");
            }
        }
        #endregion
        #region //查询报价单
        public ActionResult QueryQuot()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryQuotWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region //工单查询
        public ActionResult QueryOrder()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryOrderWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryOrderDetail()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion

        #region  //发运单查询
        public ActionResult QueryDelivery()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryDeliveryWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryDeliveryDetail()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region  //对账单查询
        public ActionResult QueryCheckSheet()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryCheckSheetWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryCheckSheetDetail()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region  //生产计划查询
        public ActionResult QueryPlan()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }

        #endregion
        #region //报表查询
        public ActionResult QueryReport()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryReportWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region //客户查询
        public ActionResult QueryCustomers()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        public ActionResult QueryCustomersWhere()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region //客户中心
        public ActionResult UserCenter()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            if (string.IsNullOrEmpty(ManageProvider.Provider.Current().ObjectId) || ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") >= 0)
            { ViewBag.isVip = "管理员"; }
            else
            {
                ViewBag.isVip = "普通用户";
            }
            return View();
        }
        public ActionResult Proinfo()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }

        public ActionResult Exit()
        {
            //清空当前登录用户信息
            ManageProvider.Provider.EmptyCurrent();
            Session.Abandon();  //取消当前会话
            Session.Clear();    //清除当前浏览器所以Session
            return RedirectToAction("WeiLogin", "WeiLogin");
        }

        public ActionResult Password()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }

            return View();
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerEntity()
        {
            try
            {
                Base_Customer base_customer = DataFactory.Database().FindEntityByWhere<Base_Customer>(" and Code = '" + ManageProvider.Provider.Current().UserId + "'");
                Base_CustomerAddress base_customeraddress = new Base_CustomerAddress();
                if (base_customer != null && base_customer.CustomerId != null)
                {
                    base_customeraddress = DataFactory.Database().FindEntityByWhere<Base_CustomerAddress>(" and CustomerId = '" + base_customer.CustomerId + "'");
                }
                var JsonData = new
                {
                    base_customer = base_customer,
                    base_customeraddress = base_customeraddress
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content("-1");
            }
        }
        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">实体对象</param>
        /// <param name="CustomerAddressListJson">客户地址</param>
        /// <returns></returns>
        public ActionResult SubmitFormCustomer(Base_Customer entity, Base_CustomerAddress base_customeraddress)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                entity.Modify(entity.CustomerId);
                database.Update(entity, isOpenTrans);
                if (base_customeraddress != null && !string.IsNullOrEmpty(base_customeraddress.CustomerAddressId))
                {
                    base_customeraddress.Modify(base_customeraddress.CustomerAddressId);
                    database.Update(base_customeraddress, isOpenTrans);
                }
                else
                {
                    base_customeraddress.Create();
                    database.Insert(base_customeraddress, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "保存成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        public ActionResult SetPassword(string Password, string txtOldPassword)
        {
            IDatabase database = DataFactory.Database();
            try
            {
                Base_Customer base_customer = DataFactory.Database().FindEntityByWhere<Base_Customer>(" and Code = '" + ManageProvider.Provider.Current().UserId + "'");
                if (base_customer.Password == txtOldPassword)
                {
                    base_customer.Modify(base_customer.CustomerId);
                    base_customer.Password = Password;
                    database.Update(base_customer);
                }
                else
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：原始密码错误" }.ToString());
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "保存成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 用户相关订单信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerInfo()
        {
            try
            {
                string amountSql = @"SELECT ISNULL(SUM(TotalAmount),0)  FROM dbo.BY_Delivery  WHERE 1=1  and DeleteMark = 0 ";
                string amountaSql = @"SELECT ISNULL(SUM(TotalAmount),0)  FROM dbo.BY_Delivery  WHERE 1=1 and DeleteMark = 0  and CreateDate >= cast('" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + "01" + "' as datetime) ";
                string orSql = @"  SELECT   ISNULL(COUNT(1),0)
   FROM     dbo.BY_Quotation q
            LEFT JOIN dbo.BY_Order o ON q.QuotationId = o.QuotationId
   WHERE (ISNULL(o.OrderCode,'') ='' OR o.OrderStatus ='正常生产' OR o.OrderStatus ='已暂停'
        OR o.OrderStatus ='部分暂停' OR o.OrderStatus ='部分完工' OR o.OrderStatus ='部分发运')  AND q.DeleteMark = 0  and o.DeleteMark=0  ";

                string orCSql = @"   SELECT   ISNULL(COUNT(1),0)
   FROM     dbo.BY_Quotation q
            LEFT JOIN dbo.BY_Order o ON q.QuotationId = o.QuotationId
   WHERE  o.OrderStatus ='已发运'  AND q.DeleteMark = 0  and o.DeleteMark=0  ";

                string csSql = @" SELECT ISNULL(COUNT(1),0) FROM    dbo.BY_CheckSheet WHERE 1=1 ";

                if (!isAdmin())
                {
                    amountSql += string.Format(" AND CustomerCode='{0}'", ManageProvider.Provider.Current().UserId);
                    orSql += string.Format(" AND q.CustomerCode='{0}'", ManageProvider.Provider.Current().UserId);
                    orCSql += string.Format("  AND q.CustomerCode='{0}'", ManageProvider.Provider.Current().UserId);
                    csSql += string.Format(" AND CustomerCode='{0}' ", ManageProvider.Provider.Current().UserId);
                }


                var JsonData = new
                {
                    amountSql = DataFactory.Database().FindCountBySql(amountSql),
                    amountaSql = DataFactory.Database().FindCountBySql(amountaSql),
                    orSql = DataFactory.Database().FindCountBySql(orSql),
                    orCSql = DataFactory.Database().FindCountBySql(orCSql),
                    csSql = DataFactory.Database().FindCountBySql(csSql)
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content("-1");
            }
        }
        #endregion
        #region //反馈
        public ActionResult Feedback()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
        #region //联系
        public ActionResult Contact()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                return RedirectToAction("WeiLogin", "WeiLogin");
            }
            return View();
        }
        #endregion
    }
}

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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 客户信息管理控制器
    /// </summary>
    public class CustomerController : PublicController<Base_Customer>
    {

        public virtual ActionResult SetFormForImg(string KeyValue)
        {
            Base_Customer entity = repositoryfactory.Repository().FindEntity(KeyValue);
            DataTable dt = DataFactory.Database().FindTableBySql("SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = '" + KeyValue + "' for xml path('')),1,1,'')");
            if (dt != null && dt.Rows.Count > 0)
            {
                entity.Remark = dt.Rows[0][0].ToString();
            }
            return Content(entity.ToJson());
        }




        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();


                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@" SELECT * FROM dbo.Base_Customer c WHERE 1=1 ");
                //订单单号
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["keywords"]))
                {
                    strSql.Append(" AND ( Code LIKE @keywords or FullName LIKE @keywords or ShortName LIKE @keywords )");
                    parameter.Add(DbFactory.CreateDbParameter("@keywords", '%' + QueryParameter["keywords"].ToString() + '%'));
                }

                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["Priced"]))
                {
                    if (QueryParameter["Priced"].ToString() == "1" && QueryParameter["noPriced"].ToString() != "1")  //noPriced
                    {
                        strSql.Append(@"  AND ( EXISTS (SELECT 1 FROM dbo.Base_CustomerProcess p WHERE p.CustomerId=c.CustomerId) OR 
EXISTS (SELECT 1 FROM dbo.Base_CustomerManage p WHERE p.CustomerId=c.CustomerId) OR 
EXISTS (SELECT 1 FROM dbo.Base_CustomerMaterials p WHERE p.CustomerId=c.CustomerId) )
                                    ");
                    }
                    if (QueryParameter["Priced"].ToString() != "1" && QueryParameter["noPriced"].ToString() == "1")  //noPriced
                    {
                        strSql.Append(@"  AND (NOT EXISTS ( SELECT   1
                       FROM     dbo.Base_CustomerProcess p
                       WHERE    p.CustomerId = c.CustomerId )
              AND NOT EXISTS ( SELECT    1
                          FROM      dbo.Base_CustomerManage p
                          WHERE     p.CustomerId = c.CustomerId )
              AND NOT EXISTS ( SELECT    1
                          FROM      dbo.Base_CustomerMaterials p
                          WHERE     p.CustomerId = c.CustomerId )
            )
                                    ");
                    }
                }

                DataTable ListData = new Base_CustomerBll().Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }


        Base_CustomerBll base_customerbll = new Base_CustomerBll();
        Base_CustomerAddressBll base_customeraddressbll = new Base_CustomerAddressBll();
        Base_CustomerRequeryBll base_customerrequerybll = new Base_CustomerRequeryBll();


        [LoginAuthorize]
        public  ActionResult GridPageJson1(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new DataTable();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string sql = @"SELECT
	c.*
FROM	Base_Customer c
WHERE 1=1 ";
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    ListData = base_customerbll.Repository().FindTablePageBySql(sql + WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    string sql = @"SELECT
	c.*
FROM	Base_Customer c
WHERE  1=1  ";
                    ListData = base_customerbll.Repository().FindTablePageBySql(sql,ref jqgridparam);
                }
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }



        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">实体对象</param>
        /// <param name="CustomerAddressListJson">客户地址</param>
        /// <returns></returns>
        public ActionResult SubmitFormCustomer(string KeyValue, Base_Customer entity, string CustomerAddressListJson, string CustomerRequeryListJson)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    entity.Create();
                    database.Insert(entity, isOpenTrans);
                }
                database.Delete<Base_CustomerAddress>("CustomerId", entity.CustomerId, isOpenTrans);
                List<Base_CustomerAddress> CustomerAddressList = CustomerAddressListJson.JonsToList<Base_CustomerAddress>();
                foreach (Base_CustomerAddress item in CustomerAddressList)
                {
                    item.CustomerId = entity.CustomerId;
                        item.Create();
                        database.Insert(item, isOpenTrans);
                }
                database.Delete<Base_CustomerRequery>("CustomerId", entity.CustomerId, isOpenTrans);
                List<Base_CustomerRequery> Base_CustomerRequeryList = CustomerRequeryListJson.JonsToList<Base_CustomerRequery>();
                foreach (Base_CustomerRequery item in Base_CustomerRequeryList)
                {
                    item.CustomerId = entity.CustomerId;
                        item.Create();
                        database.Insert(item, isOpenTrans);
                }

                List<BY_Quotation> BY_Quotation = DataFactory.Database().FindList<BY_Quotation>("CustomerId", entity.CustomerId);
                foreach (BY_Quotation item in BY_Quotation)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    item.PaymentMode = entity.PaymentMode;
                    item.ShortName = entity.ShortName;
                    item.SalesMan = entity.Sellpeople;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_Order> BY_Order = DataFactory.Database().FindList<BY_Order>("CustomerId", entity.CustomerId);
                foreach (BY_Order item in BY_Order)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    item.ShortName = entity.ShortName;
                    item.SalesMan = entity.Sellpeople;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_Delivery> BY_Delivery = DataFactory.Database().FindList<BY_Delivery>("CustomerId", entity.CustomerId);
                foreach (BY_Delivery item in BY_Delivery)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.PaymentMode = entity.PaymentMode;
                    item.CustomerName = entity.FullName;
                    item.ShortName = entity.ShortName;
                    item.SalesMan = entity.Sellpeople;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_CheckSheet> BY_CheckSheet = DataFactory.Database().FindList<BY_CheckSheet>("CustomerId", entity.CustomerId);
                foreach (BY_CheckSheet item in BY_CheckSheet)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.PaymentMode = entity.PaymentMode;
                    item.CustomerName = entity.FullName;
                    item.ShortName = entity.ShortName;
                    item.SalesMan = entity.Sellpeople;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_ReimburseSheet> BY_ReimburseSheet = DataFactory.Database().FindList<BY_ReimburseSheet>("CustomerId", entity.CustomerId);
                foreach (BY_ReimburseSheet item in BY_ReimburseSheet)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_CheckCompany> BY_CheckCompany = DataFactory.Database().FindList<BY_CheckCompany>("CustomerId", entity.CustomerId);
                foreach (BY_CheckCompany item in BY_CheckCompany)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_OrderDetailBeOut> BY_OrderDetailBeOut = DataFactory.Database().FindList<BY_OrderDetailBeOut>("CustomerId", entity.CustomerId);
                foreach (BY_OrderDetailBeOut item in BY_OrderDetailBeOut)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.ShortName = entity.ShortName;
                    item.PaymentMode = entity.PaymentMode;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_OutStock> BY_OutStock = DataFactory.Database().FindList<BY_OutStock>("CustomerId", entity.CustomerId);
                foreach (BY_OutStock item in BY_OutStock)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<By_InvoiceForDetail> By_InvoiceForDetail = DataFactory.Database().FindList<By_InvoiceForDetail>("CustomerId", entity.CustomerId);
                foreach (By_InvoiceForDetail item in By_InvoiceForDetail)
                {
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }




                List<BY_SteelQualityreport> BY_SteelQualityreport = DataFactory.Database().FindList<BY_SteelQualityreport>("CustomerId", entity.CustomerId);
                foreach (BY_SteelQualityreport item in BY_SteelQualityreport)
                {
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_StoveHeatList> BY_StoveHeatList = DataFactory.Database().FindList<BY_StoveHeatList>("CustomerId", entity.CustomerId);
                foreach (BY_StoveHeatList item in BY_StoveHeatList)
                {
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.ShortName;
                    item.ShortName = entity.ShortName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_QuotationHeatBeoutDetail> BY_QuotationHeatBeoutDetail = DataFactory.Database().FindList<BY_QuotationHeatBeoutDetail>("CustomerId", entity.CustomerId);
                foreach (BY_QuotationHeatBeoutDetail item in BY_QuotationHeatBeoutDetail)
                {
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.ShortName;
                    item.ShortName = entity.ShortName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_CSBReport> BY_CSBReport = DataFactory.Database().FindList<BY_CSBReport>("CustomerId", entity.CustomerId);
                foreach (BY_CSBReport item in BY_CSBReport)
                {
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                List<BY_SteelQualityreportT> BY_SteelQualityreportT = DataFactory.Database().FindList<BY_SteelQualityreportT>("CustomerId", entity.CustomerId);
                foreach (BY_SteelQualityreportT item in BY_SteelQualityreportT)
                {
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.FullName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                
                List<BY_QuotationHeat> BY_QuotationHeat = DataFactory.Database().FindList<BY_QuotationHeat>("CustomerId", entity.CustomerId);
                foreach (BY_QuotationHeat item in BY_QuotationHeat)
                {
                    item.CustomerCode = entity.Code;
                    item.CustomerName = entity.ShortName;
                    item.ShortName = entity.ShortName;
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                //BY_SteelQualityreport
                //BY_StoveHeatList
                //BY_QuotationHeatBeoutDetail
                //BY_CSBReport
                //BY_SteelQualityreportT
                //BY_QuotationHeat
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 客户地址信息列表（返回表格JSON）
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
        public ActionResult CustomerAddressListJson(string CustomerId)
        {
            if (!string.IsNullOrEmpty(CustomerId))
            {
                List<Base_CustomerAddress> ListData = base_customeraddressbll.GetList(CustomerId);
                if (ListData.Count == 0)
                {
                    Base_CustomerAddress base_customeraddress = new Base_CustomerAddress();
                    Base_Supplier base_supplier = DataFactory.Database().FindEntity<Base_Supplier>(CustomerId);
                    base_customeraddress.Telephone = base_supplier.SellpeoplePhone;
                    base_customeraddress.Fax = base_supplier.Fax;
                    base_customeraddress.EmailAddress = null;
                    base_customeraddress.Mobile = null;
                    base_customeraddress.DetailAddress = base_supplier.Address;
                    base_customeraddress.SendPath = null;
                    base_customeraddress.Contact = base_supplier.Sellpeople;
                    base_customeraddress.AddressCode = null;
                    ListData.Add(base_customeraddress);
                }
                var JsonData = new
                {
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            else
            {
                return null;
            }
        }
        public ActionResult CustomerRequeryListJson(string CustomerId)
        {
            if (!string.IsNullOrEmpty(CustomerId))
            {
                List<Base_CustomerRequery> ListData = base_customerrequerybll.GetList(CustomerId);
                var JsonData = new
                {
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            else
            {
                return null;
            }
        }
        public ActionResult CustomerEntityByOrderid(string OrderId, string QuotationId)
        {
            if (!string.IsNullOrEmpty(OrderId))
            {
                BY_Order BY_Order = DataFactory.Database().FindEntity<BY_Order>(OrderId);
                Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(BY_Order.CustomerId);
                return Content(Base_Customer.ToJson());
            }
            else
            {
                BY_Quotation BY_Quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
                Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(BY_Quotation.CustomerId);
                return Content(Base_Customer.ToJson());
            }
        }
        /// <summary>
        /// 【自动补全】返回钢材列表JSON
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <returns></returns>
        public ActionResult GetAutoCompleJson(string keywords)
        {
            List<Base_Customer> ListData = base_customerbll.GetAutoComple(keywords);

            foreach (var item in ListData)
            {
                item.Remark =  item.ShortName + item.Code.ToLower();
            }

            return Content(ListData.ToJson());
        }
        public ActionResult GetAutoCompleJsonNXJ(string keywords)
        {
            List<Base_Customer> ListData = base_customerbll.GetAutoCompleNXJ(keywords);
            return Content(ListData.ToJson());
        }
        public ActionResult GetCusInfo(string keywords, JqGridParam jqgridparam)
        {
            string sql = string.Format(@"SELECT  c.* ,
        ca.Contact ,
        ca.Mobile + ' ' + ca.Telephone AS Mobile ,
        ca.DetailAddress ,
        ca.SendPath
FROM    dbo.Base_Customer c 
       LEFT JOIN dbo.Base_CustomerAddress ca ON c.CustomerId = ca.CustomerId
WHERE (c.Code LIKE '%{0}%' or c.FullName LIKE '%{0}%') ", keywords);
            if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
            {
                sql += string.Format(" AND c.Code='{0}' ", DNS.Utilities.ManageProvider.Provider.Current().UserId);
            }
            DataTable dt = repositoryfactory.Repository().FindTablePageBySql(sql, ref jqgridparam);
            var JsonData = new
            {
                total = jqgridparam.total,
                page = jqgridparam.page,
                records = jqgridparam.records,
                rows = dt,
            };
            return Content(JsonData.ToJson());
        }
    }
}
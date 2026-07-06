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

namespace DNS.WebApp.Areas.CheckSheetModule.Controllers
{
    /// <summary>
    /// 对账单控制器
    /// </summary>
    public class BY_CheckSheetController : PublicController<BY_CheckSheet>
    {
        public ActionResult SalesummaryList(string StartTime,string EndTime, string CustomerCode, string IsComfirm,string Sellpeople, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_checksheetbll.SalesummaryList( StartTime, EndTime, CustomerCode, IsComfirm,Sellpeople, ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult FormForQC()
        {
            return View();
        }
        public ActionResult GridPageListJson1(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();

                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@"SELECT  CustomerCode ,
        CustomerName ,
        SUM(TotalAmount) TotalAmount
FROM    dbo.BY_Delivery d
WHERE   1=1 and d.DeleteMark = 0 
");
                //订单日期

                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["LType"]))
                {
                    strSql.Append(" AND d.LType = @LType ");
                    parameter.Add(DbFactory.CreateDbParameter("@LType", QueryParameter["LType"].ToString()));

                }

                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["StartTime"]))
                {
                    strSql.Append(" AND d.CreateDate > @StartTime");
                    parameter.Add(DbFactory.CreateDbParameter("@StartTime", QueryParameter["StartTime"].ToString() + ":00"));
                }
                
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["EndTime"]))
                {
                    strSql.Append(" AND d.CreateDate < @EndTime");
                    parameter.Add(DbFactory.CreateDbParameter("@EndTime", QueryParameter["EndTime"].ToString() + ":59"));
                }
               
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["CustomerCode"]))
                {
                    strSql.Append(" AND ( d.CustomerCode LIKE '%" + QueryParameter["CustomerCode"] + "%' or  d.CustomerName LIKE '%" + QueryParameter["CustomerCode"] + "%'  or  d.ShortName LIKE '%" + QueryParameter["CustomerCode"] + "%'  )");
                }
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["Sellpeople"]))
                {
                    strSql.Append(" AND d.SalesMan = @Sellpeople");
                    parameter.Add(DbFactory.CreateDbParameter("@Sellpeople", QueryParameter["Sellpeople"].ToString()));
                }

                strSql.Append(@" GROUP BY CustomerCode ,
        CustomerName ");

                DataTable ListData = new BY_CheckSheetBll().Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);

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


        public ActionResult GridPageListJson2(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();

                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@"
SELECT  ProductName ,
        SUM(Weight) Weight,
        SUM(MaterialAmount) MaterialAmount
FROM    ( SELECT    dd.SteelDescription AS productname ,
                    dd.Weight ,
                    dd.SalesAmount  AS MaterialAmount
          FROM      dbo.BY_DeliveryDetail dd ,
                    dbo.BY_Delivery d
          WHERE     dd.SalesAmount > 0 and d.DeleteMark = 0 
                    AND dd.DeliveryId = d.DeliveryId
                    AND d.Enabled > 0
                    AND d.CreateDate > '@startDate'
                    AND d.CreateDate < '@endDate'
                    AND d.CustomerName = '@CustomerName'
                    AND d.LType != 'RCL'
                    AND d.LType = @LType 
                    @Sellpeople
          UNION ALL
          SELECT    dd.Technology AS productname ,
                    dd.Weight ,
                    dd.SalesAmount AS MaterialAmount
          FROM      dbo.BY_DeliveryDetail dd ,
                    dbo.BY_Delivery d
          WHERE      dd.DeliveryId = d.DeliveryId and d.DeleteMark = 0 
                    AND dd.SalesAmount > 0
                    AND d.Enabled > 0
                    AND d.CreateDate > '@startDate'
                    AND d.CreateDate < '@endDate'
                    AND d.CustomerName = '@CustomerName'
                    AND d.LType = 'RCL'
                    AND d.LType = @LType 
                    @Sellpeople
        ) a

 ");

                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["Sellpeople"]))
                {
                    strSql.Replace("@Sellpeople", " AND d.SalesMan =  '" + QueryParameter["Sellpeople"].ToString() + "'");
                }
                else
                {
                    strSql.Replace("@Sellpeople", "");

                }

                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["LType"]))
                {
                    strSql.Replace("AND d.LType = @LType", " AND d.LType =  '" + QueryParameter["LType"].ToString()+"'");

                }
                else
                {
                    strSql.Replace("AND d.LType = @LType", "");

                }

                //订单日期
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["StartTime"]))
                {
                    strSql.Replace("AND d.CreateDate > '@startDate'", "AND d.CreateDate > '" + QueryParameter["StartTime"].ToString() + ":00:00'");
                }
                else
                {
                    strSql.Replace("AND d.CreateDate > '@startDate'", "");

                }
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["EndTime"]))
                {
                    strSql.Replace("AND d.CreateDate < '@endDate'", "AND d.CreateDate < '" + QueryParameter["EndTime"].ToString() + ":00:00'");
                }
                else
                {
                    strSql.Replace("AND d.CreateDate < '@endDate'", "");

                }
                if (!HashtableHelper.IsNullOrDBNull(QueryParameter["CustomerCode"]))
                {
                    strSql.Replace("AND d.CustomerName = '@CustomerName'", " AND ( d.CustomerCode LIKE '%" + QueryParameter["CustomerCode"] + "%' or  d.CustomerName LIKE '%" + QueryParameter["CustomerCode"] + "%' or  d.ShortName LIKE '%" + QueryParameter["CustomerCode"] + "%'  )");
                }
                else
                {
                    strSql.Replace("AND d.CustomerName = '@CustomerName'", "");

                }

                strSql.Append("  GROUP BY ProductName ");

                DataTable ListData =new BY_CheckSheetBll().Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);

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



        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Audit(string KeyValue)
        {
            try
            {
                int IsOk = 0;

                BY_CheckSheet BY_CheckSheet = DataFactory.Database().FindEntity<BY_CheckSheet>(KeyValue);
                BY_CheckSheet.Status = BY_CheckSheet.Status == 1 ? 0 : 1;
                BY_CheckSheet.Modify(KeyValue);
                IsOk = DataFactory.Database().Update(BY_CheckSheet);
                    
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [LoginAuthorize]
        public  ActionResult GridPageJsonInvoice(string ParameterJson, JqGridParam jqgridparam)
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
	*,
	ISNULL((SELECT
		SUM(r.ZRMoney)
	FROM BY_ReimburseSheet r
	WHERE r.Code = cs.CheckSheetCode)
	, 0) ZRMoney
FROM BY_CheckSheet cs where 1=1 ";
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);

                    ListData = repositoryfactory.Repository().FindTablePageBySql(sql + WhereSql, parameter.ToArray(), ref jqgridparam);
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




        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public override ActionResult SubmitForm(BY_CheckSheet entity, string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_CheckSheet Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    entity.Modify(KeyValue);
                    List<BY_ReimburseSheet> by_reimbursesheets = DataFactory.Database().FindList<BY_ReimburseSheet>("Code", Oldentity.CheckSheetCode);
                    foreach (BY_ReimburseSheet item in by_reimbursesheets)
                    {
                        item.InvoiceNum = entity.InvoiceNum;
                        DataFactory.Database().Update(item);
                    }



                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    IsOk = repositoryfactory.Repository().Insert(entity);
                    this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_CheckSheet by_checksheet = database.FindEntity<BY_CheckSheet>(KeyValue);
                database.Delete(by_checksheet, isOpenTrans);

                 
                List<BY_Delivery> by_deliverys = database.FindList<BY_Delivery>("CheckSheetCode", by_checksheet.CheckSheetCode);
                foreach (BY_Delivery item in by_deliverys)
                {
                    item.Modify(item.DeliveryId);
                    item.CheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }


                List<BY_ReimburseSheet> by_reimbursesheets = database.FindList<BY_ReimburseSheet>("Code", by_checksheet.CheckSheetCode);
                if (by_reimbursesheets.Count > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：对账单已收款，请先删除收款信息！" }.ToString());
                }
                if (by_deliverys.Sum(p => p.GetedAmount) != by_checksheet.GetedAmount)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：送货单已收款，请先删除收款信息！" }.ToString());
                }
                List<By_InvoiceForDetail> by_invoicefordetails = database.FindList<By_InvoiceForDetail>("CheckSheetId", by_checksheet.CheckSheetId);
                if (by_invoicefordetails.Sum(p=>p.InvoiceMoney) > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：对账单已开票，请先删除发票信息！" }.ToString());
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = "删除成功" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }




        public void DownLoadExcel1(string CheckSheetCode, string YearMonth, string CustomerName)
        {
            DataTable ListData = DataFactory.Database().FindTableBySql("SELECT DeliveryCode '单号', TotalAmount '金额',GetedAmount '已收',CreateDate '日期'  FROM dbo.BY_Delivery WHERE Enabled=1 and DeleteMark = 0   and CheckSheetCode='" + CheckSheetCode + "'");
            string[] DataColumn = { "单号", "金额", "已收","日期" };
            string fileName = CustomerName+YearMonth+"对账单明细";
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }
 
        public void DownLoadExcel2(string CheckSheetCode)
        {
            DataTable ListData = DataFactory.Database().FindTableBySql(@"  SELECT CONVERT(VARCHAR(100), d.CreateDate, 23) AS '日期' ,
        '''' + d.MnemonicCode '助记码' ,
        d.DeliveryCode '送货单号' ,
        d.CustomerName '客户名称' ,
        dd.ProductName '产品名称' ,
        CASE WHEN Weigh > 0
             THEN CAST(CONVERT(FLOAT, Thick) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, Weigh) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, Length) AS VARCHAR(50))
             ELSE 'Φ' + CAST(CONVERT(FLOAT, Thick) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, Length) AS VARCHAR(50))
        END AS '规格' ,
        dd.Qty '数量' ,
        dd.TheoryWeight '理重' ,
        dd.Weight AS '重量' ,
        dd.MaterialPrice AS '材料单价' ,
        dd.MaterialAmount AS '材料合计',
        dd.SmallCost AS '加工费(kg)',
        dd.SmallAmount AS '加工费合计',
        dd.WorkPrice AS '小件(件)',
        dd.WorkAmount AS '小件合计',
        dd.SalesAmount '合计费用' ,
     '''' +    CustomerCode ,
      '''' +   d.CustomerPO '客户po',
       '''' +  dd.CustomerProduct '模号',
      '''' +  dd.ProjectCode '配件号',
        dd.DeliveryCode ,

dd.SteelDescription '材料' ,
        dd.Technology '热处理项目' ,
        dd.HeatPrice AS '热处理单价' ,
        dd.HeatAmount '热处理合计' ,dd.ShipDescription '备注'

 FROM   dbo.BY_DeliveryDetail dd
        LEFT JOIN dbo.BY_Delivery d ON dd.DeliveryId = d.DeliveryId
 WHERE  d.CheckSheetCode = '" + CheckSheetCode + "'  and d.DeleteMark = 0  ");
            string[] DataColumn = { "日期", "助记码", "送货单号", "客户名称", "产品名称", "规格", "数量", "理重", "重量", "材料单价", "材料合计", "加工费(kg)", "加工费合计", "小件(件)", "小件合计"
             , "材料" , "热处理项目" , "热处理单价" , "热处理合计", "合计费用", "客户po", "模号", "配件号","备注"};
            string fileName = "送货单明细汇总";
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }

        public void DownLoadGC(string CheckSheetCode)
        {
            DataTable ListData = DataFactory.Database().FindTableBySql(string.Format(@"  SELECT    CONVERT(VARCHAR(100), d.CreateDate, 23) AS '日期' ,
            dd.ProductName '产品名称' ,
            d.DeliveryCode '送货单号' ,
            d.CustomerPO '客户订单' ,
            SUM(dd.Weight) AS '重量' ,
            dd.MaterialPrice AS '材料单价' ,
            SUM(dd.MaterialAmount) AS '材料合计' ,
            SUM(dd.WorkAmount) AS '加工费合计' ,
            SUM(dd.SalesAmount) '合计费用',
case when SUM(dd.WorkAmount) = 0 then 0 else
 ( SELECT    SUM(dd1.WorkAmount) / SUM(CASE WHEN dd1.Qty = 0 THEN 1
                                                   ELSE dd1.Qty
                                              END)
          FROM      dbo.BY_DeliveryDetail dd1
                    LEFT JOIN dbo.BY_Delivery d1 ON dd1.DeliveryId = d1.DeliveryId
          WHERE     d1.CheckSheetCode = '{0}'  and d1.DeleteMark = 0 
                    AND dd1.WorkAmount > 0
        ) end AS '加工单价'
  FROM      dbo.BY_DeliveryDetail dd
            LEFT JOIN dbo.BY_Delivery d ON dd.DeliveryId = d.DeliveryId
  WHERE     d.CheckSheetCode = '{0}'  and d.DeleteMark = 0 
  GROUP BY  dd.ProductName ,
            dd.MaterialPrice ,
            d.DeliveryCode ,
            d.CreateDate ,
            d.CustomerPO
  ORDER BY  d.CreateDate ASC", CheckSheetCode));
            string[] DataColumn = { "日期", "产品名称", "送货单号", "客户订单", "重量", "材料单价", "材料合计", "加工单价", "加工费合计", "合计费用" };
            string fileName = "材料对账汇总" + CheckSheetCode;
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }
        public void DownLoadRCL(string CheckSheetCode)
        {
            DataTable ListData = DataFactory.Database().FindTableBySql(string.Format(@"  
SELECT  CONVERT(VARCHAR(100), d.CreateDate, 23) AS '日期' ,
        dd.SteelDescription '材料' ,
        dd.ProductName '品名' ,
        dd.Technology '热处理项目' ,
        sum(dd.Weight) AS '重量' ,
        dd.HeatPrice AS '单价' ,
        sum(dd.SalesAmount) '金额' ,
        d.DeliveryCode '送货单号'
FROM    dbo.BY_DeliveryDetail dd
        LEFT JOIN dbo.BY_Delivery d ON dd.DeliveryId = d.DeliveryId
WHERE   d.CheckSheetCode = '{0}' and d.DeleteMark = 0 
GROUP BY SteelDescription ,
        ProductName ,
        Technology ,
        d.DeliveryCode ,
        d.CreateDate ,
        dd.HeatPrice
ORDER BY d.CreateDate ASC", CheckSheetCode));
            string[] DataColumn = { "日期", "材料", "品名", "热处理项目", "重量", "单价", "金额", "送货单号" };
            string fileName = "热处理对账汇总" + CheckSheetCode;
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }
        
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SaleSummary()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult InvoiceIndex()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult InvoiceForm()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Audit()
        {
            return View();
        }
        BY_CheckSheetBll by_checksheetbll = new BY_CheckSheetBll();
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult PrintLW()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print1()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult SaleDetail()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult CustomerIndex()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print2()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult EarningsIndex()
        {
            return View();
        }
         [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult EarningsIndexCust()
        {
            return View();
        }

         [ManagerPermission(PermissionMode.Enforce)]
         [OutputCache(CacheProfile = "Cache1Hour")]
         public virtual ActionResult PrintGC()
         {
             return View();
         }

         [ManagerPermission(PermissionMode.Enforce)]
         [OutputCache(CacheProfile = "Cache1Hour")]
         public virtual ActionResult PrintRCL()
         {
             return View();
         }
        
        public void DownLoadExcel()
        {
            JqGridParam jqgridparam = new JqGridParam() { page = 1, records = 99999, rows = 99999, sidx = "CustomerCode,QuotationCode,LineCode" };
            string sql = @"     
           SELECT  *
FROM    ( SELECT    x.CustomerCode ,
                    '''' + x.月份 '月份' ,
                    x.客户 ,
                    SUM(x.欠款) '欠款' ,
                    1 SORTCODE
          FROM      ( SELECT    a.客户 ,
                                a.CustomerCode ,
                                a.月份 ,
                                a.欠款
                      FROM      ( SELECT    CustomerName '客户' ,
                                            CustomerCode ,
                                            CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                            SUM(TotalAmount - GetedAmount) '欠款'
                                  FROM      dbo.BY_Delivery
                                  WHERE     TotalAmount - GetedAmount != 0
                                            AND ( CheckSheetCode IS NULL
                                                  OR CheckSheetCode = ''
                                                )
                                            AND Enabled = 1 and DeleteMark = 0 
                                  GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                            CustomerName ,
                                            CustomerCode
                                ) a
                      UNION ALL
                      SELECT    a.客户 ,
                                a.CustomerCode ,
                                a.月份 ,
                                ISNULL(( SELECT  SUM(TotalAmount - GetedAmount)
                                        FROM    dbo.BY_CheckSheet
                                        WHERE   (YearMonth = a.月份  OR YearMonth = SUBSTRING(a.月份,1,5)+SUBSTRING(a.月份,7,1))
                                                AND CustomerCode = a.CustomerCode
                                      ), 0) '欠款'
                      FROM      ( SELECT    CustomerName '客户' ,
                                            CustomerCode ,
                                            CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                            SUM(TotalAmount - GetedAmount) '欠款'
                                  FROM      dbo.BY_Delivery
                                  WHERE     TotalAmount - GetedAmount != 0
                                            AND ( CheckSheetCode IS NOT NULL
                                                  AND CheckSheetCode != ''
                                                )
                                            AND Enabled = 1 and DeleteMark = 0 
                                  GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                            CustomerName ,
                                            CustomerCode
                                ) a
                      UNION ALL
                      SELECT    CustomerName '客户' ,
                                CustomerCode ,
                                CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                SUM(TotalAmount - GetedAmount) '欠款'
                      FROM      dbo.BY_CheckCompany
                      WHERE     TotalAmount - GetedAmount != 0
                      GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                CustomerName ,
                                CustomerCode
                      UNION ALL
                      SELECT    CustomerName '客户' ,
                                CustomerCode ,
                                CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                SUM(-CanUseMoney) '欠款'
                      FROM      dbo.BY_ReimburseSheet
                      WHERE     CanUseMoney > 0
                                AND Opions = '预收款'
                      GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                CustomerName ,
                                CustomerCode
                    ) x
          GROUP BY  x.CustomerCode ,
                    x.月份 ,
                    x.客户
          UNION ALL
          SELECT    x.CustomerCode ,
                    '共' '月份' ,
                    x.客户 ,
                    SUM(x.欠款) '欠款' ,
                    2 SORTCODE
          FROM      ( SELECT    a.客户 ,
                                a.CustomerCode ,
                                a.月份 ,
                                a.欠款
                      FROM      ( SELECT    CustomerName '客户' ,
                                            CustomerCode ,
                                            CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                            SUM(TotalAmount - GetedAmount) '欠款'
                                  FROM      dbo.BY_Delivery
                                  WHERE     TotalAmount - GetedAmount  != 0
                                            AND ( CheckSheetCode IS NULL
                                                  OR CheckSheetCode = ''
                                                )
                                            AND Enabled = 1 and DeleteMark = 0 
                                  GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                            CustomerName ,
                                            CustomerCode
                                ) a
                      UNION ALL
                      SELECT    a.客户 ,
                                a.CustomerCode ,
                                a.月份 ,
                                ISNULL(( SELECT  SUM(TotalAmount - GetedAmount)
                                        FROM    dbo.BY_CheckSheet
                                        WHERE   (YearMonth = a.月份  OR YearMonth = SUBSTRING(a.月份,1,5)+SUBSTRING(a.月份,7,1))
                                                AND CustomerCode = a.CustomerCode
                                      ), 0) '欠款'
                      FROM      ( SELECT    CustomerName '客户' ,
                                            CustomerCode ,
                                            CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                            SUM(TotalAmount - GetedAmount) '欠款'
                                  FROM      dbo.BY_Delivery
                                  WHERE     TotalAmount - GetedAmount  != 0
                                            AND ( CheckSheetCode IS NOT NULL
                                                  AND CheckSheetCode != ''
                                                )
                                            AND Enabled = 1 and DeleteMark = 0 
                                  GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                            CustomerName ,
                                            CustomerCode
                                ) a
                      UNION ALL
                      SELECT    CustomerName '客户' ,
                                CustomerCode ,
                                CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                SUM(TotalAmount - GetedAmount) '欠款'
                      FROM      dbo.BY_CheckCompany
                      WHERE     TotalAmount - GetedAmount  != 0
                      GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                CustomerName ,
                                CustomerCode
                      UNION ALL
                      SELECT    CustomerName '客户' ,
                                CustomerCode ,
                                CONVERT (VARCHAR(7), CreateDate, 120) '月份' ,
                                SUM(-CanUseMoney) '欠款'
                      FROM      dbo.BY_ReimburseSheet
                      WHERE     CanUseMoney > 0
                                AND Opions = '预收款'
                      GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120) ,
                                CustomerName ,
                                CustomerCode
                    ) x
          GROUP BY  x.CustomerCode ,
                    x.客户
        ) y
ORDER BY y.CustomerCode ,
        SORTCODE";
            DataTable ListData = DataFactory.Database().FindTableBySql(sql);
            string[] DataColumn = { "客户", "月份", "欠款" };
            string fileName = "客户欠款汇总";
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }
        [LoginAuthorize]
        public ActionResult SaleDetailPageJson(string Year,string Customer ,string type,string IsComfirm,string Sellpeople,JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_checksheetbll.GetSaleDetail(Year,Customer, type,IsComfirm, Sellpeople, ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Invoice(string KeyValue)
        {
            try
            {
                BY_CheckSheet by_checkcompany = repositoryfactory.Repository().FindEntity(KeyValue);
                int IsOk = 0;
                if (!string.IsNullOrEmpty(by_checkcompany.CheckSheetId))
                {
                    by_checkcompany.Modify(KeyValue);
                    by_checkcompany.IsInvoice = 1;
                    IsOk = repositoryfactory.Repository().Update(by_checkcompany);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "已开票" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Comfrim(string KeyValue, string Remark)
        {
            try
            {
                BY_CheckSheet by_checkcompany = repositoryfactory.Repository().FindEntity(KeyValue);
                int IsOk = 0;
                if (!string.IsNullOrEmpty(by_checkcompany.CheckSheetId))
                {
                    by_checkcompany.Modify(KeyValue);
                    by_checkcompany.IsComfirm = 1;
                    by_checkcompany.Remark = Remark;
                    IsOk = repositoryfactory.Repository().Update(by_checkcompany);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "已确认" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        public ActionResult PrintDataJson(string CheckSheetCode)
        {
            try
            {
                BY_CheckSheet Data = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", CheckSheetCode);
                List<BY_Delivery> DataDetails = DataFactory.Database().FindListBySql<BY_Delivery>(string.Format(@"  SELECT *
 FROM   ( SELECT    d.CreateDate ,
                    d.DeliveryCode ,
                    d.IsInvoice ,
                    d.Remark ,
                    d.TotalAmount ,
                    d.PrintGroup,d.DieCode
          FROM      dbo.BY_Delivery d
          WHERE     CheckSheetCode = '{0}'  and d.DeleteMark = 0 
                    
        ) dt
        LEFT JOIN ( SELECT  SUM(d1.TotalAmount) AS TotalAmountall ,
                            SUM(1) AS Rowall ,
                            d1.PrintGroup
                    FROM    dbo.BY_Delivery d1
                    WHERE   CheckSheetCode = '{0}'   and d1.DeleteMark = 0 
                            AND PrintGroup != ''
                            AND PrintGroup IS NOT NULL
                            AND Enabled > 0
                    GROUP BY d1.PrintGroup
                  ) d1t ON dt.PrintGroup = d1t.PrintGroup order by dt.PrintGroup ", CheckSheetCode));
                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", Data.CustomerCode);
                List<BY_CheckSheet> Datas = DataFactory.Database().FindListBySql<BY_CheckSheet>(string.Format(@"SELECT  ISNULL(SUM(TotalAmount), 0) TotalAmount ,
        ISNULL(SUM(GetedAmount), 0) GetedAmount ,
        CustomerCode ,
        YearMonth
FROM    ( SELECT    ISNULL(SUM(TotalAmount), 0) TotalAmount ,
                    ISNULL(SUM(GetedAmount), 0) GetedAmount ,
                    CustomerCode ,
                    CONVERT (VARCHAR(7), CreateDate, 120) YearMonth
          FROM      BY_CheckCompany cc WHERE CustomerCode='{0}'
          and     totalAmount - GetedAmount  != 0
          GROUP BY  CustomerCode ,
                    CONVERT (VARCHAR(7), CreateDate, 120)
          UNION ALL
          SELECT    TotalAmount ,
                    GetedAmount ,
                    CustomerCode ,
                    YearMonth
          FROM      BY_CheckSheet WHERE CustomerCode='{0}'
          and     TotalAmount - GetedAmount  != 0
        ) a 
GROUP BY CustomerCode ,
        YearMonth
ORDER BY a.YearMonth DESC
", Data.CustomerCode));

                Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(Data.CompanyName) ? "" : Data.CompanyName);

                var JsonData = new
                {
                    CheckEntity = Data,
                    CheckEntityList = DataDetails,
                    base_customer = base_customer,
                    CheckList = Datas,
                    CheckListTotalMoney = Datas.Sum(p => p.TotalAmount) - Datas.Sum(p => p.GetedAmount),
                    UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(Data.TotalAmount)),//大写总金额
                    company = company
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
        public class temp
        {
            public BY_DeliveryDetail by_deliverydetail { get; set; }
            public string QuotationCode { get; set; }
            public string CustomerPO { get; set; }
            public DateTime? dt { get; set; }
            public decimal? TotalAmount { get; set; }
            public int? Rows { get; set; }
        }
        public ActionResult PrintDataJson1(string CheckSheetCode)
        {
            try
            {
                BY_CheckSheet Data = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", CheckSheetCode);
                List<BY_Delivery> DataDetails = DataFactory.Database().FindList<BY_Delivery>(string.Format(" and CheckSheetCode = '{0}' and Enabled =1 ", CheckSheetCode));
                List<BY_DeliveryDetail> deliverydetails = new List<BY_DeliveryDetail>();
                List<temp> temps = new List<temp>();
                List<BY_CheckSheet> Datas = DataFactory.Database().FindListBySql<BY_CheckSheet>(string.Format(" select * from BY_CheckSheet where  CustomerCode = '{0}' and TotalAmount > GetedAmount and CheckSheetId !='{1}'", Data.CustomerCode, Data.CheckSheetId));

                foreach (BY_Delivery item in DataDetails)
                {
                    List<BY_DeliveryDetail> tempDetails = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", item.DeliveryId);
                    foreach (BY_DeliveryDetail tempDetail in tempDetails)
                    {
                        temp temp = new temp();
                        temp.by_deliverydetail = tempDetail;
                        temp.QuotationCode = item.DeliveryCode;
                        temp.CustomerPO = item.CustomerPO;
                        temp.dt = item.CreateDate;
                        temp.TotalAmount = item.TotalAmount;
                        temp.Rows = tempDetails.Count();
                        temps.Add(temp);
                    }
                    
                }
                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", Data.CustomerCode);

                Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(Data.CompanyName) ? "" : Data.CompanyName);

                var JsonData = new
                {
                    CheckEntity = Data,
                    CheckEntityList = temps.OrderBy(p => p.by_deliverydetail.CreateDate).ThenBy(p => p.by_deliverydetail.DeliveryId).ThenBy(p => p.by_deliverydetail.LineCode),
                    TotalAmount = Data.DTotalAmount,
                    base_customer = base_customer,
                    CheckList = Datas,
                    UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(Data.DTotalAmount)),//大写总金额
                    company = company
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
        public ActionResult PrintDataGC(string CheckSheetCode)
        {
            try
            {
                BY_CheckSheet Data = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", CheckSheetCode);
                List<BY_Delivery> DataDetails = DataFactory.Database().FindList<BY_Delivery>(string.Format(" and CheckSheetCode = '{0}' and Enabled =1 ", CheckSheetCode));
                List<temp> temps = new List<temp>();
                List<BY_CheckSheet> Datas = DataFactory.Database().FindListBySql<BY_CheckSheet>(string.Format(" select * from BY_CheckSheet where  CustomerCode = '{0}' and TotalAmount > GetedAmount and CheckSheetId !='{1}'", Data.CustomerCode, Data.CheckSheetId));

                foreach (BY_Delivery item in DataDetails)
                {
                    List<BY_DeliveryDetail> tempDetails = DataFactory.Database().FindListBySql<BY_DeliveryDetail>(string.Format(@"
SELECT  ProductName ,
        DeliveryCode ,
        SUM(Weight) Weight ,
        MaterialPrice ,
        SUM(MaterialAmount) MaterialAmount ,
        SUM(WorkAmount) WorkAmount ,
        SUM(SalesAmount) SalesAmount ,
        ( SELECT    SUM(d.WorkAmount) / SUM(CASE WHEN d.Qty = 0 THEN 1
                                                 ELSE d.Qty
                                            END)
          FROM      dbo.BY_DeliveryDetail d
          WHERE     d.DeliveryId = '{0}'
                    AND d.WorkAmount > 0
        ) AS WorkPrice
FROM    BY_DeliveryDetail DD
WHERE   DD.DeliveryId = '{0}'
GROUP BY ProductName ,
        MaterialPrice ,
        DeliveryCode 
", item.DeliveryId));
                    foreach (BY_DeliveryDetail tempDetail in tempDetails)
                    {
                        temp temp = new temp();
                        temp.by_deliverydetail = tempDetail;
                        temp.QuotationCode = item.DeliveryCode;
                        temp.CustomerPO = item.CustomerPO;
                        temp.dt = item.CreateDate;
                        temp.TotalAmount = item.TotalAmount;
                        temp.Rows = tempDetails.Count();
                        temps.Add(temp);
                    }
                }
                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", Data.CustomerCode);

                Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(Data.CompanyName) ? "" : Data.CompanyName);


                var JsonData = new
                {
                    CheckEntity = Data,
                    CheckEntityList = temps.OrderBy(p => p.dt).ThenBy(p => p.by_deliverydetail.DeliveryId).ThenBy(p => p.by_deliverydetail.LineCode),
                    TotalAmount = Data.DTotalAmount,
                    base_customer = base_customer,
                    CheckList = Datas,
                    UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(Data.DTotalAmount)),//大写总金额
                    company = company
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult PrintDataRCL(string CheckSheetCode)
        {
            try
            {
                BY_CheckSheet Data = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", CheckSheetCode);
                List<BY_Delivery> DataDetails = DataFactory.Database().FindList<BY_Delivery>(string.Format(" and CheckSheetCode = '{0}' and Enabled =1 ", CheckSheetCode));
                List<temp> temps = new List<temp>();
                List<BY_CheckSheet> Datas = DataFactory.Database().FindListBySql<BY_CheckSheet>(string.Format(" select * from BY_CheckSheet where  CustomerCode = '{0}' and TotalAmount > GetedAmount and CheckSheetId !='{1}'", Data.CustomerCode, Data.CheckSheetId));

                foreach (BY_Delivery item in DataDetails)
                {
                    List<BY_DeliveryDetail> tempDetails = DataFactory.Database().FindListBySql<BY_DeliveryDetail>(string.Format(@"
SELECT  d.CreateDate ,
        SteelDescription ,
        ProductName ,
        Technology ,
        SUM(dd.Weight) Weight,
        dd.HeatPrice ,
        SUM(dd.SalesAmount) SalesAmount,
        d.DeliveryCode
FROM    BY_DeliveryDetail DD ,
        dbo.BY_Delivery d
WHERE   DD.DeliveryId = '{0}'  and d.DeleteMark = 0 
        AND dd.DeliveryId = d.DeliveryId
GROUP BY SteelDescription ,
        ProductName ,
        Technology ,
        d.DeliveryCode ,
        d.CreateDate ,
        dd.HeatPrice

", item.DeliveryId));
                    foreach (BY_DeliveryDetail tempDetail in tempDetails)
                    {
                        temp temp = new temp();
                        temp.by_deliverydetail = tempDetail;
                        temp.QuotationCode = item.DeliveryCode;
                        temp.CustomerPO = item.CustomerPO;
                        temp.dt = item.CreateDate;
                        temp.TotalAmount = item.TotalAmount;
                        temp.Rows = tempDetails.Count();
                        temps.Add(temp);
                    }
                }
                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", Data.CustomerCode);
                Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(Data.CompanyName) ? "" : Data.CompanyName);

                var JsonData = new
                {
                    CheckEntity = Data,
                    CheckEntityList = temps.OrderBy(p => p.dt),
                    TotalAmount = Data.DTotalAmount,
                    base_customer = base_customer,
                    CheckList = Datas,
                    UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(Data.DTotalAmount)),//大写总金额
                    company = company
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }


        //Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        ///// <summary>
        ///// 获取自动单据编码
        ///// </summary>
        ///// <param name="OrderType">加工类型：精、粗</param>
        ///// <returns></returns>
        //private string BillCode()
        //{
        //    string UserId = ManageProvider.Provider.Current().UserId;
        //    return base_coderulebll.GetBillCode(UserId, "BYCS");
        //}
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SetForm1(string Code, string CodeType)
        {
            if (Code.IndexOf("DNS") >= 0)
            {
                CodeType = "报价单";
            }
            else if (Code.IndexOf("CS") >= 0)
            {
                CodeType = "对账单";
            }
            else if (Code.IndexOf("GC") >= 0 || Code.IndexOf("RC") >= 0 || Code.IndexOf("LJ") >= 0)
            {
                CodeType = "发运单";
            }
            else
            {
                CodeType = "收款单";
            }

            if (CodeType == "报价单")
            {
                BY_Delivery entity = DataFactory.Database().FindEntity<BY_Delivery>("QuotationCode", Code.Trim());
                return Content(new JsonMessage { Code = "1", Message = (entity.TotalAmount - entity.GetedAmount).ToString() }.ToString());
            }

            //收款单
            if (CodeType == "收款单")
            {
                BY_CheckCompany entity = DataFactory.Database().FindEntityByWhere<BY_CheckCompany>(" and IsAudit=1 and CheckCompanyCode = '" + Code.Trim()+"'");
                return Content(new JsonMessage { Code = "1", Message = (entity.TotalAmount - entity.GetedAmount).ToString() }.ToString());
            }
            //对账单
            if (CodeType == "对账单")
            {
                BY_CheckSheet entity = repositoryfactory.Repository().FindEntity("CheckSheetCode", Code.Trim());
                return Content(new JsonMessage { Code = "1", Message = (entity.TotalAmount - entity.GetedAmount).ToString(), Attribute=entity.Remark }.ToString());
            }
            //发运单
            if (CodeType == "发运单")
            {
                BY_Delivery entity = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", Code.Trim());
                return Content(new JsonMessage { Code = "1", Message = (entity.TotalAmount - entity.GetedAmount).ToString() }.ToString());
            }
            return Content(new JsonMessage { Success = false, Code = "0", Message = "获取金额失败！" }.ToString());
        }
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SetSummary(string CustomerCode)
        {
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", CustomerCode);
               
                    List<BY_Delivery> by_deliverys = DataFactory.Database().FindListBySql<BY_Delivery>(string.Format(@"SELECT *
FROM    dbo.BY_Delivery
WHERE   CustomerCode = '{0}'  
        AND ( CheckSheetCode IS NULL
              OR CheckSheetCode = ''
            )  and Enabled =1  and DeleteMark = 0 
", CustomerCode));
                List<BY_CheckCompany> by_checkcompanys = DataFactory.Database().FindListBySql<BY_CheckCompany>(string.Format(@" SELECT * FROM BY_CheckCompany WHERE  CustomerCode='{0}' ", CustomerCode));
                List<BY_CheckSheet> by_checksheets = DataFactory.Database().FindListBySql<BY_CheckSheet>(string.Format(@" SELECT *
 FROM   BY_CheckSheet
 WHERE   CustomerCode = '{0}' ", CustomerCode));

                List<BY_ReimburseSheet> by_reimbursesheets = DataFactory.Database().FindListBySql<BY_ReimburseSheet>(string.Format(@" SELECT *
FROM    dbo.BY_ReimburseSheet
WHERE   CustomerCode = '{0}'
        AND Opions = '预收款'
        AND CanUseMoney > 0 ", CustomerCode));

                var JsonData = new
                {
                    SumTotalMoney = by_deliverys.Sum(p => p.TotalAmount) + by_checkcompanys.Sum(p => p.TotalAmount) + by_checksheets.Sum(p => p.TotalAmount),
                    SumGetAmount = by_deliverys.Sum(p => p.GetedAmount) + by_checkcompanys.Sum(p => p.GetedAmount) + by_checksheets.Sum(p => p.GetedAmount) + by_reimbursesheets.Sum(p => p.CanUseMoney),
                    SumLastAmount = by_deliverys.Sum(p => p.TotalAmount) + by_checkcompanys.Sum(p => p.TotalAmount) + by_checksheets.Sum(p => p.TotalAmount) - (by_deliverys.Sum(p => p.GetedAmount) + by_checkcompanys.Sum(p => p.GetedAmount) + by_checksheets.Sum(p => p.GetedAmount) + by_reimbursesheets.Sum(p => p.CanUseMoney)),
                };
                return Content(JsonData.ToJson());
            }
            return Content(new JsonMessage { Success = false, Code = "0", Message = "获取金额失败！" }.ToString());
        }

        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        private string BillCode(string ModuleId)
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        //单个按日期计算
        public virtual ActionResult BuildCheckSheet(string Year, string Month, string CustomerId, string CheckSheetId, string deliverycodes, string BeginDate, string EndDate, decimal DTotalAmount, string LType, decimal DTotalAmountOri)
        {
            DateTime starttime = Convert.ToDateTime(BeginDate);
            DateTime endtime = Convert.ToDateTime(EndDate);
            string ModuleId = "CS";
            string YearMonth = Year + "-" + Month;
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            List<DbParameter> parameter = new List<DbParameter>();
            try
            {
                string Message = string.Empty;
                string[] ids = deliverycodes.Trim(',').Split(',');
                string idstr = "";
                for (int i = 0; i < ids.Length; i++)
                {
                    idstr = idstr + "'" + ids[i] + "'" + ",";
                }
                idstr = idstr.Trim(',');
                if (string.IsNullOrEmpty(CheckSheetId))
                {
                    Message = "对账单新增成功！";
                    parameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId.Trim()));
                    parameter.Add(DbFactory.CreateDbParameter("@YearMonth", YearMonth));
                    List<BY_Delivery> by_deliverys = database.FindListBySql<BY_Delivery>(string.Format(@"
                            SELECT  *
FROM    dbo.BY_Delivery
WHERE   Enabled = 1
        AND CustomerId = @CustomerId
        AND deliverycode IN ({0})  and DeleteMark = 0 
                                    ", idstr), parameter.ToArray());

                    var companynames = by_deliverys.Select(o => o.CompanyName).Distinct().ToList();
                    if (companynames.Count >1)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = true, Code = "-1", Message = "所选单据，有多个公司类别，请先修改送货单！" }.ToString());
                    }
                    var companynames2 = by_deliverys.Select(o => o.LType).Distinct().ToList();
                    if (companynames2.Count > 1)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = true, Code = "-1", Message = "所选单据，材料，配件，热处理混入，只能同一类型！" }.ToString());
                    }
                    Base_Customer base_customer = database.FindEntity<Base_Customer>("CustomerId", CustomerId);
                    Base_CustomerAddress base_customeraddress = database.FindEntityBySql<Base_CustomerAddress>(string.Format(@"SELECT  *
FROM    dbo.Base_CustomerAddress
WHERE CustomerId='{0}'
ORDER BY MainDefault DESC", base_customer.CustomerId));
                    if (base_customer == null || base_customer.CustomerId == null)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = true, Code = "-1", Message = base_customer.Code + "客户信息不存在！" }.ToString());
                    }


                    BY_CheckSheet by_checksheet = new BY_CheckSheet();
                    by_checksheet.CompanyName = by_deliverys.First().CompanyName;
                    by_checksheet.LType = by_deliverys[0].LType;
                    by_checksheet.BeginDate = starttime;
                    by_checksheet.EndDate = endtime;
                    by_checksheet.CheckSheetId = CommonHelper.GetGuid;
                    by_checksheet.CreateDate = DateTime.Now;
                    by_checksheet.CreateUserName = "System";
                    by_checksheet.CustomerId = base_customer.CustomerId;
                    by_checksheet.CustomerCode = base_customer.Code;
                    by_checksheet.CustomerName = base_customer.FullName;
                    by_checksheet.ShortName = base_customer.ShortName;
                    by_checksheet.ContactName = base_customeraddress.Contact;
                    by_checksheet.ContactTel = base_customeraddress.Telephone;
                    by_checksheet.ContactFax = base_customeraddress.Fax;
                    by_checksheet.ContactMail = base_customeraddress.EmailAddress;
                    by_checksheet.MobileCode = base_customeraddress.Mobile;
                    by_checksheet.CustomerAddress = base_customeraddress.DetailAddress;
                    by_checksheet.Qty = by_deliverys.Sum(p => p.Qty);
                    by_checksheet.TaxRate = base_customer.TaxRate;
                    by_checksheet.IsOver = 0;
                    by_checksheet.LessedAmount = 0; ;
                    by_checksheet.PaymentMode = base_customer.PaymentMode;
                    by_checksheet.ShipMode = base_customer.TransitMode;
                    by_checksheet.ShipGroup = "宁波天成模具钢热处理有限公司";
                    by_checksheet.SalesMode = base_customer.Category;
                    by_checksheet.SalesMan = base_customer.Sellpeople;
                    by_checksheet.SalesArea = base_customer.Sellarea;
                    by_checksheet.Weight = by_deliverys.Sum(p => p.Weight);
                    by_checksheet.ProcessingAlgorithm = base_customer.ProcessingAlgorithm;
                    by_checksheet.YearMonth = YearMonth;
                    by_checksheet.CheckSheetCode = this.BillCode(ModuleId);
                    by_checksheet.GetedAmount = by_deliverys.Count > 0 ? by_deliverys.Sum(p => p.GetedAmount) : 0;
                    by_checksheet.TotalAmount = by_deliverys.Count > 0 ? by_deliverys.Sum(p => p.TotalAmount) : DTotalAmount;
                    by_checksheet.DTotalAmount = DTotalAmount;
                    by_checksheet.DTotalAmountOri = DTotalAmountOri;
                    by_checksheet.CanInvoiceMoney = by_deliverys.Count > 0 ? by_deliverys.Where(p => p.IsInvoice == 1).Sum(p => p.TotalAmount) : DTotalAmount;
                    by_checksheet.Create();
                    database.Insert<BY_CheckSheet>(by_checksheet, isOpenTrans);

                    foreach (BY_Delivery item in by_deliverys)
                    {
                        item.CheckSheetCode = by_checksheet.CheckSheetCode;
                        database.Update<BY_Delivery>(item, isOpenTrans);
                    }
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);

                }
                else
                {
                    BY_CheckSheet by_checksheet = DataFactory.Database().FindEntity<BY_CheckSheet>(CheckSheetId);

                    Message = "对账单修改成功！";
                    parameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId.Trim()));
                    parameter.Add(DbFactory.CreateDbParameter("@YearMonth", YearMonth));
                    parameter.Add(DbFactory.CreateDbParameter("@CheckSheetCode", by_checksheet.CheckSheetCode));
                    List<BY_Delivery> by_deliverys = database.FindListBySql<BY_Delivery>(string.Format(@"
                            SELECT  *
FROM    dbo.BY_Delivery
WHERE   Enabled = 1  and DeleteMark = 0 
        AND CustomerId = @CustomerId
        AND deliverycode IN ({0})
                                    ", idstr), parameter.ToArray());

                    var companynames = by_deliverys.Select(o => o.CompanyName).Distinct().ToList();
                    if (companynames.Count > 1)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = true, Code = "-1", Message = "所选单据，有多个公司类别，请先修改送货单！" }.ToString());
                    }

                    List<BY_Delivery> by_deliverysupdatesD = new List<BY_Delivery>();
                    by_deliverysupdatesD = database.FindListBySql<BY_Delivery>(@"
                                                SELECT  *
                                                FROM    dbo.BY_Delivery
                                                WHERE   CheckSheetCode=@CheckSheetCode  and DeleteMark = 0 
                                                        ", parameter.ToArray());

                    by_checksheet.CompanyName = by_deliverys.First().CompanyName;
                    by_checksheet.LType = LType;
                    by_checksheet.BeginDate = starttime;
                    by_checksheet.EndDate = endtime;
                    by_checksheet.Qty = by_deliverys.Sum(p => p.Qty);
                    by_checksheet.DTotalAmount = by_deliverys.Sum(p => p.TotalAmount);
                    by_checksheet.IsOver = 0;
                    by_checksheet.LessedAmount = 0; ;
                    by_checksheet.ShipGroup = "宁波天成模具钢热处理有限公司";
                    by_checksheet.Weight = by_deliverys.Sum(p => p.Weight);
                    by_checksheet.YearMonth = YearMonth;
                    by_checksheet.CheckSheetCode = this.BillCode(ModuleId);
                    //by_checksheet.GetedAmount = by_deliverys.Count > 0 ? by_deliverys.Sum(p => p.GetedAmount): by_checksheet.GetedAmount;
                    by_checksheet.TotalAmount = by_deliverys.Count > 0 ? by_deliverys.Sum(p => p.TotalAmount) : DTotalAmount;
                    by_checksheet.DTotalAmount = DTotalAmount;
                    by_checksheet.DTotalAmountOri = DTotalAmountOri;
                    by_checksheet.CanInvoiceMoney = by_deliverys.Count > 0 ? by_deliverys.Where(p => p.IsInvoice == 1).Sum(p => p.TotalAmount) : DTotalAmount;

                    database.Update<BY_CheckSheet>(by_checksheet, isOpenTrans);

                    foreach (BY_Delivery item in by_deliverysupdatesD)
                    {
                        item.CheckSheetCode = "";
                        database.Update(item, isOpenTrans);
                    }
                    foreach (BY_Delivery item in by_deliverys)
                    {
                        item.CheckSheetCode = by_checksheet.CheckSheetCode;
                        database.Update<BY_Delivery>(item, isOpenTrans);
                    }
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                //Base_SysLogBll.Instance.WriteLog("checksheet", OperationType.Other, "-1", "生成对账单失败，" + ex.Message + "。时间：" + DateTime.Now.ToString());
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败！" }.ToString());
            }
        }
        //        //单个按日期计算
        //        public virtual ActionResult BuildCheckSheet(string Year, string Month, string CustomerCode, string StartTime, string EndTime)
        //        {
        //            DateTime starttime = Convert.ToDateTime(StartTime + " 00:00:00");
        //            DateTime endtime = Convert.ToDateTime(EndTime + " 23:59:59");
        //            string YearMonth = Year + "-" + Month;
        //            IDatabase database = DataFactory.Database();
        //            DbTransaction isOpenTrans = database.BeginTrans();
        //            List<DbParameter> parameter = new List<DbParameter>();
        //            try
        //            {
        //                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", CustomerCode.Trim()));
        //                parameter.Add(DbFactory.CreateDbParameter("@YearMonth", YearMonth));
        //                parameter.Add(DbFactory.CreateDbParameter("@starttime", starttime));
        //                parameter.Add(DbFactory.CreateDbParameter("@endtime", endtime));
        //                //原来的对账单
        //                BY_CheckSheet by_checksheetDelete = by_checksheetbll.Repository().FindEntityBySql(" SELECT * FROM dbo.BY_CheckSheet WHERE  CustomerCode = @CustomerCode       AND YearMonth = @YearMonth ", parameter.ToArray());
        //                List<BY_ReimburseSheet> by_reimbursesheets = database.FindListBySql<BY_ReimburseSheet>(" SELECT * FROM dbo.BY_ReimburseSheet where Status='1' ");
        //                BY_Delivery by_delivery = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  CustomerCode ,
        //                                    SUM(Qty) AS Qty ,SUM(Weight) AS Weight,
        //                                    SUM(TotalAmount) TotalAmount ,
        //                                    SUM(LessedAmount) LessedAmount
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CreateDate >= @starttime and Enabled =1
        //                                    AND CreateDate <= @endtime  AND OrderCode IS NOT NULL and CustomerCode=@CustomerCode GROUP BY CustomerCode
        //                             ", parameter.ToArray()).FirstOrDefault();
        //                List<BY_Delivery> by_deliverysupdatesA = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  *
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CreateDate >= @starttime and Enabled =1
        //                                    AND CreateDate <= @endtime  AND OrderCode IS NOT NULL and CustomerCode=@CustomerCode
        //                                    ", parameter.ToArray());
        //                List<BY_Delivery> by_deliverysupdatesD = new List<BY_Delivery>();
        //                if (by_checksheetDelete != null && by_checksheetDelete.CheckSheetId != null)
        //                {
        //                    parameter.Add(DbFactory.CreateDbParameter("@CheckSheetCode", by_checksheetDelete.CheckSheetCode));
        //                    by_deliverysupdatesD = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  *
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CheckSheetCode=@CheckSheetCode
        //                                    ", parameter.ToArray());
        //                }

        //                if (by_delivery == null)
        //                {
        //                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "找不到相关记录！" }.ToString());
        //                }
        //                Base_Customer base_customer = database.FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);
        //                Base_CustomerAddress base_customeraddress = database.FindEntityBySql<Base_CustomerAddress>(string.Format(@"SELECT  *
        //FROM    dbo.Base_CustomerAddress
        //WHERE CustomerId='{0}'
        //ORDER BY MainDefault DESC", base_customer.CustomerId));
        //                if (base_customer == null || base_customer.CustomerId == null)
        //                {
        //                    return Content(new JsonMessage { Success = true, Code = "-1", Message = base_customer.Code + "客户信息不存在！" }.ToString());
        //                }
        //                BY_CheckSheet by_checksheet = new BY_CheckSheet();
        //                by_checksheet.CheckSheetId = CommonHelper.GetGuid;
        //                by_checksheet.CreateDate = DateTime.Now;
        //                by_checksheet.CreateUserName = "System";
        //                by_checksheet.BeginDate = starttime;
        //                by_checksheet.EndDate = endtime;
        //                by_checksheet.CustomerId = base_customer.CustomerId;
        //                by_checksheet.CustomerCode = by_delivery.CustomerCode;
        //                by_checksheet.CustomerName = base_customer.FullName;
        //                by_checksheet.ShortName = base_customer.ShortName;
        //                by_checksheet.ContactName = base_customeraddress.Contact;
        //                by_checksheet.ContactTel = base_customeraddress.Telephone;
        //                by_checksheet.ContactFax = base_customeraddress.Fax;
        //                by_checksheet.ContactMail = base_customeraddress.EmailAddress;
        //                by_checksheet.MobileCode = base_customeraddress.Mobile;
        //                by_checksheet.CustomerAddress = base_customeraddress.DetailAddress;
        //                by_checksheet.Qty = by_delivery.Qty;
        //                by_checksheet.TotalAmount = by_delivery.TotalAmount * (1 + base_customer.TaxRate);
        //                by_checksheet.TaxRate = base_customer.TaxRate;
        //                by_checksheet.GetedAmount = 0;
        //                by_checksheet.IsOver = 0;
        //                by_checksheet.LessedAmount = by_delivery.LessedAmount; ;
        //                by_checksheet.PaymentMode = base_customer.PaymentMode;
        //                by_checksheet.ShipMode = base_customer.TransitMode;
        //                by_checksheet.ShipGroup = "北仑机械锻造厂";
        //                by_checksheet.SalesMode = base_customer.Category;
        //                by_checksheet.SalesMan = base_customer.Sellpeople;
        //                by_checksheet.SalesArea = base_customer.Sellarea;
        //                by_checksheet.Weight = by_delivery.Weight;
        //                by_checksheet.ProcessingAlgorithm = base_customer.ProcessingAlgorithm;
        //                by_checksheet.YearMonth = YearMonth;
        //                by_checksheet = by_checksheetbll.GetCheckSheetCode(by_checksheet, by_checksheetDelete, Year, Month);
        //                by_checksheet = by_checksheetbll.GetCheckSheetGetedMoney(by_checksheet, by_reimbursesheets, by_deliverysupdatesA);
        //                if (by_checksheetDelete.CheckSheetId != null)
        //                {
        //                    database.Delete(by_checksheetDelete, isOpenTrans);

        //                }
        //                foreach (BY_Delivery item in by_deliverysupdatesD)
        //                {
        //                    item.CheckSheetCode = "";
        //                    database.Update(item, isOpenTrans);
        //                }
        //                database.Insert<BY_CheckSheet>(by_checksheet, isOpenTrans);
        //                foreach (BY_Delivery item in by_deliverysupdatesA)
        //                {
        //                    item.CheckSheetCode = by_checksheet.CheckSheetCode;
        //                    database.Update<BY_Delivery>(item, isOpenTrans);
        //                }

        //                database.Commit();
        //                return Content(new JsonMessage { Success = true, Code = "1", Message = "生成对账单成功！" }.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                database.Rollback();
        //                //Base_SysLogBll.Instance.WriteLog("checksheet", OperationType.Other, "-1", "生成对账单失败，" + ex.Message + "。时间：" + DateTime.Now.ToString());
        //                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败！" }.ToString());
        //            }
        //        }
        //        public virtual ActionResult BuildCheckSheetForXJ(string Year, string Month)
        //        {
        //            DateTime starttime = Convert.ToDateTime(Year + "-" + Month + "-" + "1" + " 00:00:00");
        //            DateTime endtime = DateTimeHelper.LastDayOfMonth(Convert.ToDateTime(Year + "-" + Month + "-1" + " 23:59:59"));

        //            string YearMonth = Year + "-" + Month;
        //            IDatabase database = DataFactory.Database();
        //            DbTransaction isOpenTrans = database.BeginTrans();
        //            List<DbParameter> parameter = new List<DbParameter>();
        //            try
        //            {
        //                parameter.Add(DbFactory.CreateDbParameter("@YearMonth", YearMonth));
        //                parameter.Add(DbFactory.CreateDbParameter("@starttime", starttime));
        //                parameter.Add(DbFactory.CreateDbParameter("@endtime", endtime));
        //                //原来的对账单
        //                BY_CheckSheet by_checksheetDelete = by_checksheetbll.Repository().FindEntityBySql(" SELECT * FROM dbo.BY_CheckSheet WHERE  CustomerCode = 'XJMS'       AND YearMonth = @YearMonth ", parameter.ToArray());
        //                List<BY_ReimburseSheet> by_reimbursesheets = database.FindListBySql<BY_ReimburseSheet>(" SELECT * FROM dbo.BY_ReimburseSheet where Status='1' ");
        //                BY_Delivery by_delivery = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  'XJMS' CustomerId,
        //                                    'XJMS' CustomerCode,
        //                                    SUM(Qty) AS Qty ,SUM(Weight) AS Weight,
        //                                    SUM(TotalAmount) TotalAmount ,
        //                                    SUM(LessedAmount) LessedAmount
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CreateDate >= @starttime and Enabled =1
        //                                    AND CreateDate <= @endtime  
        //AND OrderCode IS NOT NULL  AND ( SELECT    C.PaymentMode
        //                                  FROM      dbo.Base_Customer C
        //                                  WHERE     c.Code = CustomerCode )='现金'
        //
        //                             ", parameter.ToArray()).FirstOrDefault();
        //                List<BY_Delivery> by_deliverysupdatesA = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  *
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CreateDate >= @starttime and Enabled =1
        //                                    AND CreateDate <= @endtime  AND OrderCode IS NOT NULL AND ( SELECT    C.PaymentMode
        //                                  FROM      dbo.Base_Customer C
        //                                  WHERE     c.Code = CustomerCode )='现金'
        //                                    ", parameter.ToArray());
        //                List<BY_Delivery> by_deliverysupdatesD = new List<BY_Delivery>();
        //                if (by_checksheetDelete != null && by_checksheetDelete.CheckSheetId != null)
        //                {
        //                    parameter.Add(DbFactory.CreateDbParameter("@CheckSheetCode", by_checksheetDelete.CheckSheetCode));
        //                    by_deliverysupdatesD = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  *
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CheckSheetCode=@CheckSheetCode
        //                                    ", parameter.ToArray());
        //                }

        //                if (by_delivery == null)
        //                {
        //                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "找不到相关记录！" }.ToString());
        //                }


        //                BY_CheckSheet by_checksheet = new BY_CheckSheet();
        //                by_checksheet.CheckSheetId = CommonHelper.GetGuid;
        //                by_checksheet.CreateDate = DateTime.Now;
        //                by_checksheet.CreateUserName = "System";
        //                by_checksheet.BeginDate = starttime;
        //                by_checksheet.EndDate = endtime;
        //                by_checksheet.CustomerId = by_delivery.CustomerId;
        //                by_checksheet.CustomerCode = by_delivery.CustomerCode;
        //                by_checksheet.CustomerName = "门市现金";
        //                by_checksheet.ShortName = "门市现金";
        //                by_checksheet.ContactName = "门市现金";
        //                by_checksheet.ContactTel = "0";
        //                by_checksheet.ContactFax = "0";
        //                by_checksheet.ContactMail = "门市现金";
        //                by_checksheet.MobileCode = "0";
        //                by_checksheet.CustomerAddress = "门市现金";
        //                by_checksheet.Qty = by_delivery.Qty;
        //                by_checksheet.TotalAmount = by_delivery.TotalAmount;
        //                by_checksheet.GetedAmount = 0;
        //                by_checksheet.IsOver = 0;
        //                by_checksheet.LessedAmount = by_delivery.LessedAmount; ;
        //                by_checksheet.PaymentMode = "现金";
        //                by_checksheet.ShipMode = "门市现金";
        //                by_checksheet.ShipGroup = "";
        //                by_checksheet.SalesMode = "门市现金";
        //                by_checksheet.SalesMan = "门市现金";
        //                by_checksheet.SalesArea = "门市现金";
        //                by_checksheet.Weight = by_delivery.Weight;
        //                by_checksheet.ProcessingAlgorithm = "门市现金";
        //                by_checksheet.YearMonth = YearMonth;
        //                by_checksheet = by_checksheetbll.GetCheckSheetCode(by_checksheet, by_checksheetDelete, Year, Month);
        //                by_checksheet = by_checksheetbll.GetCheckSheetGetedMoney(by_checksheet, by_reimbursesheets, by_deliverysupdatesA);
        //                if (by_checksheetDelete.CheckSheetId != null)
        //                {
        //                    database.Delete(by_checksheetDelete, isOpenTrans);

        //                }
        //                foreach (BY_Delivery item in by_deliverysupdatesD)
        //                {
        //                    item.CheckSheetCode = "";
        //                    database.Update(item, isOpenTrans);
        //                }
        //                database.Insert<BY_CheckSheet>(by_checksheet, isOpenTrans);
        //                foreach (BY_Delivery item in by_deliverysupdatesA)
        //                {
        //                    item.CheckSheetCode = by_checksheet.CheckSheetCode;
        //                    database.Update<BY_Delivery>(item, isOpenTrans);
        //                }

        //                database.Commit();
        //                return Content(new JsonMessage { Success = true, Code = "1", Message = "生成对账单成功！" }.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                database.Rollback();
        //                //Base_SysLogBll.Instance.WriteLog("checksheet", OperationType.Other, "-1", "生成对账单失败，" + ex.Message + "。时间：" + DateTime.Now.ToString());
        //                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败！" }.ToString());
        //            }
        //        }
        //        public virtual ActionResult BuildCheckSheet(string Year, string Month)
        //        {
        //            DateTime starttime = Convert.ToDateTime(Year + "-" + Month + "-1" + " 00:00:00");
        //            DateTime endtime = DateTimeHelper.LastDayOfMonth(Convert.ToDateTime(Year + "-" + Month + "-1" + " 23:59:59"));
        //            IDatabase database = DataFactory.Database();
        //            DbTransaction isOpenTrans = database.BeginTrans();
        //            List<DbParameter> parameter = new List<DbParameter>();
        //            try
        //            {
        //                List<BY_CheckSheet> by_checksheets = new List<BY_CheckSheet>();
        //                parameter.Add(DbFactory.CreateDbParameter("@CreateDate1", starttime));
        //                parameter.Add(DbFactory.CreateDbParameter("@CreateDate2", endtime));
        //                //原来的对账单
        //                List<BY_CheckSheet> by_checksheetsDelete = by_checksheetbll.Repository().FindListBySql(" SELECT * FROM dbo.BY_CheckSheet WHERE BeginDate=@CreateDate1 ", parameter.ToArray());
        //                List<BY_ReimburseSheet> by_reimbursesheets = database.FindListBySql<BY_ReimburseSheet>(" SELECT * FROM dbo.BY_ReimburseSheet where Status='1' ");
        //                List<BY_Delivery> by_deliverys = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  CustomerId ,
        //                                    CustomerCode ,
        //                                    SUM(Qty) AS Qty ,SUM(Weight) AS Weight,
        //                                    SUM(TotalAmount) TotalAmount ,
        //                                    SUM(LessedAmount) LessedAmount
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CreateDate >= @CreateDate1
        //                                    AND CreateDate <= @CreateDate2 
        //                                    AND OrderCode IS NOT NULL
        //                            GROUP BY CustomerId ,
        //                                    CustomerCode ", parameter.ToArray()).OrderBy(P => P.CreateDate).ToList<BY_Delivery>();
        //                List<BY_Delivery> by_deliverysupdates = database.FindListBySql<BY_Delivery>(@"
        //                            SELECT  *
        //                            FROM    dbo.BY_Delivery
        //                            WHERE   CreateDate >= @CreateDate1
        //                                    AND CreateDate <= @CreateDate2
        //                                    AND OrderCode IS NOT NULL
        //                                    ", parameter.ToArray());
        //                foreach (BY_Delivery by_delivery in by_deliverys)
        //                {
        //                    Base_Customer base_customer = database.FindEntity<Base_Customer>(by_delivery.CustomerId);
        //                    Base_CustomerAddress base_customeraddress = database.FindEntityBySql<Base_CustomerAddress>(string.Format(@"SELECT  *
        //FROM    dbo.Base_CustomerAddress
        //WHERE CustomerId='{0}'
        //ORDER BY MainDefault DESC", by_delivery.CustomerId));
        //                    if (base_customer == null || base_customer.CustomerId == null )
        //                    {
        //                        continue;
        //                        //return Content(new JsonMessage { Success = true, Code = "-1", Message = base_customer.Code + "客户信息不存在！" }.ToString());
        //                    }
        //                    BY_CheckSheet by_checksheet = new BY_CheckSheet();
        //                    by_checksheet.CheckSheetId = CommonHelper.GetGuid;
        //                    by_checksheet.CreateDate = DateTime.Now;
        //                    by_checksheet.CreateUserName = "System";
        //                    //by_checksheet.CheckSheetCode = BillCode();
        //                    by_checksheet.BeginDate = starttime;
        //                    by_checksheet.EndDate = endtime;
        //                    by_checksheet.CustomerId = by_delivery.CustomerId;
        //                    by_checksheet.CustomerCode = by_delivery.CustomerCode;
        //                    by_checksheet.CustomerName = base_customer.FullName;
        //                    by_checksheet.ShortName = base_customer.ShortName;
        //                    by_checksheet.ContactName = base_customeraddress.Contact;
        //                    by_checksheet.ContactTel = base_customeraddress.Telephone;
        //                    by_checksheet.ContactFax = base_customeraddress.Fax;
        //                    by_checksheet.ContactMail = base_customeraddress.EmailAddress;
        //                    by_checksheet.MobileCode = base_customeraddress.Mobile;
        //                    by_checksheet.CustomerAddress = base_customeraddress.DetailAddress;
        //                    by_checksheet.Qty = by_delivery.Qty;
        //                    by_checksheet.TotalAmount = by_delivery.TotalAmount;
        //                    by_checksheet.GetedAmount = 0;
        //                    by_checksheet.IsOver = 0;
        //                    //by_checksheet.LessRate
        //                    by_checksheet.LessedAmount = by_delivery.LessedAmount; ;
        //                    by_checksheet.PaymentMode = base_customer.PaymentMode;
        //                    by_checksheet.ShipMode = base_customer.TransitMode;
        //                    by_checksheet.ShipGroup = "北仑机械锻造厂";
        //                    by_checksheet.SalesMode = base_customer.Category;
        //                    by_checksheet.SalesMan = base_customer.Sellpeople;
        //                    by_checksheet.SalesArea = base_customer.Sellarea;
        //                    by_checksheet.Weight = by_delivery.Weight;
        //                    by_checksheet.ProcessingAlgorithm = base_customer.ProcessingAlgorithm;
        //                    by_checksheets.Add(by_checksheet);
        //                    //by_checksheet.Remark
        //                    //database.Insert<BY_CheckSheet>(by_checksheet, isOpenTrans);

        //                }

        //                by_checksheets = by_checksheetbll.GetCheckSheetCode(by_checksheets, by_checksheetsDelete, Year, Month);
        //                by_checksheets = by_checksheetbll.GetCheckSheetGetedMoney(by_checksheets, by_reimbursesheets, by_deliverysupdates);
        //                if (by_checksheetsDelete.Count >= 1)
        //                {
        //                    Hashtable ht = new Hashtable();
        //                    ht.Add("BeginDate", starttime);
        //                    database.Delete("BY_CheckSheet", ht, isOpenTrans);
        //                }
        //                database.Insert<BY_CheckSheet>(by_checksheets, isOpenTrans);
        //                foreach (BY_CheckSheet item in by_checksheets)
        //                {
        //                    List<BY_Delivery> by_deliverysupdate = by_deliverysupdates.Where(p => p.CustomerId == item.CustomerId).ToList<BY_Delivery>();
        //                    foreach (BY_Delivery itemd in by_deliverysupdate)
        //                    {
        //                        itemd.CheckSheetCode = item.CheckSheetCode;
        //                        database.Update<BY_Delivery>(itemd, isOpenTrans);
        //                    }

        //                }


        //                database.Commit();
        //                //Base_SysLogBll.Instance.WriteLog("checksheet", OperationType.Other, "-1", "成功生成对账单,时间："+DateTime.Now.ToString());
        //                return Content(new JsonMessage { Success = true, Code = "1", Message = "生成对账单成功！" }.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                database.Rollback();
        //                //Base_SysLogBll.Instance.WriteLog("checksheet", OperationType.Other, "-1", "生成对账单失败，" + ex.Message + "。时间：" + DateTime.Now.ToString());
        //                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败！" }.ToString());
        //            }
        //        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderId"></param>
        ///// <returns></returns>
        public ActionResult Customerdata(string StartTime, string EndTime, string LType)
        {
            DateTime starttime = Convert.ToDateTime(StartTime);
            DateTime endtime = Convert.ToDateTime(EndTime);
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate1", starttime));
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate2", endtime));
            string sql = @" 
SELECT  top 10  SUM(TotalAmount) TotalAmount ,
                    CustomerName
          FROM      dbo.BY_Delivery d
          WHERE     CreateDate > @CreateDate1  and d.DeleteMark = 0 
                    AND CreateDate < @CreateDate2 and Enabled =1
          
";
            if (!string.IsNullOrEmpty(LType))
            {
                sql += " and d.LType = @LType ";
                parameter.Add(DbFactory.CreateDbParameter("@LType", LType));
            }


            sql += @" GROUP BY  CustomerName
 ORDER BY TotalAmount DESC  ";

            List<BY_Delivery> by_checksheets = DataFactory.Database().FindListBySql<BY_Delivery>(sql, parameter.ToArray());




            //List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindListBySql<BY_DeliveryDetail>(" SELECT * FROM BY_DeliveryDetail WHERE CreateDate>=@CreateDate1 and CreateDate<=@CreateDate2  ");
            ArrayList categoriesName = new ArrayList();
            int i = 0;
            List<HighchartsData> highchartsDatas = new List<HighchartsData>();
            string[] colors = new string[] { "#058DC7", "#50B432", "#ED561B", "#DDDF00", "#24CBE5", "#64E572", "#FF9655", "#FFF263", "#6AF9C4", "#ED561B" };
            foreach (var item in by_checksheets)
            {
                categoriesName.Add(item.CustomerName);
                HighchartsData highchartsdata = new HighchartsData();
                highchartsdata.y = item.TotalAmount;
                highchartsdata.color = colors[i];
                highchartsDatas.Add(highchartsdata);
                i++;
            }
            var JsonData = new
            {
                datas = highchartsDatas,
                categoriesName = categoriesName
            };
            return Content(JsonData.ToJson());


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderId"></param>
        ///// <returns></returns>
        public ActionResult GetMetaChart(string StartTime, string EndTime, string LType)
        {
            DateTime starttime = Convert.ToDateTime(StartTime);
            DateTime endtime = Convert.ToDateTime(EndTime);
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate1", starttime));
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate2", endtime));

            string sql = @" SELECT TOP 10
        dd.SteelCode ,
        SUM(dd.Weight) Weight
FROM    BY_DeliveryDetail dd ,
        BY_Delivery d

WHERE dd.CreateDate >=@CreateDate1 and dd.CreateDate<=@CreateDate2  
and dd.DeliveryId = d.DeliveryId
        AND d.CustMaterial != 1    and d.DeleteMark = 0 
 ";

            if (!string.IsNullOrEmpty(LType))
            {
                 sql += " and d.LType = @LType ";
                parameter.Add(DbFactory.CreateDbParameter("@LType", LType));
            }

            sql += " GROUP BY dd.SteelCode ORDER BY Weight desc";

            List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindListBySql<BY_DeliveryDetail>(sql, parameter.ToArray());
            //List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindListBySql<BY_DeliveryDetail>(" SELECT * FROM BY_DeliveryDetail WHERE CreateDate>=@CreateDate1 and CreateDate<=@CreateDate2  ");
            ArrayList categoriesName = new ArrayList();
            int i = 0;
            List<HighchartsData> highchartsDatas = new List<HighchartsData>();
            string[] colors = new string[] { "#058DC7", "#50B432", "#ED561B", "#DDDF00", "#24CBE5", "#64E572", "#FF9655", "#FFF263", "#6AF9C4", "#ED561B" };
            foreach (var item in by_deliverydetails)
            {
                categoriesName.Add(item.SteelCode);
                HighchartsData highchartsdata = new HighchartsData();
                highchartsdata.y = item.Weight;
                highchartsdata.color = colors[i];
                highchartsDatas.Add(highchartsdata);
                i++;
            }
            var JsonData = new
            {
                datas = highchartsDatas,
                categoriesName = categoriesName
            };
            return Content(JsonData.ToJson());


        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderId"></param>
        ///// <returns></returns>
        public ActionResult GetchartCust(string Year, string CustomerId)
        {
            int Yes = Convert.ToInt32(Year);
            int Yee = Convert.ToInt32(Year) + 1;

            DateTime starttime = Convert.ToDateTime(Yes + "-" + "1-1" + " 00:00:00");
            DateTime endtime = DateTimeHelper.LastDayOfMonth(Convert.ToDateTime(Yee + "-" + "1-1" + " 00:00:00"));

            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate1", starttime));
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate2", endtime));
            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
            List<EarningsChartData> EarningsChartDataR = DataFactory.Database().FindListBySql<EarningsChartData>(@"  
    SELECT    a.Date ,
            SUM(TotalAmount) AS TotalAmount
  FROM      ( SELECT    CONVERT (VARCHAR(7), dd.CreateDate, 120) Date ,
                        SUM(dd.SalesAmount - ( dd.WorkCost + dd.MaterialCost + dd.ManagerCost
                             + dd.InvoiceCost +dd.HeatCost+dd.JKCostQty*2*dd.Qty)) TotalAmount
              FROM      BY_DeliveryDetail dd,BY_Delivery d
              WHERE     dd.DeliveryId = d.DeliveryId  and d.DeleteMark = 0 
                        AND dd.CreateDate >= @CreateDate1
                        AND dd.CreateDate <= @CreateDate2 and dd.Enabled =1  and CustomerId =@CustomerId
              GROUP BY  CONVERT (VARCHAR(7), dd.CreateDate, 120)
              UNION ALL
              SELECT    CONVERT (VARCHAR(7), CreateDate, 120) Date ,
                        SUM(TotalAmount) TotalAmount
              FROM      BY_CheckCompany
              WHERE     CreateDate >= @CreateDate1
                        AND CreateDate <= @CreateDate2 and CustomerId =@CustomerId
              GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120)
            ) a
  GROUP BY  a.Date ", parameter.ToArray());
            List<EarningsChartData> EarningsChartDataC = DataFactory.Database().FindListBySql<EarningsChartData>(@"  SELECT a.Date ,
        SUM(TotalAmount) AS TotalAmount
 FROM   ( 
          SELECT    CONVERT (VARCHAR(7), CreateDate, 120) Date ,
                    SUM(TotalAmount) TotalAmount
          FROM      Base_PayCompany
          WHERE     CreateDate >= @CreateDate1
                    AND CreateDate <= @CreateDate2 and CustomerId =@CustomerId
          GROUP BY  CONVERT (VARCHAR(7), CreateDate, 120)
          
        ) a
 GROUP BY a.Date
  ", parameter.ToArray());
            decimal[] EarningsR = new decimal[12];
            decimal[] EarningsC = new decimal[12];
            decimal[] EarningsChartDataTotal = new decimal[12];
            string[] names = new string[12];

            for (int i = 1; i <= 12; i++)
            {
                names[i - 1] = Year + "年" + i + "月";
                string date = Year;
                if (i < 10)
                {
                    date = date + "-" + "0" + i;
                }
                else
                {
                    date = date + "-" + i;
                }
                EarningsChartData dataR = new EarningsChartData();
                dataR.Date = date;
                dataR.TotalAmount = EarningsChartDataR.Where(p => p.Date == date).Count() > 0 ? EarningsChartDataR.Where(p => p.Date == date).FirstOrDefault<EarningsChartData>().TotalAmount : 0;
                EarningsR[i - 1] = dataR.TotalAmount;
                EarningsChartData dataC = new EarningsChartData();
                dataC.Date = date;
                dataC.TotalAmount = EarningsChartDataC.Where(p => p.Date == date).Count() > 0 ? EarningsChartDataC.Where(p => p.Date == date).FirstOrDefault<EarningsChartData>().TotalAmount : 0;
                EarningsC[i - 1] = dataC.TotalAmount;
                EarningsChartData dataT = new EarningsChartData();
                dataT.Date = date;
                dataT.TotalAmount = dataR.TotalAmount - dataC.TotalAmount;
                EarningsChartDataTotal[i - 1] = dataT.TotalAmount;
            }


            var JsonData = new
            {
                EarningsR = EarningsR,
                EarningsC = EarningsC,
                names = names,
                EarningsChartDataTotal = EarningsChartDataTotal
            };
            return Content(JsonData.ToJson());


        }

        [LoginAuthorize]
        public ActionResult GetGridPageJsonCust(string starttime, string endtime, string CustomerId, JqGridParam jqgridparam)
        {
            BY_DeliveryBll by_deliverybll = new Business.BY_DeliveryBll();
            if (string.IsNullOrEmpty(CustomerId))
            {
                 return Content(null);
            }
            DataTable by_orderdetail = by_deliverybll.GetGridPageJsonCust( starttime,endtime,  CustomerId, ref jqgridparam);
            Stopwatch watch = CommonHelper.TimerStart();
            var JsonData = new
            {
                total = jqgridparam.total,
                page = jqgridparam.page,
                records = jqgridparam.records,
                costtime = CommonHelper.TimerEnd(watch),
                rows = by_orderdetail,
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderId"></param>
        ///// <returns></returns>
        public ActionResult Getchart(string Year, string type, string type1)
        {
            int Yes = Convert.ToInt32(Year);
            int Yee = Convert.ToInt32(Year)+1;

            DateTime starttime = Convert.ToDateTime(Yes + "-" + "1-1" + " 00:00:00");
            DateTime endtime = DateTimeHelper.LastDayOfMonth(Convert.ToDateTime(Yee + "-" + "1-1" + " 00:00:00"));
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate1", starttime));
            parameter.Add(DbFactory.CreateDbParameter("@CreateDate2", endtime));

            string ss = "";
            string[] sss = type.Split(',');
            for (int i = 0; i < sss.Length; i++)
            {
                ss += "'" + sss[i] + "',";
            }
            ss = ss.Trim(',');

            string zz = "";
            string[] zzz = type1.Split(',');
            for (int i = 0; i < zzz.Length; i++)
            {
                zz += "'" + zzz[i] + "',";
            }
            zz = zz.Trim(',');

            string sql = @"  
    SELECT  a.Date ,
        SUM(TotalAmount) AS TotalAmount
FROM    ( ";
            if (type.IndexOf("材料销售")>=0)
	{
		 sql += @" SELECT    CONVERT (VARCHAR(7), d.CreateDate, 120) Date,
                    de.SalesAmount  TotalAmount 
                     
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > @CreateDate1 AND d.CreateDate < @CreateDate2 and d.LType ='GC'  and d.DeleteMark = 0 
          UNION ALL ";
	}

            if (type.IndexOf("热处理销售")>=0)
	{
		 sql += @" SELECT    CONVERT (VARCHAR(7), d.CreateDate, 120) Date,
                    de.SalesAmount  TotalAmount 
                     
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > @CreateDate1 AND d.CreateDate < @CreateDate2 and d.LType ='RCL'  and d.DeleteMark = 0 
          UNION ALL ";
	}
         sql += @"
         SELECT
	CONVERT (VARCHAR(7), RequiredDate, 120) Date,
	GetMoney TotalAmount
FROM BY_ReimburseSheet WHERE     RequiredDate >= @CreateDate1
                    AND RequiredDate <= @CreateDate2  and type1 in ( " + ss + @") 
                    
                            ) a
GROUP BY a.Date  ";

            List<EarningsChartData> EarningsChartDataR = DataFactory.Database().FindListBySql<EarningsChartData>(sql, parameter.ToArray());
            List<EarningsChartData> EarningsChartDataC = DataFactory.Database().FindListBySql<EarningsChartData>(@" 
SELECT  a.Date ,
        SUM(TotalAmount) AS TotalAmount
FROM    ( 

         SELECT
	CONVERT (VARCHAR(7), RequiredDate, 120) Date,
	GetMoney TotalAmount
FROM BY_PayReimburseSheet WHERE     RequiredDate >= @CreateDate1
                    AND RequiredDate <= @CreateDate2 and type1 in ( " + zz + @") 
                    
                            ) a
GROUP BY a.Date 
  ", parameter.ToArray());
            decimal[] EarningsR = new decimal[12];
            decimal[] EarningsC = new decimal[12];
            decimal[] EarningsChartDataTotal = new decimal[12];
            string[] names = new string[12];

            for (int i = 1; i <= 12; i++)
            {
                names[i - 1] = Year + "年" + i + "月";
                string date = Year;
                if (i < 10)
                {
                    date = date + "-" + "0" + i;
                }
                else
                {
                    date = date + "-" + i;
                }
                EarningsChartData dataR = new EarningsChartData();
                dataR.Date = date;
                dataR.TotalAmount = EarningsChartDataR.Where(p => p.Date == date).Count() > 0 ? EarningsChartDataR.Where(p => p.Date == date).FirstOrDefault<EarningsChartData>().TotalAmount : 0;
                EarningsR[i - 1] = dataR.TotalAmount;
                EarningsChartData dataC = new EarningsChartData();
                dataC.Date = date;
                dataC.TotalAmount = EarningsChartDataC.Where(p => p.Date == date).Count() > 0 ? EarningsChartDataC.Where(p => p.Date == date).FirstOrDefault<EarningsChartData>().TotalAmount : 0;
                EarningsC[i - 1] = dataC.TotalAmount;
                EarningsChartData dataT = new EarningsChartData();
                dataT.Date = date;
                dataT.TotalAmount = dataR.TotalAmount - dataC.TotalAmount;
                EarningsChartDataTotal[i - 1] = dataT.TotalAmount;
            }


            var JsonData = new
            {
                EarningsR = EarningsR,
                EarningsC = EarningsC,
                names = names,
                EarningsChartDataTotal = EarningsChartDataTotal
            };
            return Content(JsonData.ToJson());


        }
        [LoginAuthorize]
        public virtual JsonResult GridPageJsonForMobile(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                List<BY_CheckSheet> ListData = new List<BY_CheckSheet>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
                    {
                        WhereSql += string.Format(" AND CustomerCode='{0}' ", DNS.Utilities.ManageProvider.Provider.Current().UserId);
                    }
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    ListData = repositoryfactory.Repository().FindListPage(ref jqgridparam);
                }
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    rows = ListData,
                };
                return Json(JsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }
    }
    public class EarningsChartData
    {
        public decimal TotalAmount { get; set; }
        public string Date { get; set; }
    }

    public class HighchartsData
    {
        public decimal? y { get; set; }
        public string color { get; set; }
        public Drilldown drilldown { get; set; }
    }
    public class Drilldown
    {
        public string name { get; set; }
        public ArrayList categories { get; set; }
        public ArrayList data { get; set; }
        public string color { get; set; }
    }


}
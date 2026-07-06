using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using Microsoft.Ajax.Utilities;
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

namespace DNS.WebApp.Areas.ReportModule.Controllers
{
    /// <summary>
    /// 送货单列印清单
    /// </summary>
    public class DeliveryController : PublicController<BY_Delivery>
    {
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QuoSummary()
        {
            return View();
        }


        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SummeryDetail()
        {
            return View();
        }
        public ActionResult SummeryDetail1()
        {
            return View();
        }
        public ActionResult QuoSummarygrid(string Sellpeople, string CreateUserName, string StartTime, string EndTime, string LType, string fullname, JqGridParam jqgridparam)
        {

            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@"
select * from (

SELECT  c.Code ,
        c.FullName ,
        c.Sellpeople ,
        c.Category ,

        ( SELECT    SUM(ISNULL(dd.Weight, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'GC' and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus = '已落单'   
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) gcweight ,
         ( SELECT    SUM(ISNULL(dd.SalesAmount, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'GC' and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus = '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) gcamount ,
( SELECT    SUM(ISNULL(dd.Weight, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'GC' and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus != '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) ngcweight ,
         ( SELECT    SUM(ISNULL(dd.SalesAmount, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'GC' and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus != '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) ngcamount ,
( SELECT    SUM(ISNULL(dd.Weight, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'GC' and dd.SalesAmount = 0
                    AND D.QuotationId = DD.QuotationId
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) gcweightfree ,





        ( SELECT    SUM(ISNULL(dd.Weight, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'LJ'  and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus = '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) ljweight ,
         ( SELECT    SUM(ISNULL(dd.SalesAmount, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'LJ'  and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus = '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) ljamount ,
( SELECT    SUM(ISNULL(dd.Weight, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'LJ'  and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus != '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) nljweight ,
         ( SELECT    SUM(ISNULL(dd.SalesAmount, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'LJ'  and dd.SalesAmount != 0
                    AND D.QuotationId = DD.QuotationId
                    and d.BillStatus != '已落单'
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) nljamount ,
 ( SELECT    SUM(ISNULL(dd.Weight, 0))
          FROM      BY_QuotationDetail dd ,
                    dbo.BY_Quotation d
          WHERE     d.LType = 'LJ'  and dd.SalesAmount = 0
                    AND D.QuotationId = DD.QuotationId
                    @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code  AND d.DeleteMark = 0
        ) ljweightfree ,

        ( SELECT    SUM(ISNULL(d.Weight, 0))
          FROM       
                    dbo.BY_QuotationHeat d
          WHERE     1=1  and d.SalesAmount != 0
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code
        ) rclweight ,
( SELECT    SUM(ISNULL(d.SalesAmount, 0))
          FROM       
                    dbo.BY_QuotationHeat d
          WHERE     1=1 and d.SalesAmount != 0
                     @StartTime
                    @EndTime
                    AND d.CustomerCode = c.Code
        ) rclamount 

FROM    dbo.Base_Customer c
        where 1=1 

 )a 
                where
 gcweight > 0 or   gcamount  > 0 or ngcweight  > 0 or  ngcamount  > 0 or gcweightfree   >  0  or ljweight   > 0 or ljamount  > 0 or nljweight  > 0 or  nljamount  > 0 or ljweightfree   > 0 or   rclweight   > 0 or rclamount > 0 
 
 ");

                if (!string.IsNullOrEmpty(Sellpeople))
                {
                    strSql.Append(" AND c.Sellpeople  = @Sellpeople ");
                    parameter.Add(DbFactory.CreateDbParameter("@Sellpeople", Sellpeople));
                }
                if (!string.IsNullOrEmpty(fullname))
                {
                    strSql.Append(" AND c.fullname  like @fullname ");
                    parameter.Add(DbFactory.CreateDbParameter("@fullname", '%' + fullname + '%'));
                }

                strSql.Replace("@StartTime", string.IsNullOrEmpty(StartTime) ? "" : " AND d.CreateDate >= '" + StartTime + "' ");

                if (!string.IsNullOrEmpty(EndTime))
                {
                    DateTime dt = CommonHelper.GetDateTime(EndTime).AddDays(1);
                    strSql.Replace("@EndTime", " AND d.CreateDate < '" + dt.ToShortDateString() + "' ");
                }
                else
                {
                    strSql.Replace("@EndTime", " ");

                }

                DataTable ListData = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);

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



        public ActionResult SummeryDetailgrid(string Sellpeople, string CreateUserName, string StartTime, string EndTime, string LType, string fullname, JqGridParam jqgridparam)
        {

            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@"
SELECT dd.steelcode,d.SalesMan,   SUM(ISNULL(dd.MaterialAmount, 0)) MaterialAmount,SUM(ISNULL(dd.Weight, 0)) Weight
          FROM      BY_DeliveryDetail dd ,
                    dbo.BY_Delivery d
          WHERE     D.DeliveryId = DD.DeliveryId
                    AND d.enabled = 1  and d.DeleteMark = 0 
                     @StartTime
                    @EndTime
                    @Sellpeople
                    group by dd.steelcode,d.SalesMan



 ");
                strSql.Replace("@Sellpeople", string.IsNullOrEmpty(Sellpeople) ? "" : " AND d.Sellpeople = '" + Sellpeople + "' ");

                strSql.Replace("@StartTime", string.IsNullOrEmpty(StartTime) ? "" : " AND d.CreateDate >= '" + StartTime + "' ");
                if (!string.IsNullOrEmpty(EndTime))
                {
                    DateTime dt = CommonHelper.GetDateTime(EndTime).AddDays(1);
                    strSql.Replace("@EndTime", " AND d.CreateDate < '" + dt.ToShortDateString() + "' ");
                }
                else
                {
                    strSql.Replace("@EndTime", " ");

                }

                DataTable ListData = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);

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
        public ActionResult SummeryDetailgrid2(string Sellpeople, string CreateUserName, string StartTime, string EndTime, string LType, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@"SELECT  qd.SteelDescription ,
        d.CreateUserName ,qd.Technology,
        SUM(qd.Weight)/1000 Weight, SUM(qd.SalesAmount)/10000 TotalAmount,c.Sellarea
FROM    BY_Delivery d
        LEFT JOIN dbo.BY_DeliveryDetail qd ON d.DeliveryId = qd.DeliveryId
        LEFT
JOIN Base_Customer c ON c.Code = d.CustomerCode
        JOIN dbo.Base_Steel s ON qd.SteelCode = s.SteelCode
WHERE   d.enabled = 1 and d.DeleteMark = 0 
        AND qd.SteelCode != 'FY'
 ");
                if (!string.IsNullOrEmpty(CreateUserName))
                {
                    strSql.Append(" AND d.CreateUserName  = @CreateUserName ");
                    parameter.Add(DbFactory.CreateDbParameter("@CreateUserName", CreateUserName));
                }
                if (!string.IsNullOrEmpty(StartTime))
                {
                    strSql.Append(" AND d.CreateDate >= @StartTime ");
                    parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    strSql.Append(" AND d.CreateDate <= @EndTime ");
                    parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime + " 23:59"));
                }
                if (!string.IsNullOrEmpty(LType))
                {
                    strSql.Append(" AND d.LType = @LType ");
                    parameter.Add(DbFactory.CreateDbParameter("@LType", LType));
                }
                strSql.Append(@" GROUP BY d.CreateUserName ,
        SteelDescription ,qd.Technology,c.Sellarea
  ");
                DataTable ListData = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);

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



        public ActionResult SummeryDetailgrid3(string Driver, string StartTime, string EndTime, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@"	select a.Driver,a.Qty,a.Weight,b.Awayfrom from (
		select d1.Driver,sum(d1.Weight) Weight,sum(d1.Qty) Qty
		from BY_Delivery d1
		 where 1=1 @Driver1 @StartTime1 @EndTime1 and d1.DeleteMark = 0 
	  group by d1.Driver) a
	  left join (
	  select Driver,sum(Awayfrom) Awayfrom from 
		(select  d.Driver,MAX(C.Awayfrom) Awayfrom from BY_Delivery d,Base_Customer C
		 where D.DriverPath IS NOT NULL AND D.DriverPath  != '' AND D.DriverpATH != '&nbsp;' @Driver @StartTime @EndTime AND D.CustomerId = C.CustomerId  and d.DeleteMark = 0 
	  group by d.Driver, D.DriverPath)a group by Driver) b on a.Driver = b.Driver
 ");
                if (!string.IsNullOrEmpty(Driver))
                {
                    strSql.Replace("@Driver1", " and d1.Driver = '"+ Driver + "' ");
                    strSql.Replace("@Driver", " and d.Driver = '" + Driver + "' ");
                }
                else
                {
                    strSql.Replace("@Driver1", "");
                    strSql.Replace("@Driver", "");

                }
                if (!string.IsNullOrEmpty(StartTime))
                {
                    strSql.Replace("@StartTime1", " and d1.CreateDate >= '" + StartTime + "' ");
                    strSql.Replace("@StartTime", " and d.CreateDate >= '" + StartTime + "' ");
                }
                else
                {
                    strSql.Replace("@StartTime1", "");
                    strSql.Replace("@StartTime", "");
                }

                if (!string.IsNullOrEmpty(EndTime))
                {
                    strSql.Replace("@EndTime1", " and d1.CreateDate <= '" + EndTime + "' ");
                    strSql.Replace("@EndTime", " and d.CreateDate <= '" + EndTime + "' ");
                }
                else
                {
                    strSql.Replace("@EndTime1", "");
                    strSql.Replace("@EndTime", "");
                }

                DataTable ListData = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);

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


        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult FormSummary()
        {
            return View();
        }
        public DataTable BuildSummaryTable(string StartTime, string EndTime)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT  SUM(de.MaterialAmount) MaterialAmount,  --材料费
        SUM(de.WorkAmount) WorkAmount,      --加工费
        SUM(de.HeatAmount) HeatAmount,      --热处理费
        SUM(de.SalesAmount) SalesAmount,     --送货单金额
        SUM(de.WorkCost) WorkCost,
        SUM(de.MaterialCost) MaterialCost,
        SUM(de.ManagerCost) ManagerCost,
        SUM(de.InvoiceCost) InvoiceCost,
        SUM(de.HeatCost) HeatCost,
        SUM(de.WorkCost) + SUM(de.MaterialCost) + SUM(de.ManagerCost)
        + SUM(de.InvoiceCost) + SUM(de.HeatCost) AS TotalCost ,
        SUM(de.SalesAmount) - ( SUM(de.WorkCost) + SUM(de.MaterialCost)
                                + SUM(de.ManagerCost) + SUM(de.InvoiceCost)
                                + SUM(de.HeatCost) ) AS yingli
FROM    dbo.BY_DeliveryDetail de
        INNER JOIN dbo.BY_Delivery d ON de.DeliveryId = d.DeliveryId
        INNER JOIN dbo.BY_Quotation q ON d.QuotationCode = q.QuotationCode
        INNER JOIN dbo.BY_QuotationDetail qd ON q.QuotationId = qd.QuotationId
                                                AND de.LineCode = qd.LineCode
                                                 ");
            strSql.Append(" WHERE 1=1  AND q.DeleteMark = 0  and d.DeleteMark = 0 ");

            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql.Append(" AND d.CreateDate > @StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql.Append(" AND d.CreateDate < @EndTime ");

                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
            DataTable dt = DataFactory.Database().FindTableBySql(strSql.ToString(), parameter.ToArray());
            return dt;
        }
        public ActionResult BuildSummary(string StartTime, string EndTime)
        {
            try
            {
                DataTable ListData = this.BuildSummaryTable(StartTime, EndTime);
                return Content(ListData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 报价单录入视图页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SummaryCost()
        {
            return View();
        }
        public ActionResult GridPageListJson1(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = this.GetPageList(QueryParameter, ref jqgridparam);
                for (int i = 0; i < ListData.Rows.Count; i++)
                {
                    List<Base_DataDictionaryDetail> base_datadictionarydetails = DataFactory.Database().FindListBySql<Base_DataDictionaryDetail>("SELECT *  FROM [Base_DataDictionaryDetail]  WHERE DataDictionaryId = 'f2aca0a6-2802-4b8b-a682-f044e574d964' order by SortCode asc ");

                    //string str = "NC(2.00*44.00*2.00=176.0000)&lt;br&gt;冲孔(4.98*12.00*2.00=119.5200)&lt;br&gt;斜孔(4.98*3.00*2.00=29.8800)&lt;br&gt;";
                    string[] arr = ListData.Rows[i]["process"].ToString().Split(new string[] { "&lt;br&gt;" }, StringSplitOptions.RemoveEmptyEntries);
                    string name = "";
                    decimal allqty = 0;
                    decimal detailqty = 0;
                    foreach (var item in arr)
                    {
                        name = item.Split('(')[0];
                        string a = item.Split('=')[0].Split('*')[2];
                        allqty += CommonHelper.GetDecimal(a);
                    }

                    //ListData.Rows[i]["process"].ToString().Split()
                    DataTable Ls = DataFactory.Database().FindTableBySql(string.Format("select * from BY_OrderDetailProcessDetail where OrderDetailId = '{0}'", ListData.Rows[i]["OrderDetailId"].ToString()));
                    decimal WorkCost = 0;

                    if (Ls.Rows.Count > 0)
                    {
                        ListData.Rows[i]["process2"] = "";
                        for (int j = 0; j < Ls.Rows.Count; j++)
                        {
                            string startTimeStr = "";
                            string endTimeStr = "";
                            if (Ls.Rows[j]["OperatorDate"] != null)
                            {
                                startTimeStr = Ls.Rows[j]["OperatorDate"].ToString();
                            }

                            if (Ls.Rows[j]["OperatorOverDate"] != null)
                            {
                                endTimeStr = Ls.Rows[j]["OperatorOverDate"].ToString();

                            }

                            decimal totalSeconds = 0;

                            if (string.IsNullOrEmpty(startTimeStr) || string.IsNullOrEmpty(endTimeStr))
                            {

                                totalSeconds = 0;
                                WorkCost = -1;
                            }
                            else
                            {
                                DateTime startTime = DateTime.Parse(startTimeStr);
                                DateTime endTime = DateTime.Parse(endTimeStr);

                                TimeSpan timeSpan = endTime - startTime;
                                totalSeconds = Math.Round(Convert.ToDecimal(timeSpan.TotalSeconds / 60), 2, MidpointRounding.AwayFromZero); ;
                            }

                            var data = base_datadictionarydetails.Where(p => p.FullName == Ls.Rows[j]["TechnologyName"].ToString()).FirstOrDefault();
                            decimal? SalePer = 0;
                            if (data != null && CommonHelper.GetDecimal(data.Money) > 0)
                            {
                                SalePer = data.Money;
                            }
                            else
                            {
                                WorkCost = -1;
                            }

                            decimal totals = Math.Round(Convert.ToDecimal(totalSeconds * CommonHelper.GetDecimal(SalePer)), 2, MidpointRounding.AwayFromZero);
                            if (WorkCost != -1)
                            {
                                WorkCost += totals;
                            }
                            ListData.Rows[i]["process2"] += Ls.Rows[j]["TechnologyName"] + "( " + (totalSeconds != 0? totalSeconds.ToString():"未完工扫描") + "*" + ((SalePer== 0|| SalePer ==null )?"未设置成本":SalePer.ToString()) + "*" + Ls.Rows[j]["Qty"] + "=" + totals + ")&lt;br&gt;";
                            detailqty += Ls.Rows[j]["Qty"] != null ? CommonHelper.GetDecimal(Ls.Rows[j]["Qty"]) : 0;
                        }
                    }
                    else
                    {
                        ListData.Rows[i]["process2"] = "(无扫描信息)";

                    }
                    ListData.Rows[i]["workcost"] = WorkCost > 0  && detailqty == allqty ? WorkCost : -1;
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
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 送货单列印清单列表（返回表格JSON）
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            Stopwatch watch = CommonHelper.TimerStart();
            DataTable ListData = this.GetPageList(QueryParameter, ref jqgridparam);
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
        /// <summary>
        /// 送货单列印清单列表
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"

SELECT  d.createdate ,   --落单日期
        d.SalesArea ,        --区域
        d.CustomerCode ,     --客户编号 
        d.CustomerName ,     --客户名称
        d.SalesMan ,         --销售员
        d.QuotationCode ,    --报价单号
        d.OrderCode ,        --订单号
        d.CustomerPO ,       --客户PO
        de.LineCode ,        --行号
        d.DeliveryCode ,     --送货单号
        de.SteelCode ,       --钢种 
        de.SteelState ,      --状态
        de.Thick ,           --厚
        de.Weigh ,           --宽
        de.Length ,          --长
        de.Qty ,             --数量
        de.Weight ,          --重量
        de.Weight Weight1 ,          --重量
        de.Weight5 ,          --重量
        de.MaterialPrice,   --材料单价
        de.MaterialAmount,
        de.OrderDetailId,
        de.WorkAmount ,      --加工费
        de.HeatPrice ,
        de.HeatAmount ,      --热处理费
        de.SalesAmount ,     --送货单金额
        de.JKCost ,
        d.PrintDate ,         --送货单打印日期
        de.JKCostQty * 2 * de.Qty AS jkcc ,
        de.WorkCost ,
        de.MaterialCost ,
        de.ManagerCost ,
        de.InvoiceCost ,
        
        '无' as process2,
        d.MnemonicCode ,
        de.ProductName ,
        de.CustomerProduct ,
        de.ShipDescription ,
        de.HeatCost ,
        de.MaterialCostPer ,
        de.HeatCostPer ,
        de.WorkCostPer ,
        de.JJtype ,
        de.WorkPrice ,
        de.DeliveryId ,
        d.shipclause ,
        ( SELECT    Sellpeople
          FROM      Base_Customer
          WHERE     Code = d.CustomerCode
        ) Sellpeople ,
        de.gWeight ,
        de.miancostarea ,
        CASE WHEN de.beoutworkcost > 0 THEN 0
             ELSE de.NBworkcost
        END NBworkcost ,
        de.beoutworkcost ,
        de.HeatWorkCost,d.CustMaterial,de.Technology,d.FreeBill
,  (SELECT STUFF((SELECT ' ' +bp.TechnologyName + '(' + CAST((bp.SaleMc) AS VARCHAR(50)) + '*' + CAST(bp.SalePer AS VARCHAR(50)) + '*' + CAST((bp.Qty) AS VARCHAR(50)) + '=' + CAST((bp.SaleTotal * bp.Qty) AS VARCHAR(50)) + ')<br>' FROM BY_OrderDetailProcess bp where bp.OrderDetailId = de.OrderDetailId for xml path('')),1,1,'') ) process
FROM    dbo.BY_DeliveryDetail de
        INNER JOIN dbo.BY_Delivery d ON de.DeliveryId = d.DeliveryId
WHERE   1 = 1  and d.DeleteMark = 0 and d.Enabled > 0 ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["ProductName"]))
            {
                strSql.Append(" AND de.ProductName LIKE @ProductName");
                parameter.Add(DbFactory.CreateDbParameter("@ProductName", Condition["ProductName"].ToString().Trim() + '%'));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND d.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " 00:00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " 23:59")));
            }
            //客户编号
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (d.CustomerName LIKE @CustomerCode OR d.CustomerCode LIKE @CustomerCode OR d.ShortName LIKE @CustomerCode)");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", '%' + Condition["CustomerCode"].ToString() + '%'));
            }


            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND d.LType = @LType ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["SteelCode"]))
            {
                strSql.Append(" AND de.SteelCode = @SteelCode");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", Condition["SteelCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["SteelType"]))
            {
                strSql.Append(" AND EXISTS (SELECT 1 FROM dbo.Base_Steel bs WHERE bs.SteelCode = de.SteelCode AND bs.SteelType = @SteelType) ");
                parameter.Add(DbFactory.CreateDbParameter("@SteelType", Condition["SteelType"].ToString().Trim()));
            }

            if (!CommonHelper.IsEmpty(Condition["OrderCode"]))
            {
                //单据类型
                string BillType = Condition["BillType"].ToString();
                switch (BillType)
                {
                    case "1"://加工单号
                        strSql.Append(" AND d.OrderCode LIKE @OrderCode");
                        parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
                        break;
                    case "2"://合同编号
                        strSql.Append(" AND d.QuotationCode LIKE @QuotationCode");
                        parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["OrderCode"].ToString() + '%'));
                        break;
                    case "3"://送货单号
                        strSql.Append(" AND d.DeliveryCode LIKE @DeliveryCode");
                        parameter.Add(DbFactory.CreateDbParameter("@DeliveryCode", '%' + Condition["OrderCode"].ToString() + '%'));
                        break;
                    default:
                        break;
                }
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["Simple"]))
            {
                //单类
                string Simple = Condition["Simple"].ToString();
                switch (Simple)
                {
                    case "1"://全部单类
                        break;
                    case "2"://粗料单
                        strSql.Append(" AND de.OrderType = '毛料生产'");
                        break;
                    case "3"://精料单
                        strSql.Append(" AND de.OrderType != '毛料生产'");
                        break;
                    default:
                        break;
                }
            }
            //客户编号
            if (!HashtableHelper.IsNullOrDBNull(Condition["Size"]))
            {
                string[] Sizes = Condition["Size"].ToString().Replace(" ", "").Split('*');
                if (Condition["Size"].ToString().IndexOf('Φ') > -1)
                {
                    strSql.Append(" AND de.Length like @Length AND de.Weigh=0 AND de.Thick like @Thick  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[1].Replace("Φ", "") + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[0].Replace("Φ", "") + "%"));
                }
                else
                {
                    strSql.Append(@"  AND ((de.Length like @Length AND de.Weigh like @Weigh AND de.Thick like @Thick) or  
(de.Length like @Length AND de.Weigh like @Thick AND de.Thick like @Weigh) or 

(de.Length like @Weigh AND de.Weigh like @Length AND de.Thick like @Thick) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length) or 

(de.Length like @Weigh AND de.Weigh like @Thick AND de.Thick like @Length) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length)  )  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                }

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND ( d.MnemonicCode LIKE @MnemonicCode or d.QuotationCode LIKE @MnemonicCode ) ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", '%' + Condition["MnemonicCode"].ToString().Trim() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["salesman"]))
            {
                strSql.Append(" AND d.SalesMan  = @SalesMan ");
                parameter.Add(DbFactory.CreateDbParameter("@SalesMan", Condition["salesman"].ToString().Trim()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["WorkTypeA"]))
            {
                if (Condition["WorkTypeA"].ToString() == "1")
                {
                    strSql.Append(@" AND  ( de.SalesAmount - ( de.WorkCost + de.MaterialCost + de.ManagerCost
                             + de.InvoiceCost +de.HeatCost+de.JKCostQty*2*de.Qty) ) <= 0 ");
                }
            }
            DataTable dt = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
            return dt;
        }
    }
}

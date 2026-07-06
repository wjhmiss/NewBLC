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

namespace DNS.WebApp.Areas.ReportModule.Controllers
{
    /// <summary>
    /// 生产报表
    /// </summary>
    public class ProductionController : PublicController<BY_Order>
    {

        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult WorkDetail()
        {
            return View();
        }

        /// <summary>
        /// 生产报表列表（返回表格JSON）
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public ActionResult GridPageListWorkDetail(string Condition, JqGridParam jqgridparam)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            Stopwatch watch = CommonHelper.TimerStart();
            DataTable ListData = this.GetPageWorkDetailList(QueryParameter, ref jqgridparam);
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
        public ActionResult GridPageListWorkDetail1()
        {
            DataTable ListData = this.GetPageWorkDetailList1();
            return Content(ListData.ToJson());
        }
        public DataTable GetPageWorkDetailList1()
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT  WorkerCode ,
        WorkerName ,
        ISNULL(( SELECT SUM(opd.Weight)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) AS K001 ,--开料
        ISNULL(( SELECT SUM(opd.Area)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND TechnologyId = 'X002'
               ), 0) AS X002 ,--铣边
        ISNULL(( SELECT SUM(opd.Area)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND TechnologyId = 'J001'
               ), 0) AS J001 ,--精铣
        ISNULL(( SELECT SUM(opd.Area)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND TechnologyId = 'P001'
               ), 0) AS P001 ,--铣平面
        ISNULL(( SELECT SUM(opd.Area)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND TechnologyId = 'M003'
               ), 0) AS M003 ,--磨平面
        ISNULL(( SELECT SUM(opd.Area)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND TechnologyId = 'FX01'
               ), 0) AS FX01 ,--返修
        ISNULL(( SELECT SUM(opd.Qty)
                 FROM   BY_OrderDetailProcessDetail opd
                 WHERE  opd.OperatorId = WorkerCode
                        AND CreateDate > @starttime
                        AND CreateDate < @endtime
                        AND TechnologyId = 'JY01'
               ), 0) AS JY01 --检验
FROM    BY_Worker w
 WHERE w.DepartId='f9744b4d-fa9b-468e-9b88-663d28392f49' 

 ");
            parameter.Add(DbFactory.CreateDbParameter("@starttime", Convert.ToDateTime(DNS.Utilities.DateTimeHelper.GetToday() + " 07:00:00")));
            parameter.Add(DbFactory.CreateDbParameter("@endtime", Convert.ToDateTime(DNS.Utilities.DateTimeHelper.GetDate(1) + " 07:00:00")));
            
            DataTable dt = repositoryfactory.Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
            return dt;
        }
        /// <summary>
        /// 生产报表列表
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageWorkDetailList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            //            ( SELECT TOP 1
            //            CreateDate
            //  FROM      BY_OrderDetailProcessDetail opd
            //  WHERE     opd.OrderDetailId = od.OrderDetailId
            //  ORDER BY  CreateDate DESC
            //) createdate ,
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"   SELECT    o.OrderCode ,--订单号
            od.LineCode ,--行号
            o.CustomerCode ,--客户编号
            od.BillStatus ,
            ( SELECT TOP 1
                        '已' + TechnologyName
              FROM      BY_OrderDetailProcessDetail opd
              WHERE     opd.OrderDetailId = od.OrderDetailId
              ORDER BY  CreateDate DESC
            ) BillStatus1 ,--当前状态 
            o.MnemonicCode ,
            od.Qty ,
            od.IsBeOut ,
            od.ordertype ,

            od.Thick ,
            od.Weigh ,
            od.Length ,
            o.CustomerName ,
            od.SteelDescription ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
                   ), 0) AS K001 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
                   ), 0) AS KQty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'P001'
                   ), 0) AS P001 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'P001'
                   ), 0) AS P001Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'CX01'
                   ), 0) AS CX01 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'CX01'
                   ), 0) AS CX01Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'YM01'
                   ), 0) AS YM01 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'YM01'
                   ), 0) AS YM01Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'J001'
                   ), 0) AS J001 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'J001'
                   ), 0) AS J001Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'D001'
                   ), 0) AS D001 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'D001'
                   ), 0) AS D001Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'DK01'
                   ), 0) AS DK01 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'DK01'
                   ), 0) AS DK01Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'CNC'
                   ), 0) AS CNC ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'CNC'
                   ), 0) AS CNCQty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'YM02'
                   ), 0) AS YM02 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'YM02'
                   ), 0) AS YM02Qty ,
            ISNULL(( SELECT SUM(OPD.Qty)
                     FROM   BY_OrderDetailProcessDetail opd
                     WHERE  opd.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'JX01'
                   ), 0) AS JX01 ,
            ISNULL(( SELECT OP.Qty
                     FROM   BY_OrderDetailProcess OP
                     WHERE  OP.OrderDetailId = od.OrderDetailId
                            AND TechnologyId = 'JX01'
                   ), 0) AS JX01Qty ,
            od.ProductName,
            o.CreateDate
  FROM      dbo.BY_Order o
            INNER JOIN dbo.BY_OrderDetail od ON o.OrderId = od.OrderId
  WHERE     OD.Length > 0 and o.DeleteMark=0 

 ");

            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                string[] temp = Condition["OrderCode"].ToString().Split('-');
                //if (temp.Length == 2)
                //{
                    strSql.Append(" AND o.OrderCode like '%'+@OrderCode +'%'");
                    parameter.Add(DbFactory.CreateDbParameter("@OrderCode", Condition["OrderCode"].ToString()));
                //}
                if (temp.Length == 3)
                {
                    strSql.Append(" AND o.OrderCode = @OrderCode and od.LineCode = @LineCode");
                    parameter.Add(DbFactory.CreateDbParameter("@OrderCode", temp[0] + "-" + temp[1]));
                    parameter.Add(DbFactory.CreateDbParameter("@LineCode", temp[2]));
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_StartTime"]))
            {
                strSql.Append(" AND o.CreateDate >  @Delivery_StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_StartTime", Condition["Delivery_StartTime"].ToString()+" 00:00:00"));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_EndTime"]))
            {
                strSql.Append(" AND  o.CreateDate <  @Delivery_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_EndTime", Condition["Delivery_EndTime"].ToString() + " 23:59:59"));
            }
            //客户编号
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerName"]))
            {
                strSql.Append(" AND (O.CustomerCode LIKE '%'+@CustomerName+'%' or  O.ShortName LIKE '%'+@CustomerName+'%' or O.CustomerName LIKE '%'+@CustomerName+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", Condition["CustomerName"].ToString()));
            }
            //客户编号
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND (o.MnemonicCode LIKE '%'+ @MnemonicCode+'%' or o.QuotationCode LIKE '%'+ @MnemonicCode+'%') ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCodes"]))
            {
                BY_Machine by_machine = DataFactory.Database().FindEntity<BY_Machine>("LED");
                string[] MnemonicCodes = by_machine.OrderCodeLine.Split(',');
                string temps = "";
                for (int i = 0; i < MnemonicCodes.Length; i++)
                {
                    temps += "'" + MnemonicCodes[i] + "',";
                }
                temps = temps.Trim(',');
                if (temps == "")
                {
                    temps = "''";
                }
                strSql.AppendFormat(" AND (o.MnemonicCode in ({0}) or o.QuotationCode in ({0}) ) ", temps);
            }
            DataTable dt = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
            return dt;
        }

        /// <summary>
        /// 生产报表列表（返回表格JSON）
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
        /// 生产报表列表
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  
od.orderdetailid,
o.OrderCode,--订单号
                                    od.LineCode,--行号
                                    o.ShortName,--客户简称
                                    q.SalesArea,  --销售区域                                 
                                    o.CustomerCode,--客户编号
                                    od.SteelCode,--钢种
                                    od.DieCode,--模号
                                   CASE WHEN od.Length > 0
                     THEN CASE WHEN od.Weigh = 0
                               THEN 'Φ'
                                    + CAST(CONVERT(FLOAT, od.Thick) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, od.Length) AS VARCHAR(10))
                               ELSE CAST(CONVERT(FLOAT, od.Length) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, od.Weigh) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, od.Thick) AS VARCHAR(10))
                          END
                     ELSE ''
                END AS SIZE ,--尺寸 AS  ,
                                    qd.Technology,--热处理：有值勾上，空值不勾选
                                    od.Qty -ISNULL(od.CompleteQty,0) AS UnCompleteQty,--未完成数量
                                    od.Qty -ISNULL(od.OpenQty,0) AS UnOpenQty,--未开料数
                                    od.CompleteQty,
                                    od.OpenQty,--开料数
                                    od.BillStatus,--当前状态
                                    o.CreateDate,--落单日期
									isnull( od.Delivery,o.Delivery) Delivery,--要求完工期,
                                    case when len(od.Delivery)=10 or len(od.Delivery)=9 or len(od.Delivery)=8 then
                                    - DATEDIFF(DAY,CONVERT(DATETIME,LEFT(isnull( od.Delivery,o.Delivery),10)), GETDATE())
									when len(od.Delivery)=7 then
									 - DATEDIFF(DAY,CONVERT(DATETIME,LEFT(isnull( od.Delivery,o.Delivery),10), 1), GETDATE()) end AS OverDay ,--过期天数
                                    od.Weight,--重量
                                    q.ShipRoute,--路线
                                    q.Remark,--备注
        (select isnull(sum(QTY),0) from (
SELECT isnull(QTY,0) QTY FROM Base_SteelDDBeoutDetail OB WHERE OB.QuotationDetailId = od.QuotationDetailId
union all
SELECT isnull(QTY,0) QTY FROM Base_SteelDDPerchaseDetail op WHERE op.QuotationDetailId = od.QuotationDetailId) a) AS isbeout,o.MnemonicCode,od.ProductName,od.qty,od.ordertype,
                                    od.customerproduct,
                                    (SELECT top 1 d.CreateDate FROM dbo.BY_DeliveryDetail d WHERE d.OrderDetailId = od.OrderDetailId ) ShipCreateDate,

                                            ( SELECT TOP 1
                    '已' + TechnologyName
          FROM      BY_OrderDetailProcessDetail opd
          WHERE     opd.OrderDetailId = od.OrderDetailId
          ORDER BY  CreateDate DESC
        ) BillStatus1  --当前状态 
,
        ISNULL(( SELECT SUM(odd.Qty)
                 FROM   BY_OrderDetailProcessDetail odd
                 WHERE  odd.OrderDetailId = od.OrderDetailId
                        AND odd.LineCode = od.LineCode
                        AND (odd.TechnologyId = 'K001' or odd.TechnologyName = '锯床' or odd.TechnologyId = 'FG' or odd.TechnologyName = '风割' )
               ), 0) Oqty ,
        ISNULL(( SELECT SUM(odd.weight)
                 FROM   BY_OrderDetailProcessDetail odd
                 WHERE  odd.OrderDetailId = od.OrderDetailId
                        AND odd.LineCode = od.LineCode
                        AND (odd.TechnologyId = 'K001' or odd.TechnologyName = '锯床' or odd.TechnologyId = 'FG' or odd.TechnologyName = '风割' )
               ), 0) Oweight,
 ISNULL(( SELECT SUM(dd.Qty)
                 FROM   dbo.BY_DeliveryDetail dd
                 WHERE  dd.OrderDetailId = od.OrderDetailId
               ), 0) deliveryqty ,q.SalesMan,qd.LJtype1+'-'+qd.LJtype2 'LJtype'


                            FROM    dbo.BY_Order o
                                    INNER JOIN dbo.BY_OrderDetail od ON o.OrderId = od.OrderId
                                    INNER JOIN dbo.BY_QuotationDetail qd ON qd.QuotationDetailId=od.QuotationDetailId
                                    INNER JOIN dbo.BY_Quotation q ON q.QuotationId=qd.QuotationId
                                   WHERE   1=1 and od.Length > 0  AND q.DeleteMark = 0  and o.DeleteMark=0 and od.Delivery is not null");

            if (!HashtableHelper.IsNullOrDBNull(Condition["Prcess"]))
            {
                strSql.Append(" and EXISTS  (SELECT 1 FROM    BY_OrderDetailProcess  bp  WHERE bp.OrderDetailId = od.OrderDetailId and bp.TechnologyName = @Prcess)");
                parameter.Add(DbFactory.CreateDbParameter("@Prcess", Condition["Prcess"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Prcess2"]))
            {
                strSql.Append(" and EXISTS  (SELECT 1 FROM    BY_OrderDetailProcess  bp  WHERE bp.OrderDetailId = od.OrderDetailId and bp.TechnologyName = @Prcess2)");
                parameter.Add(DbFactory.CreateDbParameter("@Prcess2", Condition["Prcess2"].ToString()));
            }
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND o.OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND o.LType = @LType");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerPO"]))
            {
                strSql.Append(" AND o.CustomerPO = @CustomerPO");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerPO", Condition["CustomerPO"].ToString()));
            }


            if (!HashtableHelper.IsNullOrDBNull(Condition["SteelCode"]))
            {
                strSql.Append(" AND od.SteelCode LIKE @SteelCode");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", '%' + Condition["SteelCode"].ToString() + '%'));
            }
            
            if (!HashtableHelper.IsNullOrDBNull(Condition["Size"]))
                {
                    string[] Sizes = Condition["Size"].ToString().Replace(" ", "").Split('*');
                if (Condition["Size"].ToString().IndexOf('Φ') > -1)
                {
                    strSql.Append(@" AND od.Length like @Length AND od.Weigh=0 AND od.Thick like @Thick  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[1].Replace("Φ", "") + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[0].Replace("Φ", "") + "%"));
                }
                else
                {
                    strSql.Append(@"  AND ((od.Length like @Length AND od.Weigh like @Weigh AND od.Thick like @Thick) or  
(od.Length like @Length AND od.Weigh like @Thick AND od.Thick like @Weigh) or 

(od.Length like @Weigh AND od.Weigh like @Length AND od.Thick like @Thick) or 

(od.Length like @Thick AND od.Weigh like @Weigh AND od.Thick like @Length) or 

(od.Length like @Weigh AND od.Weigh like @Thick AND od.Thick like @Length) or 

(od.Length like @Thick AND od.Weigh like @Weigh AND od.Thick like @Length)  )  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                }
            }

            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND o.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " 00:00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " 23:59")));
            }
            //客户编号
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (o.CustomerCode LIKE '%'+@CustomerCode+'%' or  o.ShortName LIKE '%'+@CustomerCode+'%' or o.CustomerName LIKE '%'+@CustomerCode+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", Condition["CustomerCode"].ToString() ));
            }
            //客户简称
            if (!HashtableHelper.IsNullOrDBNull(Condition["ShortName"]))
            {
                strSql.Append(" AND (o.CustomerCode LIKE '%'+@ShortName+'%' or  o.ShortName LIKE '%'+@ShortName+'%' or o.CustomerName LIKE '%'+@ShortName+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@ShortName", Condition["ShortName"].ToString() ));
            }
            //销售区域
            if (!HashtableHelper.IsNullOrDBNull(Condition["SalesArea"]))
            {
                strSql.Append(" AND q.SalesArea LIKE @SalesArea");
                parameter.Add(DbFactory.CreateDbParameter("@SalesArea", '%' + Condition["SalesArea"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderStatusA"]))
            {
                if (Condition["OrderStatusA"].ToString() == "1")
                {
                    strSql.Append(@" and o.OrderStatus != '已取消'   AND (od.BillStatus = '正常生产'
        OR od.BillStatus = '部分完工'
        OR od.BillStatus = '部分发运'
        ) AND od.BillStatus != '已取消'
                                                                 AND DATEADD(day, 1, o.Delivery) < SYSDATETIME() 
     
            
");
                }
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["Today1"]))
            {
                if (Condition["Today1"].ToString() == "1")
                {
                    strSql.Append(@" and o.Delivery = CONVERT(VARCHAR(10), DATEADD(day, 1, SYSDATETIME()), 120) 
");
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["normal"]))
            {
                if (Condition["normal"].ToString() == "1")
                {
                    strSql.Append(@" and not  EXISTS  (select 1 from BY_Delivery d1 where d1.OrderId = o.orderid)
");
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["endok"]))
            {
                if (Condition["endok"].ToString() == "1")
                {
                    strSql.Append(@" and EXISTS  (select 1 from BY_Delivery d1 where d1.OrderId = o.orderid)
");
                }
            }
            //string OrderStatus = (HashtableHelper.IsNullOrDBNull(Condition["OrderStatusA"]) ? "0" : "1") + (HashtableHelper.IsNullOrDBNull(Condition["OrderStatusB"]) ? "0" : "1") + (HashtableHelper.IsNullOrDBNull(Condition["OrderStatusC"]) ? "0" : "1");
            //switch (OrderStatus)
            //{
            //    case "000":
            //        strSql.Append(" AND 1 =2 ");
            //        break;
            //    case "001":
            //        strSql.Append(" AND DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())<0 ");
            //        break;
            //    case "010":
            //        strSql.Append(" AND DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())=0 ");
            //        break;
            //    case "011":
            //        strSql.Append(" AND DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())<=0 ");
            //        break;
            //    case "100":
            //        strSql.Append(" AND DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())>0 ");
            //        break;
            //    case "101":
            //        strSql.Append(" AND (DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())<0 OR DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())>0)");
            //        break;
            //    case "110":
            //        strSql.Append(" AND DATEDIFF(DAY,CAST(LEFT(od.Delivery,10)AS DATETIME), GETDATE())>=0 ");
            //        break;
            //    case "111":
            //        break;
            //}
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderStatusC"]))
            {
                strSql.Append(" and OrderStatus != '已完工' AND OrderStatus != '已取消' AND OrderStatus != '已发运'  ");
            }
            //加工类型
            string workType = (HashtableHelper.IsNullOrDBNull(Condition["WorkTypeA"]) ? "0" : "1") + (HashtableHelper.IsNullOrDBNull(Condition["WorkTypeB"]) ? "0" : "1");
            switch (workType)
            {
                case "00":
                    strSql.Append(" AND 1 =2 ");
                    break;
                case "01":
                    strSql.Append(" AND qd.OrderType = '毛料生产'  ");
                    
                    break;
                case "10":
                    strSql.Append(" AND qd.OrderType != '毛料生产' ");
                    break;
                case "11":
                    break;
            }
            //仅未开料
            if (!HashtableHelper.IsNullOrDBNull(Condition["WorkTypeC"]))
            {
                strSql.Append(" AND ISNULL(OpenQty,0)=0 ");
            }
            //不含已取消行
            if (!HashtableHelper.IsNullOrDBNull(Condition["CancelState"]))
            {
                strSql.Append(" AND od.BillStatus != '已取消'");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND (o.MnemonicCode LIKE '%'+@MnemonicCode+'%' or o.QuotationCode LIKE '%'+@MnemonicCode+'%') ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerProduct"]))
            {
                strSql.Append(@" and o.orderid in ( SELECT orderid FROM dbo.BY_OrderDetail od WHERE od.CustomerProduct LIKE '%'+@CustomerProduct+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerProduct", Condition["CustomerProduct"].ToString().Trim()));
            }

            DataTable dt = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
            return dt;
        }
    }
}

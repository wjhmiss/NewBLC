//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// 订单完工表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.24 15:03</date>
    /// </author>
    /// </summary>
    public class BY_OrderCompleteBll : RepositoryFactory<BY_OrderComplete>
    {
        public DataTable WriteWeihtdo(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" 
   
     SELECT od.OrderDetailId ,
            od.OrderCode ,
            od.LineCode ,
            od.SteelDescription ,
            od.SteelCode ,
            od.Length ,
            od.Weigh ,
            od.Thick ,
            od.Weight ,
            o.CustomerName ,
            o.mnemoniccode ,
            o.CustomerCode ,
            od.steeltype ,
            od.qty odqty ,od.BoreSize,
            od.Qty - ISNULL(( SELECT    SUM(ISNULL(ot.qty, 0))
                              FROM      dbo.BY_OutStock ot
                              WHERE     od.OrderDetailId = ot.OrderDetailId and (ot.OrderDetailProcessDetailId = '' OR ot.OrderDetailProcessDetailId IS NULL)
                            ), 0)
            - ISNULL(( SELECT   SUM(ISNULL(ot.qty, 0))
                       FROM     dbo.Base_SteelDDPerchaseDetail ot
                       WHERE    od.QuotationDetailId = ot.QuotationDetailId
                     ), 0) Qty ,
            od.Qty - ISNULL(( SELECT    SUM(ISNULL(ot.qty, 0))
                              FROM      dbo.BY_OutStock ot
                              WHERE     od.OrderDetailId = ot.OrderDetailId and (ot.OrderDetailProcessDetailId = '' OR ot.OrderDetailProcessDetailId IS NULL)
                            ), 0)
            - ISNULL(( SELECT   SUM(ISNULL(ot.qty, 0))
                       FROM     dbo.Base_SteelDDPerchaseDetail ot
                       WHERE    od.QuotationDetailId = ot.QuotationDetailId
                     ), 0) openqty,o.ShortName
     FROM   dbo.BY_OrderDetail od ,
            dbo.BY_Order o
     WHERE  od.orderid = o.orderid and o.DeleteMark=0 
            AND od.Qty
            - ISNULL(( SELECT   SUM(ISNULL(ot.qty, 0))
                       FROM     dbo.BY_OutStock ot
                       WHERE    od.OrderDetailId = ot.OrderDetailId and (ot.OrderDetailProcessDetailId = '' OR ot.OrderDetailProcessDetailId IS NULL)
                     ), 0)
            - ISNULL(( SELECT   SUM(ISNULL(ot.qty, 0))
                       FROM     dbo.Base_SteelDDPerchaseDetail ot
                       WHERE    od.QuotationDetailId = ot.QuotationDetailId
                     ), 0) > 0 and od.Length > 0
          @ordercode @linecode
        ");
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                if(Condition["OrderCode"].ToString().Split('-').Length == 3)
                {
                    strSql.Replace("@ordercode", " AND ( o.mnemoniccode LIKE  @OrderCode or o.ordercode LIKE  @OrderCode )");
                    parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString().Split('-')[0] + "-" + Condition["OrderCode"].ToString().Split('-')[1] + '%'));

                    strSql.Replace("@linecode", " AND ( od.LineCode = @linecode )");
                    parameter.Add(DbFactory.CreateDbParameter("@linecode", Condition["OrderCode"].ToString().Split('-')[2]));


                }
                else if (Condition["OrderCode"].ToString().Split('-').Length == 2)
                {
                    strSql.Replace("@ordercode", " AND ( o.mnemoniccode LIKE  @OrderCode or o.ordercode LIKE  @OrderCode )");
                    parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));


                    if (!HashtableHelper.IsNullOrDBNull(Condition["LineCode"]))
                    {
                        strSql.Replace("@linecode", " AND ( od.LineCode = @linecode )");
                        parameter.Add(DbFactory.CreateDbParameter("@linecode", Condition["LineCode"].ToString()));
                    }
                    else
                    {
                        strSql.Replace("@linecode", " ");

                    }

                }
                else if (Condition["OrderCode"].ToString() == "")
                {
                    return new DataTable();
                }

                

            }
            else
            {
                return new DataTable();
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }


        /// <summary>
        /// 订单已完工列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public DataTable GetOrderCompleteList1(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            //            strSql.Append(@" 
            //  SELECT  OD.OrderCode ,
            //        OD.OrderDetailId ,
            //        OD.OrderId ,
            //        OD.LineCode ,
            //        OD.BoardId ,
            //        OD.BoardDescription ,
            //        OD.OpenQty - OD.CompleteQty AS Qty ,
            //        CAST(od.Length AS VARCHAR(10)) + '×' + CAST(od.Weigh AS VARCHAR(10))
            //        + '×' + CAST(od.Thick AS VARCHAR(10)) AS SteelSize ,--尺寸
            //        CASE WHEN OD.ordertype = '毛料生产'
            //             THEN OD.WeightFact
            //                  - ISNULL(( SELECT SUM(oc.Weight)
            //                             FROM   BY_OrderComplete oc
            //                             WHERE  oc.OrderDetailId = od.OrderDetailId
            //                           ), 0)
            //             ELSE OD.Weight * ( OD.OpenQty - OD.CompleteQty ) / od.Qty
            //        END AS weight ,
            //        OD.Weight * ( OD.OpenQty - OD.CompleteQty ) / od.Qty AS theoryweight ,
            //        OD.SteelCode ,
            //        o.Delivery ,--要求完工日期
            //       
            //        od.CreateDate ,
            //        od.OrderType ,
            //        od.BillStatus ,
            //        o.CustomerName ,
            //        od.SteelDescription
            //FROM    BY_OrderDetail OD
            //        INNER JOIN dbo.BY_Order o ON OD.OrderId = O.OrderId
            //WHERE   ( OD.BillStatus = '正常生产'
            //          OR OD.BillStatus = '部分完工'
            //          OR OD.BillStatus = '部分发运'
            //          OR OD.BillStatus = '已完工'
            //        )
            //        AND OD.OpenQty - OD.CompleteQty > 0
            //        
            //        ");

            //订单单号
            //if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            if (true)
            {
                strSql.Append(@" 
    SELECT OD.OrderCode ,
        OD.OrderDetailId ,
        OD.OrderId ,
        OD.LineCode ,
        OD.BoardId ,
        OD.qty allqty ,
        OD.BoardDescription ,
        OD.Qty - OD.CompleteQty AS maxqty ,
        CASE WHEN od.Weigh = 0
             THEN 'Φ' + CAST(CONVERT(FLOAT, od.Thick) AS VARCHAR(10)) + '*'
                  + CAST(CONVERT(FLOAT, od.Length) AS VARCHAR(10))
             ELSE CAST(CONVERT(FLOAT, od.Thick) AS VARCHAR(10))   + '*'
                  + CAST(CONVERT(FLOAT, od.Weigh) AS VARCHAR(10)) + '*'
                  + CAST(CONVERT(FLOAT, od.Length) AS VARCHAR(10))
        END AS SteelSize ,--尺寸
        CASE WHEN od.Weigh != 0 THEN OD.Qty - OD.CompleteQty
             ELSE OD.Qty - OD.CompleteQty
        END AS Qty , --数量
        --CASE WHEN od.Weigh != 0
        --     THEN OD.Weight * ( OD.Qty - OD.CompleteQty ) / od.Qty
        --     ELSE ISNULL(( SELECT   SUM(Weight)
        --                   FROM     BY_OrderDetailProcessDetail
        --                   WHERE    OrderCode = od.OrderCode
        --                            AND LineCode = od.LineCode
        --                            AND IsClear = 0
        --                            AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
        --                 ), 0)
        --END AS weight ,
        OD.Weight * ( OD.Qty - OD.CompleteQty ) / od.Qty AS weight ,

         CASE WHEN od.Weigh != 0
             THEN OD.TheoryWeight * ( OD.Qty - OD.CompleteQty ) / od.Qty
             ELSE OD.TheoryWeight * ( OD.Qty - OD.CompleteQty ) / od.Qty
        END AS theoryweight ,
        OD.Weight  / od.Qty as TheorySheetWeight,
        OD.SteelCode ,
        o.Delivery ,--要求完工日期
        od.CreateDate ,
        od.OrderType ,
        od.BillStatus ,
        o.CustomerName ,
        od.SteelDescription ,
        ISNULL(( SELECT SUM(Qty)
                 FROM   BY_OrderDetailProcessDetail
                 WHERE  OrderCode = od.OrderCode
                        AND LineCode = od.LineCode
                        AND IsClear = 0
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) Qty1 ,
        ISNULL(( SELECT SUM(Weight)
                 FROM   BY_OrderDetailProcessDetail
                 WHERE  OrderCode = od.OrderCode
                        AND LineCode = od.LineCode
                        AND IsClear = 0
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) Weight1,o.mnemoniccode
FROM    BY_OrderDetail OD
        INNER JOIN dbo.BY_Order o ON OD.OrderId = O.OrderId
WHERE   ( OD.BillStatus = '正常生产'
          OR OD.BillStatus = '部分完工'
          OR OD.BillStatus = '部分发运'
          OR OD.BillStatus = '已完工'
          OR OD.BillStatus = '已发运'
        )
        AND OD.Qty - OD.CompleteQty > 0
        AND OD.Length > 0 and o.DeleteMark=0 
        
        ");
                if (Condition["mnemoniccode"].ToString() == "")
                {
                    return new DataTable();
                }
                else
                {
                    strSql.Append(" AND (o.mnemoniccode LIKE @mnemoniccode  or o.ordercode LIKE @mnemoniccode or o.QuotationCode LIKE @mnemoniccode or o.ordercode + '-' + cast(OD.LineCode as varchar(50)) LIKE @mnemoniccode )");
                    parameter.Add(DbFactory.CreateDbParameter("@mnemoniccode", '%' + Condition["mnemoniccode"].ToString() + '%'));

                }
            }
            else
            {
                strSql.Append(@" 
    SELECT OD.OrderCode ,
        OD.OrderDetailId ,
        OD.OrderId ,
        OD.LineCode ,
        OD.BoardId ,
        OD.qty allqty ,
        OD.BoardDescription ,
        OD.Qty - OD.CompleteQty AS maxqty ,
        CASE WHEN od.Weigh = 0
             THEN 'Φ' + CAST(CONVERT(FLOAT, od.Thick) AS VARCHAR(10)) + '*'
                  + CAST(CONVERT(FLOAT, od.Length) AS VARCHAR(10))
             ELSE CAST(CONVERT(FLOAT, od.Thick) AS VARCHAR(10))   + '*'
                  + CAST(CONVERT(FLOAT, od.Weigh) AS VARCHAR(10)) + '*'
                  + CAST(CONVERT(FLOAT, od.Length) AS VARCHAR(10))
        END AS SteelSize ,--尺寸
        CASE WHEN od.ordertype != '毛料生产' THEN OD.Qty - OD.CompleteQty
             ELSE OD.OpenQty - OD.CompleteQty
        END AS Qty , --数量
        CASE WHEN od.ordertype != '毛料生产'
             THEN OD.Weight * ( OD.Qty - OD.CompleteQty ) / od.Qty
             ELSE ISNULL(( SELECT   SUM(Weight)
                           FROM     BY_OrderDetailProcessDetail
                           WHERE    OrderCode = od.OrderCode
                                    AND LineCode = od.LineCode
                                    AND IsClear = 0
                                    AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
                         ), 0)
        END AS weight ,
        
         CASE WHEN od.ordertype != '毛料生产'
             THEN OD.Weight * ( OD.Qty - OD.CompleteQty ) / od.Qty
             ELSE OD.Weight * ( OD.OpenQty - OD.CompleteQty ) / od.Qty
        END AS theoryweight ,
        OD.Weight  / od.Qty as TheorySheetWeight,
        OD.SteelCode ,
        o.Delivery ,--要求完工日期
        od.CreateDate ,
        od.OrderType ,
        od.BillStatus ,
        o.CustomerName ,
        od.SteelDescription ,
        ISNULL(( SELECT SUM(Qty)
                 FROM   BY_OrderDetailProcessDetail
                 WHERE  OrderCode = od.OrderCode
                        AND LineCode = od.LineCode
                        AND IsClear = 0
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) Qty1 ,
        ISNULL(( SELECT SUM(Weight)
                 FROM   BY_OrderDetailProcessDetail
                 WHERE  OrderCode = od.OrderCode
                        AND LineCode = od.LineCode
                        AND IsClear = 0
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) Weight1,o.mnemoniccode
FROM    BY_OrderDetail OD
        INNER JOIN dbo.BY_Order o ON OD.OrderId = O.OrderId
WHERE   ( OD.BillStatus = '正常生产'
          OR OD.BillStatus = '部分完工'
          OR OD.BillStatus = '部分发运'
          OR OD.BillStatus = '已完工'
          OR OD.BillStatus = '已发运'
        )
        AND OD.Qty - OD.CompleteQty > 0 and o.DeleteMark=0 
        AND ISNULL(( SELECT SUM(op.Qty)
                     FROM   BY_OrderDetailProcess op
                     WHERE  op.OrderDetailId = od.OrderDetailId
                            
                   ), 0)
        - ISNULL(( SELECT   SUM(opd.Qty)
                   FROM     BY_OrderDetailProcessDetail opd
                   WHERE    opd.OrderDetailId = od.OrderDetailId
                 ), 0) = 0
                   
        AND OD.Length > 0

        ");
                return new DataTable();
            }
            if (Condition["LType"] != null && Condition["LType"].ToString() == "GC")
            {
                strSql.Append(@" and o.LType = 'GC'");
            }
            if (Condition["LType"] != null && Condition["LType"].ToString() == "LJ")
            {
                strSql.Append(@" and o.LType = 'LJ'");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 订单已完工列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public DataTable GetOrderCompleteList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" 
  SELECT  OD.OrderCode ,
        OD.OrderDetailId ,
        OD.OrderId ,
        OD.LineCode ,
        OD.BoardId ,
        OD.BoardDescription ,
        OD.OpenQty - OD.CompleteQty AS Qty ,
        CAST(od.Thick AS VARCHAR(10)) + 'X' + CAST(od.Weigh AS VARCHAR(10))
        + 'X' + CAST(od.Length AS VARCHAR(10)) AS SteelSize ,--尺寸
        OD.Weight * ( OD.Qty - OD.CompleteQty ) / od.Qty AS weight ,
        OD.Weight * ( OD.Qty - OD.CompleteQty ) / od.Qty AS theoryweight ,
        OD.SteelCode ,
        o.Delivery ,--要求完工日期
       
        od.CreateDate ,
        od.OrderType ,
        od.BillStatus ,
        o.CustomerName ,
        od.SteelDescription
FROM    BY_OrderDetail OD
        INNER JOIN dbo.BY_Order o ON OD.OrderId = O.OrderId
WHERE   ( OD.BillStatus = '正常生产'
          OR OD.BillStatus = '部分完工'
          OR OD.BillStatus = '部分发运'
        )
        AND OD.Qty - OD.CompleteQty > 0 and o.DeleteMark=0 
        and ScaleUser != ''
        
        ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND o.OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            ////扫描人员
            //if (!HashtableHelper.IsNullOrDBNull(Condition["CreateUserName"]))
            //{
            //    strSql.Append(" AND CreateUserName LIKE @CreateUserName");
            //    parameter.Add(DbFactory.CreateDbParameter("@CreateUserName", '%' + Condition["CreateUserName"].ToString() + '%'));
            //}
            ////扫描日期
            //if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) )
            //{
            //    strSql.Append(" AND CreateDate >= @CreateDate_StartTime  ");
            //    parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " :00")));

            //}
            //if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            //{
            //    strSql.Append(" AND CreateDate <= @CreateDate_EndTime ");
            //    parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " :59")));
            //}
            ////是否完工
            //strSql.Append(string.Format(@" AND Sured in( '{0}','{1}')", HashtableHelper.IsNullOrDBNull(Condition["IsComplete"]) ? "1" : "已确认", HashtableHelper.IsNullOrDBNull(Condition["ProductIng"]) ? "2" : "未确认"));
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 订单未完工列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public DataTable GetOrderUndoneList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  O.OrderCode ,
                                    OD.LineCode ,
                                    OD.BoardId ,
                                    OD.BoardDescription ,
                                    OD.SteelCode ,
                                    OD.SteelDescription ,
                                    OD.SteelState ,
                                    OD.Qty ,
                                    OD.Thick ,
                                    OD.Weigh ,
                                    OD.Length ,
                                    OD.Weight ,
                                    O.PrintDate ,
                                    OD.BillStatus
                            FROM    BY_OrderDetail OD
                                    LEFT JOIN BY_Order O ON O.OrderId = OD.OrderId
                            WHERE   OD.OrderId NOT IN ( SELECT  OrderId
                                                        FROM    dbo.BY_Order
                                                        WHERE   OrderStatus = '已发运' and DeleteMark=0  )
                                                        AND BillStatus !='已完工'
                                                        AND BillStatus !='暂停'  and o.DeleteMark=0 ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            //打印日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["PrintDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["PrintDate_EndTime"]))
            {
                strSql.Append(" AND PrintDate Between @PrintDate_StartTime AND @PrintDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@PrintDate_StartTime", CommonHelper.GetDateTime(Condition["PrintDate_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@PrintDate_EndTime", CommonHelper.GetDateTime(Condition["PrintDate_EndTime"] + " :59")));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}
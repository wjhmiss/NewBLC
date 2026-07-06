//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// 工单工序详情表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.03.14 11:45</date>
    /// </author>
    /// </summary>
    public class BY_OrderDetailProcessDetailBll : RepositoryFactory<BY_OrderDetailProcessDetail>
    {
        public System.Data.DataTable ClassGridsummary(System.DateTime? StartTime, System.DateTime? EndTime, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" 
SELECT  OperatorName ,
        SUM(WorkerAmount) WorkerAmount,
        SUM(Area) Area,
        SUM(Weight) Weight

FROM    BY_OrderDetailProcessDetail
WHERE 1=1 
");
            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql.Append(@"      and  CreateDate >= @StartTime          ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql.Append(@"      and  CreateDate <= @EndTime          ");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime));
            }
            strSql.Append(" GROUP BY OperatorName ");
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        public System.Data.DataTable ClassGridPageJson(System.DateTime? datetime, System.DateTime? datetime2, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" 
SELECT  CASE WHEN t1.ClassesDate IS NULL THEN t2.ClassesDate
             ELSE t1.ClassesDate
        END ClassesDate ,
        CASE WHEN t1.ClassesShift IS NULL THEN t2.ClassesShift
             ELSE t1.ClassesShift
        END ClassesShift ,
        CASE WHEN BCQTY IS NULL THEN 0
             ELSE BCQTY
        END BCQTY ,
        CASE WHEN BCQWeight IS NULL THEN 0
             ELSE BCQWeight
        END BCQWeight ,
        CASE WHEN BJQTY IS NULL THEN 0
             ELSE BJQTY
        END BJQTY ,
        CASE WHEN BJQWeight IS NULL THEN 0
             ELSE BJQWeight
        END BJQWeight ,
        CASE WHEN BCQTY + BJQTY IS NULL THEN 0
             ELSE BCQTY + BJQTY
        END totalqty ,
        CASE WHEN BCQWeight + BJQWeight IS NULL THEN 0
             ELSE BCQWeight + BJQWeight
        END totalqtyWeight ,
        CASE WHEN compeletqty IS NULL THEN 0
             ELSE compeletqty
        END compeletqty ,
        CASE WHEN compeletWeight IS NULL THEN 0
             ELSE compeletWeight
        END compeletWeight
FROM    ( SELECT    ClassesDate ,
                    ClassesShift ,
                    SUM(BCQTY) BCQTY ,
                    SUM(BCQWeight) BCQWeight ,
                    SUM(BJQTY) BJQTY ,
                    SUM(BJQWeight) BJQWeight
          FROM      ( SELECT    ClassesDate ,
                                ClassesShift ,
                                OrderType ,
                                CASE OrderType
                                  WHEN '毛料生产' THEN Qty
                                  ELSE 0
                                END BCQTY ,
                                CASE OrderType
                                  WHEN '毛料生产' THEN Weight
                                  ELSE 0
                                END BCQWeight ,
                                CASE 
                                  WHEN OrderType='精板加工' OR OrderType='光板加工' OR OrderType='精光板加工' THEN Qty
                                  ELSE 0
                                END BJQTY ,
                                CASE 
                                  WHEN OrderType='精板加工' OR OrderType='光板加工' OR OrderType='精光板加工' THEN Weight
                                  ELSE 0
                                END BJQWeight
                                
                      FROM      ( SELECT    SUM(Qty) Qty ,
                                            SUM(Weight) Weight ,
                                            ClassesDate ,
                                            ClassesShift ,
                                            OrderType
                                  FROM      dbo.BY_OrderDetailProcessDetail
                                  WHERE     (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
                                            AND ClassesDate IS NOT NULL");
            if (!HashtableHelper.IsNullOrDBNull(datetime))
            {
                strSql.Append(@"        AND ClassesDate >= @datetime              ");
            }
            if (!HashtableHelper.IsNullOrDBNull(datetime2))
            {
                strSql.Append(@"        AND ClassesDate <= @datetime2              ");
            }
            strSql.Append(@"  
                                  GROUP BY  ClassesDate ,
                                            ClassesShift ,
                                            OrderType
                                ) a
                    ) B
          GROUP BY  ClassesDate ,
                    ClassesShift
        ) t1
        FULL JOIN ( SELECT  p.ClassesDate ,
                            p.ClassesShift ,
                            SUM(p.Qty) compeletqty ,
                            SUM(p.Weight) compeletWeight
                    FROM    BY_OrderComplete p ,dbo.BY_OrderDetail od,
                            dbo.BY_Order o
                    WHERE   p.ClassesDate IS NOT NULL and o.DeleteMark=0 
							AND p.OrderDetailId = od.OrderDetailId
                            AND p.OrderId = o.OrderId
                            AND (od.OrderType='精板加工' OR od.OrderType='光板加工')
                            AND o.OrderStatus != '已取消'
                            AND o.OrderStatus != '已暂停' ");
            if (!HashtableHelper.IsNullOrDBNull(datetime))
            {
                strSql.Append(@" AND p.ClassesDate >= @datetime  ");
            }
            if (!HashtableHelper.IsNullOrDBNull(datetime2))
            {
                strSql.Append(@"        AND p.ClassesDate <= @datetime2              ");
            }
            strSql.Append(@"           GROUP BY p.ClassesDate ,
                            p.ClassesShift
                  ) t2 ON t1.ClassesDate = t2.ClassesDate
                          AND t1.ClassesShift = t2.ClassesShift ");


            if (!HashtableHelper.IsNullOrDBNull(datetime))
            {
                parameter.Add(DbFactory.CreateDbParameter("@datetime", datetime));
            }
            if (!HashtableHelper.IsNullOrDBNull(datetime2))
            {
                parameter.Add(DbFactory.CreateDbParameter("@datetime2", datetime2));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        public BY_OrderDetailProcessDetail GetClassesDateClassesShift(BY_OrderDetailProcessDetail by_orderdetailprocessdetail)
        {
            if (by_orderdetailprocessdetail.CreateDate > DateTime.Parse(DateTime.Now.ToShortDateString() + " 8:30:00") && by_orderdetailprocessdetail.CreateDate <= DateTime.Parse(DateTime.Now.ToShortDateString() + " 20:30:00"))
            {
                by_orderdetailprocessdetail.ClassesDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                by_orderdetailprocessdetail.ClassesShift = "A";
            }
            else if (by_orderdetailprocessdetail.CreateDate > DateTime.Parse(DateTime.Now.ToShortDateString() + " 20:30:00") && by_orderdetailprocessdetail.CreateDate <= DateTime.Parse(DateTime.Now.ToShortDateString() + " 23:59:59"))
            {
                by_orderdetailprocessdetail.ClassesDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                by_orderdetailprocessdetail.ClassesShift = "B";
            }
            else
            {
                by_orderdetailprocessdetail.ClassesDate = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());
                by_orderdetailprocessdetail.ClassesShift = "B";
            }
            return by_orderdetailprocessdetail;
        }
        public void GetClassesDateClassesShift(DateTime? CreateDate, out DateTime? ClassesDate, out string ClassesShift)
        {
            ClassesDate = null;
            ClassesShift = string.Empty;
            if (CreateDate > DateTime.Parse(DateTime.Now.ToShortDateString() + " 8:30:00") && CreateDate <= DateTime.Parse(DateTime.Now.ToShortDateString() + " 20:29:59"))
            {
                ClassesDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                ClassesShift = "A";
            }
            else if (CreateDate > DateTime.Parse(DateTime.Now.ToShortDateString() + " 20:30:00") && CreateDate <= DateTime.Parse(DateTime.Now.ToShortDateString() + " 23:59:59"))
            {
                ClassesDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                ClassesShift = "B";
            }
            else
            {
                ClassesDate = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());
                ClassesShift = "B";
            }
        }
        /// <summary>
        /// 获取尺寸
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="by_order"></param>
        /// <param name="by_orderdetail"></param>
        /// <param name="base_steel"></param>
        /// <returns></returns>
        public BY_OrderDetailProcessDetail GetWeightAreaCalcLength(BY_OrderDetailProcessDetail entity, BY_Order by_order, BY_OrderDetail by_orderdetail, Base_Steel base_steel)
        {
            //if (by_order.OrderType == "BC")
            //{
            //    return entity;
            //}
            //工序计算
            switch (entity.TechnologyId)
            {
                //铣一面 宽度X长度面
                case "B001":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal(by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //铣一侧 长度X厚度面
                case "B002":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal(by_orderdetail.Thick * by_orderdetail.Length * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //铣长度 宽度X厚度面
                case "B003":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal(by_orderdetail.Weigh * by_orderdetail.Thick * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //磨四侧 （长度X厚度+宽度X厚度）X2
                case "B004":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Thick) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //开粗 六个面
                case "C002":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Weigh) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //倒角 12个边长
                case "D001":
                    entity.Weight = 0;
                    entity.Area = 0;
                    entity.CalcLength = (by_orderdetail.Thick + by_orderdetail.Weigh + by_orderdetail.Length) * 4 * entity.Qty;
                    break;
                //光刀 六个面
                case "G004":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Weigh) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //开料 厚X宽 + 厚X长
                case "K001":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Thick * by_orderdetail.Weigh + by_orderdetail.Thick * by_orderdetail.Length) * by_orderdetail.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //磨平面 宽度X长度面X2
                case "M003":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Length) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //铣平面 宽度X长度面X2
                case "P001":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Length) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;

                //切头 宽度X厚 面X2
                case "Q001":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal(by_orderdetail.Weigh * by_orderdetail.Thick*2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //铣侧 厚X长度面X2
                case "X001":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal(by_orderdetail.Thick * by_orderdetail.Length*2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //精铣 4面
                case "J001":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Thick) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                //铣边 4面
                case "X002":
                    entity.Weight = by_orderdetail.Thick * by_orderdetail.Weigh * by_orderdetail.Length * entity.Qty * base_steel.Density / 1000000;
                    entity.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Weigh * by_orderdetail.Thick + by_orderdetail.Length * by_orderdetail.Thick) * 2 * entity.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
                    entity.CalcLength = 0;
                    break;
                default:
                    entity.Weight = 0;
                    entity.Area = 0;
                    entity.CalcLength = 0;
                    break;
            }
            return entity;
        }
    }
}
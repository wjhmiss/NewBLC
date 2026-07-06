using DNS.DataAccess;
using DNS.Entity;
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
    /// 销售汇总表
    /// </summary>
    public class SellSummaryController : PublicController<BY_Quotation>
    {

        /// <summary>
        /// 销售汇总表列表（返回表格JSON）
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
        /// 销售汇总表列表
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " 00:00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " 23:59")));
            }
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append(@"   
  SELECT    CONVERT(VARCHAR(10), q.CreateDate, 120) AS CreateDate ,
                        SUM(CASE WHEN qd.OrderType = '毛料生产' THEN qd.Qty
                                 ELSE 0
                            END) AS BCQty ,
                        SUM(CASE WHEN qd.OrderType = '毛料生产'
                                 THEN qd.SalesAmount
                                 ELSE 0
                            END) AS BCAmount ,
                        SUM(CASE WHEN qd.OrderType = '毛料生产' THEN qd.Weight
                                 ELSE 0
                            END) AS BCWeight ,
                        SUM(CASE WHEN qd.OrderType = '光板加工'
                                      OR qd.OrderType = '精板加工'
                                      OR qd.OrderType = '精光板加工' THEN qd.Qty
                                 ELSE 0
                            END) AS BJQty ,
                        SUM(CASE WHEN qd.OrderType = '光板加工'
                                      OR qd.OrderType = '精板加工'
                                      OR qd.OrderType = '精光板加工'
                                 THEN qd.SalesAmount
                                 ELSE 0
                            END) AS BJAmount ,
                        SUM(CASE WHEN qd.OrderType = '光板加工'
                                      OR qd.OrderType = '精板加工'
                                      OR qd.OrderType = '精光板加工' THEN qd.Weight
                                 ELSE 0
                            END) AS BJWeight ,
                        SUM(qd.Qty) AS Qty ,
                        SUM(qd.SalesAmount) AS Amount ,
                        SUM(qd.Weight) AS Weight
              FROM      dbo.BY_Delivery q
                        INNER JOIN dbo.BY_DeliveryDetail qd ON q.DeliveryId = qd.DeliveryId
              WHERE     Q.LType = 'GC' @CreateDate  and q.DeleteMark = 0 
              GROUP BY  CONVERT(VARCHAR(10), q.CreateDate, 120)
            

 ");
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql1.Replace("@CreateDate", "  AND q.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
            }


            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append(@"   
  SELECT  CONVERT(VARCHAR(10), q.CreateDate, 120) AS CreateDate,
                                SUM(CASE WHEN qd.Technology = '真空淬火'
                                         THEN qd.SalesAmount
                                         ELSE 0
                                    END) AS ZKCHAmount ,
                                SUM(CASE WHEN qd.Technology = '真空淬火'
                                         THEN qd.Weight
                                         ELSE 0
                                    END) AS ZKCHWeight ,
                                SUM(CASE WHEN qd.Technology = '软氮化'
                                         THEN qd.SalesAmount
                                         ELSE 0
                                    END) AS RDHAmount ,
                                SUM(CASE WHEN qd.Technology = '软氮化'
                                         THEN qd.Weight
                                         ELSE 0
                                    END) AS RDHWeight ,
                                SUM(CASE WHEN qd.Technology = '去应力'
                                         THEN qd.SalesAmount
                                         ELSE 0
                                    END) AS QYLAmount ,
                                SUM(CASE WHEN qd.Technology = '去应力'
                                         THEN qd.Weight
                                         ELSE 0
                                    END) AS QYLWeight ,
                                SUM(CASE WHEN qd.Technology = '调质'
                                         THEN qd.SalesAmount
                                         ELSE 0
                                    END) AS tzAmount ,
                                SUM(CASE WHEN qd.Technology = '调质'
                                         THEN qd.Weight
                                         ELSE 0
                                    END) AS tzWeight ,
                                SUM(CASE WHEN qd.Technology = '常规淬火'
                                         THEN qd.SalesAmount
                                         ELSE 0
                                    END) AS cgchAmount ,
                                SUM(CASE WHEN qd.Technology = '常规淬火'
                                         THEN qd.Weight
                                         ELSE 0
                                    END) AS cgchWeight ,
                                SUM(CASE WHEN qd.Technology != '真空淬火'
                                              AND qd.Technology != '软氮化'
                                              AND qd.Technology != '去应力'
                                              AND qd.Technology != '调质'
                                              AND qd.Technology != '常规淬火'
                                         THEN qd.SalesAmount
                                         ELSE 0
                                    END) AS qtAmount ,
                                SUM(CASE WHEN qd.Technology != '真空淬火'
                                              AND qd.Technology != '软氮化'
                                              AND qd.Technology != '去应力'
                                              AND qd.Technology != '调质'
                                              AND qd.Technology != '常规淬火'
                                         THEN qd.Weight
                                         ELSE 0
                                    END) AS qtWeight ,
                                SUM(qd.Qty) AS RCLQty ,
                                SUM(qd.SalesAmount) AS RCLAmount ,
                                SUM(qd.Weight) AS RCLWeight
                        FROM    dbo.BY_Delivery q
                                INNER JOIN dbo.BY_DeliveryDetail qd ON q.DeliveryId = qd.DeliveryId
                        WHERE   Q.LType = 'RCL' @CreateDate and q.DeleteMark = 0 
                        GROUP BY CONVERT(VARCHAR(10), q.CreateDate, 120)

 ");
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql2.Replace("@CreateDate", "  AND q.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
            }


            DataTable dt1 = repositoryfactory.Repository().FindTablePageBySql(strSql1.ToString(), parameter.ToArray(), ref jqgridparam);
            //DataTable dt2 = repositoryfactory.Repository().FindTablePageBySql(strSql2.ToString(), parameter.ToArray(), ref jqgridparam);

            //DataTable dt = Conbin(dt1, dt2);
            return dt1;
        }

        public DataTable Conbin(DataTable dt1, DataTable dt2)
        {
            DataTable newDataTable = dt1.Clone();


            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                if (dt2.Columns[i].ColumnName != "rownum" && dt2.Columns[i].ColumnName != "createdate")
                {
                    newDataTable.Columns.Add(dt2.Columns[i].ColumnName);
                }
            }


            DataRow dr3;
            int ro = 0;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                if (!newDataTable.AsEnumerable().Select(r => r["createdate"]).Contains(dt1.Rows[i][1].ToString()))
                {
                    dr3 = newDataTable.NewRow();
                    dr3[1] = dt1.Rows[i][1].ToString();
                    dr3[0] = i + 1;
                    newDataTable.Rows.Add(dr3);
                    ro += 1;
                }
            }
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                if (!newDataTable.AsEnumerable().Select(r => r["createdate"]).Contains(dt2.Rows[i][1].ToString()))
                {
                    dr3 = newDataTable.NewRow();
                    dr3[1] = dt2.Rows[i][1].ToString();
                    dr3[0] = i + ro + 1;
                    newDataTable.Rows.Add(dr3);
                }
            }

            for (int i = 0; i < newDataTable.Rows.Count; i++)
            {
                for (int x = 0; x < dt1.Rows.Count; x++)
                {
                    if (newDataTable.Rows[i][1].ToString() == dt1.Rows[x][1].ToString())
                    {
                        for (int j = 0; j < dt1.Columns.Count; j++)
                        {
                            newDataTable.Rows[i][j - 2 + dt1.Columns.Count] = dt2.Rows[x][j].ToString();
                        }
                    }
                }

                for (int x = 0; x < dt2.Rows.Count; x++)
                {
                    if (newDataTable.Rows[i][1].ToString() == dt2.Rows[x][1].ToString())
                    {
                        for (int j = 2; j < dt2.Columns.Count; j++)
                        {
                            newDataTable.Rows[i][j - 2 + dt1.Columns.Count] = dt2.Rows[x][j].ToString();
                        }
                    }
                }
            }





            return newDataTable;
        }
    }

}

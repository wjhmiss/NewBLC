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
using System.Data;
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// BY_AdjustQuery
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.03.27 18:57</date>
    /// </author>
    /// </summary>
    public class BY_AdjustQueryBll : RepositoryFactory<BY_Inventory>
    {
        /// <summary>
        /// 获得页面列表
        /// </summary>
        /// <param name="MaterialType">0-条料，1-碎料</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="BatchCode">钢材编号</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public List<BY_Inventory> GetPageList(string StoveCode)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (true)
            {

            }
            //            if (StartTime != null || EndTime != null)
            //            {
            //                strSql.Append(@"SELECT  *
            //FROM    dbo.BY_AdjustQuery i
            //WHERE   1 = 1
            //        AND (  i.ParentInventoryId = '0'
            //                   AND EXISTS ( SELECT  1
            //                                FROM    BY_AdjustQuery b
            //                                WHERE   b.ParentInventoryId = i.InventoryId 
            //                                and b.CreateDate >= @StartTime and b.CreateDate <=@EndTime))
            //                 
            //            ) ");
            //                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            //                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime == null ? EndTime : Convert.ToDateTime(EndTime).AddDays(1)));
            //            }
            //            else
            //            {
            strSql.AppendFormat(@"SELECT  *
FROM    dbo.BY_Inventory i
WHERE   1 = 1
        AND i.StoveCode = '{0}' ", StoveCode);
            //}

            return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
        }


        public DataTable GetPageList1(string BatchCode, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.AppendFormat(@"
SELECT *
FROM 
    (SELECT ot.BillCode ,
         o.OrderCode ,
         o.QuotationCode ,
         od.LineCode ,
         ot.SteelCode ,
         ot.Qty ,
         ot.Thick ,
         ot.weigh ,
         ot.Length ,
         ot.TheoryWeight ,
         ot.Weight ,
         ot.originalsize ,
         ot.nowsize ,
         ot.originalweight ,
         ot.nowweight,
         ot.Remark ,
         ot.SteelStatus ,
         ot.CreateUserName ,
         ot.CreateDate ,
         ot.OutStockType StockType ,
         ot.CustomerCode ,
         ot.CustomerName ,
         ot.steeldescription ,
         
        (SELECT sum(isnull(odp.weight,
        0))
        FROM dbo.BY_OrderDetailProcessDetail odp
        WHERE odp.OrderCode = ot.OrderCode
                AND odp.LineCode = ot.LineCode
                AND (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' ) ) gbweight , ot.SteelPurchaseCode,ot.StoveCode
        FROM dbo.BY_OutStock ot
        LEFT JOIN dbo.BY_OrderDetail od
            ON od.OrderDetailId = ot.OrderDetailId
        LEFT JOIN dbo.BY_Order o
            ON od.OrderId = o.OrderId
        WHERE 1 = 1
                AND ot.StoveCode = '{0}'
        UNION ALL 
    SELECT BillCode,
        null OrderCode ,
         NULL QuotationCode ,
         NULL LineCode ,
         SteelCode,
         Qty,
         Thick,
         Weigh,
         Length,
         TheoryWeight,
         Weight,
         NULL originalsize ,
         NULL nowsize ,
         NULL originalweight ,
         NULL nowweight,
         Remark,
         SteelStatus,
         CreateUserName,
         CreateDate,
         InStockType StockType,
         CustomerCode ,
         CustomerName ,
         steeldescription ,
         NULL gbweight ,
         SteelPurchaseCode,
         StoveCode
        FROM BY_InStock bi
        WHERE bi.StoveCode = '{0}' ) a
    ", BatchCode);
            return Repository().FindTablePageBySql(strSql.ToString(),ref jqgridparam);
        }

    }
}
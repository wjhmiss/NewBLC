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
    /// 出仓表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 22:36</date>
    /// </author>
    /// </summary>
    public class BY_OutStockBll : RepositoryFactory<BY_OutStock>
    {
        /// <summary>
        /// 获得最新入库单号
        /// </summary>
        /// <param name="by_instock">入库信息</param>
        /// <returns>入库单号</returns>



        private int ReturnArrB(ArrayList als, string strTemp)
        {
            int i = 1;
            foreach (string str in als)
            {
                if (str == strTemp)
                {
                    i = i + 1;

                }
            }
            return i;
        }

        /// <summary>
        /// 获得页面列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="BillCode">订单号</param>
        /// <param name="BatchCode">钢材编码</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(DateTime StartTime, DateTime EndTime, string SteelCode, string BillCode, string BatchCode, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT  ot.outstockid ,
        o.OrderCode ,
        od.LineCode ,
        od.SteelCode ,
        od.Qty ,
        od.Thick ,
        od.Weigh,
        od.Length ,
        ot.Remark ,
        od.SteelState ,
        ot.Weight ,
        ot.CreateUserName ,
        ot.CreateDate ,
        ot.OutStockType, ot.TheoryWeight ,ot.BillCode
FROM    dbo.BY_OutStock ot
        LEFT JOIN dbo.BY_OrderDetail od ON od.OrderDetailId = ot.OrderDetailId
        LEFT JOIN dbo.BY_Order o ON od.OrderId = o.OrderId
WHERE   1 = 1 and o.DeleteMark=0 
        AND ot.OrderDetailId != ''  ");
            if (StartTime != null)
            {
                strSql.Append(" AND ot.CreateDate>= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
            if (EndTime != null)
            {
                strSql.Append(" AND ot.CreateDate<= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime));
            }
            if (!string.IsNullOrEmpty(BatchCode))
            {
                strSql.Append(" AND ot.Remark = @BatchCode");
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", BatchCode));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND ot.SteelCode = @SteelCode");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                strSql.Append(" AND o.OrderCode = @BillCode");
                parameter.Add(DbFactory.CreateDbParameter("@BillCode", BillCode));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 获得页面列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="BillCode">订单号</param>
        /// <param name="BatchCode">钢材编码</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList1(string StartTime, string EndTime, string SteelCode, string BillCode, string BatchCode, string CustomerCode, string CreateUserId, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"
SELECT  ot.outstockid ,
        ot.BillCode ,
        o.OrderCode ,
        o.QuotationCode ,
        od.LineCode ,
        ot.SteelCode ,
        ot.Qty ,
        ot.Thick ,
        ot.weigh ,
        ot.TheoryWeight ,
        ot.Length ,
        ot.Remark ,
        ot.SteelStatus ,
        ot.Weight ,
        ot.CreateUserName ,
        ot.CreateDate ,
        ot.OutStockType ,
        ot.CustomerCode ,
        ot.CustomerName ,
        ot.steeldescription ,
        ( SELECT    sum(isnull(odp.weight,0))
          FROM      dbo.BY_OrderDetailProcessDetail odp
          WHERE     odp.OrderCode = ot.OrderCode
                    AND odp.LineCode = ot.LineCode
                    AND (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' )
        ) gbweight ,
        ot.originalsize ,
        ot.nowsize ,
        ot.originalweight ,
        ot.nowweight,ot.SteelPurchaseCode,ot.StoveCode,o.ShortName
FROM    dbo.BY_OutStock ot
        LEFT JOIN dbo.BY_OrderDetail od ON od.OrderDetailId = ot.OrderDetailId
        LEFT JOIN dbo.BY_Order o ON od.OrderId = o.OrderId
WHERE   1 = 1 ");

            if (!string.IsNullOrEmpty(CreateUserId))
            {
                strSql.Append(" AND ot.CreateUserId = @CreateUserId");
                parameter.Add(DbFactory.CreateDbParameter("@CreateUserId", CreateUserId));
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                strSql.Append(" AND ot.CreateDate>= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime" , Convert.ToDateTime(StartTime) ));
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                strSql.Append(" AND ot.CreateDate<= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", Convert.ToDateTime(EndTime).AddDays(1)));
            }
            if (!string.IsNullOrEmpty(BatchCode))
            {
                strSql.Append(" AND ot.BillCode = @BatchCode");
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", BatchCode));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND ot.SteelCode like '%'+ @SteelCode + '%'");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                strSql.Append(" AND o.OrderCode like   '%'+ @BillCode + '%'");
                parameter.Add(DbFactory.CreateDbParameter("@BillCode", BillCode));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                strSql.Append(" AND (O.CustomerCode LIKE '%'+@CustomerCode+'%' or  O.ShortName LIKE '%'+@CustomerCode+'%' or O.CustomerName LIKE '%'+@CustomerCode+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", CustomerCode));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        public DataTable GetPageList2(DateTime StartTime, DateTime EndTime, string SteelCode, string BillCode, string BatchCode, string CustomerCode,string CreateUserId , ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT  CAST(Convert(decimal(18,2),( ( SUM(ot.Weight) - SUM(ot.TheoryWeight) ) * 100 )
        / SUM(ot.TheoryWeight)) AS VARCHAR(10)) + '%' as tj
FROM    dbo.BY_OutStock ot
        LEFT JOIN dbo.BY_OrderDetail od ON od.OrderDetailId = ot.OrderDetailId
        LEFT JOIN dbo.BY_Order o ON od.OrderId = o.OrderId
WHERE   1 = 1  AND ot.Weight > ot.TheoryWeight  and o.DeleteMark=0  ");
            if (StartTime != null)
            {
                strSql.Append(" AND ot.CreateDate>= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
            if (EndTime != null)
            {
                strSql.Append(" AND ot.CreateDate<= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime.AddDays(1)));
            }
            if (!string.IsNullOrEmpty(BatchCode))
            {
                strSql.Append(" AND ot.BillCode = @BatchCode");
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", BatchCode));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND ot.SteelCode = @SteelCode");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                strSql.Append(" AND o.OrderCode = @BillCode");
                parameter.Add(DbFactory.CreateDbParameter("@BillCode", BillCode));
            }
            if (!string.IsNullOrEmpty(CreateUserId))
            {
                strSql.Append(" AND ot.CreateUserId = @CreateUserId");
                parameter.Add(DbFactory.CreateDbParameter("@CreateUserId", CreateUserId));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                strSql.Append(" AND (O.CustomerCode LIKE '%'+@CustomerCode+'%' or  O.ShortName LIKE '%'+@CustomerCode+'%' or O.CustomerName LIKE '%'+@CustomerCode+'%' )  ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", CustomerCode));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}
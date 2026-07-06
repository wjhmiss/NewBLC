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
    /// 不合格品通知单
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.28 15:31</date>
    /// </author>
    /// </summary>
    public class BY_FlawedNoticeBll : RepositoryFactory<BY_FlawedNotice>
    {
        /// <summary>
        /// 不合格品列表
        /// </summary>
        /// <param name="StartTime">起始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="BillCode">不合格品单号</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="OrderCode">加工单号</param>
        /// <param name="CustomerName">客户名称</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(string StartTime, string EndTime, string BillCode, string SteelCode, string OrderCode, string CustomerName, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT 
                                    o.OrderCode ,
                                    od.BoardDescription ,
                                    o.CustomerCode ,
                                    o.CustomerName ,
                                    od.Thick ,
                                    od.Weigh ,
                                    od.Length ,
                                    od.SteelCode ,
                                    od.BillStatus ,
                                    od.SteelDescription,
                                    qd.SalesAmount*f.Qty/qd.Qty SalesAmount,
                                    qd.Weight*f.Qty/qd.Qty Weight,
                                    f.*
                            FROM    dbo.BY_FlawedNotice f
                                    INNER JOIN dbo.BY_OrderDetail od ON f.OrderDetailId = od.OrderDetailId
                                    INNER JOIN dbo.BY_Order o ON od.OrderId = o.OrderId
                                    INNER JOIN dbo.BY_QuotationDetail qd ON qd.QuotationId = o.QuotationId
                                                                            AND qd.LineCode = od.LineCode
                                                                                WHERE 1=1   and o.DeleteMark=0 ");
                if (!string.IsNullOrEmpty(StartTime))
            {
                strSql.Append(" AND f.WriteDate>= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
                if (!string.IsNullOrEmpty(EndTime))
                {
                strSql.Append(" AND f.WriteDate<= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND od.SteelCode like '%'+ @SteelCode+'%'");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                strSql.Append(" AND f.BillCode like '%'+ @BillCode +'%'");
                parameter.Add(DbFactory.CreateDbParameter("@BillCode", BillCode));
            }
            if (!string.IsNullOrEmpty(OrderCode))
            {
                strSql.Append(" AND o.OrderCode like '%'+ @OrderCode +'%'");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", OrderCode));
            }
            if (!string.IsNullOrEmpty(CustomerName))
            {
                strSql.Append(" AND o.CustomerName like '%'+ @CustomerName+'%'");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", CustomerName));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}
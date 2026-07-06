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
    /// 原材料入仓品质信息
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.29 20:29</date>
    /// </author>
    /// </summary>
    public class BY_InStockQualityBll : RepositoryFactory<BY_InStockQuality>
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
        public DataTable GetPageList(string StartTime, string EndTime, string BillCode, string SteelCode, string InStockQualityCode, string CreateUserName, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  i.SteelCode ,
                                dd1.FullName AS SteelStatus ,
                                dd2.FullName AS PlaceOrigin,
                                dd.FullName AS SteelType,
                                i.Thick,
                                i.Weigh,
                                i.Length,
                                i.InStockDate,
                                i.FactoryCode,
                                i.StoveCode,
                                i.Weight,
                                iq.*
                        FROM    dbo.BY_InStock i
                                INNER JOIN dbo.BY_InStockQuality iq ON i.BillCode = iq.BillCode
                                LEFT JOIN Base_DataDictionaryDetail dd ON i.SteelType = dd.Code
                                                                          AND dd.DataDictionaryId IN (
                                                                          SELECT    DataDictionaryId
                                                                          FROM      dbo.Base_DataDictionary
                                                                          WHERE     Code = 'FaceType' )
                                LEFT JOIN Base_DataDictionaryDetail dd1 ON i.SteelStatus = dd1.Code
                                                                           AND dd1.DataDictionaryId IN (
                                                                           SELECT   DataDictionaryId
                                                                           FROM     dbo.Base_DataDictionary
                                                                           WHERE    Code = 'SteelStatus' )
                                LEFT JOIN Base_DataDictionaryDetail dd2 ON i.PlaceOrigin = dd2.Code
                                                                           AND dd2.DataDictionaryId IN (
                                                                           SELECT   DataDictionaryId
                                                                           FROM     dbo.Base_DataDictionary
                                                                           WHERE    Code = 'SteelSource' )
                        WHERE   1 = 1 ");
            if (StartTime != null)
            {
                strSql.Append(" AND iq.CreateDate>= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
            if (EndTime != null)
            {
                strSql.Append(" AND iq.CreateDate<= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND i.SteelCode like '%'+ @SteelCode +'%'");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                strSql.Append(" AND iq.BillCode like '%'+ @BillCode + '%'");
                parameter.Add(DbFactory.CreateDbParameter("@BillCode", BillCode));
            }
            if (!string.IsNullOrEmpty(InStockQualityCode))
            {
                strSql.Append(" AND iq.InStockQualityCode = @InStockQualityCode");
                parameter.Add(DbFactory.CreateDbParameter("@InStockQualityCode", InStockQualityCode));
            }
            if (!string.IsNullOrEmpty(CreateUserName))
            {
                strSql.Append(" AND i.CreateUserName = @CreateUserName");
                parameter.Add(DbFactory.CreateDbParameter("@CreateUserName", CreateUserName));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}
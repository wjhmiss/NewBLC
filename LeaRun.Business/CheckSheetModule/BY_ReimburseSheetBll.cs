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
    /// 费用申请单
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.04.24 22:47</date>
    /// </author>
    /// </summary>
    public class BY_ReimburseSheetBll : RepositoryFactory<BY_ReimburseSheet>
    {

        public System.Data.DataTable GetPageList(System.DateTime StartTime, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT  i.SteelCode ,
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
            
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        public System.Data.DataTable GetPageList1(System.DateTime StartTime, DateTime EndTime, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT * FROM (
 
  SELECT  Opions ,
        GetType ,
        Code AS code ,
        GETMoney ,
        CustomerName,CustomerCode,CreateDate,'1' sort,TotalAmount,WorkerName,DepartmentName
FROM    BY_ReimburseSheet
WHERE   RequiredDate > @StartTime
        AND RequiredDate < @EndTime AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 

 ) a ORDER BY a.GetType,a.sort,a.code ");

            parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime.ToShortDateString() + " 00:00:00"));
            parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime.ToShortDateString() + " 23:59:59"));

            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        public System.Data.DataTable GetPageList2(System.DateTime StartTime, DateTime EndTime, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" 
SELECT  deliverycode,
        TotalAmount , GetedAmount ,
        CustomerName,CreateDate
FROM    dbo.BY_Delivery
WHERE   CreateDate > @StartTime
        AND CreateDate < @EndTime  and DeleteMark = 0
        UNION ALL
        
   SELECT CheckCompanyCode deliverycode,
        TotalAmount , GetedAmount ,
        CustomerName,CreateDate
FROM    dbo.BY_CheckCompany
WHERE   CreateDate > @StartTime
        AND CreateDate < @EndTime
        
         ");
            parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime.ToShortDateString() + " 00:00:00"));
            parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime.ToShortDateString() + " 23:59:59"));
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public System.Data.DataTable GetPageList3(System.DateTime StartTime, DateTime EndTime, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" 
SELECT  *
FROM    ( SELECT    Opions ,
                    GetType ,
                    Code AS code ,
                    GETMoney ,
                    CustomerName ,
                    CustomerCode ,
                    CreateDate ,
                    '1' sort ,
                    TotalAmount,WorkerName,DepartmentName
          FROM      BY_PayReimburseSheet
          WHERE     RequiredDate > @StartTime
                    AND RequiredDate < @EndTime
                    AND ( PayReimburseSheetCode IS NULL
                          OR PayReimburseSheetCode = ''
                          OR Opions = '退款'
                        )
        ) a
ORDER BY a.GetType ,
        a.sort ,
        a.code 
 
         ");
            parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime.ToShortDateString() + " 00:00:00"));
            parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime.ToShortDateString() + " 23:59:59"));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
    }
}
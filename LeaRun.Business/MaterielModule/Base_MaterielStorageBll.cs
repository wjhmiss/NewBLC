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
    /// ¿â´æ
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.10 11:34</date>
    /// </author>
    /// </summary>
    public class Base_MaterielStorageBll : RepositoryFactory<Base_MaterielStorage>
    {

        public DataTable GetSummaryList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  A.MaterielCode ,A.MaterielType,
        A.MaterielName ,
        A.WorkerName ,
        A.DepartmentName ,
        getQTY ,
        backQTY,
(SELECT m1.PricePer FROM Base_MaterielType m1 WHERE m1.MaterielCode = a.MaterielCode) pepermoney,
        (ISNULL(getQTY,0) - ISNULL(backQTY,0))*(SELECT m1.PricePer FROM Base_MaterielType m1 WHERE m1.MaterielCode = a.MaterielCode) TotalAmount,
        (SELECT m2.Qty FROM Base_MaterielStorage m2 WHERE m2.MaterielCode = a.MaterielCode) qty
FROM    ( SELECT    MaterielCode ,MaterielType,
                    MaterielName ,
                    WorkerName ,DepartmentName,
                    SUM(Qty) getQTY,SUM(TotalAmount)  getmoney
          FROM      dbo.Base_MaterielGet WHERE 1=1 @WorkerName  @MaterielName @DepartmentName @CreateDate_StartTime @CreateDate_EndTime
          GROUP BY  MaterielCode ,MaterielType,
                    MaterielName ,
                    WorkerName ,
                    DepartmentName
        ) a
        LEFT JOIN ( SELECT  MaterielCode ,MaterielType,
                            MaterielName ,
                            WorkerName ,
                            DepartmentName ,
                            SUM(Qty) backQTY,SUM(TotalAmount)  getmoney
                    FROM    dbo.Base_MaterielReturn WHERE 1=1  @WorkerName  @MaterielName @DepartmentName @CreateDate_StartTime @CreateDate_EndTime
                    GROUP BY MaterielCode ,MaterielType,
                            MaterielName ,
                            WorkerName ,
                            DepartmentName
                  ) b ON a.MaterielCode = b.MaterielCode
                         AND a.WorkerName = b.WorkerName
         ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["WorkerName"]))
            {
                strSql.Replace("@WorkerName", string.Format(" and WorkerName = '{0}'",Condition["WorkerName"].ToString()));
            }
            else
            {
                strSql.Replace("@WorkerName", "");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MaterielName"]))
            {
                strSql.Replace("@MaterielName", string.Format(" and MaterielName = '{0}'", Condition["MaterielName"].ToString()));
            }
            else
            {
                strSql.Replace("@MaterielName", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["DepartmentName"]))
            {
                strSql.Replace("@DepartmentName", string.Format(" and DepartmentName = '{0}'", Condition["DepartmentName"].ToString()));
            }
            else
            {
                strSql.Replace("@DepartmentName", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]))
            {
                strSql.Replace("@CreateDate_StartTime", string.Format(" and CreateDate >= CONVERT(DATETIME, '{0} 00:00:00') ", Condition["CreateDate_StartTime"].ToString()));
            }
            else
            {
                strSql.Replace("@CreateDate_StartTime", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Replace("@CreateDate_EndTime", string.Format(" and CreateDate <= CONVERT(DATETIME, '{0} 23:59:59') ", Condition["CreateDate_EndTime"].ToString()));
            }
            else
            {
                strSql.Replace("@CreateDate_EndTime", "");

            }
           

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetSummaryList1(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  A.MaterielCode ,A.MaterielType,
        A.MaterielName ,
        A.DepartmentName ,
        getQTY ,
        backQTY,
(SELECT m1.PricePer FROM Base_MaterielType m1 WHERE m1.MaterielCode = a.MaterielCode) pepermoney,
        (ISNULL(getQTY,0) - ISNULL(backQTY,0))*(SELECT m1.PricePer FROM Base_MaterielType m1 WHERE m1.MaterielCode = a.MaterielCode) TotalAmount,
        (SELECT m2.Qty FROM Base_MaterielStorage m2 WHERE m2.MaterielCode = a.MaterielCode) qty
FROM    ( SELECT    MaterielCode ,MaterielType,
                    MaterielName ,
                    DepartmentName,
                    SUM(Qty) getQTY,SUM(TotalAmount)  getmoney
          FROM      dbo.Base_MaterielGet WHERE 1=1   @MaterielName @DepartmentName @CreateDate_StartTime @CreateDate_EndTime
          GROUP BY  MaterielCode ,MaterielType,
                    MaterielName ,
                    DepartmentName
        ) a
        LEFT JOIN ( SELECT  MaterielCode ,MaterielType,
                            MaterielName ,
                            DepartmentName ,
                            SUM(Qty) backQTY,SUM(TotalAmount)  getmoney
                    FROM    dbo.Base_MaterielReturn WHERE 1=1    @MaterielName @DepartmentName @CreateDate_StartTime @CreateDate_EndTime
                    GROUP BY MaterielCode ,MaterielType,
                            MaterielName ,
                            DepartmentName
                  ) b ON a.MaterielCode = b.MaterielCode
                         AND a.DepartmentName = b.DepartmentName
         ");

            
            if (!HashtableHelper.IsNullOrDBNull(Condition["MaterielName"]))
            {
                strSql.Replace("@MaterielName", string.Format(" and MaterielName = '{0}'", Condition["MaterielName"].ToString()));
            }
            else
            {
                strSql.Replace("@MaterielName", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["DepartmentName"]))
            {
                strSql.Replace("@DepartmentName", string.Format(" and DepartmentName = '{0}'", Condition["DepartmentName"].ToString()));
            }
            else
            {
                strSql.Replace("@DepartmentName", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]))
            {
                strSql.Replace("@CreateDate_StartTime", string.Format(" and CreateDate >= CONVERT(DATETIME, '{0} 00:00:00') ", Condition["CreateDate_StartTime"].ToString()));
            }
            else
            {
                strSql.Replace("@CreateDate_StartTime", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Replace("@CreateDate_EndTime", string.Format(" and CreateDate <= CONVERT(DATETIME, '{0} 23:59:59') ", Condition["CreateDate_EndTime"].ToString()));
            }
            else
            {
                strSql.Replace("@CreateDate_EndTime", "");

            }


            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetSummaryList2(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  A.MaterielCode ,A.MaterielType,
        A.MaterielName ,
        getQTY ,
        backQTY,
(SELECT m1.PricePer FROM Base_MaterielType m1 WHERE m1.MaterielCode = a.MaterielCode) pepermoney,
        (ISNULL(getQTY,0) - ISNULL(backQTY,0))*(SELECT m1.PricePer FROM Base_MaterielType m1 WHERE m1.MaterielCode = a.MaterielCode) TotalAmount,
        (SELECT m2.Qty FROM Base_MaterielStorage m2 WHERE m2.MaterielCode = a.MaterielCode) qty
FROM    ( SELECT    MaterielCode ,MaterielType,
                    MaterielName ,
                    
                    SUM(Qty) getQTY,SUM(TotalAmount)  getmoney
          FROM      dbo.Base_MaterielGet WHERE 1=1   @MaterielName  @CreateDate_StartTime @CreateDate_EndTime
           GROUP BY  MaterielCode ,MaterielType,
                    MaterielName 
                    
        ) a
        LEFT JOIN ( SELECT  MaterielCode ,MaterielType,
                            MaterielName ,
                            
                            SUM(Qty) backQTY,SUM(TotalAmount)  getmoney
                    FROM    dbo.Base_MaterielReturn WHERE 1=1   @MaterielName @CreateDate_StartTime @CreateDate_EndTime
                   GROUP BY MaterielCode ,MaterielType,
                            MaterielName 
                           
                  ) b ON a.MaterielCode = b.MaterielCode
         ");

            
            if (!HashtableHelper.IsNullOrDBNull(Condition["MaterielName"]))
            {
                strSql.Replace("@MaterielName", string.Format(" and MaterielName = '{0}'", Condition["MaterielName"].ToString()));
            }
            else
            {
                strSql.Replace("@MaterielName", "");

            }
            
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]))
            {
                strSql.Replace("@CreateDate_StartTime", string.Format(" and CreateDate >= CONVERT(DATETIME, '{0} 00:00:00') ", Condition["CreateDate_StartTime"].ToString()));
            }
            else
            {
                strSql.Replace("@CreateDate_StartTime", "");

            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Replace("@CreateDate_EndTime", string.Format(" and CreateDate <= CONVERT(DATETIME, '{0} 23:59:59') ", Condition["CreateDate_EndTime"].ToString()));
            }
            else
            {
                strSql.Replace("@CreateDate_EndTime", "");

            }


            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public List<Base_MaterielStorage> GetMaterielStorage(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  MaterielStorageId ,
        MaterielId ,
        MaterielCode ,
        MaterielName ,
        MaterielType ,
        MaterielSize ,
        Unit ,
        CustomerId ,
        CustomerCode ,
        CustomerName ,
        CASE WHEN PricePer = 0 THEN ( SELECT  TOP 1  t.PricePer
                                      FROM      Base_MaterielType t
                                      WHERE     t.MaterielId = Base_MaterielStorage.MaterielId
                                    )
             ELSE dbo.Base_MaterielStorage.PricePer
        END PricePer ,
        Qty ,
        CASE WHEN PricePer = 0 THEN ( SELECT  TOP 1   t.PricePer
                                      FROM      Base_MaterielType t
                                      WHERE     t.MaterielId = Base_MaterielStorage.MaterielId
                                    )
             ELSE dbo.Base_MaterielStorage.PricePer
        END * Qty TotalAmount ,
        AlarmNumber ,
        Remark
FROM    Base_MaterielStorage
WHERE   1 = 1  ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["keywords"]))
            {
                strSql.Append(@" AND (MaterielName LIKE @keyword
        OR MaterielCode LIKE @keyword
        OR MaterielType LIKE @keyword
        OR CustomerCode LIKE @keyword
        OR CustomerName LIKE @keyword
        )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + Condition["keywords"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["AlarmNumber"]))
            {
                if (Condition["AlarmNumber"].ToString() == "1")
                {
                    strSql.Append(@"  AND Qty <= AlarmNumber");
                }
            }


            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}
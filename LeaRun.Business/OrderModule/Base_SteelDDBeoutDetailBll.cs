//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2016
// Software Developers @ DNS 2016
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
    /// Base_SteelDDBeoutDetail
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.06.07 21:43</date>
    /// </author>
    /// </summary>
    public class Base_SteelDDBeoutDetailBll : RepositoryFactory<Base_SteelDDBeoutDetail>
    {
        public DataTable GetOrderQueryList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" SELECT  p.Base_SteelDDBeoutCode ,p.PayCheckSheetCode,Base_SteelDDBeoutDetailId,isback,
        p.CustomerName ,
        pd.SteelCode ,
        pd.SteelDescription ,
        pd.Length ,
        pd.AddLength ,
        pd.Weigh ,
        pd.AddWeigh ,
        pd.Thick ,
        pd.AddThick ,
        pd.Qty ,
        pd.Area ,
        pd.TheoryWeight ,
        pd.Weight ,
        pd.MaterialPrice ,
        pd.MaterialAmount ,
        pd.QuotationCode ,
        q.Customername AS Customername1 ,
        q.ShortName AS ShortName ,
        qd.MaterialPrice MaterialPrice1 ,
        qd.MaterialAmount MaterialAmount1 ,
        pd.Remark,p.CreateDate,p.Delivery
FROM    Base_SteelDDBeout p
        LEFT JOIN dbo.Base_SteelDDBeoutDetail pd ON p.Base_SteelDDBeoutId = pd.Base_SteelDDBeoutId
        LEFT JOIN dbo.BY_QuotationDetail qd ON qd.QuotationDetailId = pd.QuotationDetailId
        LEFT JOIN dbo.BY_Quotation q ON q.QuotationId = qd.QuotationId
WHERE   1 = 1 
        ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["Base_SteelDDBeoutCode"]))
            {
                strSql.Append(" AND p.Base_SteelDDBeoutCode LIKE @Base_SteelDDBeoutCode");
                parameter.Add(DbFactory.CreateDbParameter("@Base_SteelDDBeoutCode", '%' + Condition["Base_SteelDDBeoutCode"].ToString() + '%'));
            }
            //订单状态
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (p.CustomerName LIKE @CustomerCode OR p.CustomerCode LIKE @CustomerCode )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", '%' + Condition["CustomerCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["SteelCode"]))
            {
                strSql.Append(" AND (pd.SteelCode LIKE @SteelCode )");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", '%' + Condition["SteelCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Thick"]))
            {
                strSql.Append(" AND ( pd.Thick LIKE @Thick )");
                parameter.Add(DbFactory.CreateDbParameter("@Thick", Condition["Thick"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Weigh"]))
            {
                strSql.Append(" AND ( pd.Weigh LIKE @Weigh )");
                parameter.Add(DbFactory.CreateDbParameter("@Weigh", Condition["Weigh"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Length"]))
            {
                strSql.Append(" AND ( pd.Length LIKE @Length )");
                parameter.Add(DbFactory.CreateDbParameter("@Length", Condition["Length"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode1"]))
            {
                strSql.Append(" AND (q.CustomerName LIKE @CustomerCode1 OR q.CustomerCode LIKE @CustomerCode1 OR q.ShortName LIKE @CustomerCode1  )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode1", '%' + Condition["CustomerCode1"].ToString() + '%'));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["StartTime"]))
            {
                strSql.Append(" AND p.CreateDate >= @StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(Condition["StartTime"] + " 00:00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["EndTime"]))
            {
                strSql.Append(" AND p.CreateDate <= @EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(Condition["EndTime"] + " 23:59")));
            }


            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND pd.QuotationCode like  @QuotationCode ");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%'+ Condition["QuotationCode"].ToString() + '%'));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}
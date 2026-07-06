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
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// Base_Supplier
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.04 16:25</date>
    /// </author>
    /// </summary>
    public class Base_SupplierBll : RepositoryFactory<Base_Supplier>
    {

        public List<Base_Supplier> GetAutoComple(string keywords)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(keywords))
            {
                WhereSql.Append(@" AND (Code LIKE @keyword
                                    OR FullName LIKE @keyword
                                    OR Spell LIKE @keyword
                                    OR ShortName LIKE @keyword)");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keywords + '%'));
            }
            return Repository().FindListTop(10, WhereSql.ToString(), parameter.ToArray());
        }
        public List<Base_Supplier> GetAutoForSupAndCus(string keywords)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(keywords))
            {
                WhereSql.Append(@" SELECT TOP 10
        *
 FROM   ( SELECT    s.SupplierId ,
                    s.Code ,
                    s.FullName ,
                    '供应商' remark,s.ShortName
          FROM      Base_Supplier s
          UNION ALL
          SELECT    c.CustomerId SupplierId ,
                    c.Code ,
                    c.FullName ,
                    '客户' remark,c.ShortName
          FROM      Base_Customer c
        ) a
 WHERE  ( Code LIKE @keyword
          OR FullName LIKE @keyword
          OR ShortName LIKE @keyword
        )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keywords + '%'));
            }
            return Repository().FindListBySql(WhereSql.ToString(), parameter.ToArray());
        }
    }
}
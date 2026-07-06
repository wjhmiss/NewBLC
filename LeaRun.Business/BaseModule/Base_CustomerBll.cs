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
    /// 客户信息管理
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 10:41</date>
    /// </author>
    /// </summary>
    public class Base_CustomerBll : RepositoryFactory<Base_Customer>
    {
        /// <summary>
        /// 客户模糊查询
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <returns></returns>
        public List<Base_Customer> GetAutoComple(string keywords)
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
        public List<Base_Customer> GetAutoCompleNXJ(string keywords)
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
    }
}
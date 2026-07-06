//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2014
// Software Developers @ DNS 2014
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
    /// 视图查询条件表
    /// <author>
    ///		<name>she</name>
    ///		<date>2014.09.04 10:10</date>
    /// </author>
    /// </summary>
    public class Base_ViewWhereBll : RepositoryFactory<Base_ViewWhere>
    {
        /// <summary>
        /// 根据模块Id获取视图查询条件列表
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public List<Base_ViewWhere> GetViewWhereList(string ModuleId)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            WhereSql.Append(" AND ModuleId = @ModuleId");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
    }
}
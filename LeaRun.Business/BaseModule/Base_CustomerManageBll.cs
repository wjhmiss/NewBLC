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
    /// 客户热处理费折扣表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.15 10:53</date>
    /// </author>
    /// </summary>
    public class Base_CustomerManageBll : RepositoryFactory<Base_CustomerManage>
    {
        /// <summary>
        /// 热处理费折扣列表
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
        public List<Base_CustomerManage> GetList(string CustomerId)
        {
            return Repository().FindList("CustomerId", CustomerId);
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsExists(Base_CustomerManage entity)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT COUNT(1) FROM Base_CustomerManage WHERE 1 = 1  
                                    AND CustomerId = @CustomerId
									AND CraftTypeCode = @CraftTypeCode
								    AND (EnabledTimeStart  BETWEEN @EnabledTimeStart AND @EnabledTimeEnd
								    or EnabledTimeEnd  BETWEEN @EnabledTimeStart AND @EnabledTimeEnd)");
            if (!string.IsNullOrEmpty(entity.CustomerManageId))
            {
                strSql.Append(" AND CustomerManageId != @CustomerManageId");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerManageId", entity.CustomerManageId));
            }
            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", entity.CustomerId));
            parameter.Add(DbFactory.CreateDbParameter("@CraftTypeCode", entity.CraftTypeCode));
            parameter.Add(DbFactory.CreateDbParameter("@EnabledTimeStart", entity.EnabledTimeStart));
            parameter.Add(DbFactory.CreateDbParameter("@EnabledTimeEnd", entity.EnabledTimeEnd));
            int Count = Repository().FindCountBySql(strSql.ToString(), parameter.ToArray());
            if (Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
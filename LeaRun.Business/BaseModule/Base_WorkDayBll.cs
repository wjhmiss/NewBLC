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
    /// 工厂日历
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 20:48</date>
    /// </author>
    /// </summary>
    public class Base_WorkDayBll : RepositoryFactory<Base_WorkDay>
    {
        /// <summary>
        /// 获取【工作日历管理】数据列表
        /// </summary>
        /// <param name="CompanyId">公司</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <returns></returns>
        public Base_WorkDay GetList(string CompanyId, string Year, string Month)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT top 1 * FROM Base_WorkDay WHERE 1=1");
            strSql.Append(" AND CompanyId = @CompanyId");
            strSql.Append(" AND Year = @Year");
            strSql.Append(" AND Month = @Month");
            parameter.Add(DbFactory.CreateDbParameter("@CompanyId", CompanyId));
            parameter.Add(DbFactory.CreateDbParameter("@Year", Year));
            parameter.Add(DbFactory.CreateDbParameter("@Month", Month));
            IList List = Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
            foreach (Base_WorkDay item in List)
            {
                return item;
            }
            return null;
        }
    }
}
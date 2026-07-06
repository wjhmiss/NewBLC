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
    /// ¸Ö²Ä×´Ì¬ÉèÖÃ±í
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 14:42</date>
    /// </author>
    /// </summary>
    public class Base_SteelStatusBll : RepositoryFactory<Base_SteelStatus>
    {
        /// <summary>
        /// ¸ù¾Ý¸Ö²Ä±àÂë»ñÈ¡ ×´Ì¬¶ÔÏó
        /// </summary>
        /// <param name="SteelCode">¸Ö²Ä±àÂë</param>
        /// <returns></returns>
        public Base_SteelStatus GetEntityBySteelCode(string SteelCode)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT TOP 1 * FROM Base_SteelStatus WHERE SteelCode= @SteelCode");
            parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            return Repository().FindEntityBySql(strSql.ToString(), parameter.ToArray());
        }
    }
}
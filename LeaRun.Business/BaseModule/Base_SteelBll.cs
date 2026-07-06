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
    /// 钢材信息表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 14:42</date>
    /// </author>
    /// </summary>
    public class Base_SteelBll : RepositoryFactory<Base_Steel>
    {
        /// <summary>
        /// 钢材信息列表
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public List<Base_Steel> GetPageList(string keywords,List<DbParameter> parameter, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            //List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  steel.SteelId ,
                                    steel.SteelCode ,
                                    steel.RepalceMl ,
                                    steel.SteelName ,
                                    steel.OtherDescription ,
                                    steel.Warranty ,
                                    d.FullName AS SteelType ,
                                    steel.Density ,
                                    steel.Price ,
                                    steel.Enabled ,
                                    steel.CreateDate,
                                    steel.ModifyDate ,
                                    steel.ModifyUserName,steel.RemenberCode
                            FROM    Base_Steel steel
                                    LEFT JOIN Base_DataDictionaryDetail d ON d.Code = steel.SteelType AND d.DataDictionaryId = '5bfff457-4a0d-4b0d-bb3a-c12c7c51b08d'");
            if (keywords!=null &&keywords!="")
            {
                strSql.Append(" where 1=1 ");
                strSql.Append(keywords);
            }
            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 钢材信息列表（自动补全）
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<Base_Steel> GetAutoComple(string keywords, string isBanOrYuan = null)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT TOP 10 *
                            FROM    Base_Steel WHERE 1=1 ");

//            if (!string.IsNullOrEmpty(keywords) && !string.IsNullOrEmpty(type))
//            {
//                if (keywords.Length == 1)
//                {
//                    strSql.Append(@" AND ( RemenberCode = @keyword or SteelCode=@keyword )");
//                    parameter.Add(DbFactory.CreateDbParameter("@keyword", keywords));
//                    return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
//                }
//                strSql.Append(@" AND (RemenberCode LIKE @keyword+'%'  OR SteelCode LIKE @keyword+'%') ORDER BY RemenberCode
//                                    ");
//                    parameter.Add(DbFactory.CreateDbParameter("@keyword", keywords.Trim()));
//                    return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
//            }

            if (!string.IsNullOrEmpty(keywords))
            {
                strSql.Append(@" AND (SteelCode LIKE @keyword)");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keywords + '%'));
            }
            if (isBanOrYuan == "Y")
            {
                strSql.Append(@" AND ( CHARINDEX('圆',SteelName) >  0 or CHARINDEX('棒',SteelName) >  0 or CHARINDEX('管',SteelName) >  0 ) ");
            }
            if (isBanOrYuan == "B")
            {
                strSql.Append(@" AND ( CHARINDEX('圆',SteelName) <=  0 AND CHARINDEX('棒',SteelName) <=  0  AND CHARINDEX('管',SteelName) <=  0 ) ");
            }
            strSql.Append(@" ORDER BY SteelCode ");
            return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
        }
        
    }
}
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
    /// 数据字典表
    /// <author>
    ///		<name>she</name>
    ///		<date>2014.07.18 14:08</date>
    /// </author>
    /// </summary>
    public class Base_DataDictionaryBll : RepositoryFactory<Base_DataDictionary>
    {
        /// <summary>
        /// 获取数据字典明细列表
        /// </summary>
        /// <param name="DataDictionaryId">主表 主键值</param>
        /// <returns></returns>
        public List<Base_DataDictionaryDetail> GetDataDictionaryDetailList(string DataDictionaryId)
        {
            if (!string.IsNullOrEmpty(DataDictionaryId))
            {
                StringBuilder WhereSql = new StringBuilder();
                WhereSql.Append(" AND DataDictionaryId = @DataDictionaryId  Order By SortCode ASC");
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@DataDictionaryId", DataDictionaryId));
                return DataFactory.Database().FindList<Base_DataDictionaryDetail>(WhereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取数据字典明细列表
        /// </summary>
        /// <param name="Code">分类编码</param>
        /// <returns></returns>
        public List<Base_DataDictionaryDetail> GetDataDictionaryDetailListByCode(string Code)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                List<DbParameter> parameter = new List<DbParameter>();
                StringBuilder WhereSql = new StringBuilder();

                WhereSql.Append(" and ParentId = '0' and Enabled=1 AND DataDictionaryId IN(SELECT DataDictionaryId FROM Base_DataDictionary WHERE Code=@Code)");
                    WhereSql.Append(" ORDER BY SortCode ASC");
                    parameter.Add(DbFactory.CreateDbParameter("@Code", Code));
                
                return DataFactory.Database().FindList<Base_DataDictionaryDetail>(WhereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
        public List<Base_DataDictionaryDetail> AutocompleteJson(string keywords,string Code)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                List<DbParameter> parameter = new List<DbParameter>();
                StringBuilder WhereSql = new StringBuilder();

                WhereSql.Append(" and ParentId = '0' and Enabled=1 AND DataDictionaryId IN(SELECT DataDictionaryId FROM Base_DataDictionary WHERE Code=@Code  ) and (FullName like '%" + keywords + "%' or code like '%" + keywords + "%')");
                WhereSql.Append(" ORDER BY SortCode ASC");
                parameter.Add(DbFactory.CreateDbParameter("@Code", Code));

                return DataFactory.Database().FindList<Base_DataDictionaryDetail>(WhereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
        public List<Base_DataDictionaryDetail> GetDataDictionaryDetailListByCode1(string Code, string FullName)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                List<DbParameter> parameter = new List<DbParameter>();
                StringBuilder WhereSql = new StringBuilder();
                
                    Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntity<Base_DataDictionaryDetail>("FullName", FullName);
                    WhereSql.AppendFormat(@" and ParentId in (
select DataDictionaryDetailId  from Base_DataDictionaryDetail where
Enabled=1 AND DataDictionaryId IN(SELECT DataDictionaryId FROM Base_DataDictionary WHERE Code='{0}')
and FullName = '{1}')", Code, FullName);
                    WhereSql.Append(" ORDER BY SortCode ASC");
                return DataFactory.Database().FindList<Base_DataDictionaryDetail>(WhereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
        public List<Base_DataDictionaryDetail> BuildProcess(string Code, int G = 0, int J = 0, int M = 0)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                StringBuilder WhereSql = new StringBuilder();
                WhereSql.Append(" and Enabled=1 AND DataDictionaryId IN(SELECT DataDictionaryId FROM Base_DataDictionary WHERE Code=@Code)");
                List<DbParameter> parameter = new List<DbParameter>();
                if (G == 1)
                {
                    WhereSql.Append(" AND GProcess =@G  ");
                    parameter.Add(DbFactory.CreateDbParameter("@G", G));
                }
                if (J == 1)
                {
                    WhereSql.Append(" and JProcess =@J  ");
                    parameter.Add(DbFactory.CreateDbParameter("@J", J));
                }
                if (M == 1)
                {
                    WhereSql.Append(" MProcess =@M  ");
                    parameter.Add(DbFactory.CreateDbParameter("@M", M));
                }
                WhereSql.Append(" ORDER BY SortCode ASC");
                parameter.Add(DbFactory.CreateDbParameter("@Code", Code));
                return DataFactory.Database().FindList<Base_DataDictionaryDetail>(WhereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <returns></returns>
        public List<Base_DataDictionaryDetail> GetAutoComple(string keywords,string parentCode)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(keywords) || !string.IsNullOrEmpty(parentCode))
            {
                WhereSql.Append(@" AND (Code LIKE @keyword OR FullName LIKE @keyword) AND Enabled=1 AND DataDictionaryId = (select DataDictionaryId FROM dbo.Base_DataDictionary WHERE Code=@Code)");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keywords + '%'));
                parameter.Add(DbFactory.CreateDbParameter("@Code", parentCode));
            }
            return DataFactory.Database().FindListTop<Base_DataDictionaryDetail>(10, WhereSql.ToString(), parameter.ToArray());
        }
    }
}
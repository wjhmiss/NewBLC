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
    /// 物料种类
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.10 11:35</date>
    /// </author>
    /// </summary>
    public class Base_MaterielTypeBll : RepositoryFactory<Base_MaterielType>
    {
        public List<Base_MaterielType> GetAutoComple(string keywords)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            
                WhereSql.Append(@"  SELECT top 10 MaterielId ,
        MaterielCode ,
        MaterielName ,
        MaterielType ,
        MaterielSize ,
        Unit ,
        CustomerId ,
        CustomerCode ,
        CustomerName ,PricePer,
        ISNULL(( SELECT Qty
                 FROM   Base_MaterielStorage ms
                 WHERE  ms.MaterielId = mt.MaterielId
               ), 0) Qty
 FROM   Base_MaterielType mt where    (MaterielCode LIKE @keyword
                                    OR MaterielType LIKE @keyword
                                    or MaterielName LIKE @keyword
                                    )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%'+ keywords + '%'));
            return Repository().FindListBySql(WhereSql.ToString(), parameter.ToArray());
        }
    }
}
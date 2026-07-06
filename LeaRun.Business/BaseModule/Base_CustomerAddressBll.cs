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
    /// 客户地址表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 13:38</date>
    /// </author>
    /// </summary>
    public class Base_CustomerAddressBll : RepositoryFactory<Base_CustomerAddress>
    {
        /// <summary>
        /// 根据客户主键获取列表
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public List<Base_CustomerAddress> GetList(string CustomerId)
        {
            return Repository().FindList("CustomerId", CustomerId);
        }
    }
}
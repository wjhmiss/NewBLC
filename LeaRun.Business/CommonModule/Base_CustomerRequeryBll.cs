//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2016
// Software Developers @ DNS 2016
//=====================================================================================

using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// Base_CustomerRequery
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.09 10:24</date>
    /// </author>
    /// </summary>
    public class Base_CustomerRequeryBll : RepositoryFactory<Base_CustomerRequery>
    {
        public List<Base_CustomerRequery> GetList(string CustomerId)
        {
            return Repository().FindList("CustomerId", CustomerId);
        }
    }
}
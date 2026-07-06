using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNS.Business.Utility.Enum
{
    /// <summary>
    /// 报价单状态
    /// </summary>
    public enum QuotationBillStatus
    {
        待审核,
        已审核,
        已驳回,
        已列印,
        已回传,
        已落单,
        暂挂,
        取消暂挂,
        合同取消
    }
}

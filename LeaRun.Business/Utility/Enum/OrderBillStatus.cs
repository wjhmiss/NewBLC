using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNS.Business.Utility.Enum
{
    /// <summary>
    /// 订单单据状态枚举
    /// </summary>
    public enum OrderBillStatus
    {
        录入,
        已审核,
        取消,
        部分取消,
        暂停,
        部分暂停,
        正常生产,
        已完工,
        部分完工,
        已发运,
        部分发运
    }
}

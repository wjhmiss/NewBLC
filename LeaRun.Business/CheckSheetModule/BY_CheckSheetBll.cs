//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace DNS.Business
{
    /// <summary>
    /// 对账单23
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.04.24 11:06</date>
    /// </author>
    /// </summary>
    public class BY_CheckSheetBll : RepositoryFactory<BY_CheckSheet>
    {

        public DataTable SalesummaryList(string StartTime, string EndTime, string CustomerCode, string IsComfirm, string Sellpeople, ref JqGridParam jqgridparam)
        {
            DateTime starttime = Convert.ToDateTime("1999-01-01 00:00:00");
            DateTime endtime = Convert.ToDateTime("2999-01-01 00:00:00");

            if (!string.IsNullOrEmpty(StartTime))
            {
                starttime = Convert.ToDateTime(StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                endtime = Convert.ToDateTime(EndTime).AddDays(1);
            }
            string str = string.Format(@"
SELECT  a.CustomerId ,
        a.CustomerName ,
        a.CustomerCode ,
        a.SalesMan,
        b.gcbefore ,
        b1.ljbefore ,
        c.rclbefore ,
        d.gcnow ,
        d1.ljnow ,
        e.rclnow ,
        f.gcreturn1 ,
        g.gcreturn2 ,
        f1.ljreturn1 ,
        g1.ljreturn2 ,
        h.rclreturn ,
        i.nowreturntotal ,
        j.gctotal ,
        j1.ljtotal ,
        k.rcltotal ,
        l.total ,
        m.gckpbefore ,
        m1.ljkpbefore ,
        n.rclkpbefore ,
        o.gckpnowed ,
        o1.ljkpnowed ,
        p.rclkpnowed ,
        q.gckptotal ,
        q1.ljkptotal ,
        r.rclkptotal,s.zrmoney
FROM    ( SELECT  DISTINCT
                    bc.CustomerId ,
                    bc.CustomerName ,
                    bc.CustomerCode ,
                    bc.SalesMan
          FROM      dbo.BY_Delivery bc
          WHERE     1 = 1 and bc.DeleteMark = 0
                    AND bc.CreateDate <= '{1}'
                    AND bc.Enabled=1
                    @CustomerCode
                    @Sellpeople
          GROUP BY  bc.CustomerId ,
                    bc.CustomerName ,
                    bc.CustomerCode ,
                    bc.SalesMan
        ) a 
------上月余额
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gcbefore
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.Enabled=1
                                        AND bc.CreateDate <= '{0}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1 
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{0}'
UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1 
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{0}'

                            ) a1
                    GROUP BY a1.CustomerId1
                  ) b ON a.CustomerId = b.CustomerId1

LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljbefore
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.Enabled=1
                                        AND bc.CreateDate <= '{0}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1 
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{0}'
UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1 
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{0}'

                            ) a1
                    GROUP BY a1.CustomerId1
                  ) b1 ON a.CustomerId = b1.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rclbefore
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.Enabled=1
                                        AND bc.CreateDate <= '{0}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND bc.RequiredDate <= '{0}'
UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1 
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{0}'

                            ) a1
                    GROUP BY a1.CustomerId1
                  ) c ON a.CustomerId = c.CustomerId1
                  -----
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gcnow
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.CreateDate >= '{0}'
                                        AND bc.CreateDate < '{1}'
                                        AND bc.LType = 'GC'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) d ON a.CustomerId = d.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljnow
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.CreateDate >= '{0}'
                                        AND bc.CreateDate < '{1}'
                                        AND bc.LType = 'LJ'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) d1 ON a.CustomerId = d1.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rclnow
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'RCL'
                                        AND bc.CreateDate >= '{0}'
                                        AND bc.CreateDate < '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) e ON a.CustomerId = e.CustomerId1
                  ----
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gcreturn1
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        AND bc.LType = 'GC'
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                                        AND ( bc.GetType = '现金'
                                              OR bc.GetType = '银行'
                                            )
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) f ON a.CustomerId = f.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljreturn1
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        AND bc.LType = 'LJ'
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                                        AND ( bc.GetType = '现金'
                                              OR bc.GetType = '银行'
                                            )
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) f1 ON a.CustomerId = f1.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gcreturn2
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        AND bc.LType = 'GC'
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                                        AND ( bc.GetType = '承兑' )
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) g ON a.CustomerId = g.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljreturn2
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        AND bc.LType = 'LJ'
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                                        AND ( bc.GetType = '承兑' )
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) g1 ON a.CustomerId = g1.CustomerId1



        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rclreturn
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) h ON a.CustomerId = h.CustomerId1
                  ----
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) nowreturntotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) i ON a.CustomerId = i.CustomerId1
                  ---------
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gctotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.Enabled=1
                                        AND bc.CreateDate <= '{1}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.LType = 'GC'
                                        AND bc.RequiredDate <= '{1}'
 UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        @CustomerCode
                                        AND bc.LType = 'GC'
                                        AND bc.RequiredDate <= '{1}'

                            ) a1
                    GROUP BY a1.CustomerId1
                  ) j ON a.CustomerId = j.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljtotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.Enabled=1
                                        AND bc.CreateDate <= '{1}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.LType = 'LJ'
                                        AND bc.RequiredDate <= '{1}'
 UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        @CustomerCode
                                        AND bc.LType = 'LJ'
                                        AND bc.RequiredDate <= '{1}'

                            ) a1
                    GROUP BY a1.CustomerId1
                  ) j1 ON a.CustomerId = j1.CustomerId1



        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rcltotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND bc.CreateDate <= '{1}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{1}'
 UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) k ON a.CustomerId = k.CustomerId1
                  --
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) total
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        @CustomerCode
                                        AND bc.Enabled=1
                                        AND bc.CreateDate <= '{1}'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.GETMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{1}'
 UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.ZRMoney, 0) AS monthnoget1
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        @CustomerCode
                                        AND bc.RequiredDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) l ON a.CustomerId = l.CustomerId1
                  
                  
                  -------------上月未开票
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gckpbefore
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.CanInvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.BY_CheckSheet bc
                              WHERE     1 = 1 
                                        @CustomerCode
                                        AND bc.LType = 'GC'
                                        AND CAST(bc.YearMonth + '-01' AS DATE) <= '{0}'
                                        and bc.remark = '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'GC'
                                        AND bc.CreateDate <= '{0}'
                                        and bc.remark != '期初'

                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.InvoiceDate <= '{0}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) m ON a.CustomerId = m.CustomerId1

LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljkpbefore
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.CanInvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.BY_CheckSheet bc
                              WHERE     1 = 1 
                                        @CustomerCode
                                        AND bc.LType = 'LJ'
                                        AND CAST(bc.YearMonth + '-01' AS DATE) <= '{0}'
                                        and bc.remark = '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'LJ'
                                        AND bc.CreateDate <= '{0}'
                                        and bc.remark != '期初'

                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.InvoiceDate <= '{0}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) m1 ON a.CustomerId = m1.CustomerId1



        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rclkpbefore
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.CanInvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.BY_CheckSheet bc
                              WHERE     1 = 1 
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND CAST(bc.YearMonth + '-01' AS DATE) <= '{0}'
                                        and bc.remark = '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND bc.CreateDate <= '{0}'
                                        and bc.remark != '期初'

                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.InvoiceDate <= '{0}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) n ON a.CustomerId = n.CustomerId1
                  --本月已开票
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gckpnowed
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.InvoiceDate >= '{0}'
                                        AND bc.InvoiceDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) o ON a.CustomerId = o.CustomerId1

        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljkpnowed
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.InvoiceDate >= '{0}'
                                        AND bc.InvoiceDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) o1 ON a.CustomerId = o1.CustomerId1



        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rclkpnowed
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'rcl'
                                        @CustomerCode
                                        AND bc.InvoiceDate >= '{0}'
                                        AND bc.InvoiceDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) p ON a.CustomerId = p.CustomerId1
                  --本月未开票
        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) gckptotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.CanInvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.BY_CheckSheet bc
                              WHERE     1 = 1 
                                        @CustomerCode
                                        AND bc.LType = 'GC'
                                        AND CAST(bc.YearMonth + '-01' AS DATE) <= '{1}'
                                        and bc.remark = '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'GC'
                                        AND bc.CreateDate <= '{1}'
                                        and bc.remark != '期初'

                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'GC'
                                        @CustomerCode
                                        AND bc.InvoiceDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) q ON a.CustomerId = q.CustomerId1

LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) ljkptotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.CanInvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.BY_CheckSheet bc
                              WHERE     1 = 1 
                                        @CustomerCode
                                        AND bc.LType = 'LJ'
                                        AND CAST(bc.YearMonth + '-01' AS DATE) <= '{1}'
                                        and bc.remark = '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'LJ'
                                        AND bc.CreateDate <= '{1}'
                                        and bc.remark != '期初'

                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        AND bc.LType = 'LJ'
                                        @CustomerCode
                                        AND bc.InvoiceDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) q1 ON a.CustomerId = q1.CustomerId1




        LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(monthnoget1), 0) rclkptotal
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.CanInvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.BY_CheckSheet bc
                              WHERE     1 = 1 
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND CAST(bc.YearMonth + '-01' AS DATE) <= '{1}'
                                        and bc.remark = '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.TotalAmount, 0) AS monthnoget1
                              FROM      dbo.BY_Delivery bc
                              WHERE     1 = 1 and bc.DeleteMark = 0
                                        AND bc.Enabled=1
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND bc.CreateDate <= '{1}'
                                        and bc.remark != '期初'
                              UNION ALL
                              SELECT    bc.CustomerId CustomerId1 ,
                                        -ISNULL(bc.InvoiceMoney, 0) AS monthnoget1
                              FROM      dbo.By_InvoiceForDetail bc
                              WHERE     1 = 1
                                        @CustomerCode
                                        AND bc.LType = 'rcl'
                                        AND bc.InvoiceDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) r ON a.CustomerId = r.CustomerId1
 LEFT JOIN ( SELECT  a1.CustomerId1 ,
                            ISNULL(SUM(ZRMoney), 0) ZRMoney
                    FROM    ( SELECT    bc.CustomerId CustomerId1 ,
                                        ISNULL(bc.ZRMoney, 0) AS ZRMoney
                              FROM      dbo.BY_ReimburseSheet bc
                              WHERE     1 = 1
                                        @CustomerCode
                                        AND bc.RequiredDate >= '{0}'
                                        AND bc.RequiredDate <= '{1}'
                            ) a1
                    GROUP BY a1.CustomerId1
                  ) s ON a.CustomerId = s.CustomerId1
                  
", starttime.ToShortDateString(), endtime.ToShortDateString());
            str = str.Replace("@CustomerCode", string.IsNullOrEmpty(CustomerCode) ? "" : " and bc.CustomerCode ='" + CustomerCode + "' ");
            str = str.Replace("@Sellpeople", string.IsNullOrEmpty(Sellpeople) ? "" : " and bc.SalesMan ='" + Sellpeople + "' ");

                    

            return Repository().FindTablePageBySql(str, ref jqgridparam);

        }


        /// <summary>
        /// 获得最新入库单号
        /// </summary>
        /// <param name="by_instock">入库信息</param>
        /// <returns>入库单号</returns>
        //public BY_CheckSheet GetCheckSheetCode(BY_CheckSheet by_checksheet, BY_CheckSheet by_checksheetsDelete, string Year, string Month)
        //{
        //    string seriousCode = "00001";
        //    string starttime = "CS" + Year + Month;
        //    BY_CheckSheet maxBY_InStock = Repository().FindEntityBySql("SELECT TOP 1 * FROM   BY_CheckSheet where CheckSheetCode like '" + starttime + "%' ORDER BY CheckSheetCode DESC   ");
        //    if (maxBY_InStock.CheckSheetCode != null)
        //    {
        //        //取出流水号
        //        seriousCode = maxBY_InStock.CheckSheetCode.Substring(maxBY_InStock.CheckSheetCode.Length - 5, 5);
        //        seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5, '0');
        //    }


        //    if (by_checksheet.YearMonth == by_checksheetsDelete.YearMonth && by_checksheet.CustomerId == by_checksheetsDelete.CustomerId)
        //    {
        //        by_checksheet.CheckSheetCode = by_checksheetsDelete.CheckSheetCode;
        //        by_checksheet.IsComfirm = by_checksheetsDelete.IsComfirm;
        //        by_checksheet.IsInvoice = by_checksheetsDelete.IsInvoice;
        //        by_checksheet.IsOver = by_checksheetsDelete.IsOver;
        //        //seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5, '0');
        //    }
        //    if (string.IsNullOrEmpty(by_checksheet.CheckSheetCode))
        //    {

        //        by_checksheet.CheckSheetCode = starttime + "-" + seriousCode;
        //        seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5, '0');
        //    }
        //    return by_checksheet;
        //}
        /// <summary>
        /// 获得最新入库单号
        /// </summary>
        /// <param name="by_instock">入库信息</param>
        /// <returns>入库单号</returns>
        //public List<BY_CheckSheet> GetCheckSheetCode(List<BY_CheckSheet> by_checksheets, List<BY_CheckSheet> by_checksheetsDelete, string Year, string Month)
        //{
        //    string seriousCode = "00001";
        //    string starttime = "CS" + Convert.ToDateTime(Year + "-" + Month + "-1" + " 00:00:00").ToString("yyyyMM").Split(' ')[0];
        //    BY_CheckSheet maxBY_InStock = Repository().FindEntityBySql("SELECT TOP 1 * FROM   BY_CheckSheet where CheckSheetCode like '" + starttime + "%' ORDER BY CheckSheetCode DESC   ");
        //    if (maxBY_InStock.CheckSheetCode != null)
        //    {
        //        //取出流水号
        //        seriousCode = maxBY_InStock.CheckSheetCode.Substring(maxBY_InStock.CheckSheetCode.Length - 5, 5);
        //        seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5, '0');
        //    }
        //    foreach (BY_CheckSheet by_checksheet in by_checksheets)
        //    {
        //        foreach (BY_CheckSheet by_checksheetdelete in by_checksheetsDelete)
        //        {
        //            if (by_checksheet.BeginDate == by_checksheetdelete.BeginDate && by_checksheet.EndDate == by_checksheetdelete.EndDate && by_checksheet.CustomerId == by_checksheetdelete.CustomerId)
        //            {
        //                by_checksheet.CheckSheetCode = by_checksheetdelete.CheckSheetCode;
        //                by_checksheet.IsComfirm = by_checksheetdelete.IsComfirm;
        //                by_checksheet.IsInvoice = by_checksheetdelete.IsInvoice;
        //                by_checksheet.IsOver = by_checksheetdelete.IsOver;
        //                //seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5, '0');
        //            }
        //        }
        //        if (string.IsNullOrEmpty(by_checksheet.CheckSheetCode))
        //        {
        //            string spec = "CS" + by_checksheet.BeginDate.ToString("yyyyMM").Split(' ')[0];
        //            by_checksheet.CheckSheetCode = spec + "-" + seriousCode;
        //            seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5, '0');
        //        }
        //    }
        //    return by_checksheets;
        //}
        //public BY_CheckSheet GetCheckSheetGetedMoney(BY_CheckSheet by_checksheet, List<BY_ReimburseSheet> by_reimbursesheets, List<BY_Delivery> by_deliverysupdates)
        //{

        //        by_checksheet.GetedAmount = (from entity in by_reimbursesheets
        //                                     where entity.Code == by_checksheet.CheckSheetCode && entity.Status == "1"
        //                                     select entity).ToArray<BY_ReimburseSheet>().Sum(p => p.RequiredMoney);
        //        by_checksheet.GetedAmount += (from entity in by_deliverysupdates

        //                                      select entity).ToArray<BY_Delivery>().Sum(p => p.GetedAmount);
        //    return by_checksheet;
        //}
        //public List<BY_CheckSheet> GetCheckSheetGetedMoney(List<BY_CheckSheet> by_checksheets, List<BY_ReimburseSheet> by_reimbursesheets, List<BY_Delivery> by_deliverysupdates)
        //{
        //    foreach (BY_CheckSheet by_checksheet in by_checksheets)
        //    {
        //        by_checksheet.GetedAmount = (from entity in by_reimbursesheets
        //                                     where entity.Code == by_checksheet.CheckSheetCode && entity.Status=="1"
        //                                     select entity).ToArray<BY_ReimburseSheet>().Sum(p=>p.RequiredMoney);
        //        by_checksheet.GetedAmount += (from entity in by_deliverysupdates
        //                                     where entity.CheckSheetCode == by_checksheet.CheckSheetCode
        //                                      select entity).ToArray<BY_Delivery>().Sum(p => p.GetedAmount);
        //    }
        //    return by_checksheets;
        //}
        public DataTable GetSaleDetail(string Year, string CustomerName, string type, string IsComfirm, string Sellpeople, ref JqGridParam jqgridparam)
        {
            string str = string.Format(@"select wok.*,c.Sellpeople from ( SELECT * FROM (SELECT *,'送货收款' type  FROM  
( SELECT  
                                bc.CustomerId ,
                                bc.CustomerName ,
                                bc.CustomerCode ,
                                '现结' PaymentMode
                      FROM      dbo.BY_Delivery bc
                      WHERE     DATEPART(year, bc.CreateDate) = {0}   @CustomerName
                      GROUP BY bc.CustomerId ,
                                bc.CustomerName ,
                                bc.CustomerCode
                    ) c

LEFT JOIN 
( SELECT  SUM(monthtotal1) monthtotal1,
                                        SUM(monthgeted1) monthgeted1,
                                        SUM(monthnoget1) monthnoget1,
                                        CustomerId1
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal1 ,
                                                    SUM(bc.GetedAmount) AS monthgeted1 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget1 ,
                                                    bc.CustomerId CustomerId1
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal1 ,
                                                    SUM(bc.GetedAmount) AS monthgeted1 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget1 ,
                                                    bc.CustomerId CustomerId1
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-1'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k1 GROUP BY  k1.CustomerId1
                              ) m1 ON c.CustomerId = m1.CustomerId1

    LEFT JOIN 
( SELECT  SUM(monthtotal2) monthtotal2,
                                        SUM(monthgeted2) monthgeted2,
                                        SUM(monthnoget2) monthnoget2,
                                        CustomerId2
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,
                                                    SUM(bc.GetedAmount) AS monthgeted2 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget2 ,
                                                    bc.CustomerId CustomerId2
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,
                                                    SUM(bc.GetedAmount) AS monthgeted2 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget2 ,
                                                    bc.CustomerId CustomerId2
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-2'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k2 GROUP BY  k2.CustomerId2

        ) m2 ON c.CustomerId = m2.CustomerId2
            LEFT JOIN 
( SELECT  SUM(monthtotal3) monthtotal3,
                                        SUM(monthgeted3) monthgeted3,
                                        SUM(monthnoget3) monthnoget3,
                                        CustomerId3
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,
                                                    SUM(bc.GetedAmount) AS monthgeted3 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget3 ,
                                                    bc.CustomerId CustomerId3
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                   
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,
                                                    SUM(bc.GetedAmount) AS monthgeted3 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget3 ,
                                                    bc.CustomerId CustomerId3
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-3'
                                                    @CustomerName
                                                    
                                                   
                                          GROUP BY  bc.CustomerId
                                        )  k3 GROUP BY  k3.CustomerId3
                              ) m3 ON c.CustomerId = m3.CustomerId3

        
            LEFT JOIN 
( SELECT  SUM(monthtotal4) monthtotal4,
                                        SUM(monthgeted4) monthgeted4,
                                        SUM(monthnoget4) monthnoget4,
                                        CustomerId4
                                FROM    (
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,
                                                    SUM(bc.GetedAmount) AS monthgeted4 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget4 ,
                                                    bc.CustomerId CustomerId4
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,
                                                    SUM(bc.GetedAmount) AS monthgeted4 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget4 ,
                                                    bc.CustomerId CustomerId4
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-4'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k4 GROUP BY  k4.CustomerId4
                              ) m4 ON c.CustomerId = m4.CustomerId4

        
            LEFT JOIN 
( SELECT  SUM(monthtotal5) monthtotal5,
                                        SUM(monthgeted5) monthgeted5,
                                        SUM(monthnoget5) monthnoget5,
                                        CustomerId5
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,
                                                    SUM(bc.GetedAmount) AS monthgeted5 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget5 ,
                                                    bc.CustomerId CustomerId5
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled =1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,
                                                    SUM(bc.GetedAmount) AS monthgeted5 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget5 ,
                                                    bc.CustomerId CustomerId5
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-5'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k5 GROUP BY  k5.CustomerId5
                              ) m5 ON c.CustomerId = m5.CustomerId5

        
            LEFT JOIN 
( SELECT  SUM(monthtotal6) monthtotal6,
                                        SUM(monthgeted6) monthgeted6,
                                        SUM(monthnoget6) monthnoget6,
                                        CustomerId6
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,
                                                    SUM(bc.GetedAmount) AS monthgeted6 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget6 ,
                                                    bc.CustomerId CustomerId6
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,
                                                    SUM(bc.GetedAmount) AS monthgeted6 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget6 ,
                                                    bc.CustomerId CustomerId6
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-6'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k6 GROUP BY  k6.CustomerId6
                              ) m6 ON c.CustomerId = m6.CustomerId6

        
            LEFT JOIN 
( SELECT  SUM(monthtotal7) monthtotal7,
                                        SUM(monthgeted7) monthgeted7,
                                        SUM(monthnoget7) monthnoget7,
                                        CustomerId7
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,
                                                    SUM(bc.GetedAmount) AS monthgeted7 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget7 ,
                                                    bc.CustomerId CustomerId7
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,
                                                    SUM(bc.GetedAmount) AS monthgeted7 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget7 ,
                                                    bc.CustomerId CustomerId7
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-7'
                                                    @CustomerName
                                                   
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k7 GROUP BY  k7.CustomerId7
                              ) m7 ON c.CustomerId = m7.CustomerId7

        
            LEFT JOIN 

( SELECT  SUM(monthtotal8) monthtotal8,
                                        SUM(monthgeted8) monthgeted8,
                                        SUM(monthnoget8) monthnoget8,
                                        CustomerId8
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,
                                                    SUM(bc.GetedAmount) AS monthgeted8 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget8 ,
                                                    bc.CustomerId CustomerId8
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
 SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,
                                                    SUM(bc.GetedAmount) AS monthgeted8 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget8 ,
                                                    bc.CustomerId CustomerId8
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-8'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k8 GROUP BY  k8.CustomerId8
                              ) m8 ON c.CustomerId = m8.CustomerId8


            LEFT JOIN 
( SELECT  SUM(monthtotal9) monthtotal9,
                                        SUM(monthgeted9) monthgeted9,
                                        SUM(monthnoget9) monthnoget9,
                                        CustomerId9
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,
                                                    SUM(bc.GetedAmount) AS monthgeted9 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget9 ,
                                                    bc.CustomerId CustomerId9
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,
                                                    SUM(bc.GetedAmount) AS monthgeted9 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget9 ,
                                                    bc.CustomerId CustomerId9
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-9'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k9 GROUP BY  k9.CustomerId9
                              ) m9 ON c.CustomerId = m9.CustomerId9

        
            LEFT JOIN 
( SELECT  SUM(monthtotal10) monthtotal10,
                                        SUM(monthgeted10) monthgeted10,
                                        SUM(monthnoget10) monthnoget10,
                                        CustomerId10
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal10 ,
                                                    SUM(bc.GetedAmount) AS monthgeted10 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget10 ,
                                                    bc.CustomerId CustomerId10
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal10 ,
                                                    SUM(bc.GetedAmount) AS monthgeted10 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget10 ,
                                                    bc.CustomerId CustomerId10
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-10'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k10 GROUP BY  k10.CustomerId10
                              ) m10 ON c.CustomerId = m10.CustomerId10

        
            LEFT JOIN 

( SELECT  SUM(monthtotal11) monthtotal11,
                                        SUM(monthgeted11) monthgeted11,
                                        SUM(monthnoget11) monthnoget11,
                                        CustomerId11
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal11 ,
                                                    SUM(bc.GetedAmount) AS monthgeted11 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget11 ,
                                                    bc.CustomerId CustomerId11
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                   
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal11 ,
                                                    SUM(bc.GetedAmount) AS monthgeted11 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget11 ,
                                                    bc.CustomerId CustomerId11
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-11'
                                                    @CustomerName
                                                    
                                                   
                                          GROUP BY  bc.CustomerId
                                        )  k11 GROUP BY  k11.CustomerId11
                              ) m11 ON c.CustomerId = m11.CustomerId11


            LEFT JOIN 
( SELECT  SUM(monthtotal12) monthtotal12,
                                        SUM(monthgeted12) monthgeted12,
                                        SUM(monthnoget12) monthnoget12,
                                        CustomerId12
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotal12 ,
                                                    SUM(bc.GetedAmount) AS monthgeted12 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget12 ,
                                                    bc.CustomerId CustomerId12
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                    
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotal12 ,
                                                    SUM(bc.GetedAmount) AS monthgeted12 ,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnoget12 ,
                                                    bc.CustomerId CustomerId12
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth = '{0}-12'
                                                    @CustomerName
                                                    
                                                    
                                          GROUP BY  bc.CustomerId
                                        )  k12 GROUP BY  k12.CustomerId12
                              ) m12 ON c.CustomerId = m12.CustomerId12

        
LEFT JOIN 
( SELECT  SUM(monthtotalall) monthtotalall,
                                        SUM(monthgetedall) monthgetedall,
                                        SUM(monthnogetall) monthnogetall,
                                        CustomerIdall
                                FROM    ( 
                                          SELECT    SUM(bc.TotalAmount) AS monthtotalall,
                                                    SUM(bc.GetedAmount) AS monthgetedall,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnogetall,
                                                    bc.CustomerId CustomerIdall
                                          FROM      dbo.BY_Delivery bc
                                          WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                                    AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') >= bc.CreateDate
                                                    @CustomerName
                                                    AND Enabled = 1 and (CheckSheetCode is null or CheckSheetCode = '')
                                                     
                                          GROUP BY  bc.CustomerId
union all
SELECT    SUM(bc.TotalAmount) AS monthtotalall,
                                                    SUM(bc.GetedAmount) AS monthgetedall,
                                                    SUM(bc.TotalAmount
                                                        - bc.GetedAmount) AS monthnogetall,
                                                    bc.CustomerId CustomerIdall
                                          FROM      dbo.BY_CheckSheet bc
                                          WHERE     YearMonth like '{0}%'
                                                    @CustomerName
                                                    
                                                     
                                          GROUP BY  bc.CustomerId
                                        )  kall GROUP BY  kall.CustomerIdall
                              ) mall ON c.CustomerId = mall.CustomerIdall


) cs
        
      union all
      
      SELECT * FROM (SELECT *,'其他收款' type FROM (SELECT bc.CustomerId,bc.CustomerName,bc.CustomerCode,'其他' as PaymentMode
FROM dbo.BY_CheckCompany bc WHERE  CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate and bc.IsAudit =1 @CustomerName @type2 GROUP BY bc.CustomerId,bc.CustomerName,bc.CustomerCode) c

LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal1,
        SUM(bc.GetedAmount) AS monthgeted1,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget1,bc.CustomerId CustomerId1
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m1 ON c.CustomerId =m1.CustomerId1
        
    LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal2,
        SUM(bc.GetedAmount) AS monthgeted2,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget2,bc.CustomerId CustomerId2
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m2 ON c.CustomerId =m2.CustomerId2
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal3,
        SUM(bc.GetedAmount) AS monthgeted3,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget3,bc.CustomerId CustomerId3
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m3 ON c.CustomerId =m3.CustomerId3
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal4,
        SUM(bc.GetedAmount) AS monthgeted4,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget4,bc.CustomerId CustomerId4
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m4 ON c.CustomerId =m4.CustomerId4
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal5,
        SUM(bc.GetedAmount) AS monthgeted5,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget5,bc.CustomerId CustomerId5
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m5 ON c.CustomerId =m5.CustomerId5
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal6,
        SUM(bc.GetedAmount) AS monthgeted6,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget6,bc.CustomerId CustomerId6
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m6 ON c.CustomerId =m6.CustomerId6
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal7,
        SUM(bc.GetedAmount) AS monthgeted7,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget7,bc.CustomerId CustomerId7
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m7 ON c.CustomerId =m7.CustomerId7
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal8,
        SUM(bc.GetedAmount) AS monthgeted8,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget8,bc.CustomerId CustomerId8
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m8 ON c.CustomerId =m8.CustomerId8
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal9,
        SUM(bc.GetedAmount) AS monthgeted9,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget9,bc.CustomerId CustomerId9
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m9 ON c.CustomerId =m9.CustomerId9
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal10,
        SUM(bc.GetedAmount) AS monthgeted10,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget10,bc.CustomerId CustomerId10
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m10 ON c.CustomerId =m10.CustomerId10
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal11,
        SUM(bc.GetedAmount) AS monthgeted11,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget11,bc.CustomerId CustomerId11
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m11 ON c.CustomerId =m11.CustomerId11
        
            LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotal12,
        SUM(bc.GetedAmount) AS monthgeted12,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnoget12,bc.CustomerId CustomerId12
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m12 ON c.CustomerId =m12.CustomerId12
        
      LEFT JOIN (
SELECT  SUM(bc.TotalAmount) AS monthtotalall,
        SUM(bc.GetedAmount) AS monthgetedall,
        SUM(bc.TotalAmount - bc.GetedAmount) AS monthnogetall,bc.CustomerId CustomerIdall
FROM    dbo.BY_CheckCompany bc
WHERE   CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') >= bc.CreateDate @CustomerName
        GROUP BY bc.CustomerId) m13 ON c.CustomerId =m13.CustomerIdall



) cs2 ) wok,Base_Customer c where c.CustomerId=wok.CustomerId  @Sellpeople", Year, Convert.ToInt32(Year) + 1);

            str = str.Replace("@YearMonth1", "YearMonth = '" + Year + "-1" + "'");
            str = str.Replace("@YearMonth2", "YearMonth = '" + Year + "-2" + "'");
            str = str.Replace("@YearMonth3", "YearMonth = '" + Year + "-3" + "'");
            str = str.Replace("@YearMonth4", "YearMonth = '" + Year + "-4" + "'");
            str = str.Replace("@YearMonth5", "YearMonth = '" + Year + "-5" + "'");
            str = str.Replace("@YearMonth6", "YearMonth = '" + Year + "-6" + "'");
            str = str.Replace("@YearMonth7", "YearMonth = '" + Year + "-7" + "'");
            str = str.Replace("@YearMonth8", "YearMonth = '" + Year + "-8" + "'");
            str = str.Replace("@YearMonth9", "YearMonth = '" + Year + "-9" + "'");
            str = str.Replace("@YearMontha1", "YearMonth = '" + Year + "-10" + "'");
            str = str.Replace("@YearMonthb1", "YearMonth = '" + Year + "-11" + "'");
            str = str.Replace("@YearMonthc1", "YearMonth = '" + Year + "-12" + "'");
            str = str.Replace("@YearMonthall", "YearMonth like '" + Year + "%'");

            if (string.IsNullOrEmpty(CustomerName))
            {
                str = str.Replace("@CustomerName", "");
            }
            else
            {
                str = str.Replace("@CustomerName", " and (CustomerCode='" + CustomerName + "' or CustomerName like '%" + CustomerName + "%')");
            }
            if (!string.IsNullOrEmpty(IsComfirm) && IsComfirm == "true")
            {
                str = str.Replace("@IsComfirm", "   AND (bc.IsComfirm = 1 OR bc.GetedAmount >0) ");
            }
            else
            {
                str = str.Replace("@IsComfirm", "");
            }


            if (type == "送货收款")
            {
                str = str.Replace("@type1", "");
                str = str.Replace("@type2", " and 1=2 ");
            }
            if (type == "其他收款")
            {
                str = str.Replace("@type1", "  and 1=2 ");
                str = str.Replace("@type2", "");
            }
            if (string.IsNullOrEmpty(type))
            {
                str = str.Replace("@type1", "");
                str = str.Replace("@type2", "");
            }

            if (string.IsNullOrWhiteSpace(Sellpeople))
            {
                str = str.Replace("@Sellpeople", "");

            }
            else
            {
                str = str.Replace("@Sellpeople", " and c.Sellpeople = '" + Sellpeople + "' ");

            }
            return Repository().FindTablePageBySql(str, ref jqgridparam);



        }
    }
}
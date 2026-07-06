//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// 物料购买申请
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.10 11:52</date>
    /// </author>
    /// </summary>
    public class Base_MaterielPurchaseBll : RepositoryFactory<Base_MaterielPurchase>
    {
        public DataTable GridPageJsonCostPerYear1(string Year, string CustomerName, string type, ref JqGridParam jqgridparam)
        {
            string str = string.Format(@"SELECT * FROM (SELECT  *,'委外加工' type
FROM    ( SELECT    bc.CustomerName ,
                    bc.CustomerCode,bc.CustomerId
          FROM      dbo.Base_SteelDDBeout bc
          WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                    AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate
                    @CustomerName @type1
          GROUP BY  bc.CustomerCode  ,bc.CustomerId,
                    bc.CustomerName
        ) c
        LEFT JOIN ( SELECT  * ,
                            monthtotal1 -ISNULL(monthgeted1,0)  AS monthnoget1
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal1 ,SUM(bc.GetedAmount) AS monthgeted1,
                                        bc.CustomerId CustomerCode11s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m1 ON c.CustomerId = m1.CustomerCode11s
        LEFT JOIN ( SELECT  * ,
                            monthtotal2 -ISNULL(monthgeted2,0) AS monthnoget2
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,SUM(bc.GetedAmount) AS monthgeted2,
                                        bc.CustomerId CustomerCode12s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m2 ON c.CustomerId = m2.CustomerCode12s
        LEFT JOIN ( SELECT  * ,
                            monthtotal3 -ISNULL(monthgeted3,0) AS monthnoget3
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,SUM(bc.GetedAmount) AS monthgeted3,
                                        bc.CustomerId CustomerCode13s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m3 ON c.CustomerId = m3.CustomerCode13s
        LEFT JOIN ( SELECT  * ,
                            monthtotal4 -ISNULL(monthgeted4,0) AS monthnoget4
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,SUM(bc.GetedAmount) AS monthgeted4,
                                        bc.CustomerId CustomerCode14s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m4 ON c.CustomerId = m4.CustomerCode14s
        LEFT JOIN ( SELECT  * ,
                            monthtotal5 -ISNULL(monthgeted5,0) AS monthnoget5
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,SUM(bc.GetedAmount) AS monthgeted5,
                                        bc.CustomerId CustomerCode15s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m5 ON c.CustomerId = m5.CustomerCode15s
        LEFT JOIN ( SELECT  * ,
                            monthtotal6 -ISNULL(monthgeted6,0) AS monthnoget6
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,SUM(bc.GetedAmount) AS monthgeted6,
                                        bc.CustomerId CustomerCode16s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m6 ON c.CustomerId = m6.CustomerCode16s
        LEFT JOIN ( SELECT  * ,
                            (monthtotal7 - ISNULL(monthgeted7,0)) AS monthnoget7
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,SUM(bc.GetedAmount) AS monthgeted7,
                                        bc.CustomerId CustomerCode17s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m7 ON c.CustomerId = m7.CustomerCode17s
        LEFT JOIN ( SELECT  * ,
                            monthtotal8 -ISNULL(monthgeted8,0) AS monthnoget8
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,SUM(bc.GetedAmount) AS monthgeted8,
                                        bc.CustomerId CustomerCode18s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m8 ON c.CustomerId = m8.CustomerCode18s
        LEFT JOIN ( SELECT  * ,
                            monthtotal9 -ISNULL(monthgeted9,0) AS monthnoget9
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,SUM(bc.GetedAmount) AS monthgeted9,
                                        bc.CustomerId CustomerCode19s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m9 ON c.CustomerId = m9.CustomerCode19s
        LEFT JOIN ( SELECT  * ,
                            monthtotal10 -ISNULL(monthgeted10,0) AS monthnoget10
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal10 ,SUM(bc.GetedAmount) AS monthgeted10,
                                        bc.CustomerId CustomerCode110s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                           
                  ) m10 ON c.CustomerId = m10.CustomerCode110s
        LEFT JOIN ( SELECT  * ,
                            monthtotal11 -ISNULL(monthgeted11,0) AS monthnoget11
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal11 ,SUM(bc.GetedAmount) AS monthgeted11,
                                        bc.CustomerId CustomerCode111s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m11 ON c.CustomerId = m11.CustomerCode111s
        LEFT JOIN ( SELECT  * ,
                            monthtotal12 -ISNULL(monthgeted12,0) AS monthnoget12
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal12 ,SUM(bc.GetedAmount) AS monthgeted12,
                                        bc.CustomerId CustomerCode112s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m12 ON c.CustomerId = m12.CustomerCode112s
        LEFT JOIN ( SELECT  * ,
                            monthtotalall -ISNULL(monthgetedall,0) AS monthnogetall
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotalall ,SUM(bc.GetedAmount) AS monthgetedall,
                                        bc.CustomerId CustomerCode113s
                              FROM      dbo.Base_SteelDDBeout bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m13 ON c.CustomerId = m13.CustomerCode113s


) p1
UNION ALL
SELECT * FROM (SELECT  *,'委外采购' type
FROM    ( SELECT    bc.CustomerName ,
                    bc.CustomerCode,bc.CustomerId
          FROM      dbo.Base_SteelDDPerchase bc
          WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                    AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate
                    @CustomerName @type2
          GROUP BY  bc.CustomerCode  ,bc.CustomerId,
                    bc.CustomerName
        ) c
        LEFT JOIN ( SELECT  * ,
                            monthtotal1 -ISNULL(monthgeted1,0)  AS monthnoget1
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal1 ,SUM(bc.GetedAmount) AS monthgeted1,
                                        bc.CustomerId CustomerCode11s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m1 ON c.CustomerId = m1.CustomerCode11s
        LEFT JOIN ( SELECT  * ,
                            monthtotal2 -ISNULL(monthgeted2,0) AS monthnoget2
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,SUM(bc.GetedAmount) AS monthgeted2,
                                        bc.CustomerId CustomerCode12s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m2 ON c.CustomerId = m2.CustomerCode12s
        LEFT JOIN ( SELECT  * ,
                            monthtotal3 -ISNULL(monthgeted3,0) AS monthnoget3
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,SUM(bc.GetedAmount) AS monthgeted3,
                                        bc.CustomerId CustomerCode13s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m3 ON c.CustomerId = m3.CustomerCode13s
        LEFT JOIN ( SELECT  * ,
                            monthtotal4 -ISNULL(monthgeted4,0) AS monthnoget4
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,SUM(bc.GetedAmount) AS monthgeted4,
                                        bc.CustomerId CustomerCode14s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m4 ON c.CustomerId = m4.CustomerCode14s
        LEFT JOIN ( SELECT  * ,
                            monthtotal5 -ISNULL(monthgeted5,0) AS monthnoget5
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,SUM(bc.GetedAmount) AS monthgeted5,
                                        bc.CustomerId CustomerCode15s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m5 ON c.CustomerId = m5.CustomerCode15s
        LEFT JOIN ( SELECT  * ,
                            monthtotal6 -ISNULL(monthgeted6,0) AS monthnoget6
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,SUM(bc.GetedAmount) AS monthgeted6,
                                        bc.CustomerId CustomerCode16s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m6 ON c.CustomerId = m6.CustomerCode16s
        LEFT JOIN ( SELECT  * ,
                            (monthtotal7 - ISNULL(monthgeted7,0)) AS monthnoget7
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,SUM(bc.GetedAmount) AS monthgeted7,
                                        bc.CustomerId CustomerCode17s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m7 ON c.CustomerId = m7.CustomerCode17s
        LEFT JOIN ( SELECT  * ,
                            monthtotal8 -ISNULL(monthgeted8,0) AS monthnoget8
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,SUM(bc.GetedAmount) AS monthgeted8,
                                        bc.CustomerId CustomerCode18s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m8 ON c.CustomerId = m8.CustomerCode18s
        LEFT JOIN ( SELECT  * ,
                            monthtotal9 -ISNULL(monthgeted9,0) AS monthnoget9
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,SUM(bc.GetedAmount) AS monthgeted9,
                                        bc.CustomerId CustomerCode19s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m9 ON c.CustomerId = m9.CustomerCode19s
        LEFT JOIN ( SELECT  * ,
                            monthtotal10 -ISNULL(monthgeted10,0) AS monthnoget10
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal10 ,SUM(bc.GetedAmount) AS monthgeted10,
                                        bc.CustomerId CustomerCode110s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                           
                  ) m10 ON c.CustomerId = m10.CustomerCode110s
        LEFT JOIN ( SELECT  * ,
                            monthtotal11 -ISNULL(monthgeted11,0) AS monthnoget11
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal11 ,SUM(bc.GetedAmount) AS monthgeted11,
                                        bc.CustomerId CustomerCode111s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m11 ON c.CustomerId = m11.CustomerCode111s
        LEFT JOIN ( SELECT  * ,
                            monthtotal12 -ISNULL(monthgeted12,0) AS monthnoget12
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal12 ,SUM(bc.GetedAmount) AS monthgeted12,
                                        bc.CustomerId CustomerCode112s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m12 ON c.CustomerId = m12.CustomerCode112s
        LEFT JOIN ( SELECT  * ,
                            monthtotalall -ISNULL(monthgetedall,0) AS monthnogetall
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotalall ,SUM(bc.GetedAmount) AS monthgetedall,
                                        bc.CustomerId CustomerCode113s
                              FROM      dbo.Base_SteelDDPerchase bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m13 ON c.CustomerId = m13.CustomerCode113s

) p2
UNION ALL
SELECT * FROM (SELECT  *,'费用申请' type
FROM    ( SELECT    '费用申请' CustomerName ,
                    '费用申请' CustomerCode ,'费用申请' CustomerId WHERE 1=1 @Customer @type5
        ) c
        LEFT JOIN ( SELECT  * ,
                            monthtotall - ISNULL(monthgeted1, 0) AS monthnoget1
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotall ,SUM(bc.GetedAmount) AS monthgeted1,
                                        '费用申请' CustomerCode11s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m1 ON c.CustomerCode = m1.CustomerCode11s
        LEFT JOIN ( SELECT  * ,
                            monthtotal2 - ISNULL(monthgeted2, 0) AS monthnoget2
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,SUM(bc.GetedAmount) AS monthgeted2,
                                        '费用申请' CustomerCode12s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m2 ON c.CustomerCode = m2.CustomerCode12s
        LEFT JOIN ( SELECT  * ,
                            monthtotal3 - ISNULL(monthgeted3, 0) AS monthnoget3
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,SUM(bc.GetedAmount) AS monthgeted3,
                                        '费用申请' CustomerCode13s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m3 ON c.CustomerCode = m3.CustomerCode13s
        LEFT JOIN ( SELECT  * ,
                            monthtotal4 - ISNULL(monthgeted4, 0) AS monthnoget4
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,SUM(bc.GetedAmount) AS monthgeted4,
                                        '费用申请' CustomerCode14s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m4 ON c.CustomerCode = m4.CustomerCode14s
        LEFT JOIN ( SELECT  * ,
                            monthtotal5 - ISNULL(monthgeted5, 0) AS monthnoget5
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,SUM(bc.GetedAmount) AS monthgeted5,
                                        '费用申请' CustomerCode15s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m5 ON c.CustomerCode = m5.CustomerCode15s
        LEFT JOIN ( SELECT  * ,
                            monthtotal6 - ISNULL(monthgeted6, 0) AS monthnoget6
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,SUM(bc.GetedAmount) AS monthgeted6,
                                        '费用申请' CustomerCode16s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m6 ON c.CustomerCode = m6.CustomerCode16s
        LEFT JOIN ( SELECT  * ,
                            ( monthtotal7 - ISNULL(monthgeted7, 0) ) AS monthnoget7
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,SUM(bc.GetedAmount) AS monthgeted7,
                                        '费用申请' CustomerCode17s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m7 ON c.CustomerCode = m7.CustomerCode17s
        LEFT JOIN ( SELECT  * ,
                            monthtotal8 - ISNULL(monthgeted8, 0) AS monthnoget8
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,SUM(bc.GetedAmount) AS monthgeted8,
                                        '费用申请' CustomerCode18s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m8 ON c.CustomerCode = m8.CustomerCode18s
        LEFT JOIN ( SELECT  * ,
                            monthtotal9 - ISNULL(monthgeted9, 0) AS monthnoget9
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,SUM(bc.GetedAmount) AS monthgeted9,
                                        '费用申请' CustomerCode19s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m9 ON c.CustomerCode = m9.CustomerCode19s
        LEFT JOIN ( SELECT  * ,
                            monthtotall0 - ISNULL(monthgeted10, 0) AS monthnoget10
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotall0 ,SUM(bc.GetedAmount) AS monthgeted10,
                                        '费用申请' CustomerCode110s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m10 ON c.CustomerCode = m10.CustomerCode110s
        LEFT JOIN ( SELECT  * ,
                            monthtotall1 - ISNULL(monthgeted11, 0) AS monthnoget11
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotall1 ,SUM(bc.GetedAmount) AS monthgeted11,
                                        '费用申请' CustomerCode111s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m11 ON c.CustomerCode = m11.CustomerCode111s
        LEFT JOIN ( SELECT  * ,
                            monthtotall2 - ISNULL(monthgeted12, 0) AS monthnoget12
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotall2 ,SUM(bc.GetedAmount) AS monthgeted12,
                                        '费用申请' CustomerCode112s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m12 ON c.CustomerCode = m12.CustomerCode112s
        LEFT JOIN ( SELECT  * ,
                            monthtotalall - ISNULL(monthgetedall, 0) AS monthnogetall
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotalall ,SUM(bc.GetedAmount) AS monthgetedall,
                                        '费用申请' CustomerCode113s
                              FROM      dbo.Base_PaySheet bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @Customer
                            ) a
                            
                  ) m13 ON c.CustomerCode = m13.CustomerCode113s

) p5
UNION ALL
SELECT * FROM (SELECT  *,'条钢采购' type
FROM    ( SELECT    bc.CustomerName ,
                    bc.CustomerCode,bc.CustomerId
          FROM      dbo.Base_SteelPurchase bc
          WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                    AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate
                    @CustomerName @type3
          GROUP BY  bc.CustomerCode  ,bc.CustomerId,
                    bc.CustomerName
        ) c
        LEFT JOIN ( SELECT  * ,
                            monthtotal1 -ISNULL(monthgeted1,0)  AS monthnoget1
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal1 ,SUM(bc.GetedAmount) AS monthgeted1,
                                        bc.CustomerId CustomerCode11s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m1 ON c.CustomerId = m1.CustomerCode11s
        LEFT JOIN ( SELECT  * ,
                            monthtotal2 -ISNULL(monthgeted2,0) AS monthnoget2
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,SUM(bc.GetedAmount) AS monthgeted2,
                                        bc.CustomerId CustomerCode12s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m2 ON c.CustomerId = m2.CustomerCode12s
        LEFT JOIN ( SELECT  * ,
                            monthtotal3 -ISNULL(monthgeted3,0) AS monthnoget3
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,SUM(bc.GetedAmount) AS monthgeted3,
                                        bc.CustomerId CustomerCode13s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m3 ON c.CustomerId = m3.CustomerCode13s
        LEFT JOIN ( SELECT  * ,
                            monthtotal4 -ISNULL(monthgeted4,0) AS monthnoget4
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,SUM(bc.GetedAmount) AS monthgeted4,
                                        bc.CustomerId CustomerCode14s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m4 ON c.CustomerId = m4.CustomerCode14s
        LEFT JOIN ( SELECT  * ,
                            monthtotal5 -ISNULL(monthgeted5,0) AS monthnoget5
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,SUM(bc.GetedAmount) AS monthgeted5,
                                        bc.CustomerId CustomerCode15s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m5 ON c.CustomerId = m5.CustomerCode15s
        LEFT JOIN ( SELECT  * ,
                            monthtotal6 -ISNULL(monthgeted6,0) AS monthnoget6
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,SUM(bc.GetedAmount) AS monthgeted6,
                                        bc.CustomerId CustomerCode16s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m6 ON c.CustomerId = m6.CustomerCode16s
        LEFT JOIN ( SELECT  * ,
                            (monthtotal7 - ISNULL(monthgeted7,0)) AS monthnoget7
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,SUM(bc.GetedAmount) AS monthgeted7,
                                        bc.CustomerId CustomerCode17s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m7 ON c.CustomerId = m7.CustomerCode17s
        LEFT JOIN ( SELECT  * ,
                            monthtotal8 -ISNULL(monthgeted8,0) AS monthnoget8
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,SUM(bc.GetedAmount) AS monthgeted8,
                                        bc.CustomerId CustomerCode18s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m8 ON c.CustomerId = m8.CustomerCode18s
        LEFT JOIN ( SELECT  * ,
                            monthtotal9 -ISNULL(monthgeted9,0) AS monthnoget9
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,SUM(bc.GetedAmount) AS monthgeted9,
                                        bc.CustomerId CustomerCode19s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m9 ON c.CustomerId = m9.CustomerCode19s
        LEFT JOIN ( SELECT  * ,
                            monthtotal10 -ISNULL(monthgeted10,0) AS monthnoget10
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal10 ,SUM(bc.GetedAmount) AS monthgeted10,
                                        bc.CustomerId CustomerCode110s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                           
                  ) m10 ON c.CustomerId = m10.CustomerCode110s
        LEFT JOIN ( SELECT  * ,
                            monthtotal11 -ISNULL(monthgeted11,0) AS monthnoget11
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal11 ,SUM(bc.GetedAmount) AS monthgeted11,
                                        bc.CustomerId CustomerCode111s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m11 ON c.CustomerId = m11.CustomerCode111s
        LEFT JOIN ( SELECT  * ,
                            monthtotal12 -ISNULL(monthgeted12,0) AS monthnoget12
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal12 ,SUM(bc.GetedAmount) AS monthgeted12,
                                        bc.CustomerId CustomerCode112s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m12 ON c.CustomerId = m12.CustomerCode112s
        LEFT JOIN ( SELECT  * ,
                            monthtotalall -ISNULL(monthgetedall,0) AS monthnogetall
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotalall ,SUM(bc.GetedAmount) AS monthgetedall,
                                        bc.CustomerId CustomerCode113s
                              FROM      dbo.Base_SteelPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m13 ON c.CustomerId = m13.CustomerCode113s

) p3
UNION ALL
SELECT * FROM (SELECT  *,'物料采购' type
FROM    ( SELECT    bc.CustomerName ,
                    bc.CustomerCode,bc.CustomerId
          FROM      dbo.Base_MaterielPurchase bc
          WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                    AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate
                    @CustomerName @type4
          GROUP BY  bc.CustomerCode  ,bc.CustomerId,
                    bc.CustomerName
        ) c
        LEFT JOIN ( SELECT  * ,
                            monthtotal1 -ISNULL(monthgeted1,0)  AS monthnoget1
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal1 ,SUM(bc.GetedAmount) AS monthgeted1,
                                        bc.CustomerId CustomerCode11s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-02-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m1 ON c.CustomerId = m1.CustomerCode11s
        LEFT JOIN ( SELECT  * ,
                            monthtotal2 -ISNULL(monthgeted2,0) AS monthnoget2
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal2 ,SUM(bc.GetedAmount) AS monthgeted2,
                                        bc.CustomerId CustomerCode12s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-02-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-03-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m2 ON c.CustomerId = m2.CustomerCode12s
        LEFT JOIN ( SELECT  * ,
                            monthtotal3 -ISNULL(monthgeted3,0) AS monthnoget3
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal3 ,SUM(bc.GetedAmount) AS monthgeted3,
                                        bc.CustomerId CustomerCode13s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-03-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-04-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m3 ON c.CustomerId = m3.CustomerCode13s
        LEFT JOIN ( SELECT  * ,
                            monthtotal4 -ISNULL(monthgeted4,0) AS monthnoget4
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal4 ,SUM(bc.GetedAmount) AS monthgeted4,
                                        bc.CustomerId CustomerCode14s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-04-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-05-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m4 ON c.CustomerId = m4.CustomerCode14s
        LEFT JOIN ( SELECT  * ,
                            monthtotal5 -ISNULL(monthgeted5,0) AS monthnoget5
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal5 ,SUM(bc.GetedAmount) AS monthgeted5,
                                        bc.CustomerId CustomerCode15s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-05-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-06-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m5 ON c.CustomerId = m5.CustomerCode15s
        LEFT JOIN ( SELECT  * ,
                            monthtotal6 -ISNULL(monthgeted6,0) AS monthnoget6
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal6 ,SUM(bc.GetedAmount) AS monthgeted6,
                                        bc.CustomerId CustomerCode16s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-06-01 00:00:00.00') <= bc.CreateDate 
                                        AND CONVERT(DATETIME, '{0}-07-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m6 ON c.CustomerId = m6.CustomerCode16s
        LEFT JOIN ( SELECT  * ,
                            (monthtotal7 - ISNULL(monthgeted7,0)) AS monthnoget7
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal7 ,SUM(bc.GetedAmount) AS monthgeted7,
                                        bc.CustomerId CustomerCode17s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-07-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-08-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m7 ON c.CustomerId = m7.CustomerCode17s
        LEFT JOIN ( SELECT  * ,
                            monthtotal8 -ISNULL(monthgeted8,0) AS monthnoget8
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal8 ,SUM(bc.GetedAmount) AS monthgeted8,
                                        bc.CustomerId CustomerCode18s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-08-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-09-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m8 ON c.CustomerId = m8.CustomerCode18s
        LEFT JOIN ( SELECT  * ,
                            monthtotal9 -ISNULL(monthgeted9,0) AS monthnoget9
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal9 ,SUM(bc.GetedAmount) AS monthgeted9,
                                        bc.CustomerId CustomerCode19s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-09-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-10-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m9 ON c.CustomerId = m9.CustomerCode19s
        LEFT JOIN ( SELECT  * ,
                            monthtotal10 -ISNULL(monthgeted10,0) AS monthnoget10
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal10 ,SUM(bc.GetedAmount) AS monthgeted10,
                                        bc.CustomerId CustomerCode110s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-10-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-11-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                           
                  ) m10 ON c.CustomerId = m10.CustomerCode110s
        LEFT JOIN ( SELECT  * ,
                            monthtotal11 -ISNULL(monthgeted11,0) AS monthnoget11
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal11 ,SUM(bc.GetedAmount) AS monthgeted11,
                                        bc.CustomerId CustomerCode111s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-11-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{0}-12-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m11 ON c.CustomerId = m11.CustomerCode111s
        LEFT JOIN ( SELECT  * ,
                            monthtotal12 -ISNULL(monthgeted12,0) AS monthnoget12
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotal12 ,SUM(bc.GetedAmount) AS monthgeted12,
                                        bc.CustomerId CustomerCode112s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-12-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m12 ON c.CustomerId = m12.CustomerCode112s
        LEFT JOIN ( SELECT  * ,
                            monthtotalall -ISNULL(monthgetedall,0) AS monthnogetall
                    FROM    ( SELECT    SUM(bc.TotalAmount) AS monthtotalall ,SUM(bc.GetedAmount) AS monthgetedall,
                                        bc.CustomerId CustomerCode113s
                              FROM      dbo.Base_MaterielPurchase bc
                              WHERE     CONVERT(DATETIME, '{0}-01-01 00:00:00.00') <= bc.CreateDate
                                        AND CONVERT(DATETIME, '{1}-01-01 00:00:00.00') > bc.CreateDate @CustomerName
                              GROUP BY  bc.CustomerId
                            ) a
                            
                  ) m13 ON c.CustomerId = m13.CustomerCode113s

) p4
", Year, Convert.ToInt32(Year) + 1);


            if (string.IsNullOrEmpty(CustomerName))
            {
                str = str.Replace("@CustomerName", "");
                str = str.Replace("@Customer", "");
            }
            else
            {
                str = str.Replace("@CustomerName", " and (CustomerCode = '" + CustomerName + "' or CustomerName like '%" + CustomerName + "%' )");
                str = str.Replace("@Customer", " and 1 =2 ");
            }

            if (type == "委外加工")
            {
                str = str.Replace("@type1", "");
                str = str.Replace("@type2", " and 1=2 ");
                str = str.Replace("@type3", " and 1=2 ");
                str = str.Replace("@type4", " and 1=2 ");
                str = str.Replace("@type5", " and 1=2 ");
            }
            if (type == "委外采购")
            {
                str = str.Replace("@type1", " and 1=2 ");
                str = str.Replace("@type2", "");
                str = str.Replace("@type3", " and 1=2 ");
                str = str.Replace("@type4", " and 1=2 ");
                str = str.Replace("@type5", " and 1=2 ");
            }
            if (type == "费用申请")
            {
                str = str.Replace("@type1", " and 1=2 ");
                str = str.Replace("@type2", " and 1=2 ");
                str = str.Replace("@type3", " and 1=2 ");
                str = str.Replace("@type4", " and 1=2 ");
                str = str.Replace("@type5", "");
            }
            if (type == "条钢采购")
            {
                str = str.Replace("@type1", " and 1=2 ");
                str = str.Replace("@type2", " and 1=2 ");
                str = str.Replace("@type3", "");
                str = str.Replace("@type4", " and 1=2 ");
                str = str.Replace("@type5", " and 1=2 ");
            }
            if (type == "物料采购")
            {
                str = str.Replace("@type1", " and 1=2 ");
                str = str.Replace("@type2", " and 1=2 ");
                str = str.Replace("@type3", " and 1=2 ");
                str = str.Replace("@type4", " ");
                str = str.Replace("@type5", " and 1=2 ");
            }
            if (string.IsNullOrEmpty(type))
            {
                str = str.Replace("@type1", "");
                str = str.Replace("@type2", "");
                str = str.Replace("@type3", "");
                str = str.Replace("@type4", "");
                str = str.Replace("@type5", "");
            }

            return Repository().FindTablePageBySql(str, ref jqgridparam);
        }
        /// <summary>
        /// 获取还没有完成发货的订单
        /// </summary>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageJsonCost1(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  *
FROM    ( SELECT    '其他收款' AS TYPE ,
                    PayCompanyCode AS code ,
                    CustomerName ,
                    PricePer ,
                    Qty ,
                    TotalAmount ,
                    Status ,
                    IsPay ,
                    IsInvoice ,
                    PayType ,
                    WorkerName ,
                    DepartmentName ,
                    CreateDate ,
                    Remark
          FROM      Base_PayCompany
          UNION ALL
          SELECT    '费用申请' AS TYPE ,
                    PaySheetCode AS code ,
                    '其他' AS CustomerName ,
                    PricePer ,
                    Qty ,
                    TotalAmount ,
                    Status ,
                    IsPay ,
                    IsInvoice ,
                    PayType ,
                    WorkerName ,
                    DepartmentName ,
                    CreateDate ,
                    Remark
          FROM      Base_PaySheet
          UNION ALL
          SELECT    '物料购买' AS TYPE ,
                    MaterielPurchaseCode AS code ,
                    CustomerName ,
                    PricePer ,
                    Qty ,
                    TotalAmount ,
                    Status ,
                    IsPay ,
                    IsInvoice ,
                    PayType ,
                    WorkerName ,
                    DepartmentName ,
                    CreateDate ,
                    Remark
          FROM      dbo.Base_MaterielPurchase
          UNION ALL
          SELECT    '钢材购买' AS TYPE ,
                    SteelPurchaseCode AS code ,
                    CustomerName ,
                    0 PricePer ,
                    Qty ,
                    TotalAmount ,
                    Status ,
                    IsPay ,
                    IsInvoice ,
                    PayType ,
                    WorkerName ,
                    DepartmentName ,
                    CreateDate ,
                    Remark
          FROM      Base_SteelPurchase
          UNION ALL
          SELECT    '委外加工' AS TYPE ,
                    MnemonicCode +'-'+ CAST(LineCode AS VARCHAR(50)) AS code ,
                    CustomerName ,
                    0 AS PricePer ,
                    Qty ,
                    CostAmount AS TotalAmount ,
                    CASE WHEN BillStatus = '验收通过' THEN 1
                         ELSE 0
                    END AS Status ,
                    IsPay ,
                    IsInvoice ,
                    PaymentMode AS PayType ,
                    '其他' AS WorkerName ,
                    '其他' AS DepartmentName ,
                    CreateDate ,
                    Remark
          FROM      BY_OrderDetailBeOut
        ) a 
WHERE   1 = 1  ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]))
            {
                strSql.Append(@" AND CreateDate >= @CreateDate");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate", Convert.ToDateTime(Condition["CreateDate_StartTime"] + " 00:00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(@" AND  CreateDate <= @CreateDate1");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate1", Convert.ToDateTime(Condition["CreateDate_EndTime"] + " 23:59")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerName"]))
            {
                strSql.Append(@" AND  CustomerName like @CustomerName ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", '%' + Convert.ToString(Condition["CustomerName"]) + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["Type"]))
            {
                strSql.Append(@" AND Type = @Type");
                parameter.Add(DbFactory.CreateDbParameter("@Type", Convert.ToString(Condition["Type"])));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["status"]))
            {
                strSql.Append(@" AND  status= 0");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ispay"]))
            {
                strSql.Append(@" AND  ispay= 0");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["isinvoice"]))
            {
                strSql.Append(@" AND  isinvoice= 0");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["code"]))
            {
                strSql.Append(@" AND code = @code");
                parameter.Add(DbFactory.CreateDbParameter("@code", Convert.ToString(Condition["code"])));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["PayType"]))
            {
                strSql.Append(@" AND PayType = @PayType");
                parameter.Add(DbFactory.CreateDbParameter("@PayType", Convert.ToString(Condition["PayType"])));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["WorkerName"]))
            {
                strSql.Append(@" AND WorkerName = @WorkerName");
                parameter.Add(DbFactory.CreateDbParameter("@WorkerName", Convert.ToString(Condition["WorkerName"])));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["DepartmentName"]))
            {
                strSql.Append(@" AND DepartmentName = @DepartmentName");
                parameter.Add(DbFactory.CreateDbParameter("@DepartmentName", Convert.ToString(Condition["DepartmentName"])));
            }


            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        /// <summary>
        /// 获取还没有完成发货的订单
        /// </summary>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public List<Base_MaterielPurchase> GetMaterielStorage(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  *
FROM    dbo.Base_MaterielPurchase
WHERE   1 = 1  ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["keywords"]))
            {
                strSql.Append(@" AND (MaterielName LIKE @keyword
        OR MaterielCode LIKE @keyword
        OR MaterielType LIKE @keyword
        OR CustomerCode LIKE @keyword
        OR CustomerName LIKE @keyword
        OR ShortName LIKE @keyword
        OR WorkerName LIKE @keyword
        OR MaterielPurchaseCode LIKE @keyword )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + Condition["keywords"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Status"]))
            {
                strSql.Append(@" AND Status = @Status");
                parameter.Add(DbFactory.CreateDbParameter("@Status", Convert.ToInt32(Condition["Status"])));
            }
            //if (!HashtableHelper.IsNullOrDBNull(Condition["AlarmNumber"]))
            //{
            //    if (Condition["AlarmNumber"].ToString() == "1")
            //    {
            //        strSql.Append(@"  AND Qty <= AlarmNumber");
            //    }
            //}

            //strSql.Append(" ORDER BY o.Delivery DESC ");

            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }



    }
}
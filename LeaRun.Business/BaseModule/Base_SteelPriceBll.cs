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
using System.Linq;
using System;
using System.Data;

namespace DNS.Business
{
    /// <summary>
    /// 钢材价目表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.14 10:31</date>
    /// </author>
    /// </summary>
    public class Base_SteelPriceBll : RepositoryFactory<Base_SteelPrice>
    {
        IDatabase database = DataFactory.Database();
        /// <summary>
        /// 钢材信息列表（自动补全）
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<Base_SteelPrice> GetAutoComple(string keywords)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT TOP 10 *
                            FROM    Base_SteelPrice WHERE 1=1");
            if (!string.IsNullOrEmpty(keywords))
            {
                strSql.Append(@" AND (SteelCode LIKE @keyword
                                    OR SteelName LIKE @keyword)");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keywords + '%'));
            }
            return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
        }

        /// <summary>
        /// 判断是否存在（判断加工类型和钢材编码，厚度是否重复）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsExists(Base_SteelPrice entity)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  COUNT(1)
                            FROM    Base_SteelPrice--
                            WHERE   1 = 1
                                    --AND RoundFeed = @RoundFeed 
                                    --AND ProcessType = @ProcessType 
                                    AND SteelCode = @SteelCode
                                    AND  (( @SizeStart BETWEEN SizeStart AND SizeEnd )
                OR ( @SizeEnd BETWEEN SizeStart AND SizeEnd )) ");
            if (!string.IsNullOrEmpty(entity.SteelPriceId))
            {
                strSql.Append(" AND SteelPriceId != @SteelPriceId");
                parameter.Add(DbFactory.CreateDbParameter("@SteelPriceId", entity.SteelPriceId));
            }
            //parameter.Add(DbFactory.CreateDbParameter("@RoundFeed", entity.RoundFeed));
            parameter.Add(DbFactory.CreateDbParameter("@ProcessType", entity.ProcessType));
            parameter.Add(DbFactory.CreateDbParameter("@SteelCode", entity.SteelCode));
            parameter.Add(DbFactory.CreateDbParameter("@SizeStart", entity.SizeStart));
            parameter.Add(DbFactory.CreateDbParameter("@SizeEnd", entity.SizeEnd));
            int Count = Repository().FindCountBySql(strSql.ToString(), parameter.ToArray());
            if (Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region 获得价目折扣信息
        public decimal get四侧面积(Base_Steel base_steel, decimal Density, string OrderType, decimal Thick, decimal Weigh, decimal Length, decimal AddThick, decimal AddWeigh, decimal AddLength, decimal BoreSize, decimal BoreSizeAdd)
        {
            decimal area = 0;
            if (Thick != 0 && Length != 0 && Weigh == 0)
            {
                area = BoreSize * Length * CommonHelper.GetDecimal(3.14) / 1000000;
            }
            else
            {

                area = (Length * Thick + Weigh * Thick) * 2  / 1000000;
            }
            return area;
        }

        public decimal get两平面面积(Base_Steel base_steel, decimal Density, string OrderType, decimal Thick, decimal Weigh, decimal Length, decimal AddThick, decimal AddWeigh, decimal AddLength, decimal BoreSize, decimal BoreSizeAdd)
        {
            decimal area = 0;
            if (Thick != 0 && Length != 0 && Weigh == 0)
            {
                area = Thick * Length * CommonHelper.GetDecimal(3.14) / 1000000;
            }
            else
            {
                area = Weigh * Length * 2 / 1000000;
            }
            return area;
        }


        public decimal get内孔重量(Base_Steel base_steel, decimal Density, string OrderType, decimal Thick, decimal Weigh, decimal Length, decimal AddThick, decimal AddWeigh, decimal AddLength, decimal BoreSize, decimal BoreSizeAdd)
        {
            decimal _Weight = 0;
            if (Thick != 0 && Length != 0 && BoreSize != 0 )
            {
                _Weight = (BoreSize - BoreSizeAdd) * (BoreSize - BoreSizeAdd) * (Length + AddLength) * CommonHelper.GetDecimal(6.167) / 1000000;
            }
            return _Weight;
        }




        public decimal GetWeight(Base_Steel base_steel, decimal Density, string OrderType, decimal Thick, decimal Weigh, decimal Length, decimal AddThick, decimal AddWeigh, decimal AddLength, decimal BoreSize, decimal BoreSizeAdd)
        {
            decimal _Weight = 0;
            if (Thick != 0 && Length != 0 && Weigh == 0)
            {
                _Weight = (Thick + AddThick) * (Thick + AddThick) * (Length + AddLength) * CommonHelper.GetDecimal(Density) / 1000000;
            }
            else
            {
                _Weight = ((((Thick + AddThick) * (Weigh + AddWeigh) * (Length + AddLength)) * CommonHelper.GetDecimal(Density)) / 1000000);
            }
            return  _Weight;
        }

        public decimal GetTheoryWeight(Base_Steel base_steel, decimal Density, string OrderType, decimal Thick, decimal Weigh, decimal Length, decimal BoreSize)
        {
            decimal _Weight = 0;
            if (Thick != 0 && Length != 0 && Weigh == 0)
            {
                _Weight = (Thick  ) * (Thick ) * (Length ) * CommonHelper.GetDecimal(Density) / 1000000;
            }
            else
            {
                _Weight = ((((Thick ) * (Weigh ) * (Length )) * CommonHelper.GetDecimal(Density)) / 1000000);
            }
            return _Weight;
        }

        public decimal GetWorkWeight(Base_Steel base_steel, decimal Density, string OrderType, decimal Thick, decimal Weigh, decimal Length, decimal BoreSize, Base_Customer base_customer)
        {
            decimal _Weight = 0;
            decimal _BoreSize = 0;
            if (base_customer.MiniThick > Thick)
            {
                Thick = base_customer.MiniThick;
            }

            if (Thick != 0 && Length != 0 && Weigh == 0)
            {
                if (CommonHelper.GetDecimal(BoreSize) > 0)
                {
                    _BoreSize = (((((BoreSize / 2)) * ((BoreSize / 2)) * ((Length))) * CommonHelper.GetDecimal(Density) * CommonHelper.GetDecimal(3.14)) / 1000000);
                    //_BoreSize = (((((BoreSize)) * ((BoreSize)) * ((Length))) * CommonHelper.GetDecimal(Density)) / 1000000);
                }
                _Weight = (((((Thick / 2)) * ((Thick/2)) * ((Length)) * CommonHelper.GetDecimal(3.14)) * CommonHelper.GetDecimal(Density)) / 1000000);
                //_Weight = (((((Thick)) * ((Thick)) * ((Length))) * CommonHelper.GetDecimal(Density)) / 1000000);
                _Weight = _Weight - _BoreSize;
            }
            else
            {
                _Weight = ((((Thick) * (Weigh) * (Length)) * CommonHelper.GetDecimal(Density)) / 1000000);
            }
            return _Weight;
        }
        

        /// <summary>
        /// 获得价目折扣信息
        /// </summary>
        /// <param name="OrderType">加工类型</param>
        /// <param name="CustomerId">客户ID</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="Thick">厚</param>
        /// <param name="Weith">宽</param>
        /// <param name="Length">长</param>
        /// <returns></returns>
        public Hashtable GetSteelPrice(string OrderType, string CustomerId, string SteelCode, decimal Thick, decimal Weigh, decimal Length, decimal AddThick, decimal AddWeigh, decimal AddLength, decimal BoreSize, decimal LimitWeight, string Technology, decimal Qty, int GasCuttingSteel, int LiftingHole, int CustMaterial, string BoreSizeAdd)
        {

            List<DbParameter> materialsParameter = new List<DbParameter>();
            materialsParameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
            materialsParameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            materialsParameter.Add(DbFactory.CreateDbParameter("@OrderType", OrderType));
            materialsParameter.Add(DbFactory.CreateDbParameter("@Length", Length));
            materialsParameter.Add(DbFactory.CreateDbParameter("@Width", Weigh));
            materialsParameter.Add(DbFactory.CreateDbParameter("@Pachy", Thick));

            decimal _Weight = 0;
            decimal _TheoryWeight = 0;
            decimal _WorkWeight = 0;

       
            //CustomerId
            Base_Customer base_customer = database.FindEntity<Base_Customer>(CustomerId);


            Base_Steel base_steel = database.FindEntity<Base_Steel>("SteelCode", SteelCode);


            Hashtable priceHt = new Hashtable();

            _Weight = GetWeight(base_steel, CommonHelper.GetDecimal(base_steel.Density), OrderType, CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length), CommonHelper.GetDecimal(AddThick), CommonHelper.GetDecimal(AddWeigh), CommonHelper.GetDecimal(AddLength), CommonHelper.GetDecimal(BoreSize), CommonHelper.GetDecimal(BoreSizeAdd));
            _TheoryWeight = GetTheoryWeight(base_steel, CommonHelper.GetDecimal(base_steel.Density), OrderType, CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length), CommonHelper.GetDecimal(BoreSize));
            _WorkWeight = GetWorkWeight(base_steel, CommonHelper.GetDecimal(base_steel.Density), OrderType, CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length), CommonHelper.GetDecimal(BoreSize), base_customer);

            decimal 内孔重量 = get内孔重量(base_steel, CommonHelper.GetDecimal(base_steel.Density), OrderType, CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length), CommonHelper.GetDecimal(AddThick), CommonHelper.GetDecimal(AddWeigh), CommonHelper.GetDecimal(AddLength), CommonHelper.GetDecimal(BoreSize), CommonHelper.GetDecimal(BoreSizeAdd));
            decimal 管状重量 = _Weight - 内孔重量;
            decimal 两平面面积 = get两平面面积(base_steel, CommonHelper.GetDecimal(base_steel.Density), OrderType, CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length), CommonHelper.GetDecimal(AddThick), CommonHelper.GetDecimal(AddWeigh), CommonHelper.GetDecimal(AddLength), CommonHelper.GetDecimal(BoreSize), CommonHelper.GetDecimal(BoreSizeAdd)); ;
            decimal 四侧面积 = get四侧面积(base_steel, CommonHelper.GetDecimal(base_steel.Density), OrderType, CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length), CommonHelper.GetDecimal(AddThick), CommonHelper.GetDecimal(AddWeigh), CommonHelper.GetDecimal(AddLength), CommonHelper.GetDecimal(BoreSize), CommonHelper.GetDecimal(BoreSizeAdd)); ;
            decimal 六面面积 = 两平面面积 + 四侧面积;
            priceHt["内孔重量"] = 内孔重量;
            priceHt["管状重量"] = 管状重量;
            priceHt["两平面面积"] = 两平面面积;
            priceHt["四侧面积"] = 四侧面积;
            priceHt["六面面积"] = 六面面积;


            if (Weigh > 0)
            {
                decimal[] at = new decimal[3] { Thick, Weigh, Length };
                Array.Sort(at);
                Thick = at[0];
                Weigh = at[1];
                Length = at[2];
            }

            if (LimitWeight > 0 && LimitWeight > _Weight) {
                _Weight = LimitWeight;
            }
            if (LimitWeight > 0 && LimitWeight > _TheoryWeight)
            {
                _TheoryWeight = LimitWeight;
            }
            
            if (Length == 0  ) {
                _Weight = 0;
                _TheoryWeight = 0;
            }
            decimal Weight = Math.Round(_Weight, 2, MidpointRounding.AwayFromZero);
            decimal TheoryWeight = Math.Round(_TheoryWeight, 2, MidpointRounding.AwayFromZero);
            decimal WorkWeight = Math.Round(_WorkWeight, 2, MidpointRounding.AwayFromZero);
            if (Weigh == 0 || OrderType == "毛料生产")
            {
                WorkWeight = 0;
            }

            
            priceHt["AddThick"] = AddThick;
            priceHt["AddWeigh"] = AddWeigh;
            priceHt["AddLength"] = AddLength;
            priceHt["Weight"] = Weight;
            priceHt["TheoryWeight"] = TheoryWeight;
            priceHt["WorkWeight"] = WorkWeight;

            priceHt["MaterialPrice"] = 0;
            priceHt["WorkPrice"] = 0;
            priceHt["SmallCost"] = 0;
            priceHt["HeatPrice"] = 0;
            priceHt["CostPrice"] = 0;
            priceHt["ProductName"] = "";
            priceHt["JKCostPer"] = 0;

            int YuanSteel = 0;
            int MaoSteel = 0;
            int GuangSteel = 0;
            int JingSteel = 0;
            string processSql = string.Format(@"SELECT  *
                                                FROM    Base_CustomerProcess
                                                WHERE   (CustomerId = @CustomerId or CustomerId='520f7953-c9c0-48a6-82b5-92a842c4fd37')  
                                                     and   CustMaterial = @CustMaterial  
                                                        ");
            string materialsSql = string.Format(@"SELECT  cm.* 
                                                    FROM    Base_CustomerMaterials cm
                                                    WHERE   (CustomerId = @CustomerId or CustomerId='520f7953-c9c0-48a6-82b5-92a842c4fd37')
                                                            AND cm.SteelCode =@SteelCode  
                                                            ");
            if (OrderType == "毛料生产")
            {
                MaoSteel = 1;
                processSql += " AND MaoSteel =@MaoSteel ORDER BY SortCode ";
                materialsSql += " AND MaoSteel =@MaoSteel ORDER BY SortCode ";

            }
            else if (OrderType == "光板加工")
            {
                GuangSteel = 1;
                processSql += " AND GuangSteel =@GuangSteel ORDER BY SortCode ";
                materialsSql += " AND GuangSteel =@GuangSteel ORDER BY SortCode ";

            }
            else if (OrderType == "精板加工")
            {
                JingSteel = 1;
                processSql += " AND JingSteel =@JingSteel  ORDER BY SortCode ";
                materialsSql += " AND JingSteel =@JingSteel  ORDER BY SortCode ";
            }

   
            //锯工费 圆 、 板
            string processSql1 = "";
            if (CommonHelper.GetDecimal(Thick) != 0 && CommonHelper.GetDecimal(Weigh) == 0 && CommonHelper.GetDecimal(Length) != 0)
            {
                processSql1 = string.Format(@"SELECT  *
                                                FROM    Base_CustomerProcess
                                                WHERE   (CustomerId = @CustomerId or CustomerId='520f7953-c9c0-48a6-82b5-92a842c4fd37') and   JKCost = 1  AND MaoSteel = 0 AND JingSteel=0 AND GuangSteel=0    and   CustMaterial = @CustMaterial   ORDER BY SortCode 
                                                        ");
            }
            else
            {
                processSql1 = string.Format(@"SELECT  *
                                                FROM    Base_CustomerProcess
                                                WHERE   (CustomerId = @CustomerId or CustomerId='520f7953-c9c0-48a6-82b5-92a842c4fd37') and   JKCost1 = 1  AND MaoSteel = 0 AND JingSteel=0 AND GuangSteel=0    and   CustMaterial = @CustMaterial   ORDER BY SortCode 
                                                        ");
            }

            //切角
            string processSqlDD = string.Format(@"SELECT  *
                                                FROM    Base_CustomerProcess
                                                WHERE   (CustomerId = @CustomerId or CustomerId='520f7953-c9c0-48a6-82b5-92a842c4fd37') and   DDCost = 1  AND MaoSteel = 0 AND JingSteel=0 AND GuangSteel=0     and   CustMaterial = @CustMaterial   ORDER BY SortCode 
                                                        ");

            List<DbParameter> processParameter = new List<DbParameter>();
            processParameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
            processParameter.Add(DbFactory.CreateDbParameter("@YuanSteel", YuanSteel));
            processParameter.Add(DbFactory.CreateDbParameter("@MaoSteel", MaoSteel));
            processParameter.Add(DbFactory.CreateDbParameter("@GuangSteel", GuangSteel));
            processParameter.Add(DbFactory.CreateDbParameter("@JingSteel", JingSteel));
            processParameter.Add(DbFactory.CreateDbParameter("@CustMaterial", CustMaterial));
            //processParameter.Add(DbFactory.CreateDbParameter("@SteelType", base_steel.SteelType)); 

            materialsParameter.Add(DbFactory.CreateDbParameter("@MaoSteel", MaoSteel));
            materialsParameter.Add(DbFactory.CreateDbParameter("@GuangSteel", GuangSteel));
            materialsParameter.Add(DbFactory.CreateDbParameter("@JingSteel", JingSteel));
            
            string manageSql = string.Format(@"SELECT  *
                                                FROM    Base_CustomerManage
                                                WHERE   (CustomerId = @CustomerId or CustomerId='520f7953-c9c0-48a6-82b5-92a842c4fd37')
                                                        AND CraftTypeCode=@Technology  ORDER BY SortCode 
                                                         ");
            List<DbParameter> manageParameter = new List<DbParameter>();
            manageParameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
            manageParameter.Add(DbFactory.CreateDbParameter("@Technology", Technology));

            //priceHt["MaterialLess"] = 0;
            //priceHt["HeatLess"] = 0;
            //priceHt["WorkLess"] = 0;
            IList<Base_CustomerProcess> base_customerprocess = database.FindListBySql<Base_CustomerProcess>(processSql, processParameter.ToArray());
            IList<Base_CustomerProcess> base_customerprocess1 = database.FindListBySql<Base_CustomerProcess>(processSql1, processParameter.ToArray());

            IList<Base_CustomerProcess> base_customerprocess2 = database.FindListBySql<Base_CustomerProcess>(processSqlDD, processParameter.ToArray());

            //DD 切角
            bool isp = false;
            IList<Base_CustomerProcess> base_customerprocessttttdd = database.FindListBySql<Base_CustomerProcess>(processSqlDD, processParameter.ToArray());
            foreach (Base_CustomerProcess item in base_customerprocess2)
            {
                if (item.SortCode == null)
                {
                    isp = true;
                }
            }
            if (isp)
            {
                foreach (Base_CustomerProcess item in base_customerprocessttttdd)
                {
                    if (item.SortCode != null)
                    {
                        Base_CustomerProcess itemt = base_customerprocess2.Where(p => p.CustomerProcessId == item.CustomerProcessId).First();
                        base_customerprocess2.Remove(itemt);
                    }
                }
            }
            foreach (Base_CustomerProcess item in base_customerprocess2)
            {
                if (item.CustomerId != null)
                {
                    bool isokw = true;
                    if (CommonHelper.GetDecimal(item.KGfrom) != 0 || CommonHelper.GetDecimal(item.KGto) != 0)
                    {
                        if (CommonHelper.GetDecimal(Weight) >= CommonHelper.GetDecimal(item.KGfrom) && CommonHelper.GetDecimal(Weight) <= CommonHelper.GetDecimal(item.KGto))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }
                    if (CommonHelper.GetDecimal(item.QtyStart) != 0 || CommonHelper.GetDecimal(item.QtyEnd) != 0)
                    {
                        if (CommonHelper.GetDecimal(Qty) >= CommonHelper.GetDecimal(item.QtyStart) && CommonHelper.GetDecimal(Qty) <= CommonHelper.GetDecimal(item.QtyEnd))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }

                    if (item.OrOr == 1)
                    {
                        bool isok1 = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok1 = true;
                            }
                            else
                            {
                                isok1 = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok1 = !isok1;
                            }
                        }
                        bool isok2 = true;
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok2 = true;
                            }
                            else
                            {
                                isok2 = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok2 = !isok2;
                            }
                        }
                        bool isok3 = true;
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok3 = true;
                            }
                            else
                            {
                                isok3 = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok3 = !isok3;
                            }

                        }
                        if (CommonHelper.GetDecimal(item.PachyStart) == 0 && CommonHelper.GetDecimal(item.PachyEnd) == 0)
                        {
                            if (!isok2 && !isok3)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.WidthStart) == 0 && CommonHelper.GetDecimal(item.WidthEnd) == 0)
                        {
                            if (!isok1 && !isok3)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.LengthStart) == 0 && CommonHelper.GetDecimal(item.LengthEnd) == 0)
                        {
                            if (!isok1 && !isok2)
                            {
                                continue;
                            }
                        }
                        //if (!isok1 && !isok2 && !isok3)
                        else
                        {
                            if (!isok1 && !isok2 && !isok3)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        bool isok = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                    }

                    //AND SteelCode=@SteelCode 
                    if (item.SteelCode == SteelCode)
                    {
                        priceHt["DDCostPer"] = CommonHelper.GetDecimal(item.Price);
                        break;
                    }
                    if (string.IsNullOrEmpty(item.SteelCode.Replace("&nbsp;", "")) && CommonHelper.GetDecimal(priceHt["DDCostPer"]) == 0)
                    {
                        priceHt["DDCostPer"] = CommonHelper.GetDecimal(item.Price);
                    }
                }
            }

            if (OrderType == "毛料生产")
            {
                priceHt["DDCostPer"] = 0;
            }
            //锯工费
            isp = false;
            IList<Base_CustomerProcess> base_customerprocesstttt = database.FindListBySql<Base_CustomerProcess>(processSql1, processParameter.ToArray());
            foreach (Base_CustomerProcess item in base_customerprocess1)
            {
                if (item.SortCode == null)
                {
                    isp = true;
                }
            }
            if (isp)
            {
                foreach (Base_CustomerProcess item in base_customerprocesstttt)
                {
                    if (item.SortCode != null)
                    {
                        Base_CustomerProcess itemt = base_customerprocess1.Where(p => p.CustomerProcessId == item.CustomerProcessId).First();
                        base_customerprocess1.Remove(itemt);
                    }
                }
            }
            foreach (Base_CustomerProcess item in base_customerprocess1)
            {
                if (item.CustomerId != null)
                {
                    bool isokw = true;
                    if (CommonHelper.GetDecimal(item.KGfrom) != 0 || CommonHelper.GetDecimal(item.KGto) != 0)
                    {
                        if (CommonHelper.GetDecimal(Weight) >= CommonHelper.GetDecimal(item.KGfrom) && CommonHelper.GetDecimal(Weight) <= CommonHelper.GetDecimal(item.KGto))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }
                    if (CommonHelper.GetDecimal(item.QtyStart) != 0 || CommonHelper.GetDecimal(item.QtyEnd) != 0)
                    {
                        if (CommonHelper.GetDecimal(Qty) >= CommonHelper.GetDecimal(item.QtyStart) && CommonHelper.GetDecimal(Qty) <= CommonHelper.GetDecimal(item.QtyEnd))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }

                    if (item.OrOr == 1)
                    {
                        bool isok1 = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok1 = true;
                            }
                            else
                            {
                                isok1 = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok1 = !isok1;
                            }
                        }
                        bool isok2 = true;
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok2 = true;
                            }
                            else
                            {
                                isok2 = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok2 = !isok2;
                            }
                        }
                        bool isok3 = true;
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok3 = true;
                            }
                            else
                            {
                                isok3 = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok3 = !isok3;
                            }

                        }
                        if (CommonHelper.GetDecimal(item.PachyStart) == 0 && CommonHelper.GetDecimal(item.PachyEnd) == 0)
                        {
                            if (!isok2 && !isok3)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.WidthStart) == 0 && CommonHelper.GetDecimal(item.WidthEnd) == 0)
                        {
                            if (!isok1 && !isok3)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.LengthStart) == 0 && CommonHelper.GetDecimal(item.LengthEnd) == 0)
                        {
                            if (!isok1 && !isok2)
                            {
                                continue;
                            }
                        }
                        //if (!isok1 && !isok2 && !isok3)
                        else
                        {
                            if (!isok1 && !isok2 && !isok3)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        bool isok = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                    }
                  
                    //AND SteelCode=@SteelCode 
                    if (item.SteelCode == SteelCode)
                    {
                        priceHt["JKCostPer"] = CommonHelper.GetDecimal(item.Price);
                        break;
                    }
                    if (string.IsNullOrEmpty(item.SteelCode.Replace("&nbsp;", "")) && CommonHelper.GetDecimal(priceHt["JKCostPer"]) == 0)
                    {
                        priceHt["JKCostPer"] = CommonHelper.GetDecimal(item.Price);
                    }
                }
            }

            if (OrderType != "毛料生产")
            {
                priceHt["JKCostPer"] = 0;
            }


            //加工费
            isp = false;
            IList<Base_CustomerProcess> base_customerprocesstttt1 = database.FindListBySql<Base_CustomerProcess>(processSql, processParameter.ToArray());
            foreach (Base_CustomerProcess item in base_customerprocess)
            {
                if (item.SortCode == null)
                {
                    isp = true;
                }
            }
            if (isp)
            {
                foreach (Base_CustomerProcess item in base_customerprocesstttt1)
                {
                    if (item.SortCode != null)
                    {
                        Base_CustomerProcess itemt = base_customerprocess.Where(p => p.CustomerProcessId == item.CustomerProcessId).First();
                        base_customerprocess.Remove(itemt);
                    }
                }
            }
            foreach (Base_CustomerProcess item in base_customerprocess)
            {
                if (item.CustomerId != null)
                {
                    if (!string.IsNullOrEmpty(item.SteelCode) && item.SteelCode != SteelCode && item.SteelCode != "&nbsp;")
                    {
                        continue;
                    }

                    bool isokw = true;
                    if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0)&&Weigh != 0)
                    {
                        decimal tempscals = Length / Weigh;
                        if (tempscals >= CommonHelper.GetDecimal(item.ScaleFrom) && tempscals <= CommonHelper.GetDecimal(item.ScaleTo))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }
                    if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh == 0)
                    {
                        continue;
                    }


                    if (CommonHelper.GetDecimal(item.KGfrom) != 0 || CommonHelper.GetDecimal(item.KGto) != 0)
                    {
                        if (CommonHelper.GetDecimal(Weight) >= CommonHelper.GetDecimal(item.KGfrom) && CommonHelper.GetDecimal(Weight) <= CommonHelper.GetDecimal(item.KGto))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }
                    if (CommonHelper.GetDecimal(item.QtyStart) != 0 || CommonHelper.GetDecimal(item.QtyEnd) != 0)
                    {
                        if (CommonHelper.GetDecimal(Qty) >= CommonHelper.GetDecimal(item.QtyStart) && CommonHelper.GetDecimal(Qty) <= CommonHelper.GetDecimal(item.QtyEnd))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }

                    if (item.OrOr == 1)
                    {
                        bool isok0 = true;
                        if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh != 0)
                        {
                            decimal tempscals = Length / Weigh;
                            if (tempscals >= CommonHelper.GetDecimal(item.ScaleFrom) && tempscals <= CommonHelper.GetDecimal(item.ScaleTo))
                            {
                                isok0 = true;
                            }
                            else
                            {
                                isok0 = false;
                            }
                        }
                        if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh == 0)
                        {
                            continue;
                        }
                        bool isok1 = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok1 = true;
                            }
                            else
                            {
                                isok1 = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok1 = !isok1;
                            }
                        }
                        bool isok2 = true;
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok2 = true;
                            }
                            else
                            {
                                isok2 = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok2 = !isok2;
                            }
                        }
                        bool isok3 = true;
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok3 = true;
                            }
                            else
                            {
                                isok3 = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok3 = !isok3;
                            }

                        }


                        if (CommonHelper.GetDecimal(item.PachyStart) == 0 && CommonHelper.GetDecimal(item.PachyEnd) == 0)
                        {
                            if (!isok2 && !isok3 && !isok0)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.WidthStart) == 0 && CommonHelper.GetDecimal(item.WidthEnd) == 0)
                        {
                            if (!isok1 && !isok3 && !isok0)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.LengthStart) == 0 && CommonHelper.GetDecimal(item.LengthEnd) == 0)
                        {
                            if (!isok1 && !isok2 && !isok0)
                            {
                                continue;
                            }
                        }
                        else if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh != 0)
                        {
                            if (!isok1 && !isok2 && !isok3)
                            {
                                continue;
                            }
                        }
                        //if (!isok1 && !isok2 && !isok3)
                        else
                        {
                            if (!isok1 && !isok2 && !isok3 && !isok0)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        bool isok = true;


                        if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh != 0)
                        {
                            decimal tempscals = Length / Weigh;
                            if (tempscals >= CommonHelper.GetDecimal(item.ScaleFrom) && tempscals <= CommonHelper.GetDecimal(item.ScaleTo))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh == 0)
                        {
                            continue;
                        }

                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                    }
                    //  SmallCost  与   WorkPrice  写反了
                    if (CommonHelper.GetDecimal(item.WhereQty) <= 0)
                    {
                        //比例
                        if ((CommonHelper.GetDecimal(item.ScaleFrom) != 0 || CommonHelper.GetDecimal(item.ScaleTo) != 0) && Weigh != 0)
                        {
                            if (CommonHelper.GetDecimal(priceHt["ScaleCost"]) == 0 && string.IsNullOrEmpty(item.SteelCode.Replace("&nbsp;", "")))
                            {
                                priceHt["ScaleCost"] = CommonHelper.GetDecimal(item.Price);
                            }
                            if (item.SteelCode == SteelCode)
                            {
                                priceHt["ScaleCost"] = CommonHelper.GetDecimal(item.Price);
                                break;
                            }
                        }
                        else
                        {
                            if (CommonHelper.GetDecimal(priceHt["SmallCost"]) == 0 && string.IsNullOrEmpty(item.SteelCode.Replace("&nbsp;", "")))
                            {
                                priceHt["SmallCost"] = CommonHelper.GetDecimal(item.Price);
                            }
                            if (item.SteelCode == SteelCode)
                            {
                                priceHt["SmallCost"] = CommonHelper.GetDecimal(item.Price);
                                break;
                            }
                        }
                    }
                    else
                    {

                         //if (item.WhereQ == "重量小于等于" && CommonHelper.GetDecimal(item.WhereQty) >= TheoryWeight && CommonHelper.GetDecimal(item.WhereQty) >= WorkWeight)
                         if (item.WhereQ == "重量小于等于" && CommonHelper.GetDecimal(item.WhereQty) >= Weight && CommonHelper.GetDecimal(item.WhereQty) >= WorkWeight)
                            {
                            if (CommonHelper.GetDecimal(priceHt["WorkPrice"]) == 0 && string.IsNullOrEmpty(item.SteelCode.Replace("&nbsp;", "")))
                            {
                                priceHt["WorkPrice"] = CommonHelper.GetDecimal(item.Price);
                            }
                            if (item.SteelCode == SteelCode)
                            {
                                priceHt["WorkPrice"] = CommonHelper.GetDecimal(item.Price);
                                break;
                            }
                        }
                        
                    }
                }
            }
            if (CommonHelper.GetDecimal(priceHt["WorkPrice"]) == 0 && CommonHelper.GetDecimal(priceHt["ScaleCost"]) != 0)
            {
                priceHt["WorkPrice"] = priceHt["ScaleCost"];
            }
             
            //材料费
            IList<Base_CustomerMaterials> base_customermaterials = database.FindListBySql<Base_CustomerMaterials>(materialsSql, materialsParameter.ToArray());
            IList<Base_CustomerMaterials> base_customermaterialstttt = database.FindListBySql<Base_CustomerMaterials>(materialsSql, materialsParameter.ToArray());
            isp = false;
            foreach (Base_CustomerMaterials item in base_customermaterials)
            {
                if (item.SortCode == null)
                {
                    isp = true;
                }
            }
            if (isp)
            {
                foreach (Base_CustomerMaterials item in base_customermaterialstttt)
                {
                    if (item.SortCode != null)
                    {
                        Base_CustomerMaterials itemt = base_customermaterials.Where(p => p.CustomerMaterialsId == item.CustomerMaterialsId).First();
                        base_customermaterials.Remove(itemt);
                    }
                }
            }
            foreach (Base_CustomerMaterials item in base_customermaterials)
            {
                if (!string.IsNullOrEmpty(item.ProductName) && item.ProductName != "&nbsp;")
                {
                    priceHt["ProductName"] = item.ProductName;
                }
                if (item.CustomerId != null)
                {
                    bool isokw = true;
                    if (CommonHelper.GetDecimal(item.KGfrom) != 0 || CommonHelper.GetDecimal(item.KGto) != 0)
                    {
                        if (CommonHelper.GetDecimal(Weight) >= CommonHelper.GetDecimal(item.KGfrom) && CommonHelper.GetDecimal(Weight) <= CommonHelper.GetDecimal(item.KGto))
                        {
                            isokw = true;
                        }
                        else
                        {
                            isokw = false;
                        }
                        if (!isokw)
                        {
                            continue;
                        }
                    }

                    if (item.OrOr == 1)
                    {
                        bool isok1 = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok1 = true;
                            }
                            else
                            {
                                isok1 = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok1 = !isok1;
                            }
                        }
                        bool isok2 = true;
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok2 = true;
                            }
                            else
                            {
                                isok2 = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok2 = !isok2;
                            }
                        }
                        bool isok3 = true;
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok3 = true;
                            }
                            else
                            {
                                isok3 = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok3 = !isok3;
                            }

                        }
                        if (CommonHelper.GetDecimal(item.PachyStart) == 0 && CommonHelper.GetDecimal(item.PachyEnd) == 0)
                        {
                            if (!isok2 && !isok3)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.WidthStart) == 0 && CommonHelper.GetDecimal(item.WidthEnd) == 0)
                        {
                            if (!isok1 && !isok3)
                            {
                                continue;
                            }
                        }
                        else if (CommonHelper.GetDecimal(item.LengthStart) == 0 && CommonHelper.GetDecimal(item.LengthEnd) == 0)
                        {
                            if (!isok1 && !isok2)
                            {
                                continue;
                            }
                        }
                        //
                        else
                        {
                            if (!isok1 && !isok2 && !isok3)
                            {
                                continue;
                            }

                        }
                    }
                    else
                    {
                        bool isok = true;
                        if (CommonHelper.GetDecimal(item.PachyStart) != 0 || CommonHelper.GetDecimal(item.PachyEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Thick) >= CommonHelper.GetDecimal(item.PachyStart) && CommonHelper.GetDecimal(Thick) <= CommonHelper.GetDecimal(item.PachyEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutThick == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.WidthStart) != 0 || CommonHelper.GetDecimal(item.WidthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Weigh) >= CommonHelper.GetDecimal(item.WidthStart) && CommonHelper.GetDecimal(Weigh) <= CommonHelper.GetDecimal(item.WidthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutWidth == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                        if (CommonHelper.GetDecimal(item.LengthStart) != 0 || CommonHelper.GetDecimal(item.LengthEnd) != 0)
                        {
                            if (CommonHelper.GetDecimal(Length) >= CommonHelper.GetDecimal(item.LengthStart) && CommonHelper.GetDecimal(Length) <= CommonHelper.GetDecimal(item.LengthEnd))
                            {
                                isok = true;
                            }
                            else
                            {
                                isok = false;
                            }
                            if (item.IsOutLength == 1)
                            {
                                isok = !isok;
                            }
                            if (!isok)
                            {
                                continue;
                            }
                        }
                    }
                    //重量小于2，按12/kg计算
                    if (CommonHelper.GetDecimal(item.WhereQty) <= 0)
                    {
                        if (item.SortCode == null)
                        {
                            priceHt["MaterialPrice"] = CommonHelper.GetDecimal(item.Price);
                            break;
                        }
                        if (CommonHelper.GetDecimal(priceHt["MaterialPrice"]) == 0)
                        {
                            priceHt["MaterialPrice"] =  CommonHelper.GetDecimal(item.Price);
                        }

                    }
                    
                    //客户材料单价
                }
            }

           


            //if (tempMoneyMaterials > CommonHelper.GetDecimal(priceHt["MaterialPrice"]))
            //{
            //    priceHt["MaterialPrice"] = tempMoneyMaterials;
            //}

            if (!string.IsNullOrEmpty(Technology))
            {
                List<Base_CustomerManage> base_customermanages = database.FindListBySql<Base_CustomerManage>(manageSql, manageParameter.ToArray());
                foreach (var base_customermanage in base_customermanages)
                {
                    if (base_customermanage.CustomerId != null)
                    {
                        //priceHt["HeatLess"] = CommonHelper.GetDecimal(base_customermanage.DiscountRate);
                        //热处理费
                        if (CommonHelper.GetDecimal(priceHt["HeatPrice"]) == 0 && string.IsNullOrEmpty(base_customermanage.SteelCode.Replace("&nbsp;", "")))
                        {
                            priceHt["HeatPrice"] = CommonHelper.GetDecimal(base_customermanage.Price);
                        }
                        if (base_customermanage.SteelCode == SteelCode)
                        {
                            priceHt["HeatPrice"] = CommonHelper.GetDecimal(base_customermanage.Price);
                            break;

                        }
                    }
                }
            }





            //priceHt["WorkCost1"] = 0;
            //priceHt["MaterialCost1"] = 0;

            //#region 成本计算 
            //decimal areacost = 0;
            //decimal WorkCost1 = 0;
            //decimal MaterialCost1 = 0;
            ////string Thick, string Weigh, string Length
            //Base_SetCost base_setcost = database.FindEntity<Base_SetCost>("10000");
            //Base_Steel base_steel = database.FindEntity<Base_Steel>("SteelCode", SteelCode);
            //if (base_setcost != null && base_setcost.SetCostId != null && base_steel != null && base_steel.SteelId != null)
            //{
            //    //光板如果厚<60,则面积(长*宽*2)。如果60<=厚<80，则面积(长*宽+长*厚)*2。如果厚>=80，则面积(长*宽+长*厚+宽*厚)*2。精料(长*宽+长*厚+宽*厚)*2
            //    if (base_setcost.SetAreaType == 0)
            //    {
            //        if (OrderType == "光板加工")
            //        {
            //            if (CommonHelper.GetDecimal(Thick) < 60)
            //            {
            //                areacost = CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) * 2 / 1000000;
            //            }
            //            if (CommonHelper.GetDecimal(Thick) >= 60 && CommonHelper.GetDecimal(Thick) < 80)
            //            {
            //                areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick)) * 2 / 1000000;
            //            }
            //            if (CommonHelper.GetDecimal(Thick) >= 80)
            //            {
            //                areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick) + CommonHelper.GetDecimal(Thick) * CommonHelper.GetDecimal(Weigh)) * 2 / 1000000;
            //            }
            //        }
            //        if (OrderType == "精板加工")
            //        {
            //            areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick) + CommonHelper.GetDecimal(Thick) * CommonHelper.GetDecimal(Weigh)) * 2 / 1000000;
            //        }

            //    }
            //    //光板如果厚<60,则面积(长*宽*2)。如果60<=厚<80，则面积(长*宽+长*厚)*2。如果厚>=80，则面积(长*宽+长*厚+宽*厚)*2。精料(长*宽+长*厚+宽*厚)*2
            //    if (base_setcost.SetAreaType == 1)
            //    {
            //        if (CommonHelper.GetDecimal(Thick) < 60)
            //        {
            //            areacost = CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) * 2 / 1000000;
            //        }
            //        if (CommonHelper.GetDecimal(Thick) >= 60 && CommonHelper.GetDecimal(Thick) < 80)
            //        {
            //            areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick)) * 2 / 1000000;
            //        }
            //        if (CommonHelper.GetDecimal(Thick) >= 80)
            //        {
            //            areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick) + CommonHelper.GetDecimal(Thick) * CommonHelper.GetDecimal(Weigh)) * 2 / 1000000;
            //        }
            //    }
            //    //统一精料光板(长*宽*2)
            //    if (base_setcost.SetAreaType == 2)
            //    {
            //        areacost = CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) * 2 / 1000000;
            //    }
            //    //统一精料光板(长*宽+长*厚+宽*厚)*2
            //    if (base_setcost.SetAreaType == 3)
            //    {
            //        areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick) + CommonHelper.GetDecimal(Thick) * CommonHelper.GetDecimal(Weigh)) * 2 / 1000000;
            //    }
            //    //统一精料(长*宽+长*厚+宽*厚)*2，光板(长*宽*2)
            //    if (base_setcost.SetAreaType == 4)
            //    {
            //        if (OrderType == "光板加工")
            //        {
            //            areacost = CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) * 2 / 1000000;
            //        }
            //        if (OrderType == "精板加工")
            //        {
            //            areacost = (CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) + CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Thick) + CommonHelper.GetDecimal(Thick) * CommonHelper.GetDecimal(Weigh)) * 2 / 1000000;
            //        }
            //    }

            //    if (OrderType == "光板加工")
            //    {
            //        if (base_steel.SteelType == "软料")
            //        {
            //            if (areacost < base_setcost.GRLimiteArea)
            //            {
            //                areacost = CommonHelper.GetDecimal(base_setcost.GRLimiteArea);
            //            }
            //        }
            //        if (base_steel.SteelType.IndexOf("硬料") >= 0)
            //        {
            //            if (areacost < base_setcost.GYLimiteArea)
            //            {
            //                areacost = CommonHelper.GetDecimal(base_setcost.GYLimiteArea);
            //            }
            //        }
            //    }
            //    if (OrderType == "精板加工")
            //    {
            //        if (areacost < base_setcost.JBLimiteArea)
            //        {
            //            areacost = CommonHelper.GetDecimal(base_setcost.JBLimiteArea);
            //        }
            //    }
            //    //加工费
            //    if (OrderType == "光板加工")
            //    {
            //        if (base_steel.SteelType == "软料")
            //        {
            //            WorkCost1 = CommonHelper.GetDecimal(base_setcost.GRPrice) * areacost * Qty;
            //        }
            //        if (base_steel.SteelType.IndexOf("硬料") >= 0 )
            //        {
            //            WorkCost1 = CommonHelper.GetDecimal(base_setcost.GYPrice) * areacost * Qty;
            //        }
            //        if (base_steel.SteelType == "软料" || base_steel.SteelType.IndexOf("硬料") >= 0)
            //        {
            //            //超过长或宽，价格不同
            //            if (CommonHelper.GetDecimal(Length) >= base_setcost.GreatLong || CommonHelper.GetDecimal(Weigh) >= base_setcost.GreatWeigh)
            //            {
            //                WorkCost1 = CommonHelper.GetDecimal(base_setcost.GreatPrice) * areacost * Qty;
            //            }
            //        }
            //    }
            //    if (OrderType == "精板加工")
            //    {
            //        WorkCost1 = CommonHelper.GetDecimal(base_setcost.JBPrice) * areacost * Qty;
            //        //超过长或宽，价格不同
            //        if (CommonHelper.GetDecimal(Length) >= base_setcost.GreatLong || CommonHelper.GetDecimal(Weigh) >= base_setcost.GreatWeigh)
            //        {
            //            WorkCost1 = CommonHelper.GetDecimal(base_setcost.GreatPrice) * areacost * Qty;
            //        }
            //    }

            //    MaterialCost1 = Weight * CommonHelper.GetDecimal(base_steel.Price) * Qty;
            //    priceHt["WorkCost1"] = WorkCost1;
            //    priceHt["MaterialCost1"] = MaterialCost1;
            //}
            //#endregion
            if (CustMaterial == 1)
            {
                priceHt["MaterialPrice"] = 0;
            }
            if (Convert.ToDecimal(priceHt["WorkPrice"]) > 0)
            {
                priceHt["SmallCost"] = 0;
            }
            
            return priceHt;
        }

        public Hashtable GetProductName(string OrderType, string CustomerId, string SteelCode, int DD, int CustMaterial)
        {
            Hashtable priceHt = new Hashtable();
            string materialsSql = string.Format(@" SELECT  cm.*
FROM    Base_CustomerMaterials cm
WHERE   cm.CustomerId = @CustomerId
        --AND cm.ProcessType = @OrderType
        AND cm.SteelCode = @SteelCode
        
                                                            ");

            string RemarkSql = string.Format(@" SELECT  TOP 1  qd.*
          FROM      dbo.BY_QuotationDetail qd ,
                    dbo.BY_Quotation q
          WHERE     qd.QuotationId = q.QuotationId
                    AND q.CustomerId = @CustomerId
                    AND qd.OrderType = @OrderType
                    AND qd.SteelCode = @SteelCode
                    AND q.CustMaterial = @CustMaterial
                    AND qd.DD = @DD AND Q.DeleteMark = 0
                    ORDER BY qd.CreateDate DESC ");
            List<DbParameter> materialsParameter = new List<DbParameter>();
            materialsParameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
            materialsParameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            materialsParameter.Add(DbFactory.CreateDbParameter("@OrderType", OrderType));
            materialsParameter.Add(DbFactory.CreateDbParameter("@DD", DD));
            materialsParameter.Add(DbFactory.CreateDbParameter("@CustMaterial", CustMaterial));
            IList<Base_CustomerMaterials> base_customermaterials = database.FindListBySql<Base_CustomerMaterials>(materialsSql, materialsParameter.ToArray());
            BY_QuotationDetail BY_QuotationDetail = database.FindEntityBySql<BY_QuotationDetail>(RemarkSql, materialsParameter.ToArray());
            foreach (Base_CustomerMaterials item in base_customermaterials)
            {
                if (!string.IsNullOrEmpty(item.ProductName))
                {
                    priceHt["ProductName"] = item.ProductName;
                }
            }
            if (BY_QuotationDetail != null && BY_QuotationDetail.QuotationDetailId != null)
            {
                priceHt["Remark"] = BY_QuotationDetail.Remark;
                priceHt["AddLength"] = BY_QuotationDetail.AddLength;
                priceHt["AddWeigh"] = BY_QuotationDetail.AddWeigh;
                priceHt["Fang"] = BY_QuotationDetail.Fang;
                priceHt["DDCostPer"] = BY_QuotationDetail.DDCostPer;
            }
            return priceHt;
        }


        public Hashtable GetProductNameForQ(string Technology, string CustomerId, string QuotationId)
        {
            Hashtable priceHt = new Hashtable();
            string RemarkSql = string.Format(@"  SELECT TOP 1 qd.MaterialPrice 
 FROM   dbo.BY_QuotationDetail qd ,
        dbo.BY_Quotation q
 WHERE  qd.QuotationId = q.QuotationId AND Q.DeleteMark = 0
                    AND q.CustomerId = @CustomerId
                    AND qd.Technology = @Technology
                    ORDER BY qd.CreateDate DESC ");
            string RemarkSql1 = string.Format(@"  SELECT    ISNULL(SUM(Weight),0) Weight
          FROM      BY_QuotationDetail qd
          WHERE     qd.QuotationId = @QuotationId
                    AND CHARINDEX(@Technology, qd.Remark) > 0
                   ");

            List<DbParameter> materialsParameter = new List<DbParameter>();
            materialsParameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
            materialsParameter.Add(DbFactory.CreateDbParameter("@Technology", Technology));
            materialsParameter.Add(DbFactory.CreateDbParameter("@QuotationId", QuotationId));
           
            DataTable BY_QuotationDetail = database.FindTableBySql(RemarkSql, materialsParameter.ToArray());
            DataTable BY_QuotationDetail1 = database.FindTableBySql(RemarkSql1, materialsParameter.ToArray());
            
            if (BY_QuotationDetail.Rows.Count > 0)
            {
                priceHt["MaterialPrice"] = BY_QuotationDetail.Rows[0]["MaterialPrice"];
            }
            if (BY_QuotationDetail1.Rows.Count > 0)
            {
                priceHt["Weight"] = BY_QuotationDetail1.Rows[0]["Weight"];
            }

            return priceHt;
        }

        public Hashtable GetAdd(string OrderType, string CustomerId, string SteelCode)
        {
            Hashtable priceHt = new Hashtable();
            priceHt["AddLength"] = 0;
            priceHt["AddWeigh"] = 0;
            priceHt["AddThick"] = 0;
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(CustomerId);
            if (Base_Customer.Code == "LYJM")
            {
                
            }
            else if (Base_Customer.Code == "SSTJ")
            {
                
            }
            else if (Base_Customer.FullName.IndexOf("炜驰") >= 0)
            {

            }
            else
            {
                string RemarkSql = string.Format(@"  SELECT TOP 1
        qd.*
 FROM   dbo.BY_QuotationDetail qd ,
        dbo.BY_Quotation q
 WHERE  qd.OrderType = @OrderType AND Q.DeleteMark = 0
        AND qd.SteelCode = @SteelCode 
        AND q.QuotationId = qd.QuotationId
        AND q.CustomerId = @CustomerId
        and q.CustomerCode != 'LYJM'
        and q.CustomerCode != 'SSTJ'
        and  CHARINDEX('炜驰',CustomerName) < 1
 ORDER BY qd.CreateDate DESC  ");
                List<DbParameter> materialsParameter = new List<DbParameter>();
                materialsParameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
                materialsParameter.Add(DbFactory.CreateDbParameter("@OrderType", OrderType));
                materialsParameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
                DataTable BY_QuotationDetail = database.FindTableBySql(RemarkSql, materialsParameter.ToArray());
                if (BY_QuotationDetail.Rows.Count > 0)
                {
                    priceHt["AddLength"] = BY_QuotationDetail.Rows[0]["AddLength"];
                    priceHt["AddWeigh"] = BY_QuotationDetail.Rows[0]["AddWeigh"];
                    priceHt["AddThick"] = BY_QuotationDetail.Rows[0]["AddThick"];


                    priceHt["Remark"] = BY_QuotationDetail.Rows[0]["Remark"]; 
                    priceHt["RemarkKL"] = BY_QuotationDetail.Rows[0]["RemarkKL"]; 
                    priceHt["Fang"] = BY_QuotationDetail.Rows[0]["Fang"];


                    priceHt["NowStatus"] = BY_QuotationDetail.Rows[0]["NowStatus"];
                    priceHt["KSize"] = BY_QuotationDetail.Rows[0]["KSize"];
                    priceHt["ZSize"] = BY_QuotationDetail.Rows[0]["ZSize"];
                    priceHt["STSize"] = BY_QuotationDetail.Rows[0]["STSize"];
                    priceHt["Hardness"] = BY_QuotationDetail.Rows[0]["Hardness"];
                    priceHt["BoreSizeAdd"] = BY_QuotationDetail.Rows[0]["BoreSizeAdd"];
                    priceHt["CustomerProduct"] = BY_QuotationDetail.Rows[0]["CustomerProduct"];

                    priceHt["Kprocess"] = BY_QuotationDetail.Rows[0]["Kprocess"];
















                }
            }

            
           
            return priceHt;
        }

        #endregion
    }
}
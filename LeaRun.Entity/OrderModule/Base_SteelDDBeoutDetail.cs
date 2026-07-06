//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2016
// Software Developers @ DNS 2016
//=====================================================================================

using DNS.DataAccess.Attributes;
using DNS.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DNS.Entity
{
    /// <summary>
    /// Base_SteelDDBeoutDetail
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.06.07 21:43</date>
    /// </author>
    /// </summary>
    [Description("Base_SteelDDBeoutDetail")]
    [PrimaryKey("Base_SteelDDBeoutDetailId")]
    public class Base_SteelDDBeoutDetail : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 报价单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价单明细ID")]
        public string Base_SteelDDBeoutDetailId { get; set; }
        /// <summary>
        /// Base_SteelDDBeoutId
        /// </summary>
        /// <returns></returns>
        [DisplayName("Base_SteelDDBeoutId")]
        public string Base_SteelDDBeoutId { get; set; }
        /// <summary>
        /// Base_SteelDDBeoutId
        /// </summary>
        /// <returns></returns>
        [DisplayName("Base_SteelDDBeoutCode")]
        public string Base_SteelDDBeoutCode { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        /// <returns></returns>
        [DisplayName("行号")]
        public int? LineCode { get; set; }
        /// <summary>
        /// 板件编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("板件编码")]
        public string BoardId { get; set; }
        /// <summary>
        /// 板件描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("板件描述")]
        public string BoardDescription { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// 项目编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("项目编号")]
        public string ProjectCode { get; set; }
        /// <summary>
        /// 磨四角
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨四角")]
        public int? IsGrindAngle { get; set; }
        /// <summary>
        /// 倒角
        /// </summary>
        /// <returns></returns>
        [DisplayName("倒角")]
        public int? IsChamfer { get; set; }
        /// <summary>
        /// 钢种
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种")]
        public string SteelId { get; set; }
        /// <summary>
        /// 钢种编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种编号")]
        public string SteelCode { get; set; }
        /// <summary>
        /// 钢种描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种描述")]
        public string SteelDescription { get; set; }
        /// <summary>
        /// 钢种其它描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种其它描述")]
        public string SteelOtherDescription { get; set; }
        /// <summary>
        /// 钢种状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种状态")]
        public string SteelState { get; set; }
        /// <summary>
        /// 模号
        /// </summary>
        /// <returns></returns>
        [DisplayName("模号")]
        public string DieCode { get; set; }
        /// <summary>
        /// 厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚度")]
        public decimal? Thick { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽")]
        public decimal? Weigh { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        /// <returns></returns>
        [DisplayName("长")]
        public decimal? Length { get; set; }
        /// <summary>
        /// 计价余量(厚度)
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量(厚度)")]
        public decimal? AddThick { get; set; }
        /// <summary>
        /// 计价余量(宽)
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量(宽)")]
        public decimal? AddWeigh { get; set; }
        /// <summary>
        /// 计价余量(长)
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量(长)")]
        public decimal? AddLength { get; set; }
        /// <summary>
        /// 工艺
        /// </summary>
        /// <returns></returns>
        [DisplayName("工艺")]
        public string Technology { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("理论重量")]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 材料单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("材料单价")]
        public decimal? MaterialPrice { get; set; }
        /// <summary>
        /// 材料折扣
        /// </summary>
        /// <returns></returns>
        [DisplayName("材料折扣")]
        public decimal? MaterialLess { get; set; }
        /// <summary>
        /// 金额（材料）
        /// </summary>
        /// <returns></returns>
        [DisplayName("金额（材料）")]
        public decimal? MaterialAmount { get; set; }
        /// <summary>
        /// 小件单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("小件单价")]
        public decimal? WorkPrice { get; set; }
        /// <summary>
        /// 加工费折扣
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费折扣")]
        public decimal? WorkLess { get; set; }
        /// <summary>
        /// 小件金额（加工费）
        /// </summary>
        /// <returns></returns>
        [DisplayName("小件金额（加工费）")]
        public decimal? WorkAmount { get; set; }
        /// <summary>
        /// 热处理单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理单价")]
        public decimal? HeatPrice { get; set; }
        /// <summary>
        /// 热处理折扣
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理折扣")]
        public decimal? HeatLess { get; set; }
        /// <summary>
        /// 金额（热处理）
        /// </summary>
        /// <returns></returns>
        [DisplayName("金额（热处理）")]
        public decimal? HeatAmount { get; set; }
        /// <summary>
        /// 销售额
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售额")]
        public decimal? SalesAmount { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        /// <returns></returns>
        [DisplayName("成本")]
        public decimal? CostAmount { get; set; }
        /// <summary>
        /// 毛利润
        /// </summary>
        /// <returns></returns>
        [DisplayName("毛利润")]
        public decimal? ProfitAmount { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        /// <returns></returns>
        [DisplayName("毛利率")]
        public decimal? ProfitRate { get; set; }
        /// <summary>
        /// 送货说明
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货说明")]
        public string ShipDescription { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效")]
        public int? Enabled { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [DisplayName("排序码")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
        [DisplayName("删除标记")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建时间")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户主键")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改时间")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户主键")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 落单状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("落单状态")]
        public int? PlaceOrderStatus { get; set; }
        /// <summary>
        /// 加工类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工类型")]
        public string OrderType { get; set; }
        /// <summary>
        /// 客户订单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户订单号")]
        public string CustomerOrder { get; set; }
        /// <summary>
        /// 客户货品号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户货品号")]
        public string CustomerProduct { get; set; }
        /// <summary>
        /// 报价单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价单号")]
        public string QuotationCode { get; set; }
        /// <summary>
        /// 打底重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底重量")]
        public decimal? LimitWeight { get; set; }
        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? LimitThick { get; set; }
        /// <summary>
        /// 加工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费")]
        public decimal? WorkCost { get; set; }
        /// <summary>
        /// 材料费
        /// </summary>
        /// <returns></returns>
        [DisplayName("材料费")]
        public decimal? MaterialCost { get; set; }
        /// <summary>
        /// 管理费
        /// </summary>
        /// <returns></returns>
        [DisplayName("管理费")]
        public decimal? ManagerCost { get; set; }
        /// <summary>
        /// 税费
        /// </summary>
        /// <returns></returns>
        [DisplayName("税费")]
        public decimal? InvoiceCost { get; set; }
        /// <summary>
        /// 替代料
        /// </summary>
        /// <returns></returns>
        [DisplayName("替代料")]
        public string RepalceMl { get; set; }
        /// <summary>
        /// 计价类型：按kg，按件
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价类型：按kg，按件")]
        public string JJtype { get; set; }
        /// <summary>
        /// 品名（一般钢材名加加工类型）可修改
        /// </summary>
        /// <returns></returns>
        [DisplayName("品名（一般钢材名加加工类型）可修改")]
        public string ProductName { get; set; }
        /// <summary>
        /// 吊孔
        /// </summary>
        /// <returns></returns>
        [DisplayName("吊孔")]
        public int? LiftingHole { get; set; }
        /// <summary>
        /// 气割
        /// </summary>
        /// <returns></returns>
        [DisplayName("气割")]
        public int? GasCuttingSteel { get; set; }
        /// <summary>
        /// 孔径
        /// </summary>
        /// <returns></returns>
        [DisplayName("孔径")]
        public decimal? BoreSize { get; set; }
        /// <summary>
        /// 钢材类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材类型")]
        public string SteelType { get; set; }
        /// <summary>
        /// Area
        /// </summary>
        /// <returns></returns>
        [DisplayName("Area")]
        public decimal? Area { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("理论重量")]
        public decimal? TheoryWeight { get; set; }
        /// <summary>
        /// 磨铣
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨铣")]
        public int? MandXSteel { get; set; }
        /// <summary>
        /// 磨面
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨面")]
        public int? MorXSteel { get; set; }
        /// <summary>
        /// 类型洗边
        /// </summary>
        /// <returns></returns>
        [DisplayName("类型洗边")]
        public int? BSteel { get; set; }
        /// <summary>
        /// 0
        /// </summary>
        /// <returns></returns>
        [DisplayName("0")]
        public decimal? MianCostArea { get; set; }
        /// <summary>
        /// 面单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("面单价")]
        public decimal? MianCostPer { get; set; }
        /// <summary>
        /// 面总费用
        /// </summary>
        /// <returns></returns>
        [DisplayName("面总费用")]
        public decimal? MianCost { get; set; }
        /// <summary>
        /// BianCostLong
        /// </summary>
        /// <returns></returns>
        [DisplayName("BianCostLong")]
        public decimal? BianCostLong { get; set; }
        /// <summary>
        /// 边单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("边单价")]
        public decimal? BianCostPer { get; set; }
        /// <summary>
        /// 边总价
        /// </summary>
        /// <returns></returns>
        [DisplayName("边总价")]
        public decimal? BianCost { get; set; }
        /// <summary>
        /// 锯口费
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯口费")]
        public decimal? JKCost { get; set; }
        /// <summary>
        /// 加工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费")]
        public decimal? SmallCost { get; set; }
        /// <summary>
        /// 加工费计算员工费用
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费计算员工费用")]
        public decimal? WorkCostForWork { get; set; }
        /// <summary>
        /// 类型精洗边
        /// </summary>
        /// <returns></returns>
        [DisplayName("类型精洗边")]
        public int? JBSteel { get; set; }
        /// <summary>
        /// 加工费计算员工费用
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费计算员工费用")]
        public decimal? WorkCostForWorkTemp { get; set; }
        /// <summary>
        /// 面类型  2，两面，1，单面
        /// </summary>
        /// <returns></returns>
        [DisplayName("面类型  2，两面，1，单面")]
        public int? MianType { get; set; }
        /// <summary>
        /// 边类型，4，四边，3，长短，2，两长，1两短
        /// </summary>
        /// <returns></returns>
        [DisplayName("边类型，4，四边，3，长短，2，两长，1两短")]
        public int? BianType { get; set; }
        /// <summary>
        /// 按件计算
        /// </summary>
        /// <returns></returns>
        [DisplayName("按件计算")]
        public int? MianCostUnit { get; set; }
        /// <summary>
        /// 按件计算
        /// </summary>
        /// <returns></returns>
        [DisplayName("按件计算")]
        public int? BianCostUnit { get; set; }
        /// <summary>
        /// 按件计算
        /// </summary>
        /// <returns></returns>
        [DisplayName("按件计算")]
        public int? WorkUnit { get; set; }
        /// <summary>
        /// 铣面
        /// </summary>
        /// <returns></returns>
        [DisplayName("铣面")]
        public int? XorMSteel { get; set; }
        /// <summary>
        /// 计算公式
        /// </summary>
        /// <returns></returns>
        [DisplayName("计算公式")]
        public string CountShow { get; set; }
        /// <summary>
        /// 面总费用
        /// </summary>
        /// <returns></returns>
        [DisplayName("面总费用")]
        public decimal? MianCostWork { get; set; }
        /// <summary>
        /// 边总价
        /// </summary>
        /// <returns></returns>
        [DisplayName("边总价")]
        public decimal? BianCostWork { get; set; }
        /// <summary>
        /// CNC
        /// </summary>
        /// <returns></returns>
        [DisplayName("CNC")]
        public int? CNC { get; set; }
        /// <summary>
        /// 研磨
        /// </summary>
        /// <returns></returns>
        [DisplayName("研磨")]
        public int? YM { get; set; }
        /// <summary>
        /// 锯切
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯切")]
        public int? JQ { get; set; }
        /// <summary>
        /// Kprocess
        /// </summary>
        /// <returns></returns>
        [DisplayName("Kprocess")]
        public string Kprocess { get; set; }
        /// <summary>
        /// 锯口费单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯口费单价")]
        public decimal? JKCostPer { get; set; }
        /// <summary>
        /// 几刀
        /// </summary>
        /// <returns></returns>
        [DisplayName("几刀")]
        public int? JKCostQty { get; set; }
        /// <summary>
        /// 加工金额（加工费）
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工金额（加工费）")]
        public decimal? SmallAmount { get; set; }
        /// <summary>
        /// 二放三放
        /// </summary>
        /// <returns></returns>
        [DisplayName("二放三放")]
        public string Fang { get; set; }
        /// <summary>
        /// 切角单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("切角单价")]
        public decimal? DDCostPer { get; set; }
        /// <summary>
        /// 切角费用
        /// </summary>
        /// <returns></returns>
        [DisplayName("切角费用")]
        public decimal? DDCost { get; set; }
        /// <summary>
        /// DD
        /// </summary>
        /// <returns></returns>
        [DisplayName("DD")]
        public int? DD { get; set; }
             /// <summary>
        /// XLsize
        /// </summary>
        /// <returns></returns>
        [DisplayName("XLsize")]
        public string XLsize { get; set; }
        /// <summary>
        /// QuotationDetailId
        /// </summary>
        /// <returns></returns>
        [DisplayName("QuotationDetailId")]
        public string QuotationDetailId { get; set; }
        public decimal? Density { get; set; }
        public decimal? Weightcheng { get; set; }
        public int? isback { get; set; }

        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Base_SteelDDBeoutDetailId = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Base_SteelDDBeoutDetailId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
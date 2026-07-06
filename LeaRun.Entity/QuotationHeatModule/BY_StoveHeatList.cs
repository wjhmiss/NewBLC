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
    /// BY_StoveHeatList
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.24 13:17</date>
    /// </author>
    /// </summary>
    [Description("BY_StoveHeatList")]
    [PrimaryKey("StoveHeatListId")]
    public class BY_StoveHeatList : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 报价单明细ID
        /// </summary>
        /// <returns></returns>
        /// StoveHeatListId
        [DisplayName("报价单明细ID")]
        public string StoveHeatListId { get; set; }
        public string QuotationHeatId { get; set; }
        /// <summary>
        /// StoveId
        /// </summary>
        /// <returns></returns>
        [DisplayName("StoveId")]
        public string StoveId { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        /// <returns></returns>
        [DisplayName("行号")]
        public long? QuotationHeatCode { get; set; }
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
        /// 客户PO
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户PO")]
        public string CustomerPO { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户ID")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户编号")]
        public string CustomerCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户名称")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户简称
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户简称")]
        public string ShortName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        /// <returns></returns>
        [DisplayName("联系人")]
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("联系电话")]
        public string ContactTel { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("传真")]
        public string ContactFax { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        /// <returns></returns>
        [DisplayName("邮箱")]
        public string ContactMail { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        /// <returns></returns>
        [DisplayName("手机")]
        public string MobileCode { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户地址")]
        public string CustomerAddress { get; set; }
        /// <summary>
        /// TechnologQuery
        /// </summary>
        /// <returns></returns>
        [DisplayName("TechnologQuery")]
        public string TechnologQuery { get; set; }
        /// <summary>
        /// TechnologQueryWrite
        /// </summary>
        /// <returns></returns>
        [DisplayName("TechnologQueryWrite")]
        public string TechnologQueryWrite { get; set; }
        /// <summary>
        /// BillStatus
        /// </summary>
        /// <returns></returns>
        [DisplayName("BillStatus")]
        public string BillStatus { get; set; }
        /// <summary>
        /// HeatStatus
        /// </summary>
        /// <returns></returns>
        [DisplayName("HeatStatus")]
        public string HeatStatus { get; set; }
        /// <summary>
        /// 发运单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("发运单号")]
        public string DeliveryCode { get; set; }
        public DateTime? StartDateTime { get; set; }

        public string StoveNum	{ get; set; }
        public string StoveCode	{ get; set; }
        public string MethodBuilder	{ get; set; }
        public string StoveType	{ get; set; }
        public string BuildMethod { get; set; }

    public DateTime?    SDDateTime { get; set; }
public DateTime? DWDateTime	 { get; set; }
public DateTime? CLDateTime { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.StoveHeatListId = CommonHelper.GetGuid;
              if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }             this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.StoveHeatListId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
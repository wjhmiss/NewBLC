//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
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
    /// BY_OrderDetailBeOut
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.03 20:06</date>
    /// </author>
    /// </summary>
    [Description("BY_OrderDetailBeOut")]
    [PrimaryKey("OrderDetailOutId")]
    public class BY_OrderDetailBeOut : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 订单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单明细ID")]
        public string OrderDetailOutId { get; set; }
        /// <summary>
        /// 订单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单明细ID")]
        public string OrderDetailId { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单ID")]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单号")]
        public string OrderCode { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        /// <returns></returns>
        [DisplayName("行号")]
        public int? LineCode { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
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
        /// 执行备注信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("执行备注信息")]
        public string Remark { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("单据状态")]
        public string BillStatus { get; set; }
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
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("理论重量")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// QuotationDetailId
        /// </summary>
        /// <returns></returns>
        [DisplayName("QuotationDetailId")]
        public string QuotationDetailId { get; set; }
        
        /// <summary>
        /// 加工类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工类型")]
        public string OrderType { get; set; }
   
        /// <summary>
        /// 是否委外
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否委外")]
        public int? IsBeOut { get; set; }
        /// <summary>
        /// 委外完成时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("委外完成时间")]
        public DateTime? BeOutCompleteDate { get; set; }
        /// <summary>
        /// BeOutCompletePerson
        /// </summary>
        /// <returns></returns>
        [DisplayName("BeOutCompletePerson")]
        public string BeOutCompletePerson { get; set; }
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
        /// 材料单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("材料单价")]
        public decimal? MaterialPrice { get; set; }
        /// <summary>
        /// 金额（材料）
        /// </summary>
        /// <returns></returns>
        [DisplayName("金额（材料）")]
        public decimal? MaterialAmount { get; set; }
        /// <summary>
        /// 加工费单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费单价")]
        public decimal? WorkPrice { get; set; }
        /// <summary>
        /// 金额（加工费）
        /// </summary>
        /// <returns></returns>
        [DisplayName("金额（加工费）")]
        public decimal? WorkAmount { get; set; }
        /// <summary>
        /// 热处理单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理单价")]
        public decimal? HeatPrice { get; set; }
        /// <summary>
        /// 金额（热处理）
        /// </summary>
        /// <returns></returns>
        [DisplayName("金额（热处理）")]
        public decimal? HeatAmount { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        /// <returns></returns>
        [DisplayName("成本")]
        public decimal? CostAmount { get; set; }
        /// <summary>
        /// 过磅重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("过磅重量")]
        public decimal? WeightFact { get; set; }
        /// <summary>
        /// 报价单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价单号")]
        public string QuotationCode { get; set; }
        /// <summary>
        /// 报价单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价单号")]
        public string PaymentMode { get; set; }
        /// <summary>
        /// 已付款
        /// </summary>
        /// <returns></returns>
        [DisplayName("已付款")]
        public int? IsPay { get; set; }
        /// <summary>
        /// 已开票
        /// </summary>
        /// <returns></returns>
        [DisplayName("已开票")]
        public int? IsInvoice { get; set; }

        /// <summary>
        /// 计价面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价面积")]
        public decimal? Area { get; set; }
        /// <summary>
        /// 计价面积单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价面积单价")]
        public decimal? AreaPrice { get; set; }
        /// <summary>
        /// 罚款
        /// </summary>
        /// <returns></returns>
        [DisplayName("罚款")]
        public decimal? AmerceAmount { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        /// <returns></returns>
        [DisplayName("助记码")]
        public string MnemonicCode { get; set; }
        /// <summary>
        /// 品名（一般钢材名加加工类型）可修改
        /// </summary>
        /// <returns></returns>
        [DisplayName("品名（一般钢材名加加工类型）可修改")]
        public string ProductName { get; set; }
        /// <summary>
        /// 计价类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价类型")]
        public string JJtype { get; set; }
        /// <summary>
        /// 钢材类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材类型")]
        public string SteelType { get; set; }

        /// <summary>
        /// 对账单
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账单")]
        public string PayCheckSheetCode { get; set; }

        /// <summary>
        /// 对账单
        /// </summary>
        /// <returns></returns>
        [DisplayName("审核")]
        public string Status { get; set; }

        /// <summary>
        /// IsPurchaseOrBeout
        /// </summary>
        /// <returns></returns>
        [DisplayName("IsPurchaseOrBeout")]
        public string IsPurchaseOrBeout { get; set; }
        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OrderDetailOutId = CommonHelper.GetGuid;
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
            this.OrderDetailOutId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
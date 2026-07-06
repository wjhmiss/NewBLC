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
    /// 对账单
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.04.24 11:06</date>
    /// </author>
    /// </summary>
    [Description("对账单")]
    [PrimaryKey("CheckSheetId")]
    public class BY_CheckSheet : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 对账单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账单ID")]
        public string CheckSheetId { get; set; }
        /// <summary>
        /// 对账单单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账单单号")]
        public string CheckSheetCode { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("开始时间")]
        public DateTime? BeginDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("结束时间")]
        public DateTime? EndDate { get; set; }
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
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("总金额")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 已获金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("已获金额")]
        public decimal? GetedAmount { get; set; }
        /// <summary>
        /// 是否开票
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否开票")]
        public int? IsInvoice { get; set; }
        /// <summary>
        /// 是否已还清，1还清，0未还清
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否已还清，1还清，0未还清")]
        public int? IsOver { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        /// <returns></returns>
        [DisplayName("折扣")]
        public decimal? LessRate { get; set; }
        /// <summary>
        /// 折扣后总额
        /// </summary>
        /// <returns></returns>
        [DisplayName("折扣后总额")]
        public decimal? LessedAmount { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("付款方式")]
        public string PaymentMode { get; set; }
        /// <summary>
        /// 送货方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货方式")]
        public string ShipMode { get; set; }
        /// <summary>
        /// 出货组织
        /// </summary>
        /// <returns></returns>
        [DisplayName("出货组织")]
        public string ShipGroup { get; set; }
        /// <summary>
        /// 销售方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售方式")]
        public string SalesMode { get; set; }
        /// <summary>
        /// 销售员
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售员")]
        public string SalesMan { get; set; }
        /// <summary>
        /// 销售区域
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售区域")]
        public string SalesArea { get; set; }
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
        /// 重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("重量")]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 是否月结确认
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否月结确认")]
        public int? IsComfirm { get; set; }
        /// <summary>
        /// 加工费算法
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费算法")]
        public string ProcessingAlgorithm { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        /// <returns></returns>
        [DisplayName("月份")]
        public string YearMonth { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        /// <returns></returns>
        [DisplayName("税率")]
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// 送货单金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货单金额")]
        public decimal? DTotalAmount { get; set; }
        /// <summary>
        ///发票时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("发票时间")]
        public DateTime? InvoiceDate { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        /// <returns></returns>
        [DisplayName("发票号")]
        public string InvoiceNum { get; set; }
        /// <summary>
        /// 送货单金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货单金额")]
        public decimal? DTotalAmountOri { get; set; }


        /// <summary>
        /// 发票号
        /// </summary>
        /// <returns></returns>
        [DisplayName("发票号")]
        public string InvoiceCustomer { get; set; }

        /// <summary>
        /// 状态0未审核，1已审核，-1拒绝
        /// </summary>
        /// <returns></returns>
        [DisplayName("状态0未审核，1已审核，-1拒绝")]
        public int? Status { get; set; }


        /// <summary>
        /// 开票费
        /// </summary>
        /// <returns></returns>
        [DisplayName("开票费")]
        public decimal? InvoiceMoney { get; set; }

        /// <summary>
        /// 开票费
        /// </summary>
        /// <returns></returns>
        [DisplayName("开票费")]
        public decimal? CanInvoiceMoney { get; set; }
        public string LType { get; set; }

        public string CompanyName { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CheckSheetId = CommonHelper.GetGuid;
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
            this.CheckSheetId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
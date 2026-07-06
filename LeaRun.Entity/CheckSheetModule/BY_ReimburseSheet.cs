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
    /// 费用申请单
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.04.24 22:47</date>
    /// </author>
    /// </summary>
    [Description("费用申请单")]
    [PrimaryKey("ReimburseSheetID")]
    public class BY_ReimburseSheet : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 申请单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("申请单ID")]
        public string ReimburseSheetID { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("申请单号")]
        public string ReimburseSheetCode { get; set; }

        /// <summary>
        /// 事由类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("关联对账单")]
        public string Code { get; set; }
        /// <summary>
        /// 事由类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("事由类型")]
        public string CodeType { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        /// <returns></returns>
        [DisplayName("申请人")]
        public string RequiredMan { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("申请日期")]
        public DateTime? RequiredDate { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("申请金额")]
        public decimal? RequiredMoney { get; set; }
        /// <summary>
        /// 实际到账金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("实际到账金额")]
        public decimal? AuditMoney { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        /// <returns></returns>
        [DisplayName("审核人")]
        public string AuditMan { get; set; }
        /// <summary>
        /// 审核人备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("审核人备注")]
        public string AuditRemark { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("审核时间")]
        public DateTime? AuditDateTime { get; set; }
        
        /// <summary>
        /// 状态0未审核，1已审核，-1拒绝
        /// </summary>
        /// <returns></returns>
        [DisplayName("状态0未审核，1已审核，-1拒绝")]
        public string Status { get; set; }
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
        /// 助记码
        /// </summary>
        /// <returns></returns>
        [DisplayName("助记码")]
        public string MnemonicCode { get; set; }
        /// <summary>
        /// 报价单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价单号")]
        public string QuotationCode { get; set; }
        /// <summary>
        /// 实际到账金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("折让金额")]
        public decimal? ZRMoney { get; set; }


        /// <summary>
        /// 实际到账金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("实收")]
        public decimal? GETMoney { get; set; }
        /// <summary>
        /// 实际到账金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("未收")]
        public decimal? NotGetMoney { get; set; }


        /// <summary>
        /// 现金,支票，承兑
        /// </summary>
        /// <returns></returns>
        [DisplayName("现金,支票，承兑")]
        public string GetType { get; set; }
        /// <summary>
        /// 预收款，应收款，回退或付款
        /// </summary>
        /// <returns></returns>
        [DisplayName("预收款，应收款，回退或付款")]
        public string Opions { get; set; }

         /// <summary>
        /// 剩下可用金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("剩下可用金额")]
        public decimal? CanUseMoney { get; set; }
        
         /// <summary>
        /// 应收总额
        /// </summary>
        /// <returns></returns>
        [DisplayName("应收总额")]
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// 开票单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("开票单号")]
        public string InvoiceNum { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        /// <returns></returns>
        [DisplayName("BankNum")]
        public string BankNum { get; set; }


       
        public string ZRRemark { get; set; }

        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public string WorkerCode { get; set; }
        public string DepartmentName { get; set; }

        public string Remark1 { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Type0 { get; set; }

        public string LType { get; set; }

        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ReimburseSheetID = CommonHelper.GetGuid;
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
            this.ReimburseSheetID = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion


    }
}
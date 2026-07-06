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
    /// Base_Sheet
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.02 13:44</date>
    /// </author>
    /// </summary>
    [Description("Base_Sheet")]
    [PrimaryKey("PaySheetId")]
    public class Base_Sheet : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 付款id
        /// </summary>
        /// <returns></returns>
        [DisplayName("付款id")]
        public string PaySheetId { get; set; }
        /// <summary>
        /// 付款号
        /// </summary>
        /// <returns></returns>
        [DisplayName("付款号")]
        public string PaySheetCode { get; set; }
        /// <summary>
        /// 付款事项
        /// </summary>
        /// <returns></returns>
        [DisplayName("付款事项")]
        public string PayName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        /// <returns></returns>
        [DisplayName("规格")]
        public string Size { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价")]
        public decimal? PricePer { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public int? Qty { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("总金额")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 状态0未审核，1已审核，-1拒绝
        /// </summary>
        /// <returns></returns>
        [DisplayName("状态0未审核，1已审核，-1拒绝")]
        public int? Status { get; set; }
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
        /// 支付方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("支付方式")]
        public string PayType { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("员工ID")]
        public string WorkerId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("员工姓名")]
        public string WorkerName { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("工号")]
        public string WorkerCode { get; set; }
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
        /// DepartmentName
        /// </summary>
        /// <returns></returns>
        [DisplayName("DepartmentName")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建时间")]
        public DateTime? UseDate { get; set; }
        /// <summary>
        /// 已获金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("已获金额")]
        public decimal? GetedAmount { get; set; }
        /// <summary>
        /// PayName1
        /// </summary>
        /// <returns></returns>
        [DisplayName("PayName1")]
        public string PayName1 { get; set; }
        public string SF { get; set; }

        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Type0 { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.PaySheetId = CommonHelper.GetGuid;
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
            this.PaySheetId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
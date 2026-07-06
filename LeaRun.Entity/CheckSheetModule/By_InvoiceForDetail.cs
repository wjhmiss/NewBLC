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
    /// By_InvoiceForDetail
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.05.24 16:41</date>
    /// </author>
    /// </summary>
    [Description("By_InvoiceForDetail")]
    [PrimaryKey("InvoiceId")]
    public class By_InvoiceForDetail : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// InvoiceId
        /// </summary>
        /// <returns></returns>
        [DisplayName("InvoiceId")]
        public string InvoiceId { get; set; }
        /// <summary>
        /// 发运单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("发运单明细ID")]
 
        /// <summary>
        /// InvoiceDate
        /// </summary>
        /// <returns></returns>
        public DateTime? InvoiceDate { get; set; }
        /// <summary>
        /// InvoiceNum
        /// </summary>
        /// <returns></returns>
        [DisplayName("InvoiceNum")]
        public string InvoiceNum { get; set; }
        public string InvoiceCode { get; set; }

        
        /// <summary>
        /// 接收单位
        /// </summary>
        /// <returns></returns>
        [DisplayName("接收单位")]
        public string InvoiceCustomer { get; set; }
        /// <summary>
        /// InvoiceMoney
        /// </summary>
        /// <returns></returns>
        [DisplayName("InvoiceMoney")]
        public decimal? InvoiceMoney { get; set; }
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
        public string TypeName { get; set; }
        public string CheckSheetId { get; set; }
        public string LType { get; set; }

        public string CompanyName { get; set; }
        public string CheckSheetCode { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.InvoiceId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.InvoiceId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
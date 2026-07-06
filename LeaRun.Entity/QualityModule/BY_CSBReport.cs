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
    /// BY_CSBReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.17 21:44</date>
    /// </author>
    /// </summary>
    [Description("BY_CSBReport")]
    [PrimaryKey("CSBReportId")]
    public class BY_CSBReport : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// CSBReportId
        /// </summary>
        /// <returns></returns>
        [DisplayName("CSBReportId")]
        public string CSBReportId { get; set; }
        /// <summary>
        /// CSBReportCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("CSBReportCode")]
        public string CSBReportCode { get; set; }
        /// <summary>
        /// 订单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单明细ID")]
        public string OrderDetailId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单号")]
        public string OrderLineCode { get; set; } 
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
        /// Size
        /// </summary>
        /// <returns></returns>
        [DisplayName("Size")]
        public string Size { get; set; }
        /// <summary>
        /// 加工类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工类型")]
        public string OrderType { get; set; }
        /// <summary>
        /// 客户货品号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户货品号")]
        public string CustomerProduct { get; set; }
        /// <summary>
        /// Machine
        /// </summary>
        /// <returns></returns>
        [DisplayName("Machine")]
        public string Machine { get; set; }
        /// <summary>
        /// Pinlv
        /// </summary>
        /// <returns></returns>
        [DisplayName("Pinlv")]
        public string Pinlv { get; set; }
        /// <summary>
        /// OHJ
        /// </summary>
        /// <returns></returns>
        [DisplayName("OHJ")]
        public string OHJ { get; set; }
        /// <summary>
        /// CCD
        /// </summary>
        /// <returns></returns>
        [DisplayName("CCD")]
        public string CCD { get; set; }
        /// <summary>
        /// BZ
        /// </summary>
        /// <returns></returns>
        [DisplayName("BZ")]
        public string BZ { get; set; }
        /// <summary>
        /// BW
        /// </summary>
        /// <returns></returns>
        [DisplayName("BW")]
        public string BW { get; set; }
        /// <summary>
        /// Resilt
        /// </summary>
        /// <returns></returns>
        [DisplayName("Resilt")]
        public string Resilt { get; set; }
        /// <summary>
        /// ReportName
        /// </summary>
        /// <returns></returns>
        [DisplayName("ReportName")]
        public string ReportName { get; set; }
        /// <summary>
        /// CheckName
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckName")]
        public string CheckName { get; set; }
        /// <summary>
        /// 执行备注信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("执行备注信息")]
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
        public int? Qty { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CSBReportId = CommonHelper.GetGuid;
              if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }          this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.CSBReportId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
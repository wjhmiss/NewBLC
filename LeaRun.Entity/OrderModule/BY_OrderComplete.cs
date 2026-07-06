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
    /// 订单完工表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.24 15:03</date>
    /// </author>
    /// </summary>
    [Description("订单完工表")]
    [PrimaryKey("OrderCompleteId")]
    public class BY_OrderComplete : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 订单完工ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单完工ID")]
        public string OrderCompleteId { get; set; }
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
        /// 完工数
        /// </summary>
        /// <returns></returns>
        [DisplayName("完工数")]
        public int? Qty { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("重量")]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 完工时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("完工时间")]
        public DateTime? CompleteDate { get; set; }
        /// <summary>
        /// 完工人
        /// </summary>
        /// <returns></returns>
        [DisplayName("完工人")]
        public string CompleteUserId { get; set; }
        /// <summary>
        /// 货运单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("货运单号")]
        public string DeliveryCode { get; set; }
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
        public decimal? TheoryWeight { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        /// <returns></returns>
        [DisplayName("班次")]
        public DateTime? ClassesDate { get; set; }
        /// <summary>
        /// 班次 A 白班 B 夜班
        /// </summary>
        /// <returns></returns>
        [DisplayName("班次 A 白班 B 夜班")]
        public string ClassesShift { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OrderCompleteId = CommonHelper.GetGuid;
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
            this.OrderCompleteId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
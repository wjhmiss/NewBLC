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
    /// 落单表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 21:49</date>
    /// </author>
    /// </summary>
    [Description("落单表")]
    [PrimaryKey("BookedId")]
    public class BY_Booked : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 落单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("落单ID")]
        public string BookedId { get; set; }
        /// <summary>
        /// 单据明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("单据明细ID")]
        public string OrderDetailId { get; set; }
        /// <summary>
        /// 单号+行号
        /// </summary>
        /// <returns></returns>
        [DisplayName("单号+行号")]
        public string BillCode { get; set; }
        /// <summary>
        /// 落单类型(0-自动落单，1-手工落单)
        /// </summary>
        /// <returns></returns>
        [DisplayName("落单类型(0-自动落单，1-手工落单)")]
        public int? BookedType { get; set; }
        /// <summary>
        /// 产能日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("产能日期")]
        public DateTime? CapacityDate { get; set; }
        /// <summary>
        /// 时段
        /// </summary>
        /// <returns></returns>
        [DisplayName("时段")]
        public string DayPart { get; set; }
        /// <summary>
        /// 落单数
        /// </summary>
        /// <returns></returns>
        [DisplayName("落单数")]
        public decimal? Qty { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.BookedId = CommonHelper.GetGuid;
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
            this.BookedId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
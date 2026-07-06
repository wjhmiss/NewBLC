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
    /// 产能表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 14:10</date>
    /// </author>
    /// </summary>
    [Description("产能表")]
    [PrimaryKey("CapacityId")]
    public class BY_Capacity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 产能ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("产能ID")]
        public string CapacityId { get; set; }
        /// <summary>
        /// 产能类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("产能类型")]
        public string CapacityType { get; set; }
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
        /// 产能值
        /// </summary>
        /// <returns></returns>
        [DisplayName("产能值")]
        public decimal? CapacityValue { get; set; }
        /// <summary>
        /// 已落单数
        /// </summary>
        /// <returns></returns>
        [DisplayName("已落单数")]
        public decimal? BookedQty { get; set; }
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
            this.CapacityId = CommonHelper.GetGuid;
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
            this.CapacityId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
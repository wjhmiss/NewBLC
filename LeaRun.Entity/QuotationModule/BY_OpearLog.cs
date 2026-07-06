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
    /// 操作记录表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.29 14:48</date>
    /// </author>
    /// </summary>
    [Description("操作记录表")]
    [PrimaryKey("OpearLogId")]
    public class BY_OpearLog : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 操作ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("操作ID")]
        public string OpearLogId { get; set; }
        /// <summary>
        /// 单据ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("单据ID")]
        public string BillId { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("单号")]
        public string BillCode { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("操作人ID")]
        public string UserId { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        /// <returns></returns>
        [DisplayName("操作人")]
        public string UserName { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("操作类型")]
        public string OpearType { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OpearLogId = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;

            this.UserId = ManageProvider.Provider.Current().UserId;
            this.UserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.OpearLogId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
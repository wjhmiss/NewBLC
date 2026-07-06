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
    /// 客户地址表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 13:38</date>
    /// </author>
    /// </summary>
    [Description("客户地址表")]
    [PrimaryKey("CustomerAddressId")]
    public class Base_CustomerAddress : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 客户地址主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户地址主键")]
        public string CustomerAddressId { get; set; }
        /// <summary>
        /// 客户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户主键")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 地址编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("地址编码")]
        public string AddressCode { get; set; }
        /// <summary>
        /// 地址用途
        /// </summary>
        /// <returns></returns>
        [DisplayName("地址用途")]
        public string Application { get; set; }
        /// <summary>
        /// 送货路线
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货路线")]
        public string SendPath { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        /// <returns></returns>
        [DisplayName("详细地址")]
        public string DetailAddress { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        /// <returns></returns>
        [DisplayName("联系人")]
        public string Contact { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("电话")]
        public string Telephone { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        /// <returns></returns>
        [DisplayName("手机")]
        public string Mobile { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        /// <returns></returns>
        [DisplayName("电子邮箱")]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("传真")]
        public string Fax { get; set; }
        /// <summary>
        /// 默认
        /// </summary>
        /// <returns></returns>
        [DisplayName("默认")]
        public string MainDefault { get; set; }
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
            this.CustomerAddressId = CommonHelper.GetGuid;
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
            this.CustomerAddressId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
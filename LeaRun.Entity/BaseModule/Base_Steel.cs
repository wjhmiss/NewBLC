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
    /// 钢材信息表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.13 14:42</date>
    /// </author>
    /// </summary>
    [Description("钢材信息表")]
    [PrimaryKey("SteelId")]
    public class Base_Steel : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 钢材信息主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材信息主键")]
        public string SteelId { get; set; }
        /// <summary>
        /// 钢材编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材编码")]
        public string SteelCode { get; set; }
        /// <summary>
        /// 钢材名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材名称")]
        public string SteelName { get; set; }
        /// <summary>
        /// 替代料
        /// </summary>
        /// <returns></returns>
        [DisplayName("替代料")]
        public string RepalceMl { get; set; }
        /// <summary>
        /// 其它描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("其它描述")]
        public string OtherDescription { get; set; }
        /// <summary>
        /// 出质保书
        /// </summary>
        /// <returns></returns>
        [DisplayName("出质保书")]
        public int? Warranty { get; set; }
        /// <summary>
        /// 钢材类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材类型")]
        public string SteelType { get; set; }
        /// <summary>
        /// 密度
        /// </summary>
        /// <returns></returns>
        [DisplayName("密度")]
        public decimal? Density { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        /// <returns></returns>
        [DisplayName("产地")]
        public string Place { get; set; }
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
        /// 助记码
        /// </summary>
        /// <returns></returns>
        [DisplayName("助记码")]
        public string RemenberCode { get; set; }


        public string Hardness { get; set; }
        public string C { get; set; }
        public string Si { get; set; }
        public string Mn { get; set; }
        public string Cr { get; set; }
        public string Ni { get; set; }
        public string Mo { get; set; }
        public string P { get; set; }
        public string S { get; set; }
        public string Cu { get; set; }
        public string Al { get; set; }
        public string Ti { get; set; }
        public string V { get; set; }
        public string W { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SteelId = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.SteelId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
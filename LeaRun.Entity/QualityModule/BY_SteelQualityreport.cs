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
    /// BY_SteelQualityreport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.16 09:37</date>
    /// </author>
    /// </summary>
    [Description("BY_SteelQualityreport")]
    [PrimaryKey("SteelQualityreportId")]
    public class BY_SteelQualityreport : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// SteelQualityreportId
        /// </summary>
        /// <returns></returns>
        [DisplayName("SteelQualityreportId")]
        public string SteelQualityreportId { get; set; }
        /// <summary>
        /// SteelQualityreportCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("SteelQualityreportCode")]
        public string SteelQualityreportCode { get; set; }
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
        /// 炉号
        /// </summary>
        /// <returns></returns>
        [DisplayName("炉号")]
        public string StoveCode { get; set; }
        /// <summary>
        /// 炉号
        /// </summary>
        /// <returns></returns>
        [DisplayName("炉号")]
        public string ParentStoveCode { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        /// <returns></returns>
        [DisplayName("产地")]
        public string PlaceOrigin { get; set; }
        /// <summary>
        /// Standard
        /// </summary>
        /// <returns></returns>
        [DisplayName("Standard")]
        public string Standard { get; set; }
        /// <summary>
        /// ForgingMethod
        /// </summary>
        /// <returns></returns>
        [DisplayName("ForgingMethod")]
        public string ForgingMethod { get; set; }
        /// <summary>
        /// DeliveryStatus
        /// </summary>
        /// <returns></returns>
        [DisplayName("DeliveryStatus")]
        public string DeliveryStatus { get; set; }
        /// <summary>
        /// ProcessingeUse
        /// </summary>
        /// <returns></returns>
        [DisplayName("ProcessingeUse")]
        public string ProcessingeUse { get; set; }
        /// <summary>
        /// Size
        /// </summary>
        /// <returns></returns>
        [DisplayName("Size")]
        public string Size { get; set; }
        /// <summary>
        /// Qty
        /// </summary>
        /// <returns></returns>
        [DisplayName("Qty")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// Weight
        /// </summary>
        /// <returns></returns>
        [DisplayName("Weight")]
        public decimal? Weight { get; set; }
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
        /// C
        /// </summary>
        /// <returns></returns>
        [DisplayName("C")]
        public string C { get; set; }
        /// <summary>
        /// Mn
        /// </summary>
        /// <returns></returns>
        [DisplayName("Mn")]
        public string Mn { get; set; }
        /// <summary>
        /// Si
        /// </summary>
        /// <returns></returns>
        [DisplayName("Si")]
        public string Si { get; set; }
        /// <summary>
        /// S
        /// </summary>
        /// <returns></returns>
        [DisplayName("S")]
        public string S { get; set; }
        /// <summary>
        /// P
        /// </summary>
        /// <returns></returns>
        [DisplayName("P")]
        public string P { get; set; }
        /// <summary>
        /// Ni
        /// </summary>
        /// <returns></returns>
        [DisplayName("Ni")]
        public string Ni { get; set; }
        /// <summary>
        /// Cr
        /// </summary>
        /// <returns></returns>
        [DisplayName("Cr")]
        public string Cr { get; set; }
        /// <summary>
        /// W
        /// </summary>
        /// <returns></returns>
        [DisplayName("W")]
        public string W { get; set; }
        /// <summary>
        /// V
        /// </summary>
        /// <returns></returns>
        [DisplayName("V")]
        public string V { get; set; }
        /// <summary>
        /// Mo
        /// </summary>
        /// <returns></returns>
        [DisplayName("Mo")]
        public string Mo { get; set; }
        /// <summary>
        /// Cu
        /// </summary>
        /// <returns></returns>
        [DisplayName("Cu")]
        public string Cu { get; set; }
        /// <summary>
        /// DZ1
        /// </summary>
        /// <returns></returns>
        [DisplayName("DZ1")]
        public string DZ1 { get; set; }
        /// <summary>
        /// DZ2
        /// </summary>
        /// <returns></returns>
        [DisplayName("DZ2")]
        public string DZ2 { get; set; }
        /// <summary>
        /// QHJ1
        /// </summary>
        /// <returns></returns>
        [DisplayName("QHJ1")]
        public string QHJ1 { get; set; }
        /// <summary>
        /// QHJ2
        /// </summary>
        /// <returns></returns>
        [DisplayName("QHJ2")]
        public string QHJ2 { get; set; }
        /// <summary>
        /// CJG1
        /// </summary>
        /// <returns></returns>
        [DisplayName("CJG1")]
        public string CJG1 { get; set; }
        /// <summary>
        /// CJG2
        /// </summary>
        /// <returns></returns>
        [DisplayName("CJG2")]
        public string CJG2 { get; set; }
        /// <summary>
        /// Hardness1
        /// </summary>
        /// <returns></returns>
        [DisplayName("Hardness1")]
        public string Hardness1 { get; set; }
        /// <summary>
        /// Hardness2
        /// </summary>
        /// <returns></returns>
        [DisplayName("Hardness2")]
        public string Hardness2 { get; set; }
        /// <summary>
        /// CSBruslt
        /// </summary>
        /// <returns></returns>
        [DisplayName("CSBruslt")]
        public string CSBruslt { get; set; }




        public string    TA { get; set; }
        public string HA  { get; set; }
        public string TB { get; set; }
        public string HB { get; set; }
        public string TC { get; set; }
        public string HC { get; set; }
        public string TD { get; set; }
        public string HD { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SteelQualityreportId = CommonHelper.GetGuid;
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
            this.SteelQualityreportId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
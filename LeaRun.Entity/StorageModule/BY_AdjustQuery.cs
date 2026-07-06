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
    /// BY_AdjustQuery
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.03.27 18:57</date>
    /// </author>
    /// </summary>
    [Description("BY_AdjustQuery")]
    [PrimaryKey("InventoryId")]
    public class BY_AdjustQuery : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 库存ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("库存ID")]
        public string InventoryId { get; set; }
        /// <summary>
        /// ParentInventoryId
        /// </summary>
        /// <returns></returns>
        [DisplayName("ParentInventoryId")]
        public string ParentInventoryId { get; set; }
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
        /// 钢种其它描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种其它描述")]
        public string SteelOtherDescription { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        /// <returns></returns>
        [DisplayName("产地")]
        public string PlaceOrigin { get; set; }
        /// <summary>
        /// 材质（0-条料，1-碎料）
        /// </summary>
        /// <returns></returns>
        [DisplayName("材质（0-条料，1-碎料）")]
        public int? MaterialType { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("类型")]
        public string SteelType { get; set; }
        /// <summary>
        /// 钢板状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢板状态")]
        public string SteelStatus { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        /// <returns></returns>
        [DisplayName("批次号")]
        public string BatchCode { get; set; }
        /// <summary>
        /// 子批次（厚*宽*长）
        /// </summary>
        /// <returns></returns>
        [DisplayName("子批次（厚*宽*长）")]
        public string ChildBatch { get; set; }
        /// <summary>
        /// 件数
        /// </summary>
        /// <returns></returns>
        [DisplayName("件数")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// 炉号
        /// </summary>
        /// <returns></returns>
        [DisplayName("炉号")]
        public string StoveCode { get; set; }
        /// <summary>
        /// 钢厂单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢厂单号")]
        public string FactoryCode { get; set; }
        /// <summary>
        /// 厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚度")]
        public decimal? Thick { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽")]
        public decimal? Weigh { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        /// <returns></returns>
        [DisplayName("长")]
        public decimal? Length { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("理论重量")]
        public decimal? TheoryWeight { get; set; }
        /// <summary>
        /// 实际重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("实际重量")]
        public decimal? Weight { get; set; }
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
        /// 进仓时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("进仓时间")]
        public DateTime? InStockDate { get; set; }
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
        /// 原始尺寸
        /// </summary>
        /// <returns></returns>
        [DisplayName("原始尺寸")]
        public string OriginalSize { get; set; }
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
        /// 总价
        /// </summary>
        /// <returns></returns>
        [DisplayName("总价")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 购买单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("购买单号")]
        public string SteelPurchaseCode { get; set; }
        /// <summary>
        /// 孔径
        /// </summary>
        /// <returns></returns>
        [DisplayName("孔径")]
        public decimal? BoreSize { get; set; }
        /// <summary>
        /// 购买单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否规格料")]
        public string TypeName { get; set; }
        /// <summary>
        /// 称重重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("称重重量")]
        public decimal? Weightcheng { get; set; }
        /// <summary>
        /// 密度
        /// </summary>
        /// <returns></returns>
        [DisplayName("密度")]
        public decimal? Density { get; set; }
        /// <summary>
        /// 厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚度")]
        public decimal? OThick { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽")]
        public decimal? OWeigh { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        /// <returns></returns>
        [DisplayName("长")]
        public decimal? OLength { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        /// <returns></returns>
        [DisplayName("原始重量")]
        public decimal? OWeight { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("单号")]
        public string QuotationCodeLine { get; set; }
        public string RemarkSteelperchase { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.InventoryId = CommonHelper.GetGuid;
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
            this.InventoryId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
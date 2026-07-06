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
    /// 出仓表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 22:36</date>
    /// </author>
    /// </summary>
    [Description("出仓表")]
    [PrimaryKey("OutStockId")]
    public class BY_OutStock : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 出仓单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("出仓单ID")]
        public string OutStockId { get; set; }
        /// <summary>
        /// 出仓编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("出仓编码")]
        public string BillCode { get; set; }
        /// <summary>
        /// 出仓类型（0-条料出仓，1-碎料出仓,3-调质转型出仓）
        /// </summary>
        /// <returns></returns>
        [DisplayName("出仓类型（0-条料出仓，1-订单碎料出仓,3-调质转型出仓）")]
        public string OutStockType { get; set; }
        /// <summary>
        /// 库存ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("库存ID")]
        public string StockId { get; set; }
        /// <summary>
        /// 订单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单明细ID")]
        public string OrderDetailId { get; set; }
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
        /// 件数
        /// </summary>
        /// <returns></returns>
        [DisplayName("件数")]
        public decimal? Qty { get; set; }
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
        /// 重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("重量")]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("理论重量")]
        public decimal? TheoryWeight { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 出仓时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("出仓时间")]
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
        /// 孔径
        /// </summary>
        /// <returns></returns>
        [DisplayName("孔径")]
        public decimal? BoreSize { get; set; }


        public string OrderCode { get; set; }
        public int? LineCode { get; set; }
        public int? OpenQty { get; set; }
        public decimal? Density { get; set; }

        public decimal? Olength { get; set; }
        public decimal? Oweigh { get; set; }
        public decimal? Othick { get; set; }
        public decimal? Oweight { get; set; }
        public decimal? Opensize { get; set; }
        public decimal? NowLength { get; set; }
        public decimal? NowWeight { get; set; }


        public string NowSize { get; set; }
        public decimal? OriginalWeight { get; set; }
        public string SteelPurchaseCode { get; set; }
        public string StoveCode { get; set; }
        public string BatchCode { get; set; }
        public string OrderDetailProcessDetailId { get; set; }
        public decimal? OpenThick { get; set; }

        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OutStockId = CommonHelper.GetGuid;
            //if (this.CreateDate == null)
            //{
                this.CreateDate = DateTime.Now;
                this.CreateUserId = ManageProvider.Provider.Current().UserId;
                this.CreateUserName = ManageProvider.Provider.Current().UserName;
            //}

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.OutStockId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
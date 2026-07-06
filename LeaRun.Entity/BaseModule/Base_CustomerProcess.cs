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
    /// 客户加工费折扣表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.15 10:55</date>
    /// </author>
    /// </summary>
    [Description("客户加工费折扣表")]
    [PrimaryKey("CustomerProcessId")]
    public class Base_CustomerProcess : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 加工费折扣主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费折扣主键")]
        public string CustomerProcessId { get; set; }
        /// <summary>
        /// 客户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户主键")]
        public string CustomerId { get; set; }
        
        /// <summary>
        /// 单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        /// <returns></returns>
        [DisplayName("单位")]
        public string Unit { get; set; }
        /// <summary>
        /// 厚 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 由")]
        public decimal? PachyStart { get; set; }
        /// <summary>
        /// 厚 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 至")]
        public decimal? PachyEnd { get; set; }
        /// <summary>
        /// 宽 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽 由")]
        public decimal? WidthStart { get; set; }
        /// <summary>
        /// 宽 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽 至")]
        public decimal? WidthEnd { get; set; }
        /// <summary>
        /// 长 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 由")]
        public decimal? LengthStart { get; set; }
        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 至")]
        public decimal? LengthEnd { get; set; }
        /// <summary>
        /// 有效时间 从
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效时间 从")]
        public DateTime? EnabledTimeStart { get; set; }
        /// <summary>
        /// 有效时间 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效时间 至")]
        public DateTime? EnabledTimeEnd { get; set; }
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
        /// 厚反向
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚反向")]
        public int? IsOutThick { get; set; }
        /// <summary>
        /// 宽反向
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽反向")]
        public int? IsOutWidth { get; set; }
        /// <summary>
        /// 长反向
        /// </summary>
        /// <returns></returns>
        [DisplayName("长反向")]
        public int? IsOutLength { get; set; }

        /// <summary>
        /// 圆钢
        /// </summary>
        /// <returns></returns>
        [DisplayName("圆钢")]
        public int? YuanSteel { get; set; }
        /// <summary>
        /// 毛料
        /// </summary>
        /// <returns></returns>
        [DisplayName("毛料")]
        public int? MaoSteel { get; set; }
        /// <summary>
        /// 光板
        /// </summary>
        /// <returns></returns>
        [DisplayName("光板")]
        public int? GuangSteel { get; set; }
        /// <summary>
        /// 精板
        /// </summary>
        /// <returns></returns>
        [DisplayName("精板")]
        public int? JingSteel { get; set; }

        /// <summary>
        /// 重量小于：Z，价格小于：J
        /// </summary>
        /// <returns></returns>
        [DisplayName("重量小于：Z，价格小于：J")]
        public string WhereQ { get; set; }
        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 至")]
        public decimal? WhereQty { get; set; }
        /// <summary>
        /// 气割
        /// </summary>
        /// <returns></returns>
        [DisplayName("气割")]
        public int? GasCuttingSteel { get; set; }
        /// <summary>
        /// 吊孔
        /// </summary>
        /// <returns></returns>
        [DisplayName("吊孔")]
        public int? LiftingHole { get; set; }
        /// <summary>
        /// 长反向
        /// </summary>
        /// <returns></returns>
        [DisplayName("长反向")]
        public int? OrOr { get; set; }
        /// <summary>
        /// 客来料
        /// </summary>
        /// <returns></returns>
        [DisplayName("客来料")]
        public int? CustMaterial { get; set; }

        /// <summary>
        /// 重量小于：Z，价格小于：J
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材类型")]
        public string SteelType { get; set; }
        /// <summary>
        /// 客来料
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨铣类型")]
        public int? MandXSteel { get; set; }
        /// <summary>
        /// 客来料
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨类型")]
        public int? MorXSteel { get; set; }

        /// <summary>
        /// 客来料
        /// </summary>
        /// <returns></returns>
        [DisplayName("洗边类型")]
        public int? BSteel { get; set; }

        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("KG从")]
        public decimal? KGfrom { get; set; }

        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("KG至")]
        public decimal? KGto { get; set; }

                /// <summary>
        /// 锯工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯工费")]
        public int? JKCost { get; set; }
        /// <summary>
        /// 板锯工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("板锯工费")]
        public int? JKCost1 { get; set; }
        /// <summary>
        /// 精洗边类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("精洗边类型")]
        public int? JBSteel { get; set; }

        /// <summary>
        /// 钢材主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材主键")]
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
        /// 长 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 由")]
        public decimal? QtyStart { get; set; }
        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 至")]
        public decimal? QtyEnd { get; set; }

        /// <summary>
        /// 切角
        /// </summary>
        /// <returns></returns>
        [DisplayName("切角")]
        public int? DDCost { get; set; }

        [DisplayName("长 由")]
        public decimal? ScaleFrom { get; set; }
        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 至")]
        public decimal? ScaleTo { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CustomerProcessId = CommonHelper.GetGuid;
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
            this.CustomerProcessId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
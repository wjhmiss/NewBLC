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
    /// 订单明细表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 21:41</date>
    /// </author>
    /// </summary>
    [Description("订单明细表")]
    [PrimaryKey("OrderDetailId")]
    public class BY_OrderDetail : BaseEntity
    {
        #region 获取/设置 字段值
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
        /// 订单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单号")]
        public string OrderCode { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        /// <returns></returns>
        [DisplayName("行号")]
        public int? LineCode { get; set; }
        /// <summary>
        /// 板件编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("板件编码")]
        public string BoardId { get; set; }
        /// <summary>
        /// 板件描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("板件描述")]
        public string BoardDescription { get; set; }
        public string ProjectCode { get; set; }

        

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
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
        /// 钢种状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种状态")]
        public string SteelState { get; set; }
        /// <summary>
        /// 模号
        /// </summary>
        /// <returns></returns>
        [DisplayName("模号")]
        public string DieCode { get; set; }
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
        /// 完工件数
        /// </summary>
        /// <returns></returns>
        [DisplayName("完工件数")]
        public decimal? CompleteQty { get; set; }
        /// <summary>
        /// 确认发运件数
        /// </summary>
        /// <returns></returns>
        [DisplayName("确认发运件数")]
        public decimal? PrintQty { get; set; }
        /// <summary>
        /// 执行备注信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("执行备注信息")]
        public string ExecRemark { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("单据状态")]
        public string BillStatus { get; set; }
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
        /// 磨四角
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨四角")]
        public int? IsGrindAngle { get; set; }
        /// <summary>
        /// 倒角
        /// </summary>
        /// <returns></returns>
        [DisplayName("倒角")]
        public int? IsChamfer { get; set; }
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
        public decimal? Weight { get; set; }

        /// <summary>
        /// 开料数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("开料数量")]
        public decimal? OpenQty { get; set; }
        /// <summary>
        /// 报价单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价单明细")]
        public string QuotationDetailId { get; set; }
        /// <summary>
        /// 磅重人
        /// </summary>
        /// <returns></returns>
        [DisplayName("磅重人")]
        public string ScaleUser { get; set; }
        /// <summary>
        /// 磅重时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("磅重时间")]
        public string ScaleDate { get; set; }
         /// <summary>
        /// 工序流程
        /// </summary>
        /// <returns></returns>
        [DisplayName("工序流程")]
        public string ScaleProcess { get; set; }
        /// <summary>
        /// 加工类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工类型")]
        public string OrderType { get; set; }
       

        /// <summary>
        /// 是否委外
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否委外")]
        public int? IsBeOut { get; set; }
        /// <summary>
        /// 委外完成时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("委外完成时间")]
        public DateTime? BeOutCompleteDate { get; set; }
        /// <summary>
        /// 委外验收人
        /// </summary>
        /// <returns></returns>
        [DisplayName("委外验收人")]
        public string BeOutCompletePerson { get; set; }

        /// <summary>
        /// 过磅重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("过磅重量")]
        public decimal? WeightFact { get; set; }
        /// <summary>
        /// 替代料
        /// </summary>
        /// <returns></returns>
        [DisplayName("替代料")]
        public string RepalceMl { get; set; }
        /// <summary>
        /// 送货说明
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货说明")]
        public string ShipDescription { get; set; }
        /// <summary>
        /// 客户货品号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户货品号")]
        public string CustomerProduct { get; set; }
        /// <summary>
        /// 品名（一般钢材名加加工类型）可修改
        /// </summary>
        /// <returns></returns>
        [DisplayName("品名（一般钢材名加加工类型）可修改")]
        public string ProductName { get; set; }
        /// <summary>
        /// 吊孔
        /// </summary>
        /// <returns></returns>
        [DisplayName("吊孔")]
        public int? LiftingHole { get; set; }
        /// <summary>
        /// 孔径
        /// </summary>
        /// <returns></returns>
        [DisplayName("孔径")]
        public decimal? BoreSize { get; set; }
        /// <summary>
        /// 钢材类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材类型")]
        public string SteelType { get; set; }

        /// <summary>
        /// 气割
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨铣")]
        public int? MandXSteel { get; set; }
        /// <summary>
        /// 气割
        /// </summary>
        /// <returns></returns>
        [DisplayName("磨")]
        public int? MorXSteel { get; set; }
        /// <summary>
        /// 气割
        /// </summary>
        /// <returns></returns>
        [DisplayName("洗边")]
        public int? BSteel { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("面积")]
        public decimal? MianCostArea { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("面积单价")]
        public decimal? MianCostPer { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("面加工总价")]
        public decimal? MianCost { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("边长")]
        public decimal? BianCostLong { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("边长单价")]
        public decimal? BianCostPer { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("边长成本")]
        public decimal? BianCost { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯工费")]
        public decimal? JKCost { get; set; }
        /// <summary>
        /// 理论重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("小件加收费")]
        public decimal? SmallCost { get; set; }
        /// <summary>
        /// 绩效费用
        /// </summary>
        /// <returns></returns>
        [DisplayName("绩效费用")]
        public decimal? WorkCostForWork { get; set; }
        /// <summary>
        /// 计价余量(厚度)
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量(厚度)")]
        public decimal? AddThick { get; set; }
        /// <summary>
        /// 计价余量(宽)
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量(宽)")]
        public decimal? AddWeigh { get; set; }
        /// <summary>
        /// 计价余量(长)
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量(长)")]
        public decimal? AddLength { get; set; }
        /// <summary>
        /// 精洗边类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("精洗边类型")]
        public int? JBSteel { get; set; }

        /// <summary>
        /// 按件计算
        /// </summary>
        /// <returns></returns>
        [DisplayName("按件计算")]
        public int? MianCostUnit { get; set; }

        /// <summary>
        /// 按件计算
        /// </summary>
        /// <returns></returns>
        [DisplayName("按件计算")]
        public int? BianCostUnit { get; set; }
        /// <summary>
        /// 按件计算
        /// </summary>
        /// <returns></returns>
        [DisplayName("按件计算")]
        public int? WorkUnit { get; set; }
        /// <summary>
        /// 铣平面
        /// </summary>
        /// <returns></returns>
        [DisplayName("铣平面")]
        public int? XorMSteel { get; set; }

        /// <summary>
        /// 面类型  2，两面，1，单面
        /// </summary>
        /// <returns></returns>
        [DisplayName("面类型  2，两面，1，单面")]
        public int? MianType { get; set; }
        /// <summary>
        /// 边类型，4，四边，3，长短，2，两长，1两短
        /// </summary>
        /// <returns></returns>
        [DisplayName("边类型，4，四边，3，长短，2，两长，1两短")]
        public int? BianType { get; set; }
        /// <summary>
        /// 计算公式
        /// </summary>
        /// <returns></returns>
        [DisplayName("计算公式")]
        public string CountShow { get; set; }
        /// <summary>
        /// 面加工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("面加工费")]
        public decimal? MianCostWork { get; set; }
        /// <summary>
        /// 边加工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("边加工费")]
        public decimal? BianCostWork { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// CNC
        /// </summary>
        /// <returns></returns>
        [DisplayName("铣平面")]
        public int? CNC { get; set; }

        /// <summary>
        /// 研磨
        /// </summary>
        /// <returns></returns>
        [DisplayName("铣平面")]
        public int? YM { get; set; }

        /// <summary>
        /// 锯切
        /// </summary>
        /// <returns></returns>
        [DisplayName("铣平面")]
        public int? JQ { get; set; }

        /// <summary>
        /// 客来料工序
        /// </summary>
        /// <returns></returns>
        [DisplayName("客来料工序")]
        public string Kprocess { get; set; }

        /// <summary>
        /// 锯工费单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯工费单价")]
        public decimal? JKCostPer { get; set; }

        /// <summary>
        /// 几刀
        /// </summary>
        /// <returns></returns>
        [DisplayName(" 几刀")]
        public int? JKCostQty { get; set; }
        /// <summary>
        /// 工艺
        /// </summary>
        /// <returns></returns>
        [DisplayName("工艺")]
        public string Technology { get; set; }

        /// <summary>
        /// 加工费总额
        /// </summary>
        /// <returns></returns>
        [DisplayName("边加工费")]
        public decimal? SmallAmount { get; set; }

        /// <summary>
        /// Fang
        /// </summary>
        /// <returns></returns>
        [DisplayName("Fang")]
        public string Fang { get; set; }
        /// <summary>
        /// DDCostPer
        /// </summary>
        /// <returns></returns>
        [DisplayName("DDCostPer")]
        public decimal? DDCostPer { get; set; }
        /// <summary>
        /// DDCost
        /// </summary>
        /// <returns></returns>
        [DisplayName("DDCost")]
        public decimal? DDCost { get; set; }
        /// <summary>
        /// DDCost
        /// </summary>
        /// <returns></returns>
        [DisplayName("DD")]
        public int? DD { get; set; }
        public string Brand { get; set; }

        public string BYDUnit { get; set; }
        public string BYDNum { get; set; }
        public string ProductName1 { get; set; }
        public int? Mheipi { get; set; }
        public string RemarkKL { get; set; }


        public string ScanCodeK { get; set; }
        public string ScanCodeX { get; set; }
        public string DingCode { get; set; }

        public decimal? WorkWeight { get; set; }
        public decimal? TheoryWeight { get; set; }

        public string Delivery { get; set; }


        public string KSize { get; set; }
        public string ZSize { get; set; }
        public string STSize { get; set; }
        public string Hardness { get; set; }
        public string NowStatus { get; set; }

        public string LJtype1 { get; set; }
        public string LJtype2 { get; set; }
        public string LJsize { get; set; }
        public int? listcount { get; set; }

        public string DJ { get; set; }

        public string KLRemak { get; set; }

        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OrderDetailId = CommonHelper.GetGuid;
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
            this.OrderDetailId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
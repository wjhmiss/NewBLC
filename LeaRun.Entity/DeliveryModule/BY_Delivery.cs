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
    /// ���˵�
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.24 16:29</date>
    /// </author>
    /// </summary>
    [Description("���˵�")]
    [PrimaryKey("DeliveryId")]
    public class BY_Delivery : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ���˺�
        /// </summary>
        /// <returns></returns>
        [DisplayName("���˺�")]
        public string DeliveryCode { get; set; }
        /// <summary>
        /// ���˵�ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("���˵�ID")]
        public string DeliveryId { get; set; }
        /// <summary>
        /// ���۵���
        /// </summary>
        /// <returns></returns>
        [DisplayName("���۵���")]
        public string QuotationCode { get; set; }
        /// <summary>
        /// ���˵���
        /// </summary>
        /// <returns></returns>
        [DisplayName("���˵���")]
        public string CheckSheetCode { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ID")]
        public string OrderId { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public string OrderCode { get; set; }
        /// <summary>
        /// �ͻ�PO
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ�PO")]
        public string CustomerPO { get; set; }
        /// <summary>
        /// �ͻ�ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ�ID")]
        public string CustomerId { get; set; }
        /// <summary>
        /// �ͻ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ����")]
        public string CustomerCode { get; set; }
        /// <summary>
        /// �ͻ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ�����")]
        public string CustomerName { get; set; }
        /// <summary>
        /// �ͻ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ����")]
        public string ShortName { get; set; }
        /// <summary>
        /// ��ϵ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ϵ��")]
        public string ContactName { get; set; }
        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ϵ�绰")]
        public string ContactTel { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public string ContactFax { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public string ContactMail { get; set; }
        /// <summary>
        /// �ֻ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ֻ�")]
        public string MobileCode { get; set; }
        /// <summary>
        /// �ͻ���ַ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ���ַ")]
        public string CustomerAddress { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string OrderType { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// �ܽ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ܽ��")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// �ۿ���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ۿ���")]
        public decimal? LessRate { get; set; }
        /// <summary>
        /// �ۿۺ��ܶ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ۿۺ��ܶ�")]
        public decimal? LessedAmount { get; set; }
        /// <summary>
        /// ���ʽ
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ʽ")]
        public string PaymentMode { get; set; }
        /// <summary>
        /// �ͻ���ʽ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ���ʽ")]
        public string ShipMode { get; set; }
        /// <summary>
        /// ������֯
        /// </summary>
        /// <returns></returns>
        [DisplayName("������֯")]
        public string ShipGroup { get; set; }
        /// <summary>
        /// ���۷�ʽ
        /// </summary>
        /// <returns></returns>
        [DisplayName("���۷�ʽ")]
        public string SalesMode { get; set; }
        /// <summary>
        /// ����Ա
        /// </summary>
        /// <returns></returns>
        [DisplayName("����Ա")]
        public string SalesMan { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string SalesArea { get; set; }
        /// <summary>
        /// �ͻ�·��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ�·��")]
        public string ShipRoute { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�Ƿ����")]
        public int? IsEmergency { get; set; }
        /// <summary>
        /// �յ���ַ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�յ���ַ")]
        public string CollectAddress { get; set; }
        /// <summary>
        /// �ͻ���ַ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ���ַ")]
        public string ShipAddress { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ע")]
        public string Remark { get; set; }
        /// <summary>
        /// ��ӡʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ӡʱ��")]
        public DateTime? PrintDate { get; set; }
        /// <summary>
        /// ��ӡ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ӡ��")]
        public string PrintUserId { get; set; }
        /// <summary>
        /// ����ӡʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ӡʱ��")]
        public DateTime? LastPrintDate { get; set; }
        /// <summary>
        /// ����ӡ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ӡ��")]
        public string LastPrintUserId { get; set; }
        /// <summary>
        /// ����˵��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����˵��")]
        public string ShipDescription { get; set; }
        /// <summary>
        /// ��ӡ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ӡ����")]
        public int? PrintType { get; set; }
        /// <summary>
        /// ��Ч
        /// </summary>
        /// <returns></returns>
        [DisplayName("��Ч")]
        public int? Enabled { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public int? SortCode { get; set; }
        /// <summary>
        /// ɾ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("ɾ�����")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ʱ��")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// �����û�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����û�����")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// �����û�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����û�")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�޸�ʱ��")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// �޸��û�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�޸��û�����")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// �޸��û�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�޸��û�")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string ShipClause { get; set; }
        /// <summary>
        /// �ֲ�����֤����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ֲ�����֤����")]
        public int? SteelQualityCert { get; set; }
        /// <summary>
        /// Ʒ��֤����
        /// </summary>
        /// <returns></returns>
        [DisplayName("Ʒ��֤����")]
        public int? QualityCert { get; set; }
        /// <summary>
        /// ԭ��֤����
        /// </summary>
        /// <returns></returns>
        [DisplayName("ԭ��֤����")]
        public int? FactoryCert { get; set; }
        /// <summary>
        /// �����Ƽ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����Ƽ�")]
        public int? ByPiece { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public int? CustMaterial { get; set; }
        /// <summary>
        /// ���ϵ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ϵ�")]
        public int? FreeBill { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public decimal? Weight { get; set; }
        /// <summary>
        /// �ѻ���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ѻ���")]
        public decimal? GetedAmount { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public string MnemonicCode { get; set; }

        /// <summary>
        /// �ص�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ص�ʱ��")]
        public DateTime? ComeBackDate { get; set; }
        /// <summary>
        /// �ص���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ص���")]
        public string ComeBackUserId { get; set; }
        /// <summary>
        /// ���ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ʱ��")]
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����")]
        public string AuditUserId { get; set; }

        /// <summary>
        /// ��Ʊ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��Ʊ")]
        public int? IsInvoice { get; set; }
        public string PrintGroup { get; set; }
        public decimal? TotalAmountall { get; set; }
        public decimal? Rowall { get; set; }


        public string DepartName { get; set; }
        public string KLRequery { get; set; }
        public string XMRequery { get; set; }
        public string LType { get; set; }

        public string BuildNum { get; set; }
        public string ProductCode { get; set; }
        public string BuildName { get; set; }
        public string DieName { get; set; }
        public string PicCode { get; set; }
        public string DieCode { get; set; }
        public string Driver { get; set; }
        public string DriverPath { get; set; }

        public string LJPlan { get; set; }
        public int? IsReturn { get; set; }
        public string CompanyName { get; set; }

        public string luhao { get; set; }
        public string HRC { get; set; }
        public string HV { get; set; }



        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.DeliveryId = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.DeliveryId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
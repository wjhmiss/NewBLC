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
    /// ��������
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 17:04</date>
    /// </author>
    /// </summary>
    [Description("��������")]
    [PrimaryKey("OrderId")]
    public class BY_Order : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ����ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ID")]
        public string OrderId { get; set; }
        /// <summary>
        /// ���۵�ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("���۵�ID")]
        public string QuotationId { get; set; }
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
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public string OrderCode { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        /// <returns></returns>
        [DisplayName("���")]
        public decimal? Amount { get; set; }
        /// <summary>
        /// �ͻ�PO
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ͻ�PO")]
        public string CustomerPO { get; set; }
        /// <summary>
        /// ����״̬
        /// </summary>
        /// <returns></returns>
        [DisplayName("����״̬")]
        public string OrderStatus { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        //[DisplayName("����")]
        public string Delivery { get; set; }
        /// <summary>
        /// �·���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�·���")]
        public string IssuedUserId { get; set; }
        /// <summary>
        /// �·�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�·�ʱ��")]
        public DateTime? IssuedDate { get; set; }
        /// <summary>
        /// ��˱�־
        /// </summary>
        /// <returns></returns>
        [DisplayName("��˱�־")]
        public int? CheckStatus { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����")]
        public string CheckUserId { get; set; }
        /// <summary>
        /// ���ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ʱ��")]
        public DateTime? CheckDate { get; set; }
        /// <summary>
        /// �䵥ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�䵥ʱ��")]
        public DateTime? PlaceOrderDate { get; set; }
        /// <summary>
        /// �䵥��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�䵥��")]
        public string PlaceOrderUserId { get; set; }
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
        /// ������Ա
        /// </summary>
        /// <returns></returns>
        [DisplayName("������Ա")]
        public string SalesMan { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public string MnemonicCode { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ע")]
        public string Remark { get; set; }


        /// <summary>
        /// ��Ʊ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��Ʊ")]
        public int? IsInvoice { get; set; }

        public string DepartName { get; set; }
        public string KLRequery { get; set; }
        public string XMRequery { get; set; }
        public string QuotationCode { get; set; }
        public string LType { get; set; }
        public string LJPlan { get; set; }
        public int? IsReturn { get; set; }
        public string CompanyName { get; set; }

        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.OrderId = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
            this.IssuedUserId = ManageProvider.Provider.Current().UserName;
            this.IssuedDate = DateTime.Now;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.OrderId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
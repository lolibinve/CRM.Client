using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace CRM.Model
{
    /// <summary>
    /// FBM 采购列表分页结果，对应 <c>fbmList</c> 的 data。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class FbmPurchaseListModel
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "list")]
        public List<FbmPurchaseRecordModel> List { get; set; }
    }

    /// <summary>
    /// FBM 采购记录。列表返回字段名为 <c>purchaseDate</c>、<c>buyerName</c>、<c>purchaseAccount</c> 等；
    /// 与 HttpClient 默认 JSON（camelCase）属性名一致，并辅以 <see cref="DataMemberAttribute"/>。
    /// 编辑提交 <c>fbmEdit</c> 仍使用 <see cref="PurchaseDateEdit"/>、<see cref="PurchaseAccount"/> 等映射到表单。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class FbmPurchaseRecordModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "orderId")]
        public string OrderId { get; set; }

        /// <summary>列表接口 <c>purchaseDate</c>，Unix 秒。</summary>
        [DataMember(Name = "purchaseDate")]
        public long PurchaseDate { get; set; }

        /// <summary>采购时间（界面 DatePicker）；与 <see cref="PurchaseDate"/> 互转。</summary>
        [IgnoreDataMember]
        public DateTime? PurchaseDateEdit
        {
            get =>
                PurchaseDate <= 0
                    ? (DateTime?)null
                    : DateTimeOffset.FromUnixTimeSeconds(PurchaseDate).LocalDateTime.Date;
            set =>
                PurchaseDate = value.HasValue
                    ? new DateTimeOffset(value.Value.Date).ToUnixTimeSeconds()
                    : 0;
        }

        [DataMember(Name = "expense")]
        public decimal Expense { get; set; }

        [DataMember(Name = "buyerName")]
        public string BuyerName { get; set; }

        [DataMember(Name = "purchaseAccount")]
        public string PurchaseAccount { get; set; }

        [DataMember(Name = "payment")]
        public int Payment { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        /// <summary>列表展示用。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(Payment))]
        public string PaymentDisplay
        {
            get
            {
                switch (Payment)
                {
                    case 0: return "现金支付";
                    case 1: return "账期";
                    case 2: return "诚意赊";
                    default: return Payment.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>列表勾选（仅 UI）。</summary>
        [DataMember(IsRequired = false)]
        public bool IsCheck { get; set; }
    }
}

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
    /// FBM 采购记录（现金采购），对应 <c>PurchaseRecordFbm</c> / <c>fbmEdit</c> 字段。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class FbmPurchaseRecordModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "orderId")]
        public string OrderId { get; set; }

        /// <summary>采购时间（界面编辑用）。</summary>
        [DataMember(Name = "addTime")]
        public DateTime? PurchaseDate { get; set; }

        [DataMember(Name = "expense")]
        public decimal Expense { get; set; }

        /// <summary>业务员，接口表单字段 <c>Uname</c>。</summary>
        [DataMember(Name = "uname")]
        public string BuyerName { get; set; }

        /// <summary>采购账号，接口 <c>accountName</c>。</summary>
        [DataMember(Name = "accountName")]
        public string AccountName { get; set; }

        /// <summary>支付方式：0 现金，1 账期，2 诚意赊（与后端约定一致时可调整）。</summary>
        [DataMember(Name = "payment")]
        public int Payment { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        /// <summary>列表展示用。</summary>
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
    }
}

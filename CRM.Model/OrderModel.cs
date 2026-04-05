using OfficeOpenXml;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Model
{

    [DataContract]
    public class OrderModel
    {
        [DataMember(Name = "list")]
        public List<OrderData> OrderDatalst { set; get; }

        [DataMember(Name = "count")]
        public int Count { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "totalSales")]
        public double TotalSales { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "totalCost")]
        public double TotalCost { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "totalTrans")]
        public double TotalTrans { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "totalProfit")]
        public double TotalProfit { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "profitRatio")]
        public double ProfitRatio { set; get; }

     
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "orderPrepare")]
        public int OrderPrepare { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "orderSend")]
        public int OrderSend { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "orderWrong")]
        public int OrderWrong { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "orderDone")]
        public int OrderDone { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "orderNew")]
        public int OrderNew { set; get; }


    }


    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class OrderData
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [DataMember(Name = "store")]
        public string Store { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [DataMember(Name = "date")]
        public DateTime? SaleDate { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember(Name = "orderId")]
        public string OrderNumber { get; set; }


        [DataMember(Name = "sku")]
        public string SKU { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        [DataMember(Name = "uname")]
        public string Account { get; set; }


        [DataMember(Name = "country")]
        public string Country { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [DataMember(Name = "moneyType")]
        public string MoneyType { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [DataMember(Name = "image")]
        public string ImageBase64Str { get; set; }


        /// <summary>
        /// 结算金额
        /// </summary>
        [DataMember(Name = "settleAmount")]
        public float SettleAmount { get; set; }

        /// <summary>
        /// 外汇汇率
        /// </summary>
        [DataMember(Name = "exchangeRafe")]
        public float ExchangeRafe { get; set; }

        /// <summary>
        /// 外汇金额
        /// </summary>
        [DataMember(Name = "exchangeAmount")]
        public float ExchangeAmount { get; set; }

        /// <summary>
        /// 回款汇率
        /// </summary>
        [DataMember(Name = "backExchange")]
        public float BackExchange { get; set; }

        /// <summary>
        /// 销售额（¥）
        /// </summary>
        [DataMember(Name = "salesVolume")]
        public float SalesVolume { get; set; }



        /// <summary>
        /// 成本（¥）
        /// </summary>
        [DataMember(Name = "cost")]
        public float Cost { get; set; }

        /// <summary>
        /// 运费（¥）
        /// </summary>
        [DataMember(Name = "transExpense")]
        public float TransExpense { get; set; }

        /// <summary>
        /// 退款（¥）
        /// </summary>
        [DataMember(Name = "backAmount")]
        public float BackAmount { get; set; }


       
        /// <summary>
        /// 利润（¥）
        /// </summary>
        [DataMember(Name = "profit")]
        public float Profit { get; set; }

        /// <summary>
        /// 利润率%
        /// </summary>
        [DataMember(Name = "profitRate")]
        public float ProfitRate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember(Name = "status")]
        public OrderState State { get; set; }


        [DataMember(Name = "isImport")]
        public int IsImport { set; get; }


        [DataMember(Name = "isImageUrl")]
        public int IsImageUrl { set; get; }


        /// <summary>
        /// 备注
        /// </summary>
        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [DataMember(Name = "buyerName")]
        public string Buyer { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "quantityPurchased")]
        public string QuantityPurchased { get; set; }

        /// <summary>采购方式，见 <see cref="OrderPurchaseMethod"/>（接口 <c>purchaseType</c>）。</summary>
        [DataMember(Name = "purchaseType")]
        public int PurchaseMethod { get; set; }

        /// <summary>备货/滞销采购批次（方式 2、3 时填写）。</summary>
        [DataMember(Name = "purchaseId")]
        public string PurId { get; set; }

        /// <summary>使用备货时的发货数量。</summary>
        [DataMember(Name = "sendAmount")]
        public int ShipQuantity { get; set; }

        [DataMemberAttribute(IsRequired = false)]
        public bool IsCheck { get; set; } = false;

  

        public void Clone(OrderData data)
        {
            this.Id = data.Id;
            this.Store = data.Store;
            this.OrderNumber = data.OrderNumber;
            this.Account= data.Account;
            this.SKU = data.SKU;
            this.Profit = data.Profit;
            this.ProfitRate = data.ProfitRate;
            this.SaleDate = data.SaleDate;
            this.BackAmount = data.BackAmount;
            this.BackExchange = data.BackExchange;
            this.Cost = data.Cost;
            this.Country = data.Country;
            this.ExchangeAmount = data.ExchangeAmount;
            this.Remark = data.Remark;
            this.State = data.State;
            this.ExchangeRafe = data.ExchangeRafe;
            this.MoneyType = data.MoneyType;
            this.SalesVolume = data.SalesVolume;
            this.SettleAmount = data.SettleAmount;
            this.TransExpense = data.TransExpense;
            this.IsCheck = data.IsCheck;
            this.ImageBase64Str = data.ImageBase64Str;
            this.Buyer = data.Buyer;
            this.Phone = data.Phone;
            this.QuantityPurchased = data.QuantityPurchased;
            this.PurchaseMethod = data.PurchaseMethod;
            this.PurId = data.PurId;
            this.ShipQuantity = data.ShipQuantity;

            this.IsImport = data.IsImport;
            this.IsImageUrl = data.IsImageUrl;
        }
    }

    /// <summary>订单采购方式（接口 <c>purchaseType</c> / 请求参数 <c>purchaseMethod</c>）。</summary>
    public enum OrderPurchaseMethod
    {
        /// <summary>未选择</summary>
        Unselected = 0,

        /// <summary>现金采购（FBM）</summary>
        Cash = 1,

        /// <summary>使用备货</summary>
        Stock = 2,

        /// <summary>滞留库存</summary>
        Deadstock = 3,

        /// <summary>退回重售（仅修改订单可选）</summary>
        ResellReturn = 4,
    }

    public enum OrderState
    {
        新单 = 0,

        发货 = 2,

        备货 = 1,

        妥投 = 3,

        退款 = 4,
    }
}

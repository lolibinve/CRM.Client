using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace CRM.Model
{
    /// <summary>
    /// 备货采购（备货流水）分页结果，对应 <c>stockList</c> 的 data。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class StockPurchaseListModel
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "list")]
        public List<StockPurchaseRecordModel> List { get; set; }
    }

    /// <summary>
    /// 备货流水记录；对接 <c>stockEdit</c> / <c>stockList</c> 列表行。
    /// 列表必填参数 <c>type</c> 与货件状态一致时常用：1 采购运输中，2 货件到仓（若与后端枚举不一致请改 <see cref="StockPurchaseConstants"/>）。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class StockPurchaseRecordModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>采购批次（如 LZBH-000001），一般由后端生成。</summary>
        [DataMember(Name = "purId")]
        public string PurId { get; set; }

        [DataMember(Name = "addTime")]
        public object AddTimeToken { get; set; }

        /// <summary>
        /// 采购日期
        /// </summary>
        [IgnoreDataMember]
        public DateTime? PurchaseDate { get; set; }


        [DataMember(Name = "pId")]
        public string ProductCode { get; set; }

        [DataMember(Name = "pName")]
        public string ProductName { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        /// <summary>采购金额（接口为整型，单位与后端一致，一般为元）。</summary>
        [DataMember(Name = "expense")]
        public int Expense { get; set; }

        [DataMember(Name = "unitValue")]
        public int UnitValue { get; set; }

        [DataMember(Name = "unitCost")]
        public int UnitCost { get; set; }

        [DataMember(Name = "payment")]
        public int Payment { get; set; }

        [DataMember(Name = "transFee")]
        public int TransFee { get; set; }

        [DataMember(Name = "unitTransFee")]
        public int UnitTransFee { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>货件状态：<see cref="StockPurchaseConstants.ShipmentInTransit"/> / <see cref="StockPurchaseConstants.ShipmentArrived"/>。</summary>
        [DataMember(Name = "type")]
        public int ShipmentType { get; set; }

        private DateTime? _instockDateTime;

        /// <summary>到仓日期（<see cref="DateTime"/>，界面绑定；提交接口时由 HttpClient 转为 <c>yyyy-MM-dd</c> 字符串）。</summary>
        [IgnoreDataMember]
        [AlsoNotifyFor(nameof(InstockTime))]
        public DateTime? InstockDateTime
        {
            get => _instockDateTime;
            set => _instockDateTime = value?.Date;
        }

        /// <summary>到仓时间：接口字段 <c>instockTime</c>，自然日字符串 <c>yyyy-MM-dd</c>；与 <see cref="InstockDateTime"/> 同步。</summary>
        [DataMember(Name = "instockTime")]
        [AlsoNotifyFor(nameof(InstockDateTime))]
        public string InstockTime
        {
            get => _instockDateTime.HasValue
                ? _instockDateTime.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _instockDateTime = null;
                    return;
                }

                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var a))
                    _instockDateTime = a.Date;
                else if (DateTime.TryParse(value, out var b))
                    _instockDateTime = b.Date;
                else
                    _instockDateTime = null;
            }
        }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [IgnoreDataMember]
        [DependsOn(nameof(Expense), nameof(Quantity))]
        public string UnitPurchaseCostDisplay =>
            Quantity > 0 ? (Expense / (double)Quantity).ToString("F2", CultureInfo.InvariantCulture) : "-";

        [IgnoreDataMember]
        [DependsOn(nameof(TransFee), nameof(Quantity))]
        public string UnitTransFeeDisplay =>
            Quantity > 0 ? (TransFee / (double)Quantity).ToString("F2", CultureInfo.InvariantCulture) : "-";

        [IgnoreDataMember]
        [DependsOn(nameof(Expense), nameof(Quantity), nameof(TransFee))]
        public string TotalUnitCostDisplay
        {
            get
            {
                if (Quantity <= 0)
                    return "-";
                var u1 = Expense / (double)Quantity;
                var u2 = TransFee / (double)Quantity;
                return (u1 + u2).ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        [IgnoreDataMember]
        [DependsOn(nameof(ShipmentType))]
        public string ShipmentTypeDisplay
        {
            get
            {
                if (ShipmentType == StockPurchaseConstants.StockListDeadstock)
                    return "滞销库存";
                if (ShipmentType == StockPurchaseConstants.ShipmentArrived)
                    return "货件到仓";
                return "采购运输中";
            }
        }

        [IgnoreDataMember]
        [DependsOn(nameof(Payment))]
        public string PaymentDisplay =>
            Payment == 2 ? "诚意赊" : (Payment == 1 ? "账期" : "现金支付");

        /// <summary>根据接口字段 <c>addTime</c>（<see cref="AddTimeToken"/>）填充界面用 <see cref="PurchaseDate"/>；列表/编辑打开时应调用。</summary>
        public void SyncPurchaseDateFromAddTime()
        {
            PurchaseDate = ParseAddTime(AddTimeToken);
        }

        /// <summary>保存前根据金额与数量写入 <c>unitValue</c> / <c>unitCost</c> / <c>unitTransFee</c>（均分，取整）。</summary>
        public void RecalculateUnitFieldsForSave()
        {
            if (Quantity <= 0)
            {
                UnitCost = 0;
                UnitTransFee = 0;
                UnitValue = 0;
                return;
            }

            UnitCost = (int)Math.Round(Expense / (double)Quantity);
            UnitTransFee = (int)Math.Round(TransFee / (double)Quantity);
            UnitValue = UnitCost;
        }

        private static DateTime? ParseAddTime(object token)
        {
            if (token == null)
                return null;

            if (token is DateTime dt)
                return dt.Date;

            if (token is long l)
            {
                if (l > 1_000_000_000_000L)
                    return DateTimeOffset.FromUnixTimeMilliseconds(l).LocalDateTime.Date;
                return DateTimeOffset.FromUnixTimeSeconds(l).LocalDateTime.Date;
            }

            if (token is int i)
                return DateTimeOffset.FromUnixTimeSeconds(i).LocalDateTime.Date;

            if (token is double d)
            {
                var n = (long)d;
                if (n > 1_000_000_000_000L)
                    return DateTimeOffset.FromUnixTimeMilliseconds(n).LocalDateTime.Date;
                return DateTimeOffset.FromUnixTimeSeconds(n).LocalDateTime.Date;
            }

            if (token is string s)
            {
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
                    return parsed.Date;
                if (DateTime.TryParse(s, out var parsed2))
                    return parsed2.Date;
            }

            return null;
        }
    }

    /// <summary>与后端约定的备货流水 <c>type</c> 及支付方式常量（若接口变更只改此处）。</summary>
    public static class StockPurchaseConstants
    {
        /// <summary><c>stockList</c> 筛选：模块2 采购运输库存（与行内货件「运输中」一致时常用值 1）。</summary>
        public const int StockListInTransit = 1;

        /// <summary><c>stockList</c> 筛选：模块3 到仓库库存。</summary>
        public const int StockListArrivedWarehouse = 2;

        /// <summary><c>stockList</c> 筛选：模块4 滞销库存（到仓满 60 天等由后端处理）。</summary>
        public const int StockListDeadstock = 3;

        /// <summary>行数据：货件状态「采购运输中」。</summary>
        public const int ShipmentInTransit = 1;

        /// <summary>行数据：货件状态「货件到仓」。</summary>
        public const int ShipmentArrived = 2;

        public const int PaymentCash = 0;
        public const int PaymentCredit = 2;
    }
}

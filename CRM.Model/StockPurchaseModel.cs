using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace CRM.Model
{
    /// <summary>
    /// 备货货件状态：与 <c>stockList</c> 筛选参数 <c>type</c>、行字段 <c>type</c>、<c>stockEdit</c> 提交一致（0～3）。
    /// </summary>
    public enum StockShipmentStatus
    {
        /// <summary>采购运输中 / 运输中</summary>
        InTransit = 0,

        /// <summary>货件到仓 / 到仓库存</summary>
        ArrivedWarehouse = 1,

        /// <summary>滞销库存</summary>
        Deadstock = 2,

        /// <summary>售罄库存</summary>
        SoldOut = 3,
    }

    /// <summary>
    /// 备货采购（备货流水）分页结果，对应 <c>stockList</c> 的 data。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class StockPurchaseListModel
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        /// <summary>采购运输库存条数（与界面「采购运输库存」角标一致）。</summary>
        [DataMember(Name = "intransCount")]
        public int IntransCount { get; set; }

        /// <summary>到仓库存条数。</summary>
        [DataMember(Name = "instockCount")]
        public int InstockCount { get; set; }

        /// <summary>滞销库存条数。</summary>
        [DataMember(Name = "unsaleableCount")]
        public int UnsaleableCount { get; set; }

        /// <summary>售罄库存条数。</summary>
        [DataMember(Name = "outsaleCount")]
        public int OutsaleCount { get; set; }

        [DataMember(Name = "list")]
        public List<StockPurchaseRecordModel> List { get; set; }
    }

    /// <summary><c>stockStalePurIdList</c> 返回数据：滞留剩余库存采购批次列表。</summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class StockStalePurIdListModel
    {
        [DataMember(Name = "list")]
        public List<string> List { get; set; }
    }

    /// <summary>
    /// 备货流水记录；对接 <c>stockEdit</c> / <c>stockList</c> 列表行。
    /// 货件状态统一：见 <see cref="StockShipmentStatus"/>。
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

        /// <summary>采购金额（接口为「元」；与后端约定为整数元或小数元均可反序列化）。</summary>
        [DataMember(Name = "expense")]
        public int Expense { get; set; }

        /// <summary>单件采购成本（仅采购金额均摊）；接口 <c>unitValue</c>，单位为 <b>元</b>（可含小数）。</summary>
        [DataMember(Name = "unitValue")]
        public decimal UnitValue { get; set; }

        /// <summary>单件合计成本（采购+头程均摊）；接口 <c>unitCost</c>，单位为 <b>元</b>。</summary>
        [DataMember(Name = "unitCost")]
        public decimal UnitCost { get; set; }

        /// <summary>剩余库存；<c>stockList</c> / <c>stockInfoByPurId</c> 的 <c>StockRecord</c> 字段（见 PurchaseController 文档 §7、§14）。</summary>
        [DataMember(Name = "stayQuantity")]
        public int StayQuantity { get; set; }

        [DataMember(Name = "payment")]
        public int Payment { get; set; }

        /// <summary>头程运费总额（接口整型，单位为「元」整数）。</summary>
        [DataMember(Name = "transFee")]
        public int TransFee { get; set; }

        /// <summary>单件头程运费；接口 <c>unitTransFee</c>，单位为 <b>元</b>。</summary>
        [DataMember(Name = "unitTransFee")]
        public decimal UnitTransFee { get; set; }

        /// <summary>单件头程运费；列表接口 <c>unitFee</c>，单位为 <b>元</b>。</summary>
        [DataMember(Name = "unitFee")]
        public decimal UnitFee { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>采购账号；列表/编辑与 <c>fbmEdit</c> 的 <c>purchaseAccount</c> 命名一致。</summary>
        [DataMember(Name = "purchaseAccount")]
        public string PurchaseAccount { get; set; }

        /// <summary>货件状态：<see cref="StockShipmentStatus"/>。</summary>
        [DataMember(Name = "type")]
        public int ShipmentType { get; set; }

        /// <summary>原货件状态，接口字段 <c>typeOld</c>。</summary>
        [DataMember(Name = "typeOld")]
        public int? ShipmentTypeOld { get; set; }

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

                // 接口定义：instockTime 仅返回时间戳（Unix 秒/毫秒）。
                if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unix))
                {
                    _instockDateTime = null;
                    return;
                }

                if (unix == 0)
                {
                    _instockDateTime = null;
                    return;
                }

                _instockDateTime = unix > 1_000_000_000_000L
                    ? DateTimeOffset.FromUnixTimeMilliseconds(unix).LocalDateTime.Date
                    : DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime.Date;
            }
        }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [DataMember(Name = "isDel")]
        public int IsDel { get; set; }

        /// <summary>编辑页：单件采购成本，与 <see cref="RecalculateUnitFieldsForSave"/> 中 <see cref="UnitValue"/> 一致（元）。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(Expense), nameof(Quantity))]
        public string UnitPurchaseCostDisplay =>
            Quantity > 0 ? FormatYuanDecimal(PerUnitYuanFromTotalYuan(Expense, Quantity)) : "-";

        /// <summary>编辑页：单件头程，与 <see cref="UnitTransFee"/> 一致。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(TransFee), nameof(Quantity))]
        public string UnitTransFeeDisplay =>
            Quantity > 0 ? FormatYuanDecimal(PerUnitYuanFromTotalYuan(TransFee, Quantity)) : "-";

        /// <summary>编辑页：单件合计成本，与 <see cref="UnitCost"/> 一致（采购+头程总额一次均分）。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(Expense), nameof(Quantity), nameof(TransFee))]
        public string TotalUnitCostDisplay =>
            Quantity > 0 ? FormatYuanDecimal(PerUnitTotalYuanFromPurchaseAndTrans(Expense, TransFee, Quantity)) : "-";

        /// <summary>列表页：单件采购成本，仅展示接口 <c>unitValue</c>（元）。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(UnitValue))]
        public string ListUnitPurchaseCostDisplay => FormatYuanDecimal(UnitValue);

        /// <summary>列表页：单件头程运费，展示接口 <c>unitTransFee</c>（<c>stockList</c> 行字段）。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(UnitTransFee))]
        public string ListUnitTransFeeDisplay => FormatYuanDecimal(UnitTransFee);

        /// <summary>列表页：合计成本(单件)，仅展示接口 <c>unitCost</c>。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(UnitCost))]
        public string ListTotalUnitCostDisplay => FormatYuanDecimal(UnitCost);

        /// <summary>列表：采购金额（元）两位小数，与接口「元」整型一致。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(Expense))]
        public string ExpenseYuanDisplay => FormatYuanInteger(Expense);

        /// <summary>列表：头程运费总额（元）两位小数。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(TransFee))]
        public string TransFeeYuanDisplay => FormatYuanInteger(TransFee);

        /// <summary>将「元」格式化为两位小数展示。</summary>
        private static string FormatYuanDecimal(decimal yuan) =>
            yuan.ToString("F2", CultureInfo.InvariantCulture);

        /// <summary>将「元」整型格式化为两位小数（总额类字段）。</summary>
        private static string FormatYuanInteger(int yuan) =>
            yuan.ToString("F2", CultureInfo.InvariantCulture);

        /// <summary>总「元」整数按数量摊成单件「元」，与 <see cref="RecalculateUnitFieldsForSave"/> 一致。</summary>
        private static decimal PerUnitYuanFromTotalYuan(int totalYuan, int quantity) =>
            quantity <= 0 ? 0m : Math.Round((decimal)totalYuan / quantity, 2, MidpointRounding.AwayFromZero);

        /// <summary>采购+头程总「元」按数量摊成单件合计「元」。</summary>
        private static decimal PerUnitTotalYuanFromPurchaseAndTrans(int expenseYuan, int transFeeYuan, int quantity) =>
            quantity <= 0 ? 0m : Math.Round(((decimal)expenseYuan + transFeeYuan) / quantity, 2, MidpointRounding.AwayFromZero);

        [IgnoreDataMember]
        [DependsOn(nameof(ShipmentType))]
        public string ShipmentTypeDisplay
        {
            get
            {
                if (!Enum.IsDefined(typeof(StockShipmentStatus), ShipmentType))
                    return ShipmentType.ToString(CultureInfo.InvariantCulture);
                switch ((StockShipmentStatus)ShipmentType)
                {
                    case StockShipmentStatus.InTransit:
                        return "采购运输中";
                    case StockShipmentStatus.ArrivedWarehouse:
                        return "货件到仓";
                    case StockShipmentStatus.Deadstock:
                        return "滞销库存";
                    case StockShipmentStatus.SoldOut:
                        return "售罄库存";
                    default:
                        return ShipmentType.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        [IgnoreDataMember]
        [DependsOn(nameof(Payment))]
        public string PaymentDisplay =>
            Payment == 0 ? "现金支付" : (Payment == 1 || Payment == 2) ? "诚意赊" : Payment.ToString(CultureInfo.InvariantCulture);

        /// <summary>根据接口字段 <c>addTime</c>（<see cref="AddTimeToken"/>）填充界面用 <see cref="PurchaseDate"/>；列表/编辑打开时应调用。</summary>
        public void SyncPurchaseDateFromAddTime()
        {
            PurchaseDate = ParseAddTime(AddTimeToken);
        }

        /// <summary>
        /// 保存前根据金额与数量写入 <c>unitValue</c> / <c>unitCost</c> / <c>unitTransFee</c> / <c>unitFee</c>。
        /// 单件字段均为 <b>元</b>（两位小数），与 <see cref="UnitPurchaseCostDisplay"/> 等展示及 <c>stockEdit</c> 传参一致。
        /// </summary>
        public void RecalculateUnitFieldsForSave()
        {
            if (Quantity <= 0)
            {
                UnitCost = 0m;
                UnitTransFee = 0m;
                UnitValue = 0m;
                UnitFee = 0m;
                return;
            }

            UnitValue = PerUnitYuanFromTotalYuan(Expense, Quantity);
            UnitTransFee = PerUnitYuanFromTotalYuan(TransFee, Quantity);
            UnitFee = UnitTransFee;
            UnitCost = PerUnitTotalYuanFromPurchaseAndTrans(Expense, TransFee, Quantity);
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

    /// <summary>与后端约定的备货流水支付方式（<c>stockEdit</c> 参数 <c>payment</c>；货件状态见 <see cref="StockShipmentStatus"/>）。</summary>
    public static class StockPurchaseConstants
    {
        /// <summary>现金支付</summary>
        public const int PaymentCash = 0;

        /// <summary>诚意赊（历史数据可能为 <c>2</c>，列表展示仍作诚意赊）</summary>
        public const int PaymentCredit = 1;
    }
}

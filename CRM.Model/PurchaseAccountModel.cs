using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace CRM.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountModel
    {
        [DataMember(Name = "list")]
        public List<ProcurementAccountLstModel> AccountLst { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }

    /// <summary>
    /// 采购账号列表行；后端字段 <c>addTime</c>、<c>name</c>、余额等通过 <see cref="DataMemberAttribute"/> 对应。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProcurementAccountLstModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>后端 <c>addTime</c>（Unix、字符串等）。</summary>
        [DataMember(Name = "addTime")]
        public object AddTimeToken { get; set; }

        /// <summary>界面日期；不参与 JSON 序列化，由 <see cref="AddTimeToken"/> 推导。</summary>
        [IgnoreDataMember]
        public DateTime? Date
        {
            get => ParseAddTime(AddTimeToken);
            set
            {
                if (value.HasValue)
                    AddTimeToken = value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                else
                    AddTimeToken = null;
            }
        }

        /// <summary>后端可能使用 <c>name</c> 或 <c>procurementAccount</c> 表示采购账号名称。</summary>
        [DataMember(Name = "name")]
        public string NameRaw { get; set; }

        [DataMember(Name = "procurementAccount")]
        public string ProcurementAccountRaw { get; set; }

        /// <summary>采购账号（界面绑定）；合并 <see cref="NameRaw"/> 与 <see cref="ProcurementAccountRaw"/>。</summary>
        [IgnoreDataMember]
        public string ProcurementAccount
        {
            get => NameRaw ?? ProcurementAccountRaw ?? "";
            set => NameRaw = value;
        }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 现金余额
        /// </summary>
        [DataMember(Name = "balanceCash")]
        public decimal? BalanceCash { get; set; }

        /// <summary>
        /// 赊欠余额
        /// </summary>
        [DataMember(Name = "balanceDebt")]
        public decimal? BalanceDebt { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsCheck { get; set; } = false;

        private static DateTime? ParseAddTime(object token)
        {
            if (token == null) return null;

            if (token is DateTime dt) return dt.Date;

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

    /// <summary><c>accountCheckInList</c> 分页结果。</summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountCheckInListModel
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "list")]
        public List<PurchaseAccountCheckInRecordModel> List { get; set; }

        /// <summary>现金存入合计（JSON 字段名：<c>sumCheckInCash</c>）。</summary>
        [DataMember(Name = "sumCheckInCash")]
        public decimal SumCheckInCash { get; set; }

        /// <summary>账期/诚意赊存入合计（JSON 字段名：<c>sumCheckInDebt</c>）。</summary>
        [DataMember(Name = "sumCheckInDebt")]
        public decimal SumCheckInDebt { get; set; }
    }

    /// <summary><c>purchase_account_check_in</c> 入账流水行。</summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountCheckInRecordModel
    {
        [DataMember(Name = "name")]
        public string AccountName { get; set; }

        [DataMember(Name = "amount")]
        public long Amount { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "addTime")]
        public object AddTimeToken { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [IgnoreDataMember]
        [DependsOn(nameof(Type))]
        public string TypeDisplay =>
            Type == 0 ? "现金" :
            Type == 1 ? "到期诚意赊" :
            Type.ToString(CultureInfo.InvariantCulture);

        [IgnoreDataMember]
        [DependsOn(nameof(AddTimeToken))]
        public string AddTimeDisplay => FormatCheckInTime(AddTimeToken);

        private static string FormatCheckInTime(object token)
        {
            var dt = ParseCheckInTime(token);
            return dt.HasValue
                ? dt.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                : "";
        }

        private static DateTime? ParseCheckInTime(object token)
        {
            if (token == null)
                return null;

            if (token is DateTime dt)
                return dt;

            if (token is long l)
            {
                if (l > 1_000_000_000_000L)
                    return DateTimeOffset.FromUnixTimeMilliseconds(l).LocalDateTime;
                return DateTimeOffset.FromUnixTimeSeconds(l).LocalDateTime;
            }

            if (token is int i)
                return DateTimeOffset.FromUnixTimeSeconds(i).LocalDateTime;

            if (token is double d)
            {
                var n = (long)d;
                if (n > 1_000_000_000_000L)
                    return DateTimeOffset.FromUnixTimeMilliseconds(n).LocalDateTime;
                return DateTimeOffset.FromUnixTimeSeconds(n).LocalDateTime;
            }

            if (token is string s)
            {
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var a))
                    return a;
                if (DateTime.TryParse(s, out var b))
                    return b;
            }

            return null;
        }
    }
}

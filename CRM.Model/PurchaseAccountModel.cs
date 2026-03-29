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
    /// 采购账号列表行；后端字段 <c>addTime</c>、<c>moneyIn</c>、<c>name</c>、<c>type</c> 等与前端属性通过 <see cref="DataMemberAttribute"/> 对应。
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

        [DataMember(Name = "moneyIn")]
        public decimal Amount { get; set; }

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

        /// <summary>资金类型，对应接口 <c>type</c>：<c>0</c> 现金存入，<c>1</c> 账期/诚意赊。</summary>
        [DataMember(Name = "type")]
        public int AccountType { get; set; }

        /// <summary>列表展示用资金类型文案（与 <see cref="AccountType"/> 同步）。</summary>
        [IgnoreDataMember]
        [DependsOn(nameof(AccountType))]
        public string FundTypeDisplay =>
            AccountType == 0 ? "现金存入" :
            AccountType == 1 ? "账期/诚意赊" :
            AccountType.ToString(CultureInfo.InvariantCulture);

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [DataMember(Name = "balanceCash")]
        public decimal? BalanceCash { get; set; }

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
}

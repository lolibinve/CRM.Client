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
    /// 采购账号列表行，对应后端 accountList 返回字段（JSON 映射在 CRM.HttpClient 中完成）。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProcurementAccountLstModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>原始 addTime（由接口层赋值：Unix、字符串等）</summary>
        public object AddTimeToken { get; set; }

        [DataMember(Name = "date")]
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

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "procurementAccount")]
        public string ProcurementAccount { get; set; }

        public object TypeRaw { get; set; }

        [DataMember(Name = "fundType")]
        public string FundType
        {
            get
            {
                if (TypeRaw == null) return "";
                if (TypeRaw is string s) return s;
                return TypeRaw.ToString();
            }
            set => TypeRaw = value;
        }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        public decimal? BalanceCash { get; set; }

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

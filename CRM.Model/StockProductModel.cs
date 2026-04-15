using PropertyChanged;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CRM.Model
{
    /// <summary>
    /// 备货汇总（产品库存）分页结果，对应 <c>stockManageList</c> 的 data。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class StockProductListModel
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        /// <summary>剩余库存总计，对应接口 <c>sum</c>。</summary>
        [DataMember(Name = "sum")]
        public decimal Sum { get; set; }

        [DataMember(Name = "list")]
        public List<StockProductRecordModel> List { get; set; }
    }

    /// <summary>
    /// 产品库存汇总行：可编辑项为产品编码、名称；库存类字段由后端计算。
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class StockProductRecordModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>产品编码，对应接口 <c>pId</c>。</summary>
        [DataMember(Name = "pId")]
        public string ProductCode { get; set; }

        /// <summary>产品名称，对应接口 <c>pName</c>。</summary>
        [DataMember(Name = "pName")]
        public string ProductName { get; set; }

        /// <summary>业务员，对应列表接口 <c>user</c>。</summary>
        [DataMember(Name = "user")]
        public string User { get; set; }

        /// <summary>采购运输中库存（自动计算展示）。</summary>
        [DataMember(Name = "inTrans")]
        public decimal? TransitStock { get; set; }

        /// <summary>到仓库存（自动计算展示）。</summary>
        [DataMember(Name = "inStock")]
        public decimal? WarehouseStock { get; set; }

        /// <summary>滞销库存（自动计算展示）。</summary>
        [DataMember(Name = "unsaleable")]
        public decimal? DeadStock { get; set; }

        /// <summary>售罄库存。</summary>
        [DataMember(Name = "sellOut")]
        public decimal? SellOut { get; set; }

        /// <summary>累计采购数量。</summary>
        [DataMember(Name = "pSum")]
        public decimal? SumCount { get; set; }

        /// <summary>剩余库存资金，对应列表接口 <c>sumExpense</c>。</summary>
        [DataMember(Name = "sumExpense")]
        public decimal? SumExpense { get; set; }

        /// <summary>
        /// 未售罄批次已售出数量（<c>staySum</c>）：<c>pSum - sellOut - inTrans - inStock - unsaleable</c>，由客户端根据接口字段计算，不参与序列化。
        /// </summary>
        [IgnoreDataMember]
        [DependsOn(nameof(SumCount), nameof(SellOut), nameof(TransitStock), nameof(WarehouseStock), nameof(DeadStock))]
        public decimal StaySum =>
            (SumCount ?? 0) - (SellOut ?? 0) - (TransitStock ?? 0) - (WarehouseStock ?? 0) - (DeadStock ?? 0);

        /// <summary>列表勾选（仅 UI，不参与接口序列化）。</summary>
        [DataMember(IsRequired = false)]
        public bool IsCheck { get; set; }

    }
}

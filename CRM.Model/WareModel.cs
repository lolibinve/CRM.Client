using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Model
{
    /// <summary>
    /// 产品模型
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WareModel
    {

        [DataMember(Name = "list")]
        public List<WareLstModel> Warelst { set; get; }

        [DataMember(Name = "count")]
        public int Count { set; get; }
    }

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class LanguageModel
    {
        //单语种下数据模型
        [DataMember(Name = "itemName")]
        public ObservableCollection<NameClass> ItemNames { get; set; } = new ObservableCollection<NameClass>();

        [DataMember(Name = "productDescription")]
        public string ProductDescription { get; set; }

        [DataMember(Name = "genericKeywords")]
        public string GenericKeywords { get; set; }

        [DataMember(Name = "bulletPoint1")]
        public string BulletPoint1 { get; set; }

        [DataMember(Name = "bulletPoint2")]
        public string BulletPoint2 { get; set; }

        [DataMember(Name = "bulletPoint3")]
        public string BulletPoint3 { get; set; }

        [DataMember(Name = "bulletPoint4")]
        public string BulletPoint4 { get; set; }

        [DataMember(Name = "bulletPoint5")]
        public string BulletPoint5 { get; set; }
    }

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class NameClass
    {
        [DataMember(Name = "name")]
        public string ItemDetailName { get; set; }
    }

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ChildModel
    {
        //变体数据模型
        [DataMember(Name = "childId")]
        public string ChildId { get; set; }

        [DataMember(Name = "colorName")]
        public string Color { get; set; }

        [DataMember(Name = "priceDiff")]
        public float PriceDiff { get; set; }
        
        [DataMember(Name = "sizeName")]
        public string Size { get; set; }

        [DataMember(Name = "partNumber")]
        public string PartNumber { get; set; }

        [DataMember(Name = "quantity")]
        public string Quantity { get; set; }
        
        [DataMember(Name = "externalProductId")]
        public string ExternalProductId { get; set; }

        [DataMember(Name = "mainImageUrl")]
        public string MainImageUrl { get; set; }

        [DataMember(Name = "otherImageUrl1")]
        public string OtherImageUrl1 { get; set; }

        [DataMember(Name = "otherImageUrl2")]
        public string OtherImageUrl2 { get; set; }

        [DataMember(Name = "otherImageUrl3")]
        public string OtherImageUrl3 { get; set; }

        [DataMember(Name = "otherImageUrl4")]
        public string OtherImageUrl4 { get; set; }

        [DataMember(Name = "otherImageUrl5")]
        public string OtherImageUrl5 { get; set; }

        [DataMember(Name = "otherImageUrl6")]
        public string OtherImageUrl6 { get; set; }

        [DataMember(Name = "otherImageUrl7")]
        public string OtherImageUrl7 { get; set; }


        [DataMember(Name = "standardPrice")]
        public ObservableCollection<PriceModel> StandardPrice { get; set; } = new ObservableCollection<PriceModel>();

        public void Clone(ChildModel data)
        {
            this.ChildId = data.ChildId;
            this.Color = data.Color;
            this.Size = data.Size;
            this.PartNumber = data.PartNumber;
            this.Quantity = data.Quantity;
            this.PriceDiff = data.PriceDiff;
            this.MainImageUrl = data.MainImageUrl;
            this.OtherImageUrl1 = data.OtherImageUrl1;
            this.OtherImageUrl2 = data.OtherImageUrl2;
            this.OtherImageUrl3 = data.OtherImageUrl3;
            this.OtherImageUrl4 = data.OtherImageUrl4;
            this.OtherImageUrl5 = data.OtherImageUrl5;
            this.OtherImageUrl6 = data.OtherImageUrl6;
            this.OtherImageUrl7 = data.OtherImageUrl7;
            this.ExternalProductId = data.ExternalProductId;
            //this.StandardPrice = data.StandardPrice;
        }
    }

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class PriceModel
    {
        //价格数据模型
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "exchange")]
        public float Exchange { get; set; }

        [DataMember(Name = "price")]
        public float Price { get; set; }
    }


    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class PriceLstModel
    {
        [DataMember(Name = "list")]
        public ObservableCollection<PriceModel> PriceLst { get; set; } = new ObservableCollection<PriceModel>();
    }

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WareData
    {
        //产品配置总模型
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "sku")]
        public string Sku { get; set; }

        //[DataMember(Name = "brandName")]
        //public string BrandName { get; set; }

        [DataMember(Name = "basePrice")]
        //public PriceLstModel BasePrice { get; set; }
        public ObservableCollection<PriceModel> BasePrice { get; set; } = new ObservableCollection<PriceModel>();


        [DataMember(Name = "colors")]
        public string Colors { get; set; }

        [DataMember(Name = "sizes")]
        public string Sizes { get; set; }

        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }

        //[DataMember(Name = "recommendedBrowseNodes")]
        //public string RecommendedBrowseNodes { get; set; }

        [DataMember(Name = "mainImageUrl")]
        public string MainImageUrl { get; set; }

        [DataMember(Name = "otherImageUrl")]
        public string OtherImageUrl { get; set; }

        [DataMember(Name = "feedProductType")]
        public string FeedProductType { get; set; }

        [DataMember(Name = "websiteShippingWeight")]
        public string WebsiteShippingWeight { get; set; }

        [DataMember(Name = "childInfo")]
        public ObservableCollection<ChildModel> Child { get; set; } = new ObservableCollection<ChildModel>();

        #region 语言国家

        [DataMember(Name = "english")]
        public LanguageModel English { get; set; }

        [DataMember(Name = "france")]
        public LanguageModel France { get; set; }

        [DataMember(Name = "german")]
        public LanguageModel German { get; set; }

        [DataMember(Name = "italy")]
        public LanguageModel Italy { get; set; }

        [DataMember(Name = "spain")]
        public LanguageModel Spain { get; set; }

        [DataMember(Name = "chinese")]
        public LanguageModel Chinese { get; set; }

        [DataMember(Name = "japan")]
        public LanguageModel Japan { get; set; }

        [DataMember(Name = "arabic")]
        public LanguageModel Arabic { get; set; }

        [DataMember(Name = "dutch")]
        public LanguageModel Dutch { get; set; }

        [DataMember(Name = "svenska")]
        public LanguageModel Svenska { get; set; }

        [DataMember(Name = "polski")]
        public LanguageModel Polski { get; set; }

        [DataMember(Name = "portuguese")]
        public LanguageModel Portuguese { get; set; }

        [DataMember(Name = "korean")]
        public LanguageModel Korean { get; set; }

        #endregion

        [DataMember(Name = "operator")]
        public string Operator { get; set; }

        [DataMemberAttribute(IsRequired = false)]
        public bool IsCheck { get; set; } = false;

        [DataMemberAttribute(IsRequired = false)]
        public int Admin { get; set; }
    }

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WareLstModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "sku")]
        public string Sku { get; set; }

        [DataMember(Name = "mainImageUrl")]
        public string MainImageUrl { get; set; }

        [DataMember(Name = "operator")]
        public string Operator { get; set; }

        [DataMember(Name = "date")]
        public DateTime? Date { get; set; }

        [DataMemberAttribute(IsRequired = false)]
        public bool IsCheck { get; set; } = false;

    }

}

using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CRM.Model
{
    [DataContract]
    public class ProductModel
    {
        [DataMember(Name = "list")]
        public List<ProductData> Productlst { set; get; }

        [DataMember(Name = "count")]
        public int Count { set; get; }
    }


    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProductData
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }


        [DataMember(Name = "sku")]
        public string Sku { get; set; }


        [DataMember(Name = "image")]
        public string ImageBase64Str { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [DataMember(Name = "operator")]
        public string Operator { get; set; }

        [DataMember(Name = "date")]
        public DateTime? Date { get; set; }


        [DataMemberAttribute(IsRequired = false)]
        public bool IsCheck { get; set; } = false;



        [DataMember(Name = "isImageUrl")]
        public int IsImageUrl { set; get; }


        [DataMember(Name = "imageUrl")]
        public string ImageUrl { get; set; }


        public void Clone(ProductData data)
        {
            this.Sku = data.Sku;
            this.ImageBase64Str = data.ImageBase64Str;
            this.Id = data.Id;
            this.IsCheck = data.IsCheck;
            this.Operator = data.Operator;
            this.Date = data.Date;
            this.Remark = data.Remark;
            this.IsImageUrl = data.IsImageUrl;
            this.ImageUrl = data.ImageUrl;
        }
    }

}

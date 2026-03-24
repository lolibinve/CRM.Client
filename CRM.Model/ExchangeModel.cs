using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CRM.Model
{
    [DataContract]
    public class ExchangeModel
    {
        [DataMember(Name = "list")]
        public List<ExchangeData> Exchangelst { set; get; }

        [DataMember(Name = "count")]
        public int Count { set; get; }
    }


    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ExchangeData
    {
        [DataMember(Name = "id")]
        public int Id { get; set; } = 0;


        [DataMember(Name = "country")]
        public string Country { get; set; }


        [DataMember(Name = "transRatio")]
        public string Ratio { get; set; }


        [DataMember(Name = "backRatio")]
        public string BackRatio { get; set; }

        [DataMember(Name = "initRatio")]
        public string InitRatio { get; set; }

        [DataMember(Name = "initBackRatio")]
        public string InitBackRatio { get; set; }

        [DataMember(Name = "midCurrency")]
        public string MidCurrency { get; set; }

        [DataMember(Name = "dv")]
        public string Dv { get; set; }

        [DataMember(Name = "isDvPercent")]
        public bool IsDvPercent { get; set; } = false;

        [DataMemberAttribute(IsRequired = false)]
        public bool IsCheck { get; set; } = false;

        public void Clone(ExchangeData data)
        {
            this.Id = data.Id;
            this.Country = data.Country;
            this.MidCurrency = data.MidCurrency;
            this.InitRatio = data.InitRatio;
            this.InitBackRatio = data.InitBackRatio;
            this.IsDvPercent = data.IsDvPercent;
            this.Dv = data.Dv;
            this.Ratio = data.Ratio;
            this.BackRatio = data.BackRatio;
            this.IsCheck = data.IsCheck;
        }
    }
}

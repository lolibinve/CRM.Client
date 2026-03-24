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
    public class RoleModel
    {
        [DataMember(Name = "list")]
        public List<RoleData> Orderlst { set; get; }

        [DataMember(Name = "count")]
        public int Count { set; get; }
    }


    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class RoleData
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }


        [DataMember(Name = "name")]
        public string Name { get; set; }


        [DataMember(Name = "psw")]
        public string PassWord { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [DataMember(Name = "admin")]
        public int Admin { get; set; }


        [DataMember(Name = "skuTag")]
        public string SkuTag { get; set; }


        [DataMemberAttribute(IsRequired = false)]
        public bool IsCheck { get; set; } = false;

        public void Clone(RoleData data)
        {
            this.Id = data.Id;
            this.Name = data.Name;
            this.PassWord = data.PassWord;
            this.Admin = data.Admin;
            this.SkuTag = data.SkuTag;
            this.IsCheck = data.IsCheck;
        }
    }

}

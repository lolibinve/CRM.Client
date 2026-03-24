using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Model
{
    [DataContract]
    public class OrderLstModel
    {
        /// <summary>
        /// 日期
        /// </summary>
        [DataMember(Name = "date")]
        public DateTime? SaleDate { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember(Name = "orderId")]
        public string OrderNumber { get; set; }


        [DataMember(Name = "sku")]
        public string SKU { get; set; }


        [DataMember(Name = "uname")]
        public string Account { get; set; }


        [DataMember(Name = "country")]
        public string Country { get; set; }

    }
}

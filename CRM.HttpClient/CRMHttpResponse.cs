using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace HttpLib
{
    /// <summary>
    /// Acmp响应信息
    /// </summary>
    [DataContract]
    public class CRMHttpResponse
    {
      
        [DataMember(Name = "Code")]
        public int State { get; set; }

   
        [DataMember(Name = "Message")]
        public string Desc { get; set; }
    }


    /// <summary>
    /// Acmp响应信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class CRMHttpResponse<T> : CRMHttpResponse
    {
 
        [DataMember(Name = "Data")]
        public T Value { get; set; }
    }
}

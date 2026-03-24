using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpLib
{
    public class JsonHelper
    {
        static JsonHelper()
        {
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                JsonSerializerSettings setting = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                return setting;
            });
        }

        /// <summary>
        /// 格式化JSON字符串
        /// </summary>
        /// <param name="json"></param>
        /// <param name="indented">使用换行和缩进</param>
        /// <returns></returns>
        public static string FormattingJsonString(string json, bool indented = true)
        {
            object obj = JsonConvert.DeserializeObject(json);
            string formatString = JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
            return formatString;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indented">使用换行和缩进</param>
        /// <returns></returns>
        public static string SerializeObject(object obj, bool indented = false)
        {
            return SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatting">换行和缩进格式</param>
        /// <returns></returns>
        public static string SerializeObject(object obj, Formatting formatting)
        {
            string json = JsonConvert.SerializeObject(obj, formatting);
            return json;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json)
        {
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="propertyName">节点名称</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json, string propertyName)
        {
            return DeserializeObject<T>(GetSubString(json, propertyName));
        }

        /// <summary>
        /// 反序列化为匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="anonymousTypeObject"></param>
        /// <returns></returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }

        /// <summary>
        /// 获取子串
        /// </summary>
        /// <param name="json"></param>
        /// <param name="propertyName">节点名称</param>
        /// <returns></returns>
        public static string GetSubString(string json, string propertyName)
        {
            return JObject.Parse(json).GetValue(propertyName).ToString();
        }
    }
}

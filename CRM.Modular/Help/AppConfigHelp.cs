using CLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Modular.Help
{
    public class AppConfigHelp
    {
        #region 获取配置操作

        /// <summary>
        /// 读取字符串类型
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string TryGetStringValue(string keyName, string defaultValue)
        {
            string keyValue = ConfigurationManager.AppSettings.AllKeys.Contains(keyName) ? ConfigurationManager.AppSettings[keyName] : defaultValue;
            return keyValue;
        }

        /// <summary>
        /// 尝试以字符串方式读取配置项
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetStringValue(string keyName, out string value)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(keyName))
            {
                value = ConfigurationManager.AppSettings[keyName];
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// 读取整数类型
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int TryGetIntValue(string keyName, int defaultValue, int minValue, int maxValue)
        {
            int keyValue;
            if (ConfigurationManager.AppSettings.AllKeys.Contains(keyName))
            {
                if (int.TryParse(ConfigurationManager.AppSettings[keyName], out keyValue))
                {
                    if (keyValue < minValue)
                    {
                        keyValue = minValue;
                    }
                    else if (keyValue > maxValue)
                    {
                        keyValue = maxValue;
                    }
                }
                else
                {
                    keyValue = defaultValue;
                }
            }
            else
            {
                keyValue = defaultValue;
            }
            return keyValue;
        }

        /// <summary>
        /// 读取布尔值类型
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool TryGetBooleanValue(string keyName, bool defaultValue)
        {
            bool keyValue;
            if (ConfigurationManager.AppSettings.AllKeys.Contains(keyName))
            {
                if (!bool.TryParse(ConfigurationManager.AppSettings[keyName], out keyValue))
                {
                    keyValue = defaultValue;
                }
            }
            else
            {
                keyValue = defaultValue;
            }
            return keyValue;
        }

        #endregion

        #region 写入配置文件

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Save(string key, object value)
        {
            try
            {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings[key].Value = value.ToString();
                cfa.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                TLog.Error(ex, $"配置文件保存失败：{key}-{value}");
                throw ex;
            }
        }

        #endregion
    }
}

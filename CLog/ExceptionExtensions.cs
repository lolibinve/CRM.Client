using System;
using System.Collections.Generic;
using System.Text;

namespace CLog
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 获取自上至下的异常传递列表
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IEnumerable<Exception> GetRouteExceptions(this Exception exception)
        {
            List<Exception> routeExceptions = new List<Exception>();

            Exception innerException = exception;
            do
            {
                routeExceptions.Add(innerException);
                yield return innerException;
            } while ((innerException = innerException.InnerException) != null);
        }

        /// <summary>
        /// 获取最底层异常提示信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetInnerMessage(this Exception exception)
        {
            Exception innerException = exception;

            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }

            return innerException.Message;
        }
    }
}

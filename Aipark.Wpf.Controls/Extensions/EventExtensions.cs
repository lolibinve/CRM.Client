using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace T.Wpf.Controls.Extensions
{
    public static class EventExtensions
    {
        public static T Invoke<T>(this DispatcherObject dispatcherObject, Func<T> func)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                return func();
            }
            else
            {
                return dispatcherObject.Dispatcher.Invoke(func);
            }
        }

        public static void Invoke(this DispatcherObject dispatcherObject, Action invokeAction)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                invokeAction();
            }
            else
            {
                dispatcherObject.Dispatcher.Invoke(invokeAction);
            }
        }

        /// <summary> 
        ///   Executes the specified action asynchronously with the DispatcherPriority.Background on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object where the action runs.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        /// <param name="priority">The dispatcher priority.</param> 
        public static void BeginInvoke(this DispatcherObject dispatcherObject, Action invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher.BeginInvoke(priority, invokeAction);
        }

        public static void BeginInvoke<T>(this T dispatcherObject, Action<T> invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
            where T : DispatcherObject
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher?.BeginInvoke(priority, new Action(() => invokeAction(dispatcherObject)));
        }

        /// <summary> 
        ///   Executes the specified action if the element is loaded or at the loaded event if it's not loaded.
        /// </summary>
        /// <param name="element">The element where the action should be run.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        public static void ExecuteWhenLoaded(this FrameworkElement element, Action invokeAction)
        {
            if (element.IsLoaded)
            {
                element.Invoke(invokeAction);
            }
            else
            {
                void ElementLoaded(object o, RoutedEventArgs a)
                {
                    element.Loaded -= ElementLoaded;
                    element.Invoke(invokeAction);
                }

                element.Loaded += ElementLoaded;
            }
        }

        /// <summary>
        /// 查找控件的对应的事件注册的方法名
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="eventName">事件</param>
        /// <returns>返回事件注册的方法名，若未找到则返回null</returns>
        public static string GetBindingMethod(this FrameworkElement control)
        {
            if (control == null)
                return null;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
            EventInfo[] eventInfos = control.GetType().GetEvents(bindingFlags);

            foreach (var eventInfo in eventInfos)
            {
                FieldInfo fieldInfo = control.GetType().GetField(eventInfo.Name, bindingFlags);
                if (fieldInfo != null)
                {

                }
            }


            //    PropertyInfo propertyInfo = control.GetType().GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
            //if (propertyInfo == null) { return null; }

            //EventHandlerList eventList = (EventHandlerList)propertyInfo.GetValue(control, null);
            //FieldInfo fieldInfo = typeof(object).GetField("Event" + "", BindingFlags.Static | BindingFlags.NonPublic);
            //if (fieldInfo == null) { return null; }

            //Delegate delegateInfo = eventList[fieldInfo.GetValue(control)];
            //if (delegateInfo == null) { return null; }
            //Delegate[] delegateList = delegateInfo.GetInvocationList();

            //return delegateList[delegateList.Length - 1].Method.Name;


            return null;
        }

        /// <summary>
        /// 清除一个对象的某个事件所挂钩的delegate
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="eventName">事件名称，默认的</param>
        public static void ClearEvents(this FrameworkElement control, string eventName = "_EventAll")
        {
            if (control == null)
                return;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
            EventInfo[] events = control.GetType().GetEvents(bindingFlags);
            if (events == null || events.Length < 1) return;

            for (int i = 0; i < events.Length; i++)
            {
                try
                {
                    EventInfo ei = events[i];
                    //只删除指定的方法，默认是_EventAll，前面加_是为了和系统的区分，防以后雷同
                    if (eventName != "_EventAll" && ei.Name != eventName) continue;

                    /********************************************************
                     * class的每个event都对应了一个同名(变了，前面加了Event前缀)的private的delegate类
                     * 型成员变量（这点可以用Reflector证实）。因为private成
                     * 员变量无法在基类中进行修改，所以为了能够拿到base 
                     * class中声明的事件，要从EventInfo的DeclaringType来获取
                     * event对应的成员变量的FieldInfo并进行修改
                     ********************************************************/
                    FieldInfo fi = ei.DeclaringType.GetField("Event_" + ei.Name, bindingFlags);
                    if (fi != null)
                    {
                        // 将event对应的字段设置成null即可清除所有挂钩在该event上的delegate
                        fi.SetValue(control, null);
                    }
                }
                catch { }
            }
        }
    }
}

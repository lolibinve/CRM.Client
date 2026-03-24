using System;
using System.Collections.Generic;
using System.Windows;

namespace Aipark.Wpf.Controls
{
    public abstract class MutexWindow : Window
    {
        private static readonly Dictionary<string, MutexWindow> _cache = new Dictionary<string, MutexWindow>();

        public MutexWindow()
        {
            this.Loaded += MutexWindow_Loaded;
            this.Closed += MutexWindow_Closed;
        }

        private void MutexWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.Mutex))
            {
                if (_cache.ContainsKey(this.Mutex))
                {
                    if (_cache[this.Mutex] != null)
                    {
                        _cache[this.Mutex].Close();
                    }
                    _cache[this.Mutex] = this;
                }
                else
                {
                    _cache.Add(this.Mutex, this);
                }
            }
        }

        private void MutexWindow_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.Mutex))
            {
                if (_cache.TryGetValue(this.Mutex, out var window) && window == this)
                {
                    _cache.Remove(this.Mutex);
                }
            }
        }

        /// <summary>
        /// 窗口互斥信号
        /// </summary>
        public string Mutex
        {
            get { return (string)GetValue(MutexProperty); }
            set { SetValue(MutexProperty, value); }
        }
        public static readonly DependencyProperty MutexProperty =
            DependencyProperty.Register("Mutex", typeof(string), typeof(MutexWindow), new FrameworkPropertyMetadata(string.Empty));
    }
}

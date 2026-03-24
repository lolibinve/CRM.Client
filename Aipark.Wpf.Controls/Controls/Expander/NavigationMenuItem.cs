using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Aipark.Wpf.Controls
{
    public class NavigationMenuItem : INotifyPropertyChanged
    {
        public NavigationMenuItem()
        {
            IconWidth = 16;
            IconHeight = 16;
            NameMargin = new Thickness(6, 0, 0, 0);
        }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 图标宽度
        /// </summary>
        public int IconWidth { get; set; }

        /// <summary>
        /// 图标高度
        /// </summary>
        public int IconHeight { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文字图标的间距
        /// </summary>
        public Thickness NameMargin { get; set; }

        /// <summary>
        /// 实例类型
        /// </summary>
        public Type InstanceType { get; set; }

        private bool isChecked;
        /// <summary>
        /// 选中
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                this.RefrshProperty();
            }
        }

        /// <summary>
        /// 下级子项
        /// </summary>
        public ObservableCollection<NavigationMenuItem> Children { get; set; }


        /// <summary>
        /// 属性改变通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性刷新
        /// </summary>
        /// <param name="name">发生改变的属性名称</param>
        public virtual void RefrshProperty([CallerMemberName] string name = "")
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }


}

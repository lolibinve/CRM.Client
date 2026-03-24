using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// DropdownComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class DropdownComboBox : UserControl
    {
        /// <summary>
        /// 选中项事件
        /// </summary>
        public event Action<DropdownComboBox, IEnumerable> SelectionChanged;

        public DropdownComboBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            itemsPopup.IsOpen = true;
        }


        /// <summary>
        /// 水印文本
        /// </summary>
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(DropdownComboBox), new PropertyMetadata(""));


        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DropdownComboBox), new PropertyMetadata(null));


        /// <summary>
        /// 绑定对象时属性名称
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(DropdownComboBox), new PropertyMetadata(""));


        /// <summary>
        /// 下拉框展示位置
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(DropdownComboBox), new PropertyMetadata(PlacementMode.Bottom));


        /// <summary>
        /// 选中模式
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(DropdownComboBox), new PropertyMetadata(SelectionMode.Single));


        /// <summary>
        /// 选中项变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<object> list = new List<object>();
            if (ItemsSource != null && listBox.SelectedItems != null)
            {
                foreach (var item in ItemsSource)
                {
                    if (listBox.SelectedItems.Contains(item))
                    {
                        list.Add(item);
                    }
                }

                if (string.IsNullOrWhiteSpace(DisplayMemberPath))
                {
                    Text = string.Join(",", list);
                }
                else
                {
                    Text = string.Join(",", list.Select(x => x.GetType().GetProperty(DisplayMemberPath).GetValue(x)));
                }
            }
            else
            {
                Text = "";
            }

            this.SelectionChanged?.Invoke(this, list);
        }

        /// <summary>
        /// 选中项文字展示
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DropdownComboBox), new PropertyMetadata(""));

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// DropdownTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class DropdownTextBox : UserControl
    {
        /// <summary>
        /// 文本变更事件
        /// </summary>
        public event Action<DropdownTextBox, string> TextChanged;
        /// <summary>
        /// 选中项改变事件
        /// </summary>
        public event Action<DropdownTextBox, object> SelectionChanged;

        public DropdownTextBox()
        {
            InitializeComponent();
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
            DependencyProperty.Register("Watermark", typeof(string), typeof(DropdownTextBox), new PropertyMetadata(""));


        /// <summary>
        /// 下拉框弹出位置,仅支持Bottom和Right
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(DropdownTextBox), new PropertyMetadata(PlacementMode.Bottom, OnPlacementPropertyChanged));
        public static void OnPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropdownTextBox sender)
            {
                if (e.NewValue is PlacementMode placement)
                {
                    sender.itemsPopup.Placement = placement;
                    if (placement == PlacementMode.Bottom)
                    {
                        sender.itemsPopup.HorizontalOffset = 0;
                        sender.itemsPopup.VerticalOffset = 1;
                    }
                    else if (placement == PlacementMode.Right)
                    {
                        sender.itemsPopup.HorizontalOffset = 2;
                        sender.itemsPopup.VerticalOffset = 0;
                    }
                }
            }
        }


        /// <summary>
        /// 下拉框宽度
        /// </summary>
        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }
        public static readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register("PopupWidth", typeof(double), typeof(DropdownTextBox), new PropertyMetadata(120.0, OnPlacementPropertyChanged));
        public static void OnPopupWidthPropertyyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropdownTextBox sender)
            {
                if (e.NewValue is double width)
                {
                    sender.itemsBorder.Width = width;
                }
            }
        }


        /// <summary>
        /// 下拉框高度
        /// </summary>
        public double PopupHeight
        {
            get { return (double)GetValue(PopupHeightProperty); }
            set { SetValue(PopupHeightProperty, value); }
        }
        public static readonly DependencyProperty PopupHeightProperty =
            DependencyProperty.Register("PopupHeight", typeof(double), typeof(DropdownTextBox), new PropertyMetadata(160.0, OnPopupHeightPropertyChanged));
        public static void OnPopupHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropdownTextBox sender)
            {
                if (e.NewValue is double height)
                {
                    sender.itemsBorder.Height = height;
                }
            }
        }


        /// <summary>
        /// 可选项数据集合
        /// </summary>
        public ICollection ItemsSource
        {
            get { return (ICollection)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ICollection), typeof(DropdownTextBox), new PropertyMetadata(new List<string>() { }, OnItemsSourcePropertyChanged));
        public static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropdownTextBox sender)
            {
                if (e.NewValue is ICollection items)
                {
                    sender.itemsControl.ItemsSource = items;
                    sender.scrollViewer.ScrollToHome();
                }
            }
        }


        /// <summary>
        /// 当前选中的项
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(DropdownTextBox), new PropertyMetadata(null, OnSelectedItemPropertyChanged));
        public static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropdownTextBox sender)
            {
                string text = string.Empty;
                if (e.NewValue is object item)
                {
                    if (string.IsNullOrWhiteSpace(sender.DisplayMemberPath))
                    {
                        text = item.ToString();
                    }
                    else
                    {
                        try
                        {
                            object obj = item.GetType().GetProperty(sender.DisplayMemberPath).GetValue(item);
                            text = Convert.ToString(obj);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    //
                    sender.selectedTbx.TextChanged -= sender.SelectedTbx_TextChanged;
                    sender.selectedTbx.Text = text;
                    sender.selectedTbx.TextChanged += sender.SelectedTbx_TextChanged;
                }
            }
        }

        /// <summary>
        /// 当前文本框文字
        /// </summary>
        public string Text => this.selectedTbx.Text;

        /// <summary>
        /// 启用筛选
        /// </summary>
        public bool KeywordFiltering
        {
            get { return (bool)GetValue(KeywordFilteringProperty); }
            set { SetValue(KeywordFilteringProperty, value); }
        }
        public static readonly DependencyProperty KeywordFilteringProperty =
            DependencyProperty.Register("KeywordFiltering", typeof(bool), typeof(DropdownTextBox), new PropertyMetadata(true));

        /// <summary>
        /// 绑定对象时属性名称
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(DropdownTextBox), new PropertyMetadata(""));


        public void Reset()
        {
            this.SelectedItem = null;
            this.selectedTbx.Text = "";
        }


        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            itemsPopup.IsOpen = true;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.DataContext is object obj)
            {
                this.SelectedItem = obj;
                SelectionChanged?.Invoke(this, obj);
            }
            this.itemsPopup.IsOpen = false;
        }

        private void SelectedTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            string text = this.selectedTbx.Text;
            this.itemsPopup.IsOpen = true;
            this.SelectedItem = null;

            if (KeywordFiltering && ItemsSource != null)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    this.itemsControl.ItemsSource = this.ItemsSource;
                }
                else
                {
                    List<object> list = new List<object>();
                    foreach (var item in ItemsSource)
                    {
                        if (string.IsNullOrWhiteSpace(this.DisplayMemberPath))
                        {
                            if (item.ToString().Contains(text))
                            {
                                list.Add(item);
                            }
                        }
                        else
                        {
                            try
                            {
                                object obj = item.GetType().GetProperty(this.DisplayMemberPath).GetValue(item);
                                if (obj.ToString().Contains(text))
                                {
                                    list.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                    this.itemsControl.ItemsSource = list;
                }
            }

            TextChanged?.Invoke(this, text);
        }
    }
}

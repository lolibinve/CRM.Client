using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 文本超链接控件
    /// </summary>
    [TemplatePart(Name = TextBlockName, Type = typeof(TextBlock))]
    public class TextHyperlink : Control
    {
        private const string TextBlockName = "TemplatePart_TextBlock";

        public event Action<TextHyperlink, string> Click;

        /// <summary>
        /// 提示文字
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextHyperlink), new PropertyMetadata("超链接"));


        /// <summary>
        /// 鼠标悬浮时文字颜色
        /// </summary>
        public Brush HoverForeground
        {
            get { return (Brush)GetValue(HoverForegroundProperty); }
            set { SetValue(HoverForegroundProperty, value); }
        }
        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(TextHyperlink), new PropertyMetadata(new SolidColorBrush(Colors.DarkGreen)));



        static TextHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextHyperlink), new FrameworkPropertyMetadata(typeof(TextHyperlink)));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextBlock textBlock = GetTemplateChild<TextBlock>(TextBlockName);
            (textBlock.Inlines.FirstInline as Hyperlink).Click += TextHyperlink_Click;
        }

        private void TextHyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.Click?.Invoke(this, Text);
        }

        private T GetTemplateChild<T>(string childName) where T : FrameworkElement, new()
        {
            return (GetTemplateChild(childName) as T) ?? new T();
        }
    }
}

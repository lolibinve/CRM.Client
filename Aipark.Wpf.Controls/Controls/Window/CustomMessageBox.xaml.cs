using System;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// CustomMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class CustomMessageBox
    {
        private MessageBoxResult Result = MessageBoxResult.None;

        private CustomMessageBox()
        {
            InitializeComponent();

            this.MaxWidth = SystemParameters.WorkArea.Width * 0.8;
            this.MaxHeight = SystemParameters.WorkArea.Height;
        }

        /// <summary>
        /// 显示具有消息、 标题栏标题和按钮，则一个消息框它返回的结果。
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            CustomMessageBox messageBox = new CustomMessageBox();
            messageBox.msgTextBlock.Text = messageBoxText;
            messageBox.Title = caption;

            switch (button)
            {
                case MessageBoxButton.OK:
                    messageBox.YesButton.Visibility = Visibility.Collapsed;
                    messageBox.NoButton.Visibility = Visibility.Collapsed;
                    messageBox.CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    messageBox.YesButton.Visibility = Visibility.Collapsed;
                    messageBox.NoButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    messageBox.OkButton.Visibility = Visibility.Collapsed;
                    messageBox.CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageBox.OkButton.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            Size desiredSize = CalculateDesiredSize(messageBoxText);
            messageBox.Width = desiredSize.Width + 50;
            messageBox.Height = desiredSize.Height + 82;

            messageBox.ShowDialog();

            return messageBox.Result;
        }
        /// <summary>
        /// 显示具有消息和标题栏标题; 一个消息框它返回的结果。
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            CustomMessageBox messageBox = new CustomMessageBox();
            messageBox.msgTextBlock.Text = messageBoxText;
            messageBox.Title = caption;

            messageBox.YesButton.Visibility = Visibility.Collapsed;
            messageBox.NoButton.Visibility = Visibility.Collapsed;
            messageBox.CancelButton.Visibility = Visibility.Collapsed;

            Size desiredSize = CalculateDesiredSize(messageBoxText);
            messageBox.Width = desiredSize.Width + 50;
            messageBox.Height = desiredSize.Height + 82;

            messageBox.ShowDialog();

            return messageBox.Result;
        }
        /// <summary>
        /// 显示一个消息框具有一条消息，并且它返回的结果。
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(string messageBoxText)
        {
            CustomMessageBox messageBox = new CustomMessageBox();
            messageBox.msgTextBlock.Text = messageBoxText;

            messageBox.YesButton.Visibility = Visibility.Collapsed;
            messageBox.NoButton.Visibility = Visibility.Collapsed;
            messageBox.CancelButton.Visibility = Visibility.Collapsed;

            Size desiredSize = CalculateDesiredSize(messageBoxText);
            messageBox.Width = desiredSize.Width + 50;
            messageBox.Height = desiredSize.Height + 82;

            messageBox.ShowDialog();

            return messageBox.Result;
        }


        private static Size CalculateDesiredSize(string messageBoxText)
        {
            TextBlock txtBlock = new TextBlock { Text = messageBoxText, FontSize = 13 };
            txtBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double width = Math.Max(txtBlock.DesiredSize.Width + 32, 200);
            double height = Math.Max(txtBlock.DesiredSize.Height + 32, 68);

            return new Size(width, height);
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.Close();
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            this.Close();
        }
    }
}

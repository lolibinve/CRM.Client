using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// SuperTimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class SuperTimePicker : UserControl
    {
        public SuperTimePicker()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.hourRadionButton.MinWidth = this.hourRadionButton.ActualWidth;
            this.minuteRadionButton.MinWidth = this.minuteRadionButton.ActualWidth;
            this.secondRadionButton.MinWidth = this.secondRadionButton.ActualWidth;
        }

        /// <summary>
        /// 选中的时间
        /// </summary>
        public DateTime? SelectedTime
        {
            get
            {
                if (int.TryParse(this.Hour, out int hour) && int.TryParse(this.Minute, out int minute) && int.TryParse(this.Second, out int second))
                {
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second);
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    string[] timeFields = value.Value.ToString("HH:mm:ss").Split(':');
                    this.Hour = timeFields[0];
                    this.Minute = timeFields[1];
                    this.Second = timeFields[2];
                }
                else
                {
                    this.Hour = "00";
                    this.Minute = "00";
                    this.Second = "00";
                }
            }
        }

        /// <summary>
        /// 标题按钮文本大小
        /// </summary>
        public double TimeFontSize
        {
            get { return (double)GetValue(TimeFontSizeProperty); }
            set { SetValue(TimeFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TimeFontSizeProperty =
            DependencyProperty.Register(nameof(TimeFontSize), typeof(double), typeof(SuperTimePicker), new PropertyMetadata(14.0));

        /// <summary>
        /// 光标焦点
        /// </summary>
        public CursorPosition CursorPosition
        {
            get { return (CursorPosition)GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }
        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register(nameof(CursorPosition), typeof(CursorPosition), typeof(SuperTimePicker), new PropertyMetadata(CursorPosition.Default, OnCursorPositionPropertyChanged));
        public static void OnCursorPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuperTimePicker dateTimePicker && e.NewValue is CursorPosition cursorPosition)
            {
                switch (cursorPosition)
                {
                    case CursorPosition.Hour:
                        dateTimePicker.hourRadionButton.IsChecked = true;
                        break;
                    case CursorPosition.Minute:
                        dateTimePicker.minuteRadionButton.IsChecked = true;
                        break;
                    case CursorPosition.Second:
                        dateTimePicker.secondRadionButton.IsChecked = true;
                        break;
                    default:
                        dateTimePicker.hourRadionButton.IsChecked = false;
                        dateTimePicker.minuteRadionButton.IsChecked = false;
                        dateTimePicker.secondRadionButton.IsChecked = false;
                        break;
                }
            }
        }

        /// <summary>
        /// 时
        /// </summary>
        public string Hour
        {
            get { return (string)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register(nameof(Hour), typeof(string), typeof(SuperTimePicker), new PropertyMetadata("00", OnHourPropertyChanged));
        public static void OnHourPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuperTimePicker dateTimePicker && e.NewValue is string hour)
            {
                dateTimePicker.hourRadionButton.Content = hour;
            }
        }

        /// <summary>
        /// 分
        /// </summary>
        public string Minute
        {
            get { return (string)GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }
        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register(nameof(Minute), typeof(string), typeof(SuperTimePicker), new PropertyMetadata("00", OnMinutePropertyChanged));
        public static void OnMinutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuperTimePicker dateTimePicker && e.NewValue is string minute)
            {
                dateTimePicker.minuteRadionButton.Content = minute;
            }
        }

        /// <summary>
        /// 秒
        /// </summary>
        public string Second
        {
            get { return (string)GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }
        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register(nameof(Second), typeof(string), typeof(SuperTimePicker), new PropertyMetadata("00", OnSecondPropertyChanged));
        public static void OnSecondPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuperTimePicker dateTimePicker && e.NewValue is string second)
            {
                dateTimePicker.secondRadionButton.Content = second;
            }
        }


        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.CursorPosition == CursorPosition.Default)
                return;

            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                case Key.D1:
                case Key.NumPad1:
                case Key.D2:
                case Key.NumPad2:
                case Key.D3:
                case Key.NumPad3:
                case Key.D4:
                case Key.NumPad4:
                case Key.D5:
                case Key.NumPad5:
                case Key.D6:
                case Key.NumPad6:
                case Key.D7:
                case Key.NumPad7:
                case Key.D8:
                case Key.NumPad8:
                case Key.D9:
                case Key.NumPad9:
                    {
                        var key = e.Key.ToString().Last();
                        FieldPadRight(key);
                        e.Handled = true;
                    }
                    break;

                case Key.Up:
                    FieldOffset(1);
                    e.Handled = true;
                    break;
                case Key.Down:
                    FieldOffset(-1);
                    e.Handled = true;
                    break;

                case Key.Tab:

                    break;

                case Key.Enter:
                    this.CursorPosition = CursorPosition.Default;
                    //this.TimeSelected?.Invoke(this, this.SelectedDateTime);
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 字段右侧追加字符
        /// </summary>
        /// <param name="character"></param>
        private void FieldPadRight(char character)
        {
            switch (this.CursorPosition)
            {
                case CursorPosition.Hour:
                    if (this.Hour.Length == 2)
                    {
                        this.Hour = character.ToString();
                    }
                    else
                    {
                        string hour = $"{this.Hour}{character}";
                        if (int.Parse(hour) > 23)
                        {
                            this.Hour = character.ToString();
                        }
                        else
                        {
                            this.Hour = hour;
                        }
                    }
                    break;

                case CursorPosition.Minute:
                    if (this.Minute.Length == 2)
                    {
                        this.Minute = character.ToString();
                    }
                    else
                    {
                        string minute = $"{this.Minute}{character}";
                        if (int.Parse(minute) > 59)
                        {
                            this.Minute = character.ToString();
                        }
                        else
                        {
                            this.Minute = minute;
                        }
                    }
                    break;

                case CursorPosition.Second:
                    if (this.Second.Length == 2)
                    {
                        this.Second = character.ToString();
                    }
                    else
                    {
                        string second = $"{this.Second}{character}";
                        if (int.Parse(second) > 59)
                        {
                            this.Second = character.ToString();
                        }
                        else
                        {
                            this.Second = second;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 字段值偏移
        /// </summary>
        /// <param name="offset"></param>
        private void FieldOffset(int offset)
        {
            switch (this.CursorPosition)
            {
                case CursorPosition.Hour:
                    int hour = int.Parse(this.Hour) + offset;
                    if (hour > 23)
                    {
                        this.Hour = "00";
                    }
                    else if (hour < 0)
                    {
                        this.Hour = "23";
                    }
                    else
                    {
                        this.Hour = hour.ToString().PadLeft(2, '0');
                    }
                    break;

                case CursorPosition.Minute:
                    int minute = int.Parse(this.Minute) + offset;
                    if (minute > 59)
                    {
                        this.Minute = "00";
                    }
                    else if (minute < 0)
                    {
                        this.Minute = "59";
                    }
                    else
                    {
                        this.Minute = minute.ToString().PadLeft(2, '0');
                    }
                    break;

                case CursorPosition.Second:
                    int second = int.Parse(this.Second) + offset;
                    if (second > 59)
                    {
                        this.Second = "00";
                    }
                    else if (second < 0)
                    {
                        this.Second = "59";
                    }
                    else
                    {
                        this.Second = second.ToString().PadLeft(2, '0');
                    }
                    break;

                default:
                    break;
            }
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.CursorPosition == CursorPosition.Default)
                return;

            if (e.Delta > 0)
            {
                FieldOffset(1);
            }
            else
            {
                FieldOffset(-1);
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CursorPosition = CursorPosition.Default;
        }

        private void DateTimeFieldRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && Enum.TryParse(radioButton.Tag.ToString(), out CursorPosition cursorPosition))
            {
                this.CursorPosition = cursorPosition;
            }
        }

        private void DateTimeFieldRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && Enum.TryParse(radioButton.Tag.ToString(), out CursorPosition cursorPosition))
            {
                switch (cursorPosition)
                {
                    case CursorPosition.Hour:
                        if (this.Hour.Length != 2)
                        {
                            this.Hour = this.Hour.PadLeft(2, '0');
                        }
                        break;

                    case CursorPosition.Minute:
                        if (this.Minute.Length != 2)
                        {
                            this.Minute = this.Minute.PadLeft(2, '0');
                        }
                        break;

                    case CursorPosition.Second:
                        if (this.Second.Length != 2)
                        {
                            this.Second = this.Second.PadLeft(2, '0');
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }
}

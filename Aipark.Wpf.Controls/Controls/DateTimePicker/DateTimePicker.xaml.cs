using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// DateTimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        /// <summary>
        /// 时间选定事件
        /// </summary>
        public event Action<object, DateTime?> OnDateTimeSelected;

        public DateTimePicker()
        {
            InitializeComponent();

            string[] dateFields = DateTime.Now.ToString("yyyy-MM-dd").Split('-');
            string[] timeFields = DateTime.Now.ToString("HH:mm:ss").Split(':');

            this.Year = dateFields[0];
            this.Month = dateFields[1];
            this.Day = dateFields[2];
            this.Hour = timeFields[0];
            this.Minute = timeFields[1];
            this.Second = timeFields[2];
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.yearRadionButton.MinWidth = this.yearRadionButton.ActualWidth;
            this.monthRadionButton.MinWidth = this.monthRadionButton.ActualWidth;
            this.dayRadionButton.MinWidth = this.dayRadionButton.ActualWidth;
            this.hourRadionButton.MinWidth = this.hourRadionButton.ActualWidth;
            this.minuteRadionButton.MinWidth = this.minuteRadionButton.ActualWidth;
            this.secondRadionButton.MinWidth = this.secondRadionButton.ActualWidth;
        }

        public DateTime? SelectedDateTime
        {
            get
            {
                if (int.TryParse(this.Year, out int year) && int.TryParse(this.Month, out int month) && int.TryParse(this.Day, out int day) && int.TryParse(this.Hour, out int hour) && int.TryParse(this.Minute, out int minute) && int.TryParse(this.Second, out int second))
                {
                    return new DateTime(year, month, day, hour, minute, second);
                }
                return null;
            }
            set
            {
                DateTime dateTime = value ?? DateTime.Now;

                string[] dateFields = dateTime.ToString("yyyy-MM-dd").Split('-');
                string[] timeFields = dateTime.ToString("HH:mm:ss").Split(':');

                this.Year = dateFields[0];
                this.Month = dateFields[1];
                this.Day = dateFields[2];
                this.Hour = timeFields[0];
                this.Minute = timeFields[1];
                this.Second = timeFields[2];
            }
        }

        /// <summary>
        /// 光标焦点
        /// </summary>
        public CursorPosition CursorPosition
        {
            get { return (CursorPosition)GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }
        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register("CursorPosition", typeof(CursorPosition), typeof(DateTimePicker), new PropertyMetadata(CursorPosition.Default, OnCursorPositionPropertyChanged));
        public static void OnCursorPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is CursorPosition cursorPosition)
            {
                switch (cursorPosition)
                {
                    case CursorPosition.Year:
                        dateTimePicker.yearRadionButton.IsChecked = true;
                        break;
                    case CursorPosition.Month:
                        dateTimePicker.monthRadionButton.IsChecked = true;
                        break;
                    case CursorPosition.Day:
                        dateTimePicker.dayRadionButton.IsChecked = true;
                        break;
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
                        dateTimePicker.yearRadionButton.IsChecked = false;
                        dateTimePicker.monthRadionButton.IsChecked = false;
                        dateTimePicker.dayRadionButton.IsChecked = false;
                        dateTimePicker.hourRadionButton.IsChecked = false;
                        dateTimePicker.minuteRadionButton.IsChecked = false;
                        dateTimePicker.secondRadionButton.IsChecked = false;
                        break;
                }
            }
        }

        /// <summary>
        /// 年
        /// </summary>
        public string Year
        {
            get { return (string)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }
        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnYearPropertyChanged));
        public static void OnYearPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is string year)
            {
                dateTimePicker.yearRadionButton.Content = year;
            }
        }

        /// <summary>
        /// 月
        /// </summary>
        public string Month
        {
            get { return (string)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }
        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register("Month", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnMonthPropertyChanged));
        public static void OnMonthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is string month)
            {
                dateTimePicker.monthRadionButton.Content = month;
            }
        }

        /// <summary>
        /// 日
        /// </summary>
        public string Day
        {
            get { return (string)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }
        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnDayPropertyChanged));
        public static void OnDayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is string day)
            {
                dateTimePicker.dayRadionButton.Content = day;
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
            DependencyProperty.Register("Hour", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnHourPropertyChanged));
        public static void OnHourPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is string hour)
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
            DependencyProperty.Register("Minute", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnMinutePropertyChanged));
        public static void OnMinutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is string minute)
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
            DependencyProperty.Register("Second", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnSecondPropertyChanged));
        public static void OnSecondPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker && e.NewValue is string second)
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

                case Key.Left:
                    FocusLeft();
                    e.Handled = true;
                    break;
                case Key.Right:
                    FocusRight();
                    e.Handled = true;
                    break;

                case Key.Tab:

                    break;

                case Key.Enter:
                    this.CursorPosition = CursorPosition.Default;
                    OnDateTimeSelected?.Invoke(this, this.SelectedDateTime);
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
                case CursorPosition.Year:
                    if (this.Year.Length == 4)
                    {
                        this.Year = character == '0' ? "2" : character.ToString();
                    }
                    else
                    {
                        this.Year = $"{this.Year}{character}";
                        CheckDayValue();
                    }
                    break;

                case CursorPosition.Month:
                    if (this.Month.Length == 2)
                    {
                        this.Month = character.ToString();
                    }
                    else
                    {
                        string month = $"{this.Month}{character}";
                        if (int.Parse(month) > 12)
                        {
                            this.Month = character.ToString();
                        }
                        else
                        {
                            this.Month = month;
                            CheckDayValue();
                        }
                    }
                    break;

                case CursorPosition.Day:
                    if (this.Day.Length == 2)
                    {
                        this.Day = character.ToString();
                    }
                    else
                    {
                        string day = $"{this.Day}{character}";
                        if (int.Parse(day) > DateTime.DaysInMonth(int.Parse(Year), int.Parse(Month)))
                        {
                            this.Day = character.ToString();
                        }
                        else
                        {
                            this.Day = day;
                        }
                    }
                    break;

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
                case CursorPosition.Year:
                    if (this.Year.Length != 4)
                    {
                        this.Year = this.Year.PadRight(4, '0');
                    }
                    int year = int.Parse(this.Year) + offset;
                    if (year > 9999 || year < 1000)
                    {
                        this.Year = DateTime.Now.ToString("yyyy-MM-dd").Split('-')[0];
                    }
                    else
                    {
                        this.Year = year.ToString();
                    }
                    CheckDayValue();
                    break;

                case CursorPosition.Month:
                    int month = int.Parse(this.Month) + offset;
                    if (month > 12)
                    {
                        this.Month = "01";
                    }
                    else if (month < 1)
                    {
                        this.Month = "12";
                    }
                    else
                    {
                        this.Month = month.ToString().PadLeft(2, '0');
                    }
                    CheckDayValue();
                    break;

                case CursorPosition.Day:
                    int day = int.Parse(this.Day) + offset;
                    int days = DateTime.DaysInMonth(int.Parse(Year), int.Parse(Month));
                    if (day > days)
                    {
                        this.Day = "01";
                    }
                    else if (day < 1)
                    {
                        this.Day = days.ToString();
                    }
                    else
                    {
                        this.Day = day.ToString().PadLeft(2, '0');
                    }
                    break;

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

        private void FocusLeft()
        {
            switch (this.CursorPosition)
            {
                case CursorPosition.Year:
                    this.CursorPosition = CursorPosition.Second;
                    break;
                case CursorPosition.Month:
                    this.CursorPosition = CursorPosition.Year;
                    break;
                case CursorPosition.Day:
                    this.CursorPosition = CursorPosition.Month;
                    break;
                case CursorPosition.Hour:
                    this.CursorPosition = CursorPosition.Day;
                    break;
                case CursorPosition.Minute:
                    this.CursorPosition = CursorPosition.Hour;
                    break;
                case CursorPosition.Second:
                    this.CursorPosition = CursorPosition.Minute;
                    break;
                default:
                    break;
            }
        }
        private void FocusRight()
        {
            switch (this.CursorPosition)
            {
                case CursorPosition.Year:
                    this.CursorPosition = CursorPosition.Month;
                    break;
                case CursorPosition.Month:
                    this.CursorPosition = CursorPosition.Day;
                    break;
                case CursorPosition.Day:
                    this.CursorPosition = CursorPosition.Hour;
                    break;
                case CursorPosition.Hour:
                    this.CursorPosition = CursorPosition.Minute;
                    break;
                case CursorPosition.Minute:
                    this.CursorPosition = CursorPosition.Second;
                    break;
                case CursorPosition.Second:
                    this.CursorPosition = CursorPosition.Year;
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
                    case CursorPosition.Year:
                        if (this.Year.Length != 4)
                        {
                            this.Year = DateTime.Now.ToString("yyyy-MM-dd").Split('-')[0];
                            CheckDayValue();
                        }
                        break;
                    case CursorPosition.Month:
                        if (this.Month.Length != 2)
                        {
                            this.Month = this.Month.Equals("0") ? "01" : this.Month.PadLeft(2, '0');
                            CheckDayValue();
                        }
                        break;
                    case CursorPosition.Day:
                        if (this.Day.Length != 2)
                        {
                            this.Day = this.Day.Equals("0") ? "01" : this.Day.PadLeft(2, '0');
                        }
                        break;
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

        /// <summary>
        /// 限定日期的最大值
        /// </summary>
        private void CheckDayValue()
        {
            if (this.Year.Length == 4 && this.Month.Length == 2)
            {
                int days = DateTime.DaysInMonth(int.Parse(Year), int.Parse(Month));
                if (int.Parse(Day) > days)
                {
                    Day = days.ToString();
                }
            }
        }

    }


    /// <summary>
    /// 操作焦点位置
    /// </summary>
    public enum CursorPosition
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,

        Year,

        Month,

        Day,

        Hour,

        Minute,

        Second
    }


}

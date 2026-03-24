using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// SuperDatePicker.xaml 的交互逻辑
    /// </summary>
    public partial class SuperDatePicker : UserControl
    {
        public SuperDatePicker()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.SelectedDate.HasValue)
            {
                this.VisualState = SuperCalendarVisualState.日;
                this.Year = this.SelectedDate.Value.Year;
                this.Month = this.SelectedDate.Value.Month;
                this.Decade = this.SelectedDate.Value.Year - this.SelectedDate.Value.Year % 10;
                this.RefreshViewByVisualState();
                this.RefreshMonthDays(0);
            }
            else
            {
                this.VisualState = SuperCalendarVisualState.日;
                this.Year = DateTime.Now.Year;
                this.Month = DateTime.Now.Month;
                this.Decade = DateTime.Now.Year - DateTime.Now.Year % 10;
                this.RefreshViewByVisualState();
                this.RefreshMonthDays(0);
            }
        }

        /// <summary>
        /// 标题按钮文本大小
        /// </summary>
        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }
        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(SuperDatePicker), new PropertyMetadata(20.0));

        /// <summary>
        /// 选项按钮文本大小
        /// </summary>
        public double ItemFontSize
        {
            get { return (double)GetValue(ItemFontSizeProperty); }
            set { SetValue(ItemFontSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemFontSizeProperty =
            DependencyProperty.Register(nameof(ItemFontSize), typeof(double), typeof(SuperDatePicker), new PropertyMetadata(16.0));

        /// <summary>
        /// 选中的日期
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(SuperDatePicker), new PropertyMetadata(null, OnSelectedDatePropertyChanged));
        public static void OnSelectedDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuperDatePicker datePicker)
            {
                if (datePicker.SelectedDate.HasValue)
                {
                    datePicker.VisualState = SuperCalendarVisualState.日;
                    datePicker.Year = datePicker.SelectedDate.Value.Year;
                    datePicker.Month = datePicker.SelectedDate.Value.Month;
                    datePicker.Decade = datePicker.SelectedDate.Value.Year - datePicker.SelectedDate.Value.Year % 10;
                    datePicker.RefreshViewByVisualState();
                    datePicker.RefreshMonthDays(0);
                }
                else
                {
                    datePicker.VisualState = SuperCalendarVisualState.日;
                    datePicker.Year = DateTime.Now.Year;
                    datePicker.Month = DateTime.Now.Month;
                    datePicker.Decade = DateTime.Now.Year - DateTime.Now.Year % 10;
                    datePicker.RefreshViewByVisualState();
                    datePicker.RefreshMonthDays(0);
                }
            }
        }

        /// <summary>
        /// 面板年代
        /// </summary>
        public int Decade
        {
            get { return (int)GetValue(DecadeProperty); }
            set { SetValue(DecadeProperty, value); }
        }
        public static readonly DependencyProperty DecadeProperty =
            DependencyProperty.Register(nameof(Decade), typeof(int), typeof(SuperDatePicker), new PropertyMetadata(2020));

        /// <summary>
        /// 面板年份
        /// </summary>
        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }
        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register(nameof(Year), typeof(int), typeof(SuperDatePicker), new PropertyMetadata(DateTime.Now.Year));

        /// <summary>
        /// 面板月份
        /// </summary>
        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }
        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register(nameof(Month), typeof(int), typeof(SuperDatePicker), new PropertyMetadata(DateTime.Now.Month));

        /// <summary>
        /// 当前交互面板
        /// </summary>
        public SuperCalendarVisualState VisualState { get; set; } = SuperCalendarVisualState.日;

        /// <summary>
        /// 向前按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (VisualState == SuperCalendarVisualState.日)
            {
                RefreshMonthDays(-1);
            }
            else if (VisualState == SuperCalendarVisualState.月)
            {
                RefreshYearMonths(-1);
            }
            else if (VisualState == SuperCalendarVisualState.年)
            {
                RefreshYears(-1);
            }
        }
        /// <summary>
        /// 向后按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (VisualState == SuperCalendarVisualState.日)
            {
                RefreshMonthDays(1);
            }
            else if (VisualState == SuperCalendarVisualState.月)
            {
                RefreshYearMonths(1);
            }
            else if (VisualState == SuperCalendarVisualState.年)
            {
                RefreshYears(1);
            }
        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.VisualState)
            {
                case SuperCalendarVisualState.日:
                    this.VisualState = SuperCalendarVisualState.月;
                    RefreshViewByVisualState();
                    break;
                case SuperCalendarVisualState.月:
                    this.VisualState = SuperCalendarVisualState.年;
                    RefreshViewByVisualState();
                    break;
                case SuperCalendarVisualState.年:
                    break;
            }
        }

        private void RefreshMonthDays(int offsetMonths)
        {
            DateTime firstDayOfMonth = new DateTime(this.Year, this.Month, 1).AddMonths(offsetMonths);
            this.Month = firstDayOfMonth.Month;
            this.Year = firstDayOfMonth.Year;
            this.HeaderButton.Content = firstDayOfMonth.ToString("yyyy年MM月");

            DateTime firstDayOfView = firstDayOfMonth;

            var dayOfWeek = firstDayOfMonth.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    firstDayOfView = firstDayOfMonth;
                    break;
                case DayOfWeek.Tuesday:
                    firstDayOfView = firstDayOfMonth.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    firstDayOfView = firstDayOfMonth.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    firstDayOfView = firstDayOfMonth.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    firstDayOfView = firstDayOfMonth.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    firstDayOfView = firstDayOfMonth.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    firstDayOfView = firstDayOfMonth.AddDays(-6);
                    break;
            }

            int offset = 0;
            foreach (FrameworkElement element in this.MonthDaysGrid.Children)
            {
                if (element is Button button)
                {
                    DateTime buttonDateTime = firstDayOfView.AddDays(offset++);

                    if (buttonDateTime.Month == firstDayOfMonth.Month)
                    {
                        button.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        button.Foreground = new SolidColorBrush(Colors.Silver);
                    }

                    if (buttonDateTime.Date.Equals(DateTime.Now.Date))
                    {
                        button.Background = new SolidColorBrush(Colors.LightBlue);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    if (this.SelectedDate.HasValue && buttonDateTime.Date.Equals(this.SelectedDate.Value.Date))
                    {
                        button.BorderThickness = new Thickness(2);
                        button.BorderBrush = new SolidColorBrush(Colors.Blue);
                    }
                    else
                    {
                        button.BorderThickness = new Thickness(1);
                        button.BorderBrush = new SolidColorBrush(Colors.Gainsboro);
                    }

                    button.Tag = buttonDateTime;
                    button.Content = buttonDateTime.Day;
                }
            }
        }

        private void RefreshYearMonths(int offsetYears)
        {
            DateTime firstMonthOfYear = new DateTime(this.Year, 1, 1).AddYears(offsetYears);
            this.Year = firstMonthOfYear.Year;
            this.HeaderButton.Content = $"{firstMonthOfYear.Year}年";

            int offset = 0;
            foreach (FrameworkElement element in this.YearMonthsView.Children)
            {
                if (element is Button button)
                {
                    DateTime buttonMonthTime = firstMonthOfYear.AddMonths(offset++);

                    if (buttonMonthTime.Year == DateTime.Now.Year && buttonMonthTime.Month == DateTime.Now.Month)
                    {
                        button.Background = new SolidColorBrush(Colors.LightBlue);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    //if (this.SelectedYear.HasValue && buttonMonthTime.Year == this.SelectedYear.Value && this.SelectedMonth.HasValue && buttonMonthTime.Month == this.SelectedMonth.Value)
                    //{
                    //    button.BorderThickness = new Thickness(2);
                    //    button.BorderBrush = new SolidColorBrush(Colors.Blue);
                    //}
                    //else
                    //{
                    //    button.BorderThickness = new Thickness(1);
                    //    button.BorderBrush = new SolidColorBrush(Colors.Gainsboro);
                    //}

                    button.Tag = buttonMonthTime;
                    button.Content = $"{buttonMonthTime.Month}月";
                }
            }
        }

        private void RefreshYears(int offsetDecades)
        {
            this.Decade += offsetDecades * 10;

            this.HeaderButton.Content = $"{this.Decade}-{this.Decade + 9}";

            int offset = -1;
            foreach (FrameworkElement element in this.YearsView.Children)
            {
                if (element is Button button)
                {
                    int buttonYear = this.Decade + offset;

                    if (buttonYear == DateTime.Now.Year)
                    {
                        button.Background = new SolidColorBrush(Colors.LightBlue);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    //if (this.SelectedYear.HasValue && buttonYear == this.SelectedYear)
                    //{
                    //    button.BorderThickness = new Thickness(2);
                    //    button.BorderBrush = new SolidColorBrush(Colors.Blue);
                    //}
                    //else
                    //{
                    //    button.BorderThickness = new Thickness(1);
                    //    button.BorderBrush = new SolidColorBrush(Colors.Gainsboro);
                    //}

                    button.Tag = buttonYear;
                    button.Content = buttonYear;

                    offset++;
                }
            }
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DateTime dateTime)
            {
                //this.SelectedYear = dateTime.Year;
                //this.SelectedMonth = dateTime.Month;
                this.Year = dateTime.Year;
                this.Month = dateTime.Month;
                this.SelectedDate = dateTime;
                this.VisualState = SuperCalendarVisualState.日;
                this.RefreshViewByVisualState();

                Debug.WriteLine($"选中的日期：{this.SelectedDate.Value.ToString("yyyy-MM-dd")}");
            }
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DateTime dateTime)
            {
                //this.SelectedYear = dateTime.Year;
                //this.SelectedMonth = dateTime.Month;
                this.Year = dateTime.Year;
                this.Month = dateTime.Month;
                this.VisualState = SuperCalendarVisualState.日;
                this.RefreshViewByVisualState();
            }
        }

        private void YearButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int year)
            {
                //this.SelectedYear = year;
                this.Year = year;
                this.VisualState = SuperCalendarVisualState.月;
                this.RefreshViewByVisualState();
            }
        }

        private void RefreshViewByVisualState()
        {
            switch (VisualState)
            {
                case SuperCalendarVisualState.日:
                    this.MonthDaysView.Visibility = Visibility.Visible;
                    this.YearMonthsView.Visibility = Visibility.Hidden;
                    this.YearsView.Visibility = Visibility.Hidden;
                    this.RefreshMonthDays(0);
                    break;

                case SuperCalendarVisualState.月:
                    this.MonthDaysView.Visibility = Visibility.Hidden;
                    this.YearMonthsView.Visibility = Visibility.Visible;
                    this.YearsView.Visibility = Visibility.Hidden;
                    this.RefreshYearMonths(0);
                    break;

                case SuperCalendarVisualState.年:
                    this.MonthDaysView.Visibility = Visibility.Hidden;
                    this.YearMonthsView.Visibility = Visibility.Hidden;
                    this.YearsView.Visibility = Visibility.Visible;
                    this.RefreshYears(0);
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 交互面板状态
    /// </summary>
    public enum SuperCalendarVisualState
    {
        日,

        月,

        年
    }
}

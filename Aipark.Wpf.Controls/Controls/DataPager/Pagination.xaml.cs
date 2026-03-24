using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// Pagination.xaml 的交互逻辑
    /// </summary>
    public partial class Pagination : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<Pagination, NumberChangedEventArgs> PageNumberChanged;

        public Pagination()
        {
            InitializeComponent();

            SelectedPageSizeOption = PageSizeOptions[2];
            PageNumber = 1;
        }


        /// <summary>
        /// 分页选项可见
        /// </summary>
        public bool PageSizeOptionsEnable
        {
            get { return (bool)GetValue(PageSizeOptionsEnableProperty); }
            set { SetValue(PageSizeOptionsEnableProperty, value); }
        }
        public static readonly DependencyProperty PageSizeOptionsEnableProperty =
            DependencyProperty.Register("PageSizeOptionsEnable", typeof(bool), typeof(Pagination), new PropertyMetadata(true));


        /// <summary>
        /// 分页可选项集合
        /// </summary>
        public List<string> PageSizeOptions
        {
            get { return (List<string>)GetValue(PageSizeOptionsProperty); }
            private set { SetValue(PageSizeOptionsProperty, value); }
        }
        public static readonly DependencyProperty PageSizeOptionsProperty =
            DependencyProperty.Register("PageSizeOptions", typeof(List<string>), typeof(Pagination), new PropertyMetadata(new List<string>() { "10 条/页", "20 条/页", "50 条/页", "100 条/页" }));


        /// <summary>
        /// 选中的分页项
        /// </summary>
        public string SelectedPageSizeOption
        {
            get { return (string)GetValue(SelectedPageSizeOptionProperty); }
            private set { SetValue(SelectedPageSizeOptionProperty, value); }
        }
        public static readonly DependencyProperty SelectedPageSizeOptionProperty =
            DependencyProperty.Register("SelectedPageSizeOption", typeof(string), typeof(Pagination), new PropertyMetadata(null, OnSelectedPageSizeOptionPropertyChanged));
        public static void OnSelectedPageSizeOptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Pagination control && e.NewValue is string option)
            {
                int index = control.PageSizeOptions.IndexOf(option);
                switch (index)
                {
                    case 0:
                        control.PageSize = 10;
                        break;
                    case 1:
                        control.PageSize = 20;
                        break;
                    case 2:
                        control.PageSize = 50;
                        break;
                    case 3:
                        control.PageSize = 100;
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// 当前分页大小
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(Pagination), new PropertyMetadata(50, OnPageSizePropertyChanged));
        public static void OnPageSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Pagination control && e.NewValue is int size)
            {
                if (size < 10)
                {
                    control.PageSize = 10;
                    return;
                }

                control.PagesCount = (int)(Math.Ceiling(control.ItemsCount * 1.0 / size));
                control.PageNumber = 1;
            }
        }


        /// <summary>
        /// 分页总数
        /// </summary>
        public int PagesCount
        {
            get { return (int)GetValue(PagesCountProperty); }
            set { SetValue(PagesCountProperty, value); }
        }
        public static readonly DependencyProperty PagesCountProperty =
            DependencyProperty.Register("PagesCount", typeof(int), typeof(Pagination), new PropertyMetadata(1, OnPagesCountPropertyChanged));
        public static void OnPagesCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Pagination control && e.NewValue is int count)
            {
                if (count < 1)
                {
                    control.PagesCount = 1;
                }

                control.RefreshUI();
            }
        }


        /// <summary>
        /// 记录总数
        /// </summary>
        public int ItemsCount
        {
            get { return (int)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }
        public static readonly DependencyProperty ItemsCountProperty =
            DependencyProperty.Register("ItemsCount", typeof(int), typeof(Pagination), new PropertyMetadata(0, OnItemsCountPropertyChanged));
        public static void OnItemsCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Pagination control && e.NewValue is int count)
            {
                if (count < 1)
                {
                    control.PagesCount = 1;
                    control.PageNumber = 1;
                }
                else
                {
                    control.PagesCount = (int)(Math.Ceiling(count * 1.0 / control.PageSize));
                    if (control.PageNumber > control.PagesCount)
                    {
                        control.PageNumber = control.PagesCount;
                    }
                }
            }
        }


        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber
        {
            get { return (int)GetValue(PageNumberProperty); }
            set { SetValue(PageNumberProperty, value); }
        }
        public static readonly DependencyProperty PageNumberProperty =
            DependencyProperty.Register("PageNumber", typeof(int), typeof(Pagination), new PropertyMetadata(0, OnPageNumberPropertyChanged));
        public static void OnPageNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Pagination control && e.NewValue is int number)
            {
                if (number < 1)
                {
                    control.PageNumber = 1;
                    return;
                }

                if (number > control.PagesCount)
                {
                    control.PageNumber = control.PagesCount;
                    return;
                }

                control.RefreshUI();
            }
        }

        private void RefreshUI()
        {
            //按钮显隐控制
            this.button2.Visibility = PagesCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            this.button3.Visibility = PagesCount > 2 ? Visibility.Visible : Visibility.Collapsed;
            this.button4.Visibility = PagesCount > 3 ? Visibility.Visible : Visibility.Collapsed;
            this.button5.Visibility = PagesCount > 4 ? Visibility.Visible : Visibility.Collapsed;
            this.button6.Visibility = PagesCount > 5 ? Visibility.Visible : Visibility.Collapsed;
            this.button7.Visibility = PagesCount > 6 ? Visibility.Visible : Visibility.Collapsed;

            this.numberTextBox.Value = this.PageNumber;

            //按钮值和选中状态控制
            if (this.PagesCount < 8)
            {
                this.button1.Content = 1;
                this.button2.Content = 2;
                this.button3.Content = 3;
                this.button4.Content = 4;
                this.button5.Content = 5;
                this.button6.Content = 6;
                this.button7.Content = 7;

                this.preButton5.Visibility = Visibility.Collapsed;
                this.button2.Margin = new Thickness(3, 0, 0, 0);
                this.nextButton5.Visibility = Visibility.Collapsed;
                this.button7.Margin = new Thickness(3, 0, 0, 0);

                switch (this.PageNumber)
                {
                    case 1:
                        this.button1.IsChecked = true;
                        break;
                    case 2:
                        this.button2.IsChecked = true;
                        break;
                    case 3:
                        this.button3.IsChecked = true;
                        break;
                    case 4:
                        this.button4.IsChecked = true;
                        break;
                    case 5:
                        this.button5.IsChecked = true;
                        break;
                    case 6:
                        this.button6.IsChecked = true;
                        break;
                    case 7:
                        this.button7.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                this.button1.Content = 1;
                this.button7.Content = this.PagesCount;

                if (this.PageNumber < 5)
                {
                    this.button2.Content = 2;
                    this.button3.Content = 3;
                    this.button4.Content = 4;
                    this.button5.Content = 5;
                    this.button6.Content = 6;

                    this.preButton5.Visibility = Visibility.Collapsed;
                    this.button2.Margin = new Thickness(3, 0, 0, 0);
                    this.nextButton5.Visibility = Visibility.Visible;
                    this.button7.Margin = new Thickness(0, 0, 0, 0);

                    switch (this.PageNumber)
                    {
                        case 1:
                            this.button1.IsChecked = true;
                            break;
                        case 2:
                            this.button2.IsChecked = true;
                            break;
                        case 3:
                            this.button3.IsChecked = true;
                            break;
                        case 4:
                            this.button4.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                }
                else if (this.PageNumber > this.PagesCount - 4)
                {
                    this.button6.Content = this.PagesCount - 1;
                    this.button5.Content = this.PagesCount - 2;
                    this.button4.Content = this.PagesCount - 3;
                    this.button3.Content = this.PagesCount - 4;
                    this.button2.Content = this.PagesCount - 5;

                    this.preButton5.Visibility = Visibility.Visible;
                    this.button2.Margin = new Thickness(0, 0, 0, 0);
                    this.nextButton5.Visibility = Visibility.Collapsed;
                    this.button7.Margin = new Thickness(3, 0, 0, 0);

                    int offset = this.PagesCount - this.PageNumber;
                    switch (offset)
                    {
                        case 0:
                            this.button7.IsChecked = true;
                            break;
                        case 1:
                            this.button6.IsChecked = true;
                            break;
                        case 2:
                            this.button5.IsChecked = true;
                            break;
                        case 3:
                            this.button4.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    this.button2.Content = this.PageNumber - 2;
                    this.button3.Content = this.PageNumber - 1;
                    this.button4.Content = this.PageNumber;
                    this.button5.Content = this.PageNumber + 1;
                    this.button6.Content = this.PageNumber + 2;

                    this.preButton5.Visibility = Visibility.Visible;
                    this.button2.Margin = new Thickness(0, 0, 0, 0);
                    this.nextButton5.Visibility = Visibility.Visible;
                    this.button7.Margin = new Thickness(0, 0, 0, 0);

                    this.button4.IsChecked = true;
                }
            }
        }

        private void PreviousOneStepButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageNumberSteps(-1);
        }

        private void PreviousFiveStepsButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageNumberSteps(-5);
        }

        private void NextOneStepButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageNumberSteps(1);
        }

        private void NextFiveStepsButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageNumberSteps(5);
        }

        private void UpdatePageNumberSteps(int steps)
        {
            NumberChangedEventArgs eventArgs = new NumberChangedEventArgs() { OldNumber = this.PageNumber };

            int number = this.PageNumber + steps;
            if (steps < 0)
            {
                this.PageNumber = number < 1 ? 1 : number;
            }
            else
            {
                this.PageNumber = number > this.PagesCount ? this.PagesCount : number;
            }

            if (eventArgs.SetNewNumber(this.PageNumber))
                PageNumberChanged?.Invoke(this, eventArgs);
        }

        private void PageNumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio && int.TryParse(radio.Content.ToString(), out int number))
            {
                NumberChangedEventArgs eventArgs = new NumberChangedEventArgs() { OldNumber = this.PageNumber };

                this.PageNumber = number;

                if (eventArgs.SetNewNumber(this.PageNumber))
                    PageNumberChanged?.Invoke(this, eventArgs);
            }
        }

        private void GotoPageNumberButton_Click(object sender, RoutedEventArgs e)
        {
            NumberChangedEventArgs eventArgs = new NumberChangedEventArgs() { OldNumber = this.PageNumber };

            this.PageNumber = this.numberTextBox.Value;

            if (eventArgs.SetNewNumber(this.PageNumber))
                PageNumberChanged?.Invoke(this, eventArgs);
        }
    }

}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Aipark.Wpf.Controls
{
    [TemplatePart(Name = DateTimeBoxName, Type = typeof(TextBoxEx))]
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
    [TemplatePart(Name = DatePickerName, Type = typeof(SuperDatePicker))]
    [TemplatePart(Name = TimePickerName, Type = typeof(SuperTimePicker))]
    [TemplatePart(Name = NowButtonName, Type = typeof(ButtonEx))]
    [TemplatePart(Name = OkButtonName, Type = typeof(ButtonEx))]

    public class SuperDateTimePicker : Control
    {
        private const string DateTimeBoxName = "TemplatePart_DateTimeBox";
        private const string PopupName = "TemplatePart_Popup";
        private const string DatePickerName = "TemplatePart_DatePicker";
        private const string TimePickerName = "TemplatePart_TimePicker";
        private const string NowButtonName = "TemplatePart_NowButton";
        private const string OkButtonName = "TemplatePart_OkButton";

        private TextBoxEx _dateTimeBox;
        private Popup _popup;
        private SuperDatePicker _datePicker;
        private SuperTimePicker _timePicker;
        private ButtonEx _nowButton;
        private ButtonEx _okButton;

        /// <summary>
        /// 选择的日期或时间变化事件
        /// </summary>
        public event Action<SuperDateTimePicker, DateTime?> DateTimeSelected;

        /// <summary>
        /// 选中的日期和时间
        /// </summary>
        public DateTime? SelectedDateTime
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Text) && DateTime.TryParse(this.Text, out DateTime dateTime))
                {
                    return dateTime;
                }
                return null;
            }
            set
            {
                this.Text = value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }

        /// <summary>
        /// 只读模式
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(SuperDateTimePicker), new PropertyMetadata(false));

        /// <summary>
        /// 日期时间描述信息
        /// </summary>
        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(SuperDateTimePicker), new FrameworkPropertyMetadata(null, OnContentPropertyChanged));
        private static void OnContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is SuperDateTimePicker dateTimePicker)
            {
                dateTimePicker.RemoveLogicalChild(e.OldValue);
                dateTimePicker.AddLogicalChild(e.NewValue);
            }
        }

        /// <summary>
        /// 文本框内容
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SuperDateTimePicker), new PropertyMetadata(string.Empty, OnTextPropertyChanged));
        public static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuperDateTimePicker dateTimePicker)
            {
                if (string.IsNullOrWhiteSpace(e.NewValue.ToString()) || DateTime.TryParse(e.NewValue.ToString(), out _))
                {
                    dateTimePicker.IsCorrectFormat = true;
                }
                else
                {
                    dateTimePicker.IsCorrectFormat = false;
                }
            }
        }

        /// <summary>
        /// 格式正确
        /// </summary>
        public bool IsCorrectFormat
        {
            get { return (bool)GetValue(IsCorrectFormatProperty); }
            private set { SetValue(IsCorrectFormatProperty, value); }
        }
        public static readonly DependencyProperty IsCorrectFormatProperty =
            DependencyProperty.Register(nameof(IsCorrectFormat), typeof(bool), typeof(SuperDateTimePicker), new PropertyMetadata(true));

        /// <summary>
        /// 边角弧度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SuperDateTimePicker), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// 标题按钮文本大小
        /// </summary>
        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }
        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(SuperDateTimePicker), new PropertyMetadata(16.0));

        /// <summary>
        /// 选项按钮文本大小
        /// </summary>
        public double ItemFontSize
        {
            get { return (double)GetValue(ItemFontSizeProperty); }
            set { SetValue(ItemFontSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemFontSizeProperty =
            DependencyProperty.Register(nameof(ItemFontSize), typeof(double), typeof(SuperDateTimePicker), new PropertyMetadata(12.0));

        /// <summary>
        /// 时间选择文本大小
        /// </summary>
        public double TimeFontSize
        {
            get { return (double)GetValue(TimeFontSizeProperty); }
            set { SetValue(TimeFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TimeFontSizeProperty =
            DependencyProperty.Register(nameof(TimeFontSize), typeof(double), typeof(SuperDateTimePicker), new PropertyMetadata(14.0));

        /// <summary>
        /// 按钮文本大小
        /// </summary>
        public double ButtonFontSize
        {
            get { return (double)GetValue(ButtonFontSizeProperty); }
            set { SetValue(ButtonFontSizeProperty, value); }
        }
        public static readonly DependencyProperty ButtonFontSizeProperty =
            DependencyProperty.Register(nameof(ButtonFontSize), typeof(double), typeof(SuperDateTimePicker), new PropertyMetadata(12.0));

        /// <summary>
        /// 下拉框宽度
        /// </summary>
        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }
        public static readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register(nameof(PopupWidth), typeof(double), typeof(SuperDateTimePicker), new PropertyMetadata(200.0));

        /// <summary>
        /// 下拉框高度
        /// </summary>
        public double PopupHeight
        {
            get { return (double)GetValue(PopupHeightProperty); }
            set { SetValue(PopupHeightProperty, value); }
        }
        public static readonly DependencyProperty PopupHeightProperty =
            DependencyProperty.Register(nameof(PopupHeight), typeof(double), typeof(SuperDateTimePicker), new PropertyMetadata(200.0));

        /// <summary>
        /// 下拉框弹出位置
        /// </summary>
        public PlacementMode PopupPlacement
        {
            get { return (PlacementMode)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register(nameof(PopupPlacement), typeof(PlacementMode), typeof(SuperDateTimePicker), new PropertyMetadata(PlacementMode.Bottom));

        /// <summary>
        /// 下拉按钮间距
        /// </summary>
        public Thickness DropdownButtonPadding
        {
            get { return (Thickness)GetValue(DropdownButtonPaddingProperty); }
            set { SetValue(DropdownButtonPaddingProperty, value); }
        }
        public static readonly DependencyProperty DropdownButtonPaddingProperty =
            DependencyProperty.Register(nameof(DropdownButtonPadding), typeof(Thickness), typeof(SuperDateTimePicker), new PropertyMetadata(new Thickness(4, 0, 4, 0)));

        /// <summary>
        /// 下拉按钮缩放尺度
        /// </summary>
        public double DropdownButtonScaleTransform
        {
            get { return (double)GetValue(DropdownButtonScaleTransformProperty); }
            set { SetValue(DropdownButtonScaleTransformProperty, value); }
        }
        public static readonly DependencyProperty DropdownButtonScaleTransformProperty =
            DependencyProperty.Register(nameof(DropdownButtonScaleTransform), typeof(double), typeof(SuperDatePicker), new PropertyMetadata(1.0));



        static SuperDateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuperDateTimePicker), new FrameworkPropertyMetadata(typeof(SuperDateTimePicker)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //文本输入框
            _dateTimeBox = GetTemplateChild<TextBoxEx>(DateTimeBoxName);
            _dateTimeBox.KeyDown += DateTimeBox_KeyDown;
            _dateTimeBox.SelectionChanged += DateTimeBox_SelectionChanged;
            _dateTimeBox.MouseWheel += _dateTimeBox_MouseWheel;

            //
            _popup = GetTemplateChild<Popup>(PopupName);
            _popup.Opened += Popup_Opened;

            //日期时间选择
            _datePicker = GetTemplateChild<SuperDatePicker>(DatePickerName);
            _timePicker = GetTemplateChild<SuperTimePicker>(TimePickerName);

            _nowButton = GetTemplateChild<ButtonEx>(NowButtonName);
            _nowButton.Click += NowButton_Click;

            _okButton = GetTemplateChild<ButtonEx>(OkButtonName);
            _okButton.Click += OkButton_Click;
        }

        private void _dateTimeBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.IsReadOnly)
                return;

            if (!string.IsNullOrWhiteSpace(_dateTimeBox.Text) && _dateTimeBox.Text.Length == 19 && DateTime.TryParse(_dateTimeBox.Text, out var dateTime))
            {
                int offset = e.Delta > 0 ? 1 : -1;

                int selectionStart = _dateTimeBox.SelectionStart;
                int selectionLength = _dateTimeBox.SelectionLength;

                int position = selectionStart + selectionLength;
                if (position < 5)
                {
                    //年
                    _dateTimeBox.Text = dateTime.AddYears(offset).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (position >= 5 && position < 8)
                {
                    //月
                    _dateTimeBox.Text = dateTime.AddMonths(offset).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (position >= 8 && position < 11)
                {
                    //日
                    _dateTimeBox.Text = dateTime.AddDays(offset).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (position >= 11 && position < 14)
                {
                    //时
                    _dateTimeBox.Text = dateTime.AddHours(offset).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (position >= 14 && position < 17)
                {
                    //分
                    _dateTimeBox.Text = dateTime.AddMinutes(offset).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (position >= 17 && position < 20)
                {
                    //秒
                    _dateTimeBox.Text = dateTime.AddSeconds(offset).ToString("yyyy-MM-dd HH:mm:ss");
                }

                _dateTimeBox.SelectionStart = selectionStart;
                _dateTimeBox.SelectionLength = selectionLength;
            }
        }

        private void DateTimeBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"日期时间选中位置：{_dateTimeBox.SelectionStart}-{_dateTimeBox.SelectionLength}");

            if (_dateTimeBox.SelectionLength > 0)
            {
                string selection = _dateTimeBox.Text.Substring(_dateTimeBox.SelectionStart, _dateTimeBox.SelectionLength);
                if (!string.IsNullOrWhiteSpace(selection.Trim()))
                {
                    //重新定位选中结尾位置，鼠标双击默认不向前选中空格
                    if (selection.EndsWith(" "))
                    {
                        int length = selection.TrimEnd().Length;
                        _dateTimeBox.SelectionLength = length;
                    }
                }
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.Text) && DateTime.TryParse(this.Text, out DateTime dateTime))
            {
                this._datePicker.SelectedDate = dateTime;
                this._timePicker.SelectedTime = dateTime;
            }
        }

        private void DateTimeBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (!string.IsNullOrWhiteSpace(this.Text) && DateTime.TryParse(this.Text, out var dateTime))
                        {
                            this.DateTimeSelected?.Invoke(this, dateTime);
                        }
                    }
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        private void NowButton_Click(object sender, RoutedEventArgs e)
        {
            this._datePicker.SelectedDate = DateTime.Now;
            this._timePicker.SelectedTime = DateTime.Now;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            DateTime? selectedDate = _datePicker.SelectedDate;
            if (selectedDate.HasValue)
            {
                selectedDateTime = new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, selectedDate.Value.Day, 0, 0, 0);
            }

            DateTime? selectedTime = _timePicker.SelectedTime;
            if (selectedTime.HasValue)
            {
                selectedDateTime = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, selectedTime.Value.Hour, selectedTime.Value.Minute, selectedTime.Value.Second);
            }

            this.Text = selectedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            _popup.IsOpen = false;

            this.DateTimeSelected?.Invoke(this, selectedDateTime);
        }

        private T GetTemplateChild<T>(string childName) where T : FrameworkElement, new()
        {
            return (GetTemplateChild(childName) as T) ?? new T();
        }
    }
}

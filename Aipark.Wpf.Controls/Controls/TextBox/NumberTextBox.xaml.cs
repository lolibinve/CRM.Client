using System;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// NumberTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class NumberTextBox : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<NumberTextBox, NumberChangedEventArgs> NumberChanged;

        public NumberTextBox()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 最大值
        /// </summary>
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(NumberTextBox), new PropertyMetadata(1));


        /// <summary>
        /// 最小值
        /// </summary>
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(NumberTextBox), new PropertyMetadata(0));


        /// <summary>
        /// 当前值
        /// </summary>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumberTextBox), new PropertyMetadata(0, OnValuePropertyChanged));
        public static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberTextBox control && e.NewValue is int number)
            {
                if (number < control.Minimum)
                {
                    control.Value = control.Minimum;
                    return;
                }

                if (number > control.Maximum)
                {
                    control.Value = control.Maximum;
                    return;
                }

                control.Text = control.Value.ToString();
            }
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            private set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NumberTextBox), new PropertyMetadata(string.Empty, OnTextPropertyChanged));
        public static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberTextBox control && e.NewValue is string text)
            {
                if (int.TryParse(text, out int number))
                {
                    control.Value = number;
                }
                else
                {
                    control.numberTbx.Text = e.OldValue.ToString();
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateNumberSteps(1);
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateNumberSteps(-1);
        }

        private void UpdateNumberSteps(int steps)
        {
            NumberChangedEventArgs eventArgs = new NumberChangedEventArgs() { OldNumber = this.Value };

            int number = this.Value + steps;
            if (steps < 0)
            {
                this.Value = number < this.Minimum ? this.Minimum : number;
            }
            else
            {
                this.Value = number > this.Maximum ? this.Maximum : number;
            }


            if (eventArgs.SetNewNumber(this.Value))
                NumberChanged?.Invoke(this, eventArgs);
        }
    }
}

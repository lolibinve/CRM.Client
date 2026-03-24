using CRM.Modular.ViewModels;
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

namespace CRM.Modular.Views
{
    /// <summary>
    /// ExchangeRateView.xaml 的交互逻辑
    /// </summary>
    public partial class ExchangeRateView : UserControl
    {
        public ExchangeRateView()
        {
            InitializeComponent();
        }


        private void Lst_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void Lst_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExchangeLst.CancelEdit();
        }
    }
}

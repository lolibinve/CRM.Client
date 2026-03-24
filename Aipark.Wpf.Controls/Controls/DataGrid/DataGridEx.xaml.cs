using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// DataGridEx.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridEx : DataGrid
    {
        public DataGridEx()
        {
            InitializeComponent();
        }

        private void DataGridEx_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}

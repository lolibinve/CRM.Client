using System.Windows.Controls;

namespace CRM.Modular.Views
{
    public partial class StockPurchaseView : UserControl
    {
        public StockPurchaseView()
        {
            InitializeComponent();
        }

        private void Lst_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}

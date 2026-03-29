using System.Windows.Controls;

namespace CRM.Modular.Views
{
    public partial class FbmPurchaseView : UserControl
    {
        public FbmPurchaseView()
        {
            InitializeComponent();
        }

        private void Lst_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}

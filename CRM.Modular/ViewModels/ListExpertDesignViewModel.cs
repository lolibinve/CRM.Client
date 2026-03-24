using Caliburn.Micro;
using CLog;
using HttpLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using CRM.Model;
using PropertyChanged;
using Screen = Caliburn.Micro.Screen;
using MessageBox = System.Windows.MessageBox;
using System.Collections;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 导出弹窗
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    internal class ListExpertDesignViewModel : Screen
    {
        #region 导出参数

        public ObservableCollection<string> Countriess { set; get; } = new ObservableCollection<string>();


        public string UPCPar { set; get; } = "UPC";
        public string BrandName { set; get; }
        public string RecommendedBrowseNodes { set; get; }
        public bool IsUpdate { set; get; } = true;
        public IList SelectedContries { set; get; }

        #endregion

        public ListExpertDesignViewModel()
        {
            InitCountries();
        }

        public async void InitCountries()
        {
            var source = await CRMRequest.GetCountryCurrencyInfo();
            if (source != null && source.PriceLst.Count > 0)
            {
                var lst = source.PriceLst.Select(x => x.Country).ToList();
                this.Countriess = new ObservableCollection<string>(lst);
            }
        }

        public async void Export()
        {
            System.Windows.Controls.ListBox listBoxCountry = ((FrameworkElement)this.GetView()).FindName("CountriesListBox") as System.Windows.Controls.ListBox;
            if (listBoxCountry.SelectedItems == null || listBoxCountry.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择国家！");
                return;
            }

            SelectedContries = listBoxCountry.SelectedItems;

            var temp = this.GetView();
            if (temp is System.Windows.Window win)
            {
                win.DialogResult = true;
            }
            await TryCloseAsync();
        }


        //public Task CloseForm()
        //{
        //    var temp = this.GetView();
        //    if (temp is System.Windows.Window win)
        //    {
        //        win.DialogResult = false;
        //    }
        //    return TryCloseAsync();
        //}
    }
}

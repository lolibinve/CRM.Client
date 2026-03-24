using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Controls.TextBox;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class StandardPriceViewModel : Screen
    {
        public ObservableCollection<PriceModel> PriceLst { set; get; } = new ObservableCollection<PriceModel>();
        public ChildModel Child { set; get; } = new ChildModel();
        public string Title { set; get; }
        public WareData Ware { set; get; }


        public StandardPriceViewModel(ChildModel data,WareData ware)
        {
            Title = "设置价格_变种";
            this.Child.Clone(data);
            this.Ware = ware;
            InitView(data);
        }

        private void InitView(ChildModel ChildData)
        {
            if (ChildData.StandardPrice != null
                   && ChildData.StandardPrice.Count > 0)
            {
                var json = JsonHelper.SerializeObject(ChildData.StandardPrice.ToList());
                var temp = JsonHelper.DeserializeObject<List<PriceModel>>(json);
                PriceLst = new ObservableCollection<PriceModel>(temp);
            }
            else
            {
                if (this.Ware.BasePrice == null || this.Ware.BasePrice.Count == 0)
                    return;

                var json = JsonHelper.SerializeObject(this.Ware.BasePrice.ToList());
                PriceLst = JsonHelper.DeserializeObject<ObservableCollection<PriceModel>>(json);
            }
            Child.StandardPrice = PriceLst;
        }


        public async void SavePrices(object sender, RoutedEventArgs e)
        {
            var temp = this.GetView();
            if (temp is System.Windows.Window win)
            {
                win.DialogResult = true;
            }
            await TryCloseAsync();
        }


        public Task CloseForm()
        {
            var temp = this.GetView();
            if (temp is System.Windows.Window win)
            {
                win.DialogResult = false;
            }
            return TryCloseAsync();
        }


        public void EditPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            var json = JsonHelper.SerializeObject(this.Ware.BasePrice.ToList());
            var PriceLstTemp = JsonHelper.DeserializeObject<ObservableCollection<PriceModel>>(json);

            if(sender is TextBox textBox && int.TryParse(textBox.Text, out var addPrice))
            {
                foreach (var item in PriceLstTemp)
                {
                    var temp = item.Price + (item.Exchange * addPrice);
                    item.Price = (float)Math.Round(temp, 1);
                }
            }

            PriceLst = PriceLstTemp;
        }

    }
}

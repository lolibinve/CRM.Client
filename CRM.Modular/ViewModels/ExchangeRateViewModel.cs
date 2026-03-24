using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using Screen = Caliburn.Micro.Screen;
using TextBox = System.Windows.Controls.TextBox;


namespace CRM.Modular.ViewModels
{
    public class ExchangeRateViewModel : Screen
    {

        private BindableCollection<ExchangeData> _exchangeLst = new BindableCollection<ExchangeData>();

        public ExchangeData SelectData { set; get; }
        private readonly IWindowManager windowManager;

        public BindableCollection<ExchangeData> ExchangeLst
        {
            get { return _exchangeLst; }
            set { Set(ref _exchangeLst, value); }
        }

    

        public ExchangeRateViewModel(IWindowManager manager)
        {
            _ = Query();
            this.windowManager = manager;
        }


        public async Task Query()
        {
            var result = await CRMRequest.ExchangeList();

            if (result != null)
            {
                var lst = result.Exchangelst;
                ExchangeLst = new BindableCollection<ExchangeData>(lst);
            }
        }

        public async void Delete()
        {
            ExchangeData first = ExchangeLst.FirstOrDefault(x => x.IsCheck == true);
            if (first != null)
            {
                var success = await CRMRequest.DeleteExchange(first.Id.ToString());
                if (success)
                {
                    ExchangeLst.Remove(first);
                }
            }
        }


        public async void Modify()
        {
            var model = ExchangeLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                AddExchangeRateViewModel addViewModel = new AddExchangeRateViewModel(SelectData, true);
                var result = await windowManager.ShowDialogAsync(addViewModel);
                if (result == true)
                {
                    model.Clone(addViewModel.exchange);
                }
            }
            else
            {
                MessageBox.Show("请先勾选订单");
            }
        }

        public async void ExchangeLst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectData != null)
            {
                AddExchangeRateViewModel addViewModel = new AddExchangeRateViewModel(SelectData, true);
                var result = await windowManager.ShowDialogAsync(addViewModel);
                if (result == true)
                {
                    //SelectData.Clone(addViewModel.exchange);

                    await Query();
                }
            }
        }

        public async void Add()
        {
            AddExchangeRateViewModel addViewModel = new AddExchangeRateViewModel(SelectData, false);
            var result = await windowManager.ShowDialogAsync(addViewModel);
            if(result == true)
            {
                await Query();
            }
        }

        public void OrderItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (ExchangeLst != null && ExchangeLst.Count > 0 && sender != null)
            {
                if (((FrameworkElement)sender).DataContext is ExchangeData data)
                {
                    foreach (var item in ExchangeLst)
                    {
                        if (item.Id != data.Id && item.IsCheck)
                        {
                            item.IsCheck = false;
                        }
                    }
                }
            }
        }




        public void OrderItem_IsDvPercentClick(object sender, RoutedEventArgs e)
        {
            if (ExchangeLst != null && ExchangeLst.Count > 0 && sender != null)
            {
                if (((FrameworkElement)sender).DataContext is ExchangeData data)
                {

                    //data.IsDvPercent = data.IsDvPercent ? false : true;

                    //foreach (var item in ExchangeLst)
                    //{
                    //    if (item.Id == data.Id)
                    //    {
                    //        if(item.IsDvPercent)
                    //        {
                    //            item.IsDvPercent = true;
                    //            var intValue = double.Parse(data.Dv) * 100;
                    //            item.Dv = intValue + "%";
                    //        }
                    //        else
                    //        {
                    //            item.IsDvPercent = false;
                    //        }
                    //        break;
                    //    }
                    //}
                }
            }
        }
    }
}

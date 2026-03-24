using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using CRM.Modular.Views;
using HttpLib;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace CRM.Modular.ViewModels
{

    public class AddExchangeRateViewModel : Screen
    {

        private ExchangeData _exchange = new ExchangeData();
        public ExchangeData exchange
        {
            get { return _exchange; }
            set { Set(ref _exchange, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }


        public AddExchangeRateViewModel(ExchangeData data, bool IsModify = false)
        {
            Title = IsModify ? "修改汇率信息" : "新增汇率信息";
            if (IsModify)
            {
                this.exchange.Clone(data);
            }
        }

        public async void Sure()
        {
            var result = await CRMRequest.ModifyExchange(exchange);
            if (result != null)
            {
                exchange.Clone(result);
                var temp = this.GetView();
                if (temp is Window win)
                {
                    win.DialogResult = true;
                }
                await TryCloseAsync();
            }
        }

        public Task CloseForm()
        {
            var temp = this.GetView();
            if (temp is Window win)
            {
                win.DialogResult = false;
            }
            return TryCloseAsync();
        }
    }
}

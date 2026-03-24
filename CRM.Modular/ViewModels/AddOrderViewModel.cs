using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using PropertyChanged;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddOrderViewModel : Screen
    {

        public OrderData order { set; get; } = new OrderData();

        public string Title { set; get; }

        public bool IsAdmin { set; get; } = true;

        public bool CostIsEnable { set; get; } = true;
        public bool AccountIsReadOnly { set; get; } = true;
        public bool TransExpenseIsEnable { set; get; } = true;
        

        public AddOrderViewModel(OrderData data, bool IsModify = false)
        {
            Title = IsModify ? "修改订单" : "新增订单";

            this.order.Clone(data);
            if(!IsModify)
            {
                this.order.Id = 0;
                this.order.State = OrderState.新单;
                this.order.SalesVolume = 0;
                this.order.SettleAmount = 0;
                this.order.SaleDate = DateTime.Now;
            }

            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;
            if(!IsAdmin)
            {
                if(order.Cost != 0)
                {
                    CostIsEnable = false;
                }
                if(order.TransExpense !=0 )
                {
                    TransExpenseIsEnable = false;
                }
            }
            else
            {
                AccountIsReadOnly = false;
            }
        }

        public async void Sure()
        {
            var result = await CRMRequest.AddOrder(order);
            if (result)
            {
                var temp =  this.GetView();
                if(temp is Window win)
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

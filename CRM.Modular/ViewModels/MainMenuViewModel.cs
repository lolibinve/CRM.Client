using Caliburn.Micro;
using CRM.Modular.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Modular.ViewModels
{
    public class MainMenuViewModel : Screen
    {
        public RoleManageViewModel Role { get; set; }
        public OrderManageViewModel Order { get; set; }
        public ProductConfigViewModel Product { get; set; }
        public ExchangeRateViewModel Exchange { get; set; }
        public ListManageViewModel ListManage { get; set; }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { Set(ref _isAdmin, value); }
        }


        public MainMenuViewModel()
        {
            Role = IoC.Get<RoleManageViewModel>();
            Order = IoC.Get<OrderManageViewModel>();
            Product = IoC.Get<ProductConfigViewModel>();
            Exchange = IoC.Get<ExchangeRateViewModel>();
            ListManage = IoC.Get<ListManageViewModel>();

            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;
        }


    }
}
    
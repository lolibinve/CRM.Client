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

        public PurchaseAccountViewModel PurchaseAccount { get; set; }
        public FbmPurchaseViewModel FbmPurchase { get; set; }
        public StockPurchaseViewModel StockPurchase { get; set; }
        public StockProductViewModel StockProduct { get; set; }

        

        //public ListManageViewModel ListManage { get; set; }

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

            PurchaseAccount = IoC.Get<PurchaseAccountViewModel>();
            FbmPurchase = IoC.Get<FbmPurchaseViewModel>();
            StockPurchase = IoC.Get<StockPurchaseViewModel>();
            StockProduct = IoC.Get<StockProductViewModel>();

            //ListManage = IoC.Get<ListManageViewModel>();

            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;
        }


    }
}
    
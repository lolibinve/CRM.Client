using Caliburn.Micro;
using CRM.Client.ViewModels;
using CRM.Modular.Models;
using CRM.Modular.ViewModels;
using HttpLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CRM.Client
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container;

        public Bootstrapper()
        {
            Initialize();
            LogManager.GetLog = type => new DebugLog(type);
        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Instance(container);

            container
                  .Singleton<IWindowManager, WindowManager>()
                  .Singleton<IEventAggregator, EventAggregator>();

            container.RegisterInstance(typeof(CacheInfo), "loginData", new CacheInfo());

            //foreach (var assembly in SelectAssemblies())
            //{
            //    assembly.GetTypes()
            //   .Where(type => type.IsClass)
            //   .Where(type => type.Name.EndsWith("ViewModel"))
            //   .ToList()
            //   .ForEach(viewModelType => container.RegisterPerRequest(
            //       viewModelType, viewModelType.ToString(), viewModelType));
            //}
            container.PerRequest<LoginViewModel>();
            container.PerRequest<ShellViewModel>();
            container.PerRequest<MainMenuViewModel>();
            container.PerRequest<OrderManageViewModel>();
            container.PerRequest<RoleManageViewModel>();
            container.PerRequest<ProductConfigViewModel>();
            container.PerRequest<ExchangeRateViewModel>();
            container.PerRequest<ListManageViewModel>();
            container.PerRequest<AddOrderViewModel>();
            container.PerRequest<StandardPriceViewModel>();

            container.PerRequest<AddPurchaseAccountViewModel>();
            container.PerRequest<PurchaseAccountCheckInViewModel>();
            container.PerRequest<PurchaseAccountCheckInListViewModel>();
            container.PerRequest<PurchaseAccountViewModel>();

            container.PerRequest<FbmPurchaseViewModel>();
            container.PerRequest<AddFbmPurchaseViewModel>();

            container.PerRequest<StockProductViewModel>();
            container.PerRequest<AddStockProductViewModel>();

            container.PerRequest<StockPurchaseViewModel>();
            container.PerRequest<AddStockPurchaseViewModel>();

            SessionAccessDenied.AccessDenied += OnSessionAccessDenied;
        }

        private void OnSessionAccessDenied()
        {
            var app = Application.Current;
            if (app == null)
            {
                SessionAccessDenied.ResetGate();
                return;
            }

            app.Dispatcher.BeginInvoke(new System.Action(HandleSessionExpiredRestartApp));
        }

        /// <summary>
        /// 登录失效：提示后结束当前进程并启动新进程（重新出现登录界面），避免在同进程内换 Cookie/窗口。
        /// </summary>
        private void HandleSessionExpiredRestartApp()
        {
            try
            {
                var app = Application.Current;
                if (app == null)
                {
                    return;
                }

                bool hasShell = app.Windows.Cast<Window>().Any(w => w.GetType().Name == "ShellView");
                if (!hasShell)
                {
                    SessionAccessDenied.ResetGate();
                    return;
                }

                MessageBox.Show("登录已失效，请重新登录。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);

                string path = Assembly.GetEntryAssembly()?.Location;
                if (string.IsNullOrEmpty(path))
                {
                    MessageBox.Show("无法定位程序路径，请手动重新启动应用程序。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown(0);
                    return;
                }

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法自动重新启动：" + ex.Message + "\n请手动重新打开程序。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                Application.Current.Shutdown(0);
            }
            finally
            {
                SessionAccessDenied.ResetGate();
            }
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetExecutingAssembly(),
                //Assembly.Load("Caliburn.Micro.Test.ViewModel"),
                Assembly.Load("CRM.Modular"),
            };

            return assemblies;
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<LoginViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show(e.Exception.Message, "An error as occurred", MessageBoxButton.OK);
        }

    }
}

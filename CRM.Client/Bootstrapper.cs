using Caliburn.Micro;
using CRM.Client.ViewModels;
using CRM.Modular.Models;
using CRM.Modular.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

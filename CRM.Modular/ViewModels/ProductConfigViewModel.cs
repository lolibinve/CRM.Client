using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Help;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
    [AddINotifyPropertyChangedInterface]
    public class ProductConfigViewModel : Screen
    {
        public string Sku { set; get; }

        public bool IsProgressIndeterminate { set; get; }
        public PageInfoModel PageInfo { set; get; } = new PageInfoModel() { PageNum = 1 };

        public ObservableCollection<RoleData> RoleSource { set; get; }
        public BindableCollection<ProductData> ProductLst { set; get; } = new BindableCollection<ProductData>();

        public bool IsAdmin { set; get; }
        public const int PageSizeConst = 50;
        public RoleData SelectRole { set; get; }
        public ProductData SelectData { set; get; }
        public DateTime? SelectedStartDate { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime? SelectedEndDate { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private readonly IWindowManager windowManager;

        public ProductConfigViewModel(IWindowManager manager)
        {
            this.windowManager = manager;
            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;
            InitRoleSource();
            _ = Query(Sku);
        }

        public async void InitRoleSource()
        {
            var source = await CRMRequest.RoleList(null);
            RoleSource = new ObservableCollection<RoleData>(source.Orderlst);
            RoleSource.Insert(0, new RoleData()
            {
                Name = "全部",
            });
        }
        public async Task Query(string sku)
        {
            IsProgressIndeterminate = true;

            var saleMan = SelectRole?.Name;
            if (SelectRole == null)
            {
                var info = IoC.Get<CacheInfo>();
                if (info.IsAdmin == false)
                {
                    saleMan = info.LoginAccount;
                    SelectRole = new RoleData() { Admin = 0, Name = info.LoginAccount };
                }
            }
            else
            {
                if (saleMan == "全部")
                {
                    saleMan = "";
                }
            }

            var result = await CRMRequest.ProductList(sku,saleMan, SelectedStartDate, SelectedEndDate);

            if (result != null)
            {
                UpdateView(result, 1);
            }

            IsProgressIndeterminate = false;

        }

        public async void Pagination_OnPageNumberChanged(Aipark.Wpf.Controls.Pagination arg1, Aipark.Wpf.Controls.NumberChangedEventArgs arg2)
        {
            IsProgressIndeterminate = true;

            var saleMan = SelectRole?.Name;
            if (saleMan == "全部")
            {
                saleMan = "";
            }
            var result = await CRMRequest.ProductList(Sku,saleMan, SelectedStartDate, SelectedEndDate, pageNum: arg1.PageNumber, pageSize: PageSizeConst);
            if (result != null)
            {
                UpdateView(result, arg1.PageNumber);
            }

            IsProgressIndeterminate = false;
        }

        private void UpdateView(ProductModel result,int pageNum)
        {
            var lst = result.Productlst;
            if (lst == null)
            {
                ProductLst = new BindableCollection<ProductData>();
                PageInfo = new PageInfoModel();
                return;
            }
            ProductLst = new BindableCollection<ProductData>(lst);
            PageInfo = new PageInfoModel()
            {
                Total = result.Count,
                PageNum = pageNum,
                PageSize = PageSizeConst,
                PagesCount = (int)Math.Ceiling((result.Count * 1.0) / PageSizeConst),
            };
        }


        public async void Delete()
        {
            ProductData first = ProductLst.FirstOrDefault(x => x.IsCheck == true);
            if (first != null)
            {
                var success = await CRMRequest.DeleteProduct(first.Id.ToString());
                if (success)
                {
                    ProductLst.Remove(first);
                }
            }
        }


        public async void Modify()
        {
            var model = ProductLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                AddProductViewModel addViewModel = new AddProductViewModel(SelectData, true);
                var result = await windowManager.ShowDialogAsync(addViewModel);
                if (result == true)
                {
                    model.Clone(addViewModel.product);
                }
            }
            else
            {
                MessageBox.Show("请先勾选订单");
            }
        }

        public async void ProductLst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectData != null)
            {
                AddProductViewModel addProductViewModel = new AddProductViewModel(SelectData, true);
                var result = await windowManager.ShowDialogAsync(addProductViewModel);
                if (result == true)
                {
                    SelectData.Clone(addProductViewModel.product);
                }
            }
        }

        public async void Add()
        {
            AddProductViewModel addViewModel = new AddProductViewModel(SelectData, false);
            var result = await windowManager.ShowDialogAsync(addViewModel);
            if (result == true)
            {
                ProductLst.Add(addViewModel.product);
            }
        }


        public void OrderItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (ProductLst != null && ProductLst.Count > 0 && sender != null)
            {
                if (((FrameworkElement)sender).DataContext is ProductData data)
                {
                    foreach (var item in ProductLst)
                    {
                        if (item.Id != data.Id && item.IsCheck)
                        {
                            item.IsCheck = false;
                        }
                    }
                }
            }
        }
    }
}

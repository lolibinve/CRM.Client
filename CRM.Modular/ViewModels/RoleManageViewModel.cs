using Caliburn.Micro;
using CRM.Client.ViewModels;
using CRM.Model;
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
    public class RoleManageViewModel : Screen
    {
        private readonly IWindowManager windowManager;
        public RoleData SelectData { set; get; }

        private BindableCollection<RoleData> _orderLst = new BindableCollection<RoleData>();
        public BindableCollection<RoleData> OrderLst
        {
            get { return _orderLst; }
            set { Set(ref _orderLst, value); }
        }

        public RoleManageViewModel(IWindowManager manager)
        {
            this.windowManager = manager;
            _ = Query();
        }


        public async Task Query(string name = null)
        {
            var result = await CRMRequest.RoleList(name);

            if (result != null)
            {
                var lst = result.Orderlst;
                OrderLst = new BindableCollection<RoleData>(lst);
            }
        }

        public async void Delete()
        {
            RoleData first = OrderLst.FirstOrDefault(x => x.IsCheck == true);
            if (first != null)
            {
                var success = await CRMRequest.DeleteRole(first.Id.ToString());
                if (success)
                {
                    OrderLst.Remove(first);
                }
            }
        }


        public async void Modify()
        {
            var model = _orderLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                AddRoleViewModel addViewModel = new AddRoleViewModel(SelectData, true);
                var result = await windowManager.ShowDialogAsync(addViewModel);
                if (result == true)
                {
                    model.Clone(addViewModel.role);
                }
            }
            else
            {
                MessageBox.Show("请先勾选订单");
            }
        }

        public async void OrderLst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectData != null)
            {
                AddRoleViewModel addRoleViewModel = new AddRoleViewModel(SelectData, true);
                var result = await windowManager.ShowDialogAsync(addRoleViewModel);
                if (result == true)
                {
                    SelectData.Clone(addRoleViewModel.role);
                }
            }
        }

        public async void Add()
        {
            AddRoleViewModel addViewModel = new AddRoleViewModel(SelectData, false);
            var result = await windowManager.ShowDialogAsync(addViewModel);
            if(result == true)
            {
               await Query();
            }
        }

        public void OrderItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (OrderLst != null && OrderLst.Count > 0 && sender != null)
            {
                if (((FrameworkElement)sender).DataContext is RoleData data)
                {
                    foreach (var item in OrderLst)
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

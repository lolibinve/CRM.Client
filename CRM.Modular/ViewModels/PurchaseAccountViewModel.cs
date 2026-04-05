using Aipark.Wpf.Controls;
using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        /// <summary>与主菜单「角色管理」等一致：仅管理员为 true，业务员不可见新增/删除。</summary>
        public bool IsAdmin { get; set; }

        public BindableCollection<ProcurementAccountLstModel> AccountLst { get; set; } = new BindableCollection<ProcurementAccountLstModel>();

        public ProcurementAccountLstModel SelectItem { get; set; }

        public PurchaseAccountViewModel(IWindowManager manager)
        {
            windowManager = manager;
            IsAdmin = IoC.Get<CacheInfo>().IsAdmin;
            Query();
        }

        /// <summary>
        /// 从 <c>accountList</c> 分页拉取（每页 <see cref="PageSizeConst"/> 条）。
        /// </summary>
        public async void Query()
        {
            await QueryBase(1);
        }

        public async void Pagination_OnPageNumberChanged(Pagination arg1, NumberChangedEventArgs arg2)
        {
            await QueryBase(arg1.PageNumber);
        }

        private async Task QueryBase(int pageNum = 1)
        {
            IsProgressIndeterminate = true;
            try
            {
                var data = await CRMRequest.PurchaseAccountList(pageNum, PageSizeConst);

                if (data != null)
                {
                    var ordered = (data.AccountLst ?? new List<ProcurementAccountLstModel>())
                        .OrderByDescending(x => x.Date)
                        .ToList();
                    AccountLst = new BindableCollection<ProcurementAccountLstModel>(ordered);
                    var pages = (int)Math.Ceiling((data.Count * 1.0) / PageSizeConst);
                    PageInfo = new PageInfoModel
                    {
                        Total = data.Count,
                        PageNum = pageNum,
                        PageSize = PageSizeConst,
                        PagesCount = pages < 1 ? 1 : pages,
                    };
                }
                else
                {
                    AccountLst = new BindableCollection<ProcurementAccountLstModel>();
                    PageInfo = new PageInfoModel
                    {
                        Total = 0,
                        PageNum = 1,
                        PageSize = PageSizeConst,
                        PagesCount = 1,
                    };
                }
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        public async void Add()
        {
            AddPurchaseAccountViewModel vm = new AddPurchaseAccountViewModel(null, false);
            await windowManager.ShowDialogAsync(vm);
            if (!vm.WasSuccessful)
                return;

            await QueryBase(PageInfo?.PageNum ?? 1);
        }

        /// <summary>列表行「转入资金」：从按钮所在行的 <see cref="FrameworkElement.DataContext"/> 取当前行，再带入该行 <c>Id</c> 打开入账弹窗。</summary>
        public async void CheckInClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe) || !(fe.DataContext is ProcurementAccountLstModel row))
            {
                MessageBox.Show("无效的采购账号，无法入账。");
                return;
            }

            if (row.Id <= 0)
            {
                MessageBox.Show("无效的采购账号 id，无法入账。");
                return;
            }

            var vm = new PurchaseAccountCheckInViewModel(row);
            await windowManager.ShowDialogAsync(vm);
            if (!vm.WasSuccessful)
                return;

            IsProgressIndeterminate = true;
            try
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        /// <summary>查看入账流水：<c>accountCheckInList</c>，<c>name</c> 为当前行采购账号名称，<c>type=-1</c>。</summary>
        public async void CheckInListClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe) || !(fe.DataContext is ProcurementAccountLstModel row))
            {
                MessageBox.Show("无效的采购账号。");
                return;
            }

            var name = (row.ProcurementAccount ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("采购账号名称为空，无法查询流水。");
                return;
            }

            var vm = new PurchaseAccountCheckInListViewModel(name);
            await windowManager.ShowDialogAsync(vm);
        }

        /// <summary>调用 <c>crm/login/taskPurchaseAccountBalance</c> 触发余额刷新，成功后刷新当前页。</summary>
        public async void RefreshBalance()
        {
            IsProgressIndeterminate = true;
            try
            {
                var ok = await CRMRequest.TaskPurchaseAccountBalance();
                if (ok)
                {
                    await QueryBase(PageInfo?.PageNum ?? 1);
                }
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        public async void Delete()
        {
            var checkedItem = AccountLst?.FirstOrDefault(x => x.IsCheck);
            if (checkedItem == null || checkedItem.Id <= 0)
            {
                MessageBox.Show("请先勾选要删除的记录。");
                return;
            }

            var label = string.IsNullOrWhiteSpace(checkedItem.ProcurementAccount)
                ? $"ID {checkedItem.Id}"
                : checkedItem.ProcurementAccount.Trim();
            if (MessageBox.Show($"确定删除采购账号记录「{label}」吗？", "确认删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            IsProgressIndeterminate = true;
            try
            {
                var ok = await CRMRequest.PurchaseAccountDelete(checkedItem.Id);
                if (ok)
                {
                    await QueryBase(PageInfo?.PageNum ?? 1);
                }
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        /// <summary>与角色列表一致：同一时刻仅允许勾选一个。</summary>
        public void AccountItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (AccountLst == null || AccountLst.Count == 0 || sender == null)
            {
                return;
            }

            if (((FrameworkElement)sender).DataContext is ProcurementAccountLstModel data)
            {
                foreach (var item in AccountLst)
                {
                    if (item.Id != data.Id && item.IsCheck)
                    {
                        item.IsCheck = false;
                    }
                }
            }
        }

        public async void Account_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectItem == null)
            {
                return;
            }
            await ModifyBase();
        }

        private async Task ModifyBase()
        {
            AddPurchaseAccountViewModel vm = new AddPurchaseAccountViewModel(SelectItem, true);
            var result = await windowManager.ShowDialogAsync(vm);
            if (result == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }
    }
}

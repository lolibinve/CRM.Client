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

        public BindableCollection<ProcurementAccountLstModel> AccountLst { get; set; } = new BindableCollection<ProcurementAccountLstModel>();

        public ProcurementAccountLstModel SelectItem { get; set; }

        public PurchaseAccountViewModel(IWindowManager manager)
        {
            windowManager = manager;
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
            var result = await windowManager.ShowDialogAsync(vm);
            if (result == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
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

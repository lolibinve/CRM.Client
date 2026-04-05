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
    /// <summary>
    /// 产品库存（备货汇总）列表：<c>stockManageList</c>，每页 20 条。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class StockProductViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        public BindableCollection<StockProductRecordModel> RecordLst { get; set; } = new BindableCollection<StockProductRecordModel>();

        public StockProductRecordModel SelectItem { get; set; }

        public StockProductViewModel(IWindowManager manager)
        {
            windowManager = manager;
            _ = QueryBase(1);
        }

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
                var result = await CRMRequest.StockManageList(pageNum, PageSizeConst);
                if (result != null)
                {
                    RecordLst = new BindableCollection<StockProductRecordModel>(result.List ?? new List<StockProductRecordModel>());
                    var pages = (int)Math.Ceiling((result.Count * 1.0) / PageSizeConst);
                    PageInfo = new PageInfoModel
                    {
                        Total = result.Count,
                        PageNum = pageNum,
                        PageSize = PageSizeConst,
                        PagesCount = pages < 1 ? 1 : pages,
                    };
                }
                else
                {
                    RecordLst = new BindableCollection<StockProductRecordModel>();
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
            var vm = new AddStockProductViewModel(null, false);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }

        /// <summary>调用 <c>crm/login/taskStockManageSummary</c> 触发备货汇总库存更新，成功后刷新当前页。</summary>
        public async void UpdateStockSummary()
        {
            IsProgressIndeterminate = true;
            try
            {
                var ok = await CRMRequest.TaskStockManageSummary();
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
            var checkedItem = RecordLst?.FirstOrDefault(x => x.IsCheck);
            if (checkedItem == null || checkedItem.Id == 0)
            {
                MessageBox.Show("请先勾选要删除的产品。");
                return;
            }

            var code = string.IsNullOrWhiteSpace(checkedItem.ProductCode) ? $"ID {checkedItem.Id}" : checkedItem.ProductCode;
            if (MessageBox.Show($"确定删除产品「{code}」吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            IsProgressIndeterminate = true;
            try
            {
                var ok = await CRMRequest.StockManageDelete(checkedItem.Id);
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
        public void RecordItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (RecordLst == null || RecordLst.Count == 0 || sender == null)
            {
                return;
            }

            if (((FrameworkElement)sender).DataContext is StockProductRecordModel data)
            {
                foreach (var item in RecordLst)
                {
                    if (item.Id != data.Id && item.IsCheck)
                    {
                        item.IsCheck = false;
                    }
                }
            }
        }

        public async void Record_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectItem == null)
            {
                return;
            }

            var vm = new AddStockProductViewModel(SelectItem, true);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }
    }
}

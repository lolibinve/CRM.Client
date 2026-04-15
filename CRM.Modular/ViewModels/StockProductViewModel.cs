using Aipark.Wpf.Controls;
using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 产品库存（备货汇总）列表：<c>stockManageList</c>，每页 20 条；
    /// 业务员筛选与 FBM 采购一致：仅管理员可见下拉；非管理员传 <c>user</c> 为当前登录账号；
    /// 管理员选「全部」不传 <c>user</c>，选具体业务员则传 <c>user</c>。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class StockProductViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        /// <summary>与采购账户页面一致：仅管理员可见「更新库存」等。</summary>
        public bool IsAdmin { get; set; }

        /// <summary>业务员筛选：下拉含「全部」，来自 <c>roleList</c>（与 <see cref="FbmPurchaseViewModel"/> 一致）。</summary>
        public ObservableCollection<RoleData> RoleSource { get; set; } = new ObservableCollection<RoleData>();

        public RoleData SelectRole { get; set; }

        /// <summary>接口 <c>stockManageList</c> 返回的 <c>sum</c>：剩余库存总计。</summary>
        public decimal SumRemainingStock { get; set; }

        public BindableCollection<StockProductRecordModel> RecordLst { get; set; } = new BindableCollection<StockProductRecordModel>();

        public StockProductRecordModel SelectItem { get; set; }

        public StockProductViewModel(IWindowManager manager)
        {
            windowManager = manager;
            var info = IoC.Get<CacheInfo>();
            IsAdmin = info?.IsAdmin ?? false;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await RefreshRoleSourceAsync(resetSelection: true);
            await QueryBase(1);
        }

        /// <summary>从 <c>roleList</c> 刷新业务员下拉（与 FBM 采购列表一致）。</summary>
        private async Task RefreshRoleSourceAsync(bool resetSelection)
        {
            var previousName = SelectRole?.Name;
            var rm = await CRMRequest.RoleList(null);
            RoleSource = new ObservableCollection<RoleData>();
            RoleSource.Add(new RoleData { Name = "全部" });
            if (rm?.Orderlst != null)
            {
                foreach (var r in rm.Orderlst.OrderBy(x => x.Name))
                {
                    RoleSource.Add(r);
                }
            }

            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;

            if (!resetSelection && !string.IsNullOrEmpty(previousName) && RoleSource.Any(x => x.Name == previousName))
            {
                SelectRole = RoleSource.First(x => x.Name == previousName);
                return;
            }

            if (!IsAdmin)
            {
                SelectRole = RoleSource.FirstOrDefault(x => x.Name == info.LoginAccount)
                    ?? RoleSource.FirstOrDefault(x => x.Name == "全部");
            }
            else
            {
                SelectRole = RoleSource.FirstOrDefault();
            }
        }

        public async void Query()
        {
            await RefreshRoleSourceAsync(resetSelection: false);
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
                var info = IoC.Get<CacheInfo>();
                IsAdmin = info.IsAdmin;
                string userForList = null;
                if (!info.IsAdmin)
                {
                    userForList = (info.LoginAccount ?? "").Trim();
                }
                else if (SelectRole != null && SelectRole.Name != "全部")
                {
                    userForList = (SelectRole.Name ?? "").Trim();
                }

                var result = await CRMRequest.StockManageList(pageNum, PageSizeConst, userForList);
                if (result != null)
                {
                    RecordLst = new BindableCollection<StockProductRecordModel>(result.List ?? new List<StockProductRecordModel>());
                    SumRemainingStock = result.Sum;
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
                    SumRemainingStock = 0;
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

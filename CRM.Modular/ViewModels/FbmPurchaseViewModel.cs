using Aipark.Wpf.Controls;
using Caliburn.Micro;
using CRM.Modular.Models;
using CRM.Model;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// FBM 采购列表：筛选、分页、对接 <c>fbmList</c>；不提供行内修改（仅新增/删除）。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class FbmPurchaseViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel() { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        public DateTime? SelectedStartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime? SelectedEndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public string OrderIdSearch { get; set; }

        public ObservableCollection<RoleData> RoleSource { get; set; } = new ObservableCollection<RoleData>();
        public RoleData SelectRole { get; set; }

        public ObservableCollection<string> AccountFilterList { get; set; } = new ObservableCollection<string>();
        public string SelectedFilterAccount { get; set; }

        public bool IsAdmin { get; set; }

        public BindableCollection<FbmPurchaseRecordModel> RecordLst { get; set; } = new BindableCollection<FbmPurchaseRecordModel>();

        public FbmPurchaseRecordModel SelectItem { get; set; }

        public FbmPurchaseViewModel(IWindowManager manager)
        {
            windowManager = manager;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await RefreshRoleSourceAsync(resetSelection: true);
            await RefreshAccountFilterAsync(resetSelection: true);
            await QueryBase(1);
        }

        /// <summary>从接口刷新业务员下拉；<paramref name="resetSelection"/> 为 false 时尽量保留当前选中项（仍存在列表中则不变）。</summary>
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

        /// <summary>从接口刷新采购账号下拉；<paramref name="resetSelection"/> 为 false 时若原选项仍在列表中则保留。</summary>
        private async Task RefreshAccountFilterAsync(bool resetSelection)
        {
            var previous = SelectedFilterAccount;
            AccountFilterList = new ObservableCollection<string>();
            AccountFilterList.Add("全部");
            var acc = await CRMRequest.PurchaseAccountList(1, 2000);
            if (acc?.AccountLst != null)
            {
                foreach (var name in acc.AccountLst.Select(x => x.ProcurementAccount).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(x => x))
                {
                    AccountFilterList.Add(name);
                }
            }

            if (!resetSelection && !string.IsNullOrWhiteSpace(previous) && AccountFilterList.Contains(previous))
            {
                SelectedFilterAccount = previous;
            }
            else
            {
                SelectedFilterAccount = "全部";
            }
        }

        public async void Query()
        {
            await RefreshRoleSourceAsync(resetSelection: false);
            await RefreshAccountFilterAsync(resetSelection: false);
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
                var buyerName = SelectRole?.Name;
                if (SelectRole == null)
                {
                    var info = IoC.Get<CacheInfo>();
                    if (!info.IsAdmin)
                    {
                        buyerName = info.LoginAccount;
                    }
                    else
                    {
                        buyerName = "";
                    }
                }
                else if (buyerName == "全部")
                {
                    buyerName = "";
                }

                var purchaseAccount = string.IsNullOrEmpty(SelectedFilterAccount) || SelectedFilterAccount == "全部"
                    ? ""
                    : SelectedFilterAccount;

                var result = await CRMRequest.FbmList(
                    pageNum,
                    PageSizeConst,
                    purchaseAccount,
                    buyerName,
                    OrderIdSearch,
                    SelectedStartDate?.ToString("yyyy-MM-dd"),
                    SelectedEndDate?.ToString("yyyy-MM-dd"));

                if (result != null)
                {
                    RecordLst = new BindableCollection<FbmPurchaseRecordModel>(result.List ?? new List<FbmPurchaseRecordModel>());
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
                    RecordLst = new BindableCollection<FbmPurchaseRecordModel>();
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
            var vm = new AddFbmPurchaseViewModel(null, false);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }

        public async void Delete()
        {
            var checkedItem = RecordLst?.FirstOrDefault(x => x.IsCheck);
            if (checkedItem == null || checkedItem.Id <= 0)
            {
                MessageBox.Show("请先勾选要删除的 FBM 采购记录。");
                return;
            }

            var label = string.IsNullOrWhiteSpace(checkedItem.OrderId)
                ? $"ID {checkedItem.Id}"
                : $"订单「{checkedItem.OrderId}」";
            if (MessageBox.Show($"确定删除 {label} 的这条记录吗？", "确认删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            IsProgressIndeterminate = true;
            try
            {
                var ok = await CRMRequest.FbmPurchaseDelete(checkedItem.Id);
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
        public void FbmItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (RecordLst == null || RecordLst.Count == 0 || sender == null)
            {
                return;
            }

            if (((FrameworkElement)sender).DataContext is FbmPurchaseRecordModel data)
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
    }
}

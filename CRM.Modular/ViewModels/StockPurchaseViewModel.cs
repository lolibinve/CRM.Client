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
    /// 备货采购列表：<c>stockList</c>。筛选：采购批次、采购账号（与 <c>fbmList</c> 一致）、产品编码下拉（含「全部」），查询时刷新选项；
    /// 业务员筛选仅管理员可见；非管理员按当前登录账号过滤。
    /// <c>type</c> 区分模块2～5：采购运输 / 到仓 / 滞销 / 售罄（与 <see cref="StockShipmentStatus"/> 一致）；
    /// 采购运输下支持删除（<c>stockRecordDel</c>），交互参考 FBM 采购列表。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class StockPurchaseViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        /// <summary>筛选：采购批次 <c>purId</c>。</summary>
        public string FilterPurId { get; set; }

        /// <summary>筛选：业务员 <c>buyer_name</c>；下拉含「全部」，来自 <c>roleList</c>。</summary>
        public ObservableCollection<RoleData> RoleSource { get; set; } = new ObservableCollection<RoleData>();

        public RoleData SelectRole { get; set; }

        /// <summary>筛选：采购账号 <c>purchaseAccount</c>；下拉含「全部」，来自 <c>accountList</c>（与 FBM 采购一致）。</summary>
        public ObservableCollection<string> AccountFilterList { get; set; } = new ObservableCollection<string>();

        public string SelectedFilterAccount { get; set; }

        /// <summary>筛选：产品编码 <c>p_id</c>；下拉含「全部」，来自 <c>stockManageList</c>。</summary>
        public ObservableCollection<string> ProductCodeFilterList { get; set; } = new ObservableCollection<string>();

        public string SelectedProductCode { get; set; }

        public bool IsAdmin { get; set; }

        /// <summary>筛选：列表必填 <c>type</c>（模块2～5 库存视图）。</summary>
        public int FilterShipmentType { get; set; } = (int)StockShipmentStatus.InTransit;

        /// <summary>库存视图芯片：与 <c>FilterShipmentType</c> 同步（默认模块2）。</summary>
        public bool statusTransit { get; set; } = true;
        public bool statusWarehouse { get; set; }
        public bool statusDeadstock { get; set; }
        public bool statusSoldOut { get; set; }

        /// <summary>
        /// 库存视图角标（与 <c>stockList</c> 返回的 <c>intransCount</c>～<c>outsaleCount</c> 对应）：
        /// 0 采购运输、1 到仓、2 滞销、3 售罄。
        /// </summary>
        public string StockViewBadge0 { get; set; } = "";
        public string StockViewBadge1 { get; set; } = "";
        public string StockViewBadge2 { get; set; } = "";
        public string StockViewBadge3 { get; set; } = "";

        /// <summary><c>stockList</c> 返回的 <c>sumAmount</c>：当前筛选条件下金额合计（元，可含小数）。</summary>
        public decimal SumAmount { get; set; }

        /// <summary>模块3～5 列表为只读查看；模块2 可新增与编辑。</summary>
        [DependsOn(nameof(FilterShipmentType))]
        public bool IsReadOnlyStockView =>
            FilterShipmentType == (int)StockShipmentStatus.ArrivedWarehouse
            || FilterShipmentType == (int)StockShipmentStatus.Deadstock
            || FilterShipmentType == (int)StockShipmentStatus.SoldOut;

        /// <summary>删除按钮是否可用：按当前库存视图与勾选行实时计算。</summary>
        public bool CanDeleteStockPurchase { get; set; }

        public BindableCollection<StockPurchaseRecordModel> RecordLst { get; set; } = new BindableCollection<StockPurchaseRecordModel>();

        public StockPurchaseRecordModel SelectItem { get; set; }

        public StockPurchaseViewModel(IWindowManager manager)
        {
            windowManager = manager;
            var info = IoC.Get<CacheInfo>();
            IsAdmin = info?.IsAdmin ?? false;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await RefreshRoleSourceAsync(resetSelection: true);
            await RefreshAccountFilterAsync(resetSelection: true);
            await RefreshProductCodeFilterAsync(resetSelection: true);
            await QueryBase(1);
        }

        /// <summary>从 <c>roleList</c> 刷新业务员下拉；查询时 <paramref name="resetSelection"/> 为 false 以保留当前选中（仍存在则不变）。</summary>
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

        /// <summary>从 <c>accountList</c> 刷新采购账号下拉（与 <see cref="FbmPurchaseViewModel.RefreshAccountFilterAsync"/> 一致）。</summary>
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

        /// <summary>从 <c>stockManageList</c> 刷新产品编码下拉。</summary>
        private async Task RefreshProductCodeFilterAsync(bool resetSelection)
        {
            var previous = SelectedProductCode;
            ProductCodeFilterList = new ObservableCollection<string>();
            ProductCodeFilterList.Add("全部");
            var data = await CRMRequest.StockManageList(1, 2000);
            if (data?.List != null)
            {
                foreach (var code in data.List.Select(x => x.ProductCode).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(x => x))
                {
                    ProductCodeFilterList.Add(code);
                }
            }

            if (!resetSelection && !string.IsNullOrWhiteSpace(previous) && ProductCodeFilterList.Contains(previous))
            {
                SelectedProductCode = previous;
            }
            else
            {
                SelectedProductCode = "全部";
            }
        }

        public async void Query()
        {
            await RefreshRoleSourceAsync(resetSelection: false);
            await RefreshAccountFilterAsync(resetSelection: false);
            await RefreshProductCodeFilterAsync(resetSelection: false);
            await QueryBase(1);
        }

        /// <summary>切换库存视图类型并重新查询第一页（与订单管理 <c>StateQuery</c> 类似）。</summary>
        public async void StockTypeQuery(int type)
        {
            FilterShipmentType = type;
            statusTransit = type == (int)StockShipmentStatus.InTransit;
            statusWarehouse = type == (int)StockShipmentStatus.ArrivedWarehouse;
            statusDeadstock = type == (int)StockShipmentStatus.Deadstock;
            statusSoldOut = type == (int)StockShipmentStatus.SoldOut;
            ReevaluateDeleteAvailability();
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
                var buyerName = "";
                if (!IsAdmin)
                {
                    buyerName = (IoC.Get<CacheInfo>().LoginAccount ?? "").Trim();
                }
                else if (SelectRole != null && SelectRole.Name != "全部")
                {
                    buyerName = (SelectRole.Name ?? "").Trim();
                }

                var productCode = "";
                if (!string.IsNullOrWhiteSpace(SelectedProductCode) && SelectedProductCode != "全部")
                {
                    productCode = SelectedProductCode.Trim();
                }

                var purchaseAccount = string.IsNullOrEmpty(SelectedFilterAccount) || SelectedFilterAccount == "全部"
                    ? ""
                    : SelectedFilterAccount.Trim();

                var result = await CRMRequest.StockList(
                    FilterShipmentType,
                    pageNum,
                    PageSizeConst,
                    productCode,
                    buyerName,
                    FilterPurId,
                    purchaseAccount);

                if (result != null)
                {
                    RecordLst = new BindableCollection<StockPurchaseRecordModel>(result.List ?? new List<StockPurchaseRecordModel>());
                    SumAmount = result.SumAmount;
                    var pages = (int)Math.Ceiling((result.Count * 1.0) / PageSizeConst);
                    PageInfo = new PageInfoModel
                    {
                        Total = result.Count,
                        PageNum = pageNum,
                        PageSize = PageSizeConst,
                        PagesCount = pages < 1 ? 1 : pages,
                    };
                    ApplyStockListBadges(result);
                    ReevaluateDeleteAvailability();
                }
                else
                {
                    RecordLst = new BindableCollection<StockPurchaseRecordModel>();
                    SumAmount = 0;
                    PageInfo = new PageInfoModel
                    {
                        Total = 0,
                        PageNum = 1,
                        PageSize = PageSizeConst,
                        PagesCount = 1,
                    };
                    ClearStockViewBadges();
                    ReevaluateDeleteAvailability();
                }
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        /// <summary>将 <c>stockList</c> 返回的四类库存计数填到角标（与 <see cref="StockPurchaseView"/> 芯片顺序一致）。</summary>
        private void ApplyStockListBadges(StockPurchaseListModel result)
        {
            StockViewBadge0 = result.IntransCount.ToString();
            StockViewBadge1 = result.InstockCount.ToString();
            StockViewBadge2 = result.UnsaleableCount.ToString();
            StockViewBadge3 = result.OutsaleCount.ToString();
        }

        private void ClearStockViewBadges()
        {
            StockViewBadge0 = StockViewBadge1 = StockViewBadge2 = StockViewBadge3 = "";
        }

        public async void Add()
        {
            var vm = new AddStockPurchaseViewModel(null, false, viewOnly: false);
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
                MessageBox.Show("请先勾选要删除的备货采购记录。");
                return;
            }

            if (!CanDeleteStockPurchase)
            {
                MessageBox.Show("当前勾选记录不满足删除条件。");
                return;
            }

            var label = string.IsNullOrWhiteSpace(checkedItem.PurId)
                ? $"ID {checkedItem.Id}"
                : $"采购批次「{checkedItem.PurId}」";
            if (MessageBox.Show($"确定删除 {label} 的这条记录吗？", "确认删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            IsProgressIndeterminate = true;
            try
            {
                var ok = await CRMRequest.StockPurchaseRecordDelete(checkedItem.Id);
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

        /// <summary>与 FBM 列表一致：同一时刻仅允许勾选一个。</summary>
        public void StockItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (RecordLst == null || RecordLst.Count == 0 || sender == null)
            {
                return;
            }

            if (((FrameworkElement)sender).DataContext is StockPurchaseRecordModel data)
            {
                foreach (var item in RecordLst)
                {
                    if (item.Id != data.Id && item.IsCheck)
                    {
                        item.IsCheck = false;
                    }
                }
            }

            ReevaluateDeleteAvailability();
        }

        private void ReevaluateDeleteAvailability()
        {
            var checkedItem = RecordLst?.FirstOrDefault(x => x.IsCheck);
            if (checkedItem == null || checkedItem.Id <= 0)
            {
                CanDeleteStockPurchase = false;
                return;
            }

            // 规则：
            // 1) 采购运输库存（type=0）可删；
            // 2) 到仓库存（type=1）仅 quantity == stayQuantity 可删；
            // 3) 其他库存类型均不可删。
            if (FilterShipmentType == (int)StockShipmentStatus.InTransit)
            {
                CanDeleteStockPurchase = true;
                return;
            }

            if (FilterShipmentType == (int)StockShipmentStatus.ArrivedWarehouse)
            {
                CanDeleteStockPurchase = checkedItem.Quantity == checkedItem.StayQuantity;
                return;
            }

            CanDeleteStockPurchase = false;
        }

        /// <summary>
        /// 与订单管理列表一致：双击「采购批次」复制批次号；双击其他列打开编辑/查看。
        /// </summary>
        public async void Record_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                var colName = grid.CurrentColumn?.Header?.ToString();
                if (colName == "采购批次")
                {
                    if (SelectItem != null && !string.IsNullOrEmpty(SelectItem.PurId))
                    {
                        Clipboard.SetText(SelectItem.PurId);
                    }

                    return;
                }
            }

            if (SelectItem == null)
            {
                return;
            }

            var vm = new AddStockPurchaseViewModel(SelectItem, true, viewOnly: IsReadOnlyStockView);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }
    }
}

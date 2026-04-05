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
    /// 备货采购列表：<c>stockList</c>。筛选：采购批次；业务员、产品编码为下拉（含「全部」），查询时刷新选项；
    /// <c>type</c> 区分模块2～5：采购运输 / 到仓 / 滞销 / 售罄（与 <see cref="StockShipmentStatus"/> 一致）。
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

        /// <summary>模块3～5 列表为只读查看；模块2 可新增与编辑。</summary>
        [DependsOn(nameof(FilterShipmentType))]
        public bool IsReadOnlyStockView =>
            FilterShipmentType == (int)StockShipmentStatus.ArrivedWarehouse
            || FilterShipmentType == (int)StockShipmentStatus.Deadstock
            || FilterShipmentType == (int)StockShipmentStatus.SoldOut;

        public BindableCollection<StockPurchaseRecordModel> RecordLst { get; set; } = new BindableCollection<StockPurchaseRecordModel>();

        public StockPurchaseRecordModel SelectItem { get; set; }

        public StockPurchaseViewModel(IWindowManager manager)
        {
            windowManager = manager;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await RefreshRoleSourceAsync(resetSelection: true);
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
                if (SelectRole != null && SelectRole.Name != "全部")
                {
                    buyerName = (SelectRole.Name ?? "").Trim();
                }

                var productCode = "";
                if (!string.IsNullOrWhiteSpace(SelectedProductCode) && SelectedProductCode != "全部")
                {
                    productCode = SelectedProductCode.Trim();
                }

                var result = await CRMRequest.StockList(
                    FilterShipmentType,
                    pageNum,
                    PageSizeConst,
                    productCode,
                    buyerName,
                    FilterPurId);

                if (result != null)
                {
                    RecordLst = new BindableCollection<StockPurchaseRecordModel>(result.List ?? new List<StockPurchaseRecordModel>());
                    var pages = (int)Math.Ceiling((result.Count * 1.0) / PageSizeConst);
                    PageInfo = new PageInfoModel
                    {
                        Total = result.Count,
                        PageNum = pageNum,
                        PageSize = PageSizeConst,
                        PagesCount = pages < 1 ? 1 : pages,
                    };
                    ApplyStockListBadges(result);
                }
                else
                {
                    RecordLst = new BindableCollection<StockPurchaseRecordModel>();
                    PageInfo = new PageInfoModel
                    {
                        Total = 0,
                        PageNum = 1,
                        PageSize = PageSizeConst,
                        PagesCount = 1,
                    };
                    ClearStockViewBadges();
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

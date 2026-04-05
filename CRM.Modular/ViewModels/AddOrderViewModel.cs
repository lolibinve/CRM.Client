using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    public sealed class PurchaseMethodPickItem
    {
        public int Value { get; set; }
        public string Display { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class AddOrderViewModel : Screen
    {
        public OrderData order { set; get; } = new OrderData();

        public string Title { set; get; }

        public bool IsAdmin { set; get; } = true;

        public bool AccountIsReadOnly { set; get; } = true;
        public bool TransExpenseIsEnable { set; get; } = true;

        public List<PurchaseMethodPickItem> PurchaseMethodItems { get; }

        public PurchaseMethodPickItem SelectedPurchaseMethod { get; set; }

        public string StayQuantityHint { get; set; } = "";

        [DependsOn(nameof(SelectedPurchaseMethod))]
        public bool ShowPurIdRow =>
            SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock
            || SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock;

        [DependsOn(nameof(SelectedPurchaseMethod))]
        public bool ShowPurIdTextRow => SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock;

        [DependsOn(nameof(SelectedPurchaseMethod))]
        public bool ShowPurIdComboRow => SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock;

        [DependsOn(nameof(SelectedPurchaseMethod))]
        public bool ShowShipQtyRow => SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock;

        public BindableCollection<string> DeadstockPurIdOptions { get; set; } = new BindableCollection<string>();

        public string SelectedDeadstockPurId { get; set; }

        private int _cachedStayQty;
        private decimal _cachedUnitCost;
        private bool _stockInfoLoaded;

        private bool _suppressPurchaseMethodChange = true;

        private readonly bool _isModify;
        private readonly int _originalPurchaseMethod;

        /// <summary>修改订单且原始数据中采购批次、发货数量均已有值时，锁定采购方式/采购批次/发货数量。</summary>
        public bool StockPurchaseLocked { get; }

        public bool CanEditStockPurchaseFields => !StockPurchaseLocked;

        private static List<PurchaseMethodPickItem> BuildPurchaseMethodItems(bool isModify)
        {
            var list = new List<PurchaseMethodPickItem>
            {
                new PurchaseMethodPickItem { Value = (int)OrderPurchaseMethod.Unselected, Display = "-请选择采购方式-" },
                new PurchaseMethodPickItem { Value = (int)OrderPurchaseMethod.Cash, Display = "现金采购" },
                new PurchaseMethodPickItem { Value = (int)OrderPurchaseMethod.Stock, Display = "使用备货" },
                new PurchaseMethodPickItem { Value = (int)OrderPurchaseMethod.Deadstock, Display = "滞留库存" },
            };
            if (isModify)
            {
                list.Add(new PurchaseMethodPickItem { Value = (int)OrderPurchaseMethod.ResellReturn, Display = "退回重售" });
            }

            return list;
        }

        public AddOrderViewModel(OrderData data, bool IsModify = false)
        {
            Title = IsModify ? "修改订单" : "新增订单";

            PurchaseMethodItems = BuildPurchaseMethodItems(IsModify);

            order.Clone(data);

            _isModify = IsModify;
            _originalPurchaseMethod = order.PurchaseMethod;

            StockPurchaseLocked = IsModify
                && !string.IsNullOrWhiteSpace((order.PurId ?? "").Trim())
                && order.ShipQuantity > 0;
            if (!IsModify)
            {
                order.Id = 0;
                order.State = OrderState.新单;
                order.SalesVolume = 0;
                order.SettleAmount = 0;
                order.SaleDate = DateTime.Now;
            }

            // 新增：默认不选“请选择采购方式”，避免加载时自动调接口。
            // 修改：需要回显采购方式/采购批次/发货数量，因此从 order 读取并同步到 SelectedPurchaseMethod。
            if (IsModify)
            {
                SelectedPurchaseMethod = PurchaseMethodItems.FirstOrDefault(x => x.Value == order.PurchaseMethod)
                                          ?? PurchaseMethodItems[0];
                order.PurchaseMethod = SelectedPurchaseMethod.Value;
            }
            else
            {
                SelectedPurchaseMethod = PurchaseMethodItems.FirstOrDefault(x => x.Value == (int)OrderPurchaseMethod.Unselected) ?? PurchaseMethodItems[0];
                order.PurchaseMethod = (int)OrderPurchaseMethod.Unselected;
            }

            _cachedStayQty = 0;
            _cachedUnitCost = 0;
            _stockInfoLoaded = false;

            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;
            if (!IsAdmin)
            {
                if (order.TransExpense != 0)
                {
                    TransExpenseIsEnable = false;
                }
            }
            else
            {
                AccountIsReadOnly = false;
            }

            order.PropertyChanged += OrderOnPropertyChanged;
            _ = InitAfterLoadAsync();
        }

        private void OrderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderData.TransExpense)
                || e.PropertyName == nameof(OrderData.BackAmount)
                || e.PropertyName == nameof(OrderData.SalesVolume))
            {
                RecalculateProfit();
            }

            if (e.PropertyName == nameof(OrderData.ShipQuantity)
                && SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock
                && !StockPurchaseLocked)
            {
                ApplyStockCostFromInputs();
                RecalculateProfit();
            }
        }

        private async Task InitAfterLoadAsync()
        {
            await Task.Yield();
            if (SelectedPurchaseMethod == null || SelectedPurchaseMethod.Value <= (int)OrderPurchaseMethod.Unselected)
            {
                _suppressPurchaseMethodChange = false;
                return;
            }

            // 采购方式只读时不调 FBM / stockInfo / 滞留批次列表等接口，保留服务端已回显的成本与批次信息。
            if (StockPurchaseLocked)
            {
                _suppressPurchaseMethodChange = false;
                return;
            }

            order.PurchaseMethod = SelectedPurchaseMethod.Value;
            switch ((OrderPurchaseMethod)SelectedPurchaseMethod.Value)
            {
                case OrderPurchaseMethod.Cash:
                    await LoadFbmCostAsync();
                    break;
                case OrderPurchaseMethod.Stock:
                    await RefreshStockInfoAsync();
                    break;
                case OrderPurchaseMethod.Deadstock:
                    order.Cost = 0;
                    // 回显“滞留库存”时需要先拉取下拉项，否则 ComboBox 可能为空且无法选中当前 purId。
                    await LoadDeadstockPurIdOptionsAsync();
                    await RefreshStockInfoAsync();
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.ResellReturn:
                    order.PurId = "";
                    order.ShipQuantity = 0;
                    order.Cost = 0;
                    SelectedDeadstockPurId = null;
                    RecalculateProfit();
                    break;
            }

            _suppressPurchaseMethodChange = false;
        }

        public async void RefreshPurchaseMethodLogic()
        {
            if (_suppressPurchaseMethodChange || SelectedPurchaseMethod == null)
            {
                return;
            }

            if (StockPurchaseLocked)
            {
                return;
            }

            order.PurchaseMethod = SelectedPurchaseMethod.Value;
            StayQuantityHint = "";
            _cachedStayQty = 0;
            _cachedUnitCost = 0;
            _stockInfoLoaded = false;

            if (SelectedPurchaseMethod.Value <= (int)OrderPurchaseMethod.Unselected)
            {
                order.PurId = "";
                order.ShipQuantity = 0;
                order.Cost = 0;
                RecalculateProfit();
                return;
            }

            switch ((OrderPurchaseMethod)SelectedPurchaseMethod.Value)
            {
                case OrderPurchaseMethod.Cash:
                    order.PurId = "";
                    order.ShipQuantity = 0;
                    await LoadFbmCostAsync();
                    break;
                case OrderPurchaseMethod.Stock:
                    order.Cost = 0;
                    // 选择“使用备货”时不拉接口：等用户填写采购批次后（LostFocus）再请求 stockInfoByPurId。
                    break;
                case OrderPurchaseMethod.Deadstock:
                    order.ShipQuantity = 0;
                    order.Cost = 0;
                    await LoadDeadstockPurIdOptionsAsync();
                    await RefreshStockInfoAsync();
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.ResellReturn:
                    order.PurId = "";
                    order.ShipQuantity = 0;
                    order.Cost = 0;
                    SelectedDeadstockPurId = null;
                    RecalculateProfit();
                    break;
            }
        }

        public async void OnPurIdLostFocus()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            await RefreshStockInfoAsync();
        }

        public async void OnShipQuantityLostFocus()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock)
            {
                // 若切到“使用备货”后还没加载过 stockInfo，但 PurId 已有值，则在用户输入发货数量时补拉一次，
                // 避免成本/库存校验因未加载接口数据而失效。
                if (!_stockInfoLoaded && !string.IsNullOrWhiteSpace(order.PurId))
                {
                    await RefreshStockInfoAsync();
                }

                if (_stockInfoLoaded && _cachedStayQty >= 0 && order.ShipQuantity > _cachedStayQty)
                {
                    MessageBox.Show("库存不足");
                    order.Cost = 0;
                    RecalculateProfit();
                    return;
                }

                ApplyStockCostFromInputs();
                RecalculateProfit();
            }
        }

        public async void OnDeadstockPurIdChanged()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (SelectedPurchaseMethod?.Value != (int)OrderPurchaseMethod.Deadstock)
            {
                return;
            }

            order.PurId = SelectedDeadstockPurId ?? "";
            await RefreshStockInfoAsync();
        }

        private async Task LoadFbmCostAsync()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(order.OrderNumber))
            {
                order.Cost = 0;
                RecalculateProfit();
                return;
            }

            var f = await CRMRequest.FbmInfoByOrderId(order.OrderNumber);
            if (f != null)
            {
                order.Cost = (float)(double)f.Expense;
            }

            RecalculateProfit();
        }

        private async Task RefreshStockInfoAsync()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (SelectedPurchaseMethod == null
                || (SelectedPurchaseMethod.Value != (int)OrderPurchaseMethod.Stock && SelectedPurchaseMethod.Value != (int)OrderPurchaseMethod.Deadstock))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(order.PurId))
            {
                StayQuantityHint = "";
                order.Cost = 0;
                _cachedStayQty = 0;
                _cachedUnitCost = 0;
                _stockInfoLoaded = false;
                RecalculateProfit();
                return;
            }

            var info = await CRMRequest.StockInfoByPurId(order.PurId);
            if (info == null)
            {
                _cachedStayQty = 0;
                _cachedUnitCost = 0;
                _stockInfoLoaded = false;
                return;
            }

            _cachedStayQty = info.StayQuantity;
            _cachedUnitCost = info.UnitCost;
            _stockInfoLoaded = true;
            StayQuantityHint = $"剩余库存: {_cachedStayQty}";

            if (SelectedPurchaseMethod.Value == (int)OrderPurchaseMethod.Deadstock)
            {
                order.Cost = 0;
            }
            else
            {
                // 若用户已输入“发货数量”，此时拿到接口数据后立刻校验并提示。
                if (order.ShipQuantity > _cachedStayQty)
                {
                    MessageBox.Show("库存不足");
                    order.Cost = 0;
                    RecalculateProfit();
                    return;
                }
                ApplyStockCostFromInputs();
            }

            RecalculateProfit();
        }

        private async Task LoadDeadstockPurIdOptionsAsync()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            var previous = order.PurId;
            DeadstockPurIdOptions = new BindableCollection<string>();
            SelectedDeadstockPurId = null;
            order.PurId = "";

            var list = await CRMRequest.StockStalePurIdList();
            if (list == null)
            {
                return;
            }

            foreach (var item in list.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
            {
                DeadstockPurIdOptions.Add(item.Trim());
            }

            if (!string.IsNullOrWhiteSpace(previous))
            {
                var matched = DeadstockPurIdOptions.FirstOrDefault(x => string.Equals(x, previous.Trim(), StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(matched))
                {
                    SelectedDeadstockPurId = matched;
                    order.PurId = matched;
                }
            }
        }

        private void ApplyStockCostFromInputs()
        {
            if (order.ShipQuantity <= 0 || _cachedUnitCost < 0)
            {
                order.Cost = 0;
                return;
            }

            order.Cost = (float)(_cachedUnitCost * order.ShipQuantity);
        }

        private void RecalculateProfit()
        {
            order.Profit = order.SalesVolume - order.Cost - order.TransExpense - order.BackAmount;
            if (order.SalesVolume > 0.0001f)
            {
                order.ProfitRate = order.Profit / order.SalesVolume * 100f;
            }
            else
            {
                order.ProfitRate = 0;
            }
        }

        public async void Sure()
        {
            if (SelectedPurchaseMethod == null || SelectedPurchaseMethod.Value <= (int)OrderPurchaseMethod.Unselected)
            {
                MessageBox.Show("请选择采购方式");
                return;
            }

            order.PurchaseMethod = SelectedPurchaseMethod.Value;

            switch ((OrderPurchaseMethod)SelectedPurchaseMethod.Value)
            {
                case OrderPurchaseMethod.Cash:
                    break;
                case OrderPurchaseMethod.Stock:
                    if (string.IsNullOrWhiteSpace(order.PurId))
                    {
                        MessageBox.Show("请填写采购批次");
                        return;
                    }

                    if (order.ShipQuantity <= 0)
                    {
                        MessageBox.Show("请填写发货数量");
                        return;
                    }

                    if (StockPurchaseLocked)
                    {
                        RecalculateProfit();
                        break;
                    }

                    await RefreshStockInfoAsync();
                    if (order.ShipQuantity > _cachedStayQty)
                    {
                        MessageBox.Show("库存不足");
                        return;
                    }

                    ApplyStockCostFromInputs();
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.Deadstock:
                    if (string.IsNullOrWhiteSpace(order.PurId))
                    {
                        MessageBox.Show("请填写采购批次");
                        return;
                    }

                    order.Cost = 0;
                    if (!StockPurchaseLocked)
                    {
                        await RefreshStockInfoAsync();
                    }

                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.ResellReturn:
                    order.PurId = "";
                    order.ShipQuantity = 0;
                    order.Cost = 0;
                    SelectedDeadstockPurId = null;
                    RecalculateProfit();
                    break;
            }

            var useStock = _isModify && SelectedPurchaseMethod.Value != _originalPurchaseMethod ? 1 : 0;
            var result = await CRMRequest.AddOrder(order, useStock);
            if (result)
            {
                var temp = GetView();
                if (temp is Window win)
                {
                    win.DialogResult = true;
                }

                await TryCloseAsync();
            }
        }

        public Task CloseForm()
        {
            var temp = GetView();
            if (temp is Window win)
            {
                win.DialogResult = false;
            }

            return TryCloseAsync();
        }
    }
}

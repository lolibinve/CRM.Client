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
using System.Windows.Threading;

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
        private bool _isSubmitting;

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

        [DependsOn(nameof(SelectedPurchaseMethod), nameof(StockPurchaseLocked))]
        public bool ShowPurIdStockComboRow =>
            SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock && !StockPurchaseLocked;

        [DependsOn(nameof(SelectedPurchaseMethod), nameof(StockPurchaseLocked))]
        public bool ShowPurIdStockLockedRow =>
            SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock && StockPurchaseLocked;

        [DependsOn(nameof(SelectedPurchaseMethod))]
        public bool ShowPurIdComboRow => SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock;

        [DependsOn(nameof(SelectedPurchaseMethod))]
        public bool ShowShipQtyRow =>
            SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock
            || SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock;

        public BindableCollection<string> StockPurIdOptions { get; set; } = new BindableCollection<string>();

        public string SelectedStockPurId { get; set; }

        public BindableCollection<string> DeadstockPurIdOptions { get; set; } = new BindableCollection<string>();

        public string SelectedDeadstockPurId { get; set; }

        private int _cachedStayQty;
        private decimal _cachedUnitCost;
        private bool _stockInfoLoaded;

        private bool _suppressPurchaseMethodChange = true;

        private readonly bool _isModify;
        private readonly int _originalPurchaseMethod;

        /// <summary>修改订单：异步初始化完成后的订单快照，用于判断是否与提交前一致（一致则不调用后端）。</summary>
        private OrderData _modifyBaselineOrder;

        /// <summary>修改订单且原始数据中采购批次、发货数量均已有值时，锁定采购方式/采购批次/发货数量。</summary>
        public bool StockPurchaseLocked { get; }

        /// <summary>退回重售 / 滞留库存（滞销批次）模式下成本默认占位（元），原为 0 时改为非零。</summary>
        private const float DefaultCostResellOrDeadstock = 0.01f;

        /// <summary>采购批次下拉首项，未选真实批次时展示此项；不写入 <see cref="OrderData.PurId"/>。</summary>
        private const string PurIdSelectPlaceholder = "-请选择-";

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
            try
            {
                if (SelectedPurchaseMethod == null || SelectedPurchaseMethod.Value <= (int)OrderPurchaseMethod.Unselected)
                {
                    _suppressPurchaseMethodChange = false;
                    return;
                }

                // 采购方式只读时不调 FBM / stockInfo / 滞留批次列表等接口，保留服务端已回显的成本与批次信息。
                // 仍需 HydratePurchaseMethodEchoUiOnly：用已有 order.PurId 填充采购批次下拉（滞销库存等），不请求后端。
                if (StockPurchaseLocked)
                {
                    order.PurchaseMethod = SelectedPurchaseMethod.Value;
                    HydratePurchaseMethodEchoUiOnly();
                    _suppressPurchaseMethodChange = false;
                    return;
                }

                // 回显阶段不调 FBM / 批次列表 / stockInfo；仅用户操作采购方式下拉后再走 RefreshPurchaseMethodLogic 请求后端。
                order.PurchaseMethod = SelectedPurchaseMethod.Value;
                HydratePurchaseMethodEchoUiOnly();

                if (Application.Current?.Dispatcher != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                }

                _suppressPurchaseMethodChange = false;
            }
            finally
            {
                if (_isModify)
                {
                    _modifyBaselineOrder = new OrderData();
                    _modifyBaselineOrder.Clone(order);
                }
            }
        }

        /// <summary>
        /// 修改订单打开时回显：仅根据已有 <see cref="OrderData"/> 填充下拉与利润，不请求后端。
        /// 采购方式相关接口仅在用户 <see cref="RefreshPurchaseMethodLogic"/> 变更下拉后触发。
        /// </summary>
        private void HydratePurchaseMethodEchoUiOnly()
        {
            switch ((OrderPurchaseMethod)SelectedPurchaseMethod.Value)
            {
                case OrderPurchaseMethod.Cash:
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.Stock:
                    StayQuantityHint = "";
                    _cachedStayQty = 0;
                    _cachedUnitCost = 0;
                    _stockInfoLoaded = false;
                    HydratePurIdComboForEcho(stockType: 1);
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.Deadstock:
                    StayQuantityHint = "";
                    _cachedStayQty = 0;
                    _cachedUnitCost = 0;
                    _stockInfoLoaded = false;
                    HydratePurIdComboForEcho(stockType: 2);
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.ResellReturn:
                    SelectedStockPurId = null;
                    SelectedDeadstockPurId = null;
                    RecalculateProfit();
                    break;
            }
        }

        /// <param name="stockType">1=备货；2=滞留库存</param>
        private void HydratePurIdComboForEcho(int stockType)
        {
            var options = new BindableCollection<string> { PurIdSelectPlaceholder };
            var pid = (order.PurId ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(pid) && !string.Equals(pid, PurIdSelectPlaceholder, StringComparison.Ordinal))
            {
                if (!options.Any(x => string.Equals(x, pid, StringComparison.OrdinalIgnoreCase)))
                {
                    options.Add(pid);
                }
            }

            if (stockType == 1)
            {
                StockPurIdOptions = options;
                if (string.IsNullOrWhiteSpace(pid) || string.Equals(pid, PurIdSelectPlaceholder, StringComparison.Ordinal))
                {
                    SelectedStockPurId = PurIdSelectPlaceholder;
                }
                else
                {
                    var matched = options.FirstOrDefault(x => string.Equals(x, pid, StringComparison.OrdinalIgnoreCase));
                    SelectedStockPurId = matched ?? PurIdSelectPlaceholder;
                    if (matched != null && !string.Equals(order.PurId, matched, StringComparison.Ordinal))
                    {
                        order.PurId = matched;
                    }
                }
            }
            else if (stockType == 2)
            {
                DeadstockPurIdOptions = options;
                if (string.IsNullOrWhiteSpace(pid) || string.Equals(pid, PurIdSelectPlaceholder, StringComparison.Ordinal))
                {
                    SelectedDeadstockPurId = PurIdSelectPlaceholder;
                }
                else
                {
                    var matched = options.FirstOrDefault(x => string.Equals(x, pid, StringComparison.OrdinalIgnoreCase));
                    SelectedDeadstockPurId = matched ?? PurIdSelectPlaceholder;
                    if (matched != null && !string.Equals(order.PurId, matched, StringComparison.Ordinal))
                    {
                        order.PurId = matched;
                    }
                }
            }
        }

        private static bool FloatEq(float x, float y) => Math.Abs(x - y) < 0.0001f;

        /// <summary>与 <see cref="CRMRequest.AddOrder"/> 提交字段一致的数据是否相对基线无变化。</summary>
        private static bool OrderMatchesModifyBaseline(OrderData current, OrderData baseline)
        {
            if (current == null || baseline == null)
            {
                return false;
            }

            return current.Id == baseline.Id
                && string.Equals(current.Store ?? "", baseline.Store ?? "", StringComparison.Ordinal)
                && Nullable.Equals(current.SaleDate, baseline.SaleDate)
                && string.Equals(current.OrderNumber ?? "", baseline.OrderNumber ?? "", StringComparison.Ordinal)
                && string.Equals(current.SKU ?? "", baseline.SKU ?? "", StringComparison.Ordinal)
                && string.Equals(current.Account ?? "", baseline.Account ?? "", StringComparison.Ordinal)
                && string.Equals(current.Country ?? "", baseline.Country ?? "", StringComparison.Ordinal)
                && string.Equals(current.MoneyType ?? "", baseline.MoneyType ?? "", StringComparison.Ordinal)
                && string.Equals(current.ImageBase64Str ?? "", baseline.ImageBase64Str ?? "", StringComparison.Ordinal)
                && FloatEq(current.SettleAmount, baseline.SettleAmount)
                && FloatEq(current.ExchangeRafe, baseline.ExchangeRafe)
                && FloatEq(current.ExchangeAmount, baseline.ExchangeAmount)
                && FloatEq(current.BackExchange, baseline.BackExchange)
                && FloatEq(current.SalesVolume, baseline.SalesVolume)
                && FloatEq(current.Cost, baseline.Cost)
                && FloatEq(current.TransExpense, baseline.TransExpense)
                && FloatEq(current.BackAmount, baseline.BackAmount)
                && FloatEq(current.Profit, baseline.Profit)
                && FloatEq(current.ProfitRate, baseline.ProfitRate)
                && current.State == baseline.State
                && current.IsImport == baseline.IsImport
                && current.IsImageUrl == baseline.IsImageUrl
                && string.Equals(current.Remark ?? "", baseline.Remark ?? "", StringComparison.Ordinal)
                && string.Equals(current.Buyer ?? "", baseline.Buyer ?? "", StringComparison.Ordinal)
                && string.Equals(current.Phone ?? "", baseline.Phone ?? "", StringComparison.Ordinal)
                && string.Equals(current.QuantityPurchased ?? "", baseline.QuantityPurchased ?? "", StringComparison.Ordinal)
                && current.PurchaseMethod == baseline.PurchaseMethod
                && string.Equals((current.PurId ?? "").Trim(), (baseline.PurId ?? "").Trim(), StringComparison.Ordinal)
                && current.ShipQuantity == baseline.ShipQuantity
                && current.IsCheck == baseline.IsCheck;
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
                SelectedStockPurId = null;
                SelectedDeadstockPurId = null;
                RecalculateProfit();
                return;
            }

            switch ((OrderPurchaseMethod)SelectedPurchaseMethod.Value)
            {
                case OrderPurchaseMethod.Cash:
                    order.PurId = "";
                    order.ShipQuantity = 0;
                    SelectedStockPurId = null;
                    SelectedDeadstockPurId = null;
                    await LoadFbmCostAsync();
                    break;
                case OrderPurchaseMethod.Stock:
                    order.Cost = 0;
                    await LoadStockTypePurIdOptionsAsync(1);
                    await RefreshStockInfoAsync();
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.Deadstock:
                    order.ShipQuantity = 0;
                    order.Cost = DefaultCostResellOrDeadstock;
                    await LoadStockTypePurIdOptionsAsync(2);
                    await RefreshStockInfoAsync();
                    RecalculateProfit();
                    break;
                case OrderPurchaseMethod.ResellReturn:
                    order.PurId = "";
                    order.ShipQuantity = 0;
                    order.Cost = DefaultCostResellOrDeadstock;
                    SelectedStockPurId = null;
                    SelectedDeadstockPurId = null;
                    RecalculateProfit();
                    break;
            }
        }

        public async void OnShipQuantityLostFocus()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Stock
                || SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock)
            {
                // 若切到备货/滞销后还没加载过 stockInfo，但 PurId 已有值，则在用户输入发货数量时补拉一次，
                // 避免成本/库存校验因未加载接口数据而失效。
                if (!_stockInfoLoaded && !string.IsNullOrWhiteSpace(order.PurId))
                {
                    await RefreshStockInfoAsync();
                }

                if (_stockInfoLoaded && _cachedStayQty >= 0 && order.ShipQuantity > _cachedStayQty)
                {
                    MessageBox.Show("库存不足");
                    if (SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock)
                    {
                        order.Cost = DefaultCostResellOrDeadstock;
                    }
                    else
                    {
                        order.Cost = 0;
                    }

                    RecalculateProfit();
                    return;
                }

                if (SelectedPurchaseMethod?.Value == (int)OrderPurchaseMethod.Deadstock)
                {
                    order.Cost = DefaultCostResellOrDeadstock;
                    RecalculateProfit();
                }
                else
                {
                    ApplyStockCostFromInputs();
                    RecalculateProfit();
                }
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

            order.PurId = PurIdFromComboSelection(SelectedDeadstockPurId);
            await RefreshStockInfoAsync();
        }

        public async void OnStockPurIdChanged()
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (SelectedPurchaseMethod?.Value != (int)OrderPurchaseMethod.Stock)
            {
                return;
            }

            order.PurId = PurIdFromComboSelection(SelectedStockPurId);
            await RefreshStockInfoAsync();
        }

        private static string PurIdFromComboSelection(string selected)
        {
            if (string.IsNullOrWhiteSpace(selected))
            {
                return "";
            }

            return string.Equals(selected.Trim(), PurIdSelectPlaceholder, StringComparison.Ordinal)
                ? ""
                : selected.Trim();
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
                if (SelectedPurchaseMethod.Value == (int)OrderPurchaseMethod.Stock)
                {
                    order.Cost = 0f;
                }
                else
                {
                    order.Cost = DefaultCostResellOrDeadstock;
                }

                _cachedStayQty = 0;
                _cachedUnitCost = 0;
                _stockInfoLoaded = false;
                RecalculateProfit();
                return;
            }

            var isDeadstock = SelectedPurchaseMethod.Value == (int)OrderPurchaseMethod.Deadstock;
            var info = await CRMRequest.StockInfoByPurId(order.PurId);
            if (info == null)
            {
                _cachedStayQty = 0;
                _cachedUnitCost = 0;
                _stockInfoLoaded = false;
                if (isDeadstock)
                {
                    order.Cost = DefaultCostResellOrDeadstock;
                    RecalculateProfit();
                }

                return;
            }

            _cachedStayQty = info.StayQuantity;
            _cachedUnitCost = info.UnitCost;
            _stockInfoLoaded = true;
            StayQuantityHint = $"剩余库存: {_cachedStayQty}";

            if (order.ShipQuantity > _cachedStayQty)
            {
                MessageBox.Show("库存不足");
                if (isDeadstock)
                {
                    order.Cost = DefaultCostResellOrDeadstock;
                }
                else
                {
                    order.Cost = 0;
                }

                RecalculateProfit();
                return;
            }

            if (isDeadstock)
            {
                order.Cost = DefaultCostResellOrDeadstock;
                RecalculateProfit();
                return;
            }

            ApplyStockCostFromInputs();
            RecalculateProfit();
        }

        private async Task LoadStockTypePurIdOptionsAsync(int type)
        {
            if (StockPurchaseLocked)
            {
                return;
            }

            if (type != 1 && type != 2)
            {
                return;
            }

            var previous = order.PurId;
            if (type == 1)
            {
                StockPurIdOptions = new BindableCollection<string>();
            }
            else
            {
                DeadstockPurIdOptions = new BindableCollection<string>();
            }

            var options = type == 1 ? StockPurIdOptions : DeadstockPurIdOptions;
            order.PurId = "";

            options.Add(PurIdSelectPlaceholder);

            var loginAccount = IoC.Get<CacheInfo>()?.LoginAccount;
            var list = await CRMRequest.StockTypePurIdList(type, loginAccount);
            if (list != null)
            {
                foreach (var item in list.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                {
                    var t = item.Trim();
                    if (string.Equals(t, PurIdSelectPlaceholder, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    options.Add(t);
                }
            }

            if (!string.IsNullOrWhiteSpace(previous))
            {
                var prevTrim = previous.Trim();
                if (!string.Equals(prevTrim, PurIdSelectPlaceholder, StringComparison.Ordinal))
                {
                    var matched = options.FirstOrDefault(x => string.Equals(x, prevTrim, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(matched))
                    {
                        if (type == 1)
                        {
                            SelectedStockPurId = matched;
                        }
                        else
                        {
                            SelectedDeadstockPurId = matched;
                        }

                        order.PurId = matched;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(order.PurId))
            {
                if (type == 1)
                {
                    SelectedStockPurId = PurIdSelectPlaceholder;
                }
                else
                {
                    SelectedDeadstockPurId = PurIdSelectPlaceholder;
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
            if (_isSubmitting)
            {
                return;
            }

            _isSubmitting = true;
            try
            {
                if (SelectedPurchaseMethod == null || SelectedPurchaseMethod.Value <= (int)OrderPurchaseMethod.Unselected)
                {
                    MessageBox.Show("请选择采购方式");
                    return;
                }

                order.PurchaseMethod = SelectedPurchaseMethod.Value;

                // 必须在下面 switch 之前判断：switch 会拉库存、重算成本等，会改 order 并在备货场景先请求后端。
                if (_isModify
                    && _modifyBaselineOrder != null
                    && OrderMatchesModifyBaseline(order, _modifyBaselineOrder))
                {
                    // 未调用后端，按「取消」关闭，父窗口通常只对 true 做列表刷新。
                    var viewNoOp = GetView();
                    if (viewNoOp is Window winNoOp)
                    {
                        winNoOp.DialogResult = false;
                    }

                    await TryCloseAsync();
                    return;
                }

                switch ((OrderPurchaseMethod)SelectedPurchaseMethod.Value)
                {
                    case OrderPurchaseMethod.Cash:
                        break;
                    case OrderPurchaseMethod.Stock:
                        if (string.IsNullOrWhiteSpace(order.PurId))
                        {
                            MessageBox.Show("请选择采购批次");
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
                            MessageBox.Show("请选择采购批次");
                            return;
                        }

                        if (order.ShipQuantity <= 0)
                        {
                            MessageBox.Show("请填写发货数量");
                            return;
                        }

                        if (StockPurchaseLocked)
                        {
                            order.Cost = DefaultCostResellOrDeadstock;
                            RecalculateProfit();
                            break;
                        }

                        await RefreshStockInfoAsync();
                        if (order.ShipQuantity > _cachedStayQty)
                        {
                            MessageBox.Show("库存不足");
                            return;
                        }

                        order.Cost = DefaultCostResellOrDeadstock;
                        RecalculateProfit();
                        break;
                    case OrderPurchaseMethod.ResellReturn:
                        order.PurId = "";
                        order.ShipQuantity = 0;
                        order.Cost = DefaultCostResellOrDeadstock;
                        SelectedStockPurId = null;
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
            finally
            {
                _isSubmitting = false;
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

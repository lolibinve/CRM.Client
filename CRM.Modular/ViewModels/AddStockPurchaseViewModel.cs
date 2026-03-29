using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    public sealed class ShipmentPickItem
    {
        public int Value { get; set; }
        public string Display { get; set; }
    }

    /// <summary>
    /// 备货采购新增/编辑：<c>stockEdit</c>；产品编码来自备货汇总（<c>stockManageList</c>）。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class AddStockPurchaseViewModel : Screen
    {
        public List<PaymentPickItem> PaymentItems { get; } = new List<PaymentPickItem>
        {
            new PaymentPickItem { Value = StockPurchaseConstants.PaymentCash, Display = "现金支付" },
            new PaymentPickItem { Value = StockPurchaseConstants.PaymentCredit, Display = "诚意赊" },
        };

        public PaymentPickItem SelectedPayment { get; set; }

        public List<ShipmentPickItem> ShipmentItems { get; } = new List<ShipmentPickItem>
        {
            new ShipmentPickItem { Value = StockPurchaseConstants.ShipmentInTransit, Display = "采购运输中" },
            new ShipmentPickItem { Value = StockPurchaseConstants.ShipmentArrived, Display = "货件到仓" },
        };

        public ShipmentPickItem SelectedShipment { get; set; }

        public StockPurchaseRecordModel Record { get; set; } = new StockPurchaseRecordModel();

        /// <summary>备货产品（模块一）下拉数据源。</summary>
        public BindableCollection<StockProductRecordModel> ProductOptions { get; set; } = new BindableCollection<StockProductRecordModel>();

        public StockProductRecordModel SelectedProduct { get; set; }

        /// <summary>采购账号下拉，与 <see cref="AddFbmPurchaseViewModel"/> 一致。</summary>
        public BindableCollection<string> AccountOptions { get; set; } = new BindableCollection<string>();

        public string Title { get; set; }

        public bool IsModify { get; }

        /// <summary>模块3、4 列表进入时为只读，不可保存。</summary>
        public bool IsViewOnly { get; }

        [DependsOn(nameof(IsViewOnly))]
        public bool CanEdit => !IsViewOnly;

        [DependsOn(nameof(SelectedShipment))]
        //[DependsOn(nameof(IsViewOnly))]
        public bool IsInstockEnabled => !IsViewOnly && SelectedShipment?.Value == StockPurchaseConstants.ShipmentArrived;

        public AddStockPurchaseViewModel(StockPurchaseRecordModel data, bool isModify, bool viewOnly = false)
        {
            IsModify = isModify;
            IsViewOnly = viewOnly;
            if (viewOnly && isModify)
            {
                Title = "查看备货采购";
            }
            else
            {
                Title = isModify ? "修改备货采购" : "新增备货采购";
            }
            var info = IoC.Get<CacheInfo>();

            if (isModify && data != null)
            {
                Record = Clone(data);
            }
            else
            {
                Record = new StockPurchaseRecordModel
                {
                    PurchaseDate = DateTime.Now.Date,
                    ShipmentType = StockPurchaseConstants.ShipmentInTransit,
                    Payment = StockPurchaseConstants.PaymentCash,
                    UserName = info?.LoginAccount ?? "",
                };
            }

            SelectedPayment = PaymentItems.FirstOrDefault(p => p.Value == Record.Payment) ?? PaymentItems[0];
            SelectedShipment = ShipmentItems.FirstOrDefault(s => s.Value == Record.ShipmentType) ?? ShipmentItems[0];

            _ = LoadProductOptionsAsync();
            _ = LoadAccountOptionsAsync();
            SyncSelectedProductFromRecord();
        }

        private void SyncSelectedProductFromRecord()
        {
            if (string.IsNullOrWhiteSpace(Record.ProductCode))
            {
                SelectedProduct = null;
                return;
            }

            SelectedProduct = ProductOptions.FirstOrDefault(p => string.Equals(p.ProductCode, Record.ProductCode, StringComparison.OrdinalIgnoreCase));
        }

        private async Task LoadProductOptionsAsync()
        {
            ProductOptions.Clear();
            var data = await CRMRequest.StockManageList(1);
            if (data?.List != null)
            {
                foreach (var row in data.List.OrderBy(x => x.ProductCode))
                {
                    ProductOptions.Add(row);
                }
            }

            SyncSelectedProductFromRecord();
        }

        private async Task LoadAccountOptionsAsync()
        {
            AccountOptions.Clear();
            var data = await CRMRequest.PurchaseAccountList(1, 2000);
            if (data?.AccountLst != null)
            {
                foreach (var n in data.AccountLst.Select(x => x.ProcurementAccount).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(x => x))
                {
                    AccountOptions.Add(n);
                }
            }

            if (!string.IsNullOrEmpty(Record.PurchaseAccount) && !AccountOptions.Contains(Record.PurchaseAccount))
            {
                AccountOptions.Insert(0, Record.PurchaseAccount);
            }
        }

        public void OnSelectedProductChanged()
        {
            if (SelectedProduct != null)
            {
                Record.ProductCode = SelectedProduct.ProductCode;
                Record.ProductName = SelectedProduct.ProductName;
            }
        }

        public void OnSelectedShipmentChanged()
        {
            if (SelectedShipment == null)
            {
                return;
            }

            Record.ShipmentType = SelectedShipment.Value;
            if (SelectedShipment.Value == StockPurchaseConstants.ShipmentArrived)
            {
                if (!Record.InstockDateTime.HasValue)
                    Record.InstockDateTime = DateTime.Now;
            }
            else
            {
                Record.InstockDateTime = null;
            }
        }

        public async void Sure()
        {
            if (IsViewOnly)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Record.ProductCode))
            {
                MessageBox.Show("请选择产品编码（来自备货产品）");
                return;
            }

            if (string.IsNullOrWhiteSpace(Record.ProductName))
            {
                MessageBox.Show("产品名称不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(Record.PurchaseAccount))
            {
                MessageBox.Show("请选择采购账号");
                return;
            }

            if (Record.Quantity <= 0)
            {
                MessageBox.Show("请输入有效的采购数量");
                return;
            }

            if (SelectedPayment != null)
            {
                Record.Payment = SelectedPayment.Value;
            }

            if (SelectedShipment != null)
            {
                Record.ShipmentType = SelectedShipment.Value;
            }

            if (Record.ShipmentType == StockPurchaseConstants.ShipmentArrived && !Record.InstockDateTime.HasValue)
            {
                Record.InstockDateTime = DateTime.Now;
            }

            if (Record.ShipmentType == StockPurchaseConstants.ShipmentInTransit)
            {
                Record.InstockDateTime = null;
            }

            if (Record.ShipmentType == StockPurchaseConstants.ShipmentArrived && Record.TransFee <= 0)
            {
                MessageBox.Show("缺少运费");
                return;
            }

            if (!Record.PurchaseDate.HasValue)
            {
                Record.PurchaseDate = DateTime.Now.Date;
            }

            var ok = await CRMRequest.StockPurchaseEdit(Record);
            if (!ok)
            {
                return;
            }

            var temp = GetView();
            if (temp is Window win)
            {
                win.DialogResult = true;
            }

            await TryCloseAsync();
        }

        public Task CloseForm()
        {
            var temp = GetView();
            if (temp is Window w)
            {
                w.DialogResult = false;
            }

            return TryCloseAsync();
        }

        private static StockPurchaseRecordModel Clone(StockPurchaseRecordModel s)
        {
            var r = new StockPurchaseRecordModel
            {
                Id = s.Id,
                PurId = s.PurId,
                AddTimeToken = s.AddTimeToken,
                ProductCode = s.ProductCode,
                ProductName = s.ProductName,
                Quantity = s.Quantity,
                Expense = s.Expense,
                UnitValue = s.UnitValue,
                UnitCost = s.UnitCost,
                Payment = s.Payment,
                TransFee = s.TransFee,
                UnitTransFee = s.UnitTransFee,
                UserName = s.UserName,
                PurchaseAccount = s.PurchaseAccount,
                ShipmentType = s.ShipmentType,
                InstockDateTime = s.InstockDateTime,
                Remark = s.Remark,
            };
            r.SyncPurchaseDateFromAddTime();
            if (!r.PurchaseDate.HasValue && s.PurchaseDate.HasValue)
            {
                r.PurchaseDate = s.PurchaseDate;
            }

            return r;
        }
    }
}

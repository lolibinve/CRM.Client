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

namespace CRM.Modular.ViewModels
{
    public sealed class PaymentPickItem
    {
        public int Value { get; set; }
        public string Display { get; set; }
    }

    /// <summary>
    /// 新增/编辑 FBM 采购记录，对接 <c>fbmEdit</c>。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class AddFbmPurchaseViewModel : Screen
    {
        public List<PaymentPickItem> PaymentItems { get; } = new List<PaymentPickItem>
        {
            new PaymentPickItem { Value = 0, Display = "现金支付" },
            new PaymentPickItem { Value = 1, Display = "账期" },
            new PaymentPickItem { Value = 2, Display = "诚意赊" },
        };

        public PaymentPickItem SelectedPayment { get; set; }

        public FbmPurchaseRecordModel Record { get; set; } = new FbmPurchaseRecordModel();

        public BindableCollection<string> AccountOptions { get; set; } = new BindableCollection<string>();

        public string Title { get; set; }

        public AddFbmPurchaseViewModel(FbmPurchaseRecordModel data, bool isModify)
        {
            Title = isModify ? "修改 FBM 采购记录" : "新增 FBM 采购记录";
            if (isModify && data != null)
            {
                Record = Clone(data);
            }
            else
            {
                var info = IoC.Get<CacheInfo>();
                Record = new FbmPurchaseRecordModel
                {
                    PurchaseDate = DateTime.Now.Date,
                    BuyerName = info?.LoginAccount,
                    Payment = 0,
                };
            }

            SelectedPayment = PaymentItems.FirstOrDefault(p => p.Value == Record.Payment) ?? PaymentItems[0];
            _ = LoadAccountOptionsAsync();
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

            if (!string.IsNullOrEmpty(Record.AccountName) && !AccountOptions.Contains(Record.AccountName))
            {
                AccountOptions.Insert(0, Record.AccountName);
            }
        }

        public async void Sure()
        {
            if (string.IsNullOrWhiteSpace(Record.OrderId))
            {
                MessageBox.Show("请填写订单号");
                return;
            }

            if (!Record.PurchaseDate.HasValue)
            {
                MessageBox.Show("请选择采购时间");
                return;
            }

            if (string.IsNullOrWhiteSpace(Record.AccountName))
            {
                MessageBox.Show("请选择采购账号");
                return;
            }

            if (SelectedPayment != null)
            {
                Record.Payment = SelectedPayment.Value;
            }

            var ok = await CRMRequest.FbmPurchaseEdit(Record);
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
            if (temp is Window win)
            {
                win.DialogResult = false;
            }

            return TryCloseAsync();
        }

        private static FbmPurchaseRecordModel Clone(FbmPurchaseRecordModel s)
        {
            return new FbmPurchaseRecordModel
            {
                Id = s.Id,
                OrderId = s.OrderId,
                PurchaseDate = s.PurchaseDate,
                Expense = s.Expense,
                BuyerName = s.BuyerName,
                AccountName = s.AccountName,
                Payment = s.Payment,
                Remark = s.Remark,
            };
        }
    }
}

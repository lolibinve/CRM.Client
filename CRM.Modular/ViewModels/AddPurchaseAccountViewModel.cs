using Caliburn.Micro;
using CRM.Model;
using HttpLib;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddPurchaseAccountViewModel : Screen
    {
        public ProcurementAccountLstModel Account { get; set; } = new ProcurementAccountLstModel();
        public string Title { get; set; }

        /// <summary>资金类型：现金存入 = 0。</summary>
        public bool IsCashType
        {
            get => Account.AccountType == 0;
            set
            {
                if (value)
                {
                    Account.AccountType = 0;
                    RefreshFundTypeUi();
                }
            }
        }

        /// <summary>资金类型：账期/诚意赊 = 1。</summary>
        public bool IsCreditType
        {
            get => Account.AccountType == 1;
            set
            {
                if (value)
                {
                    Account.AccountType = 1;
                    RefreshFundTypeUi();
                }
            }
        }

        public AddPurchaseAccountViewModel(ProcurementAccountLstModel data, bool isModify = false)
        {
            Title = isModify ? "修改采购账号记录" : "新增采购账号记录";
            if (isModify && data != null)
            {
                CopyData(data, Account);
            }
            else
            {
                Account.Date = System.DateTime.Now.Date;
                Account.AccountType = 0;
            }

            NormalizeFundType();
            RefreshFundTypeUi();
        }

        private void NormalizeFundType()
        {
            if (Account.AccountType != 0 && Account.AccountType != 1)
            {
                Account.AccountType = 0;
            }
        }

        private void RefreshFundTypeUi()
        {
            NotifyOfPropertyChange(nameof(IsCashType));
            NotifyOfPropertyChange(nameof(IsCreditType));
        }

        public async void Sure()
        {
            if (!Account.Date.HasValue)
            {
                MessageBox.Show("请选择日期");
                return;
            }

            if (string.IsNullOrWhiteSpace(Account.ProcurementAccount))
            {
                MessageBox.Show("请输入采购账号");
                return;
            }

            var ok = await CRMRequest.PurchaseAccountEdit(Account);
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

        private static void CopyData(ProcurementAccountLstModel source, ProcurementAccountLstModel target)
        {
            target.Id = source.Id;
            target.AddTimeToken = source.AddTimeToken;
            target.Amount = source.Amount;
            target.NameRaw = source.NameRaw;
            target.ProcurementAccountRaw = source.ProcurementAccountRaw;
            target.AccountType = source.AccountType;
            target.Remark = source.Remark;
            target.BalanceCash = source.BalanceCash;
            target.BalanceDebt = source.BalanceDebt;
            target.IsCheck = source.IsCheck;
        }
    }
}

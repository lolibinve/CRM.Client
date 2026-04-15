using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 采购账号入账弹窗：<c>crm/purchase/accountCheckIn</c>。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountCheckInViewModel : Screen
    {
        private bool _isSubmitting;

        public int AccountId { get; }

        public string Title { get; set; }

        /// <summary>界面输入的转入金额（元；支持小数与负数）。</summary>
        public string AmountInput { get; set; } = "";

        private int _fundType;

        /// <summary>资金类型：<c>0</c> 现金，<c>1</c> 到期诚意赊。</summary>
        public bool IsCashType
        {
            get => _fundType == 0;
            set
            {
                if (!value)
                    return;
                _fundType = 0;
                NotifyOfPropertyChange(nameof(IsCashType));
                NotifyOfPropertyChange(nameof(IsCreditType));
            }
        }

        public bool IsCreditType
        {
            get => _fundType == 1;
            set
            {
                if (!value)
                    return;
                _fundType = 1;
                NotifyOfPropertyChange(nameof(IsCashType));
                NotifyOfPropertyChange(nameof(IsCreditType));
            }
        }

        public string Remark { get; set; } = "";

        /// <summary>入账接口已成功返回，用于关闭弹窗后刷新列表（不依赖 <c>DialogResult</c> 是否被正确传回）。</summary>
        public bool WasSuccessful { get; private set; }

        public PurchaseAccountCheckInViewModel(ProcurementAccountLstModel account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            AccountId = account.Id;
            var name = string.IsNullOrWhiteSpace(account.ProcurementAccount) ? $"ID {account.Id}" : account.ProcurementAccount.Trim();
            Title = $"转入资金 — {name}";
            _fundType = 0;
        }

        public async void Sure()
        {
            if (_isSubmitting)
            {
                return;
            }

            if (AccountId <= 0)
            {
                MessageBox.Show("无效的采购账号。");
                return;
            }

            var trimmed = (AmountInput ?? "").Trim().Replace(",", "").Replace(" ", "");
            if (!decimal.TryParse(trimmed, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var amount) || amount == 0)
            {
                MessageBox.Show("请输入非零的转入金额（支持小数与负数）。");
                return;
            }

            _isSubmitting = true;
            try
            {
                var loginUser = IoC.Get<CacheInfo>()?.LoginAccount;
                var ok = await CRMRequest.PurchaseAccountCheckIn(AccountId, amount, _fundType, Remark, loginUser);
                if (!ok)
                {
                    return;
                }

                WasSuccessful = true;

                var temp = GetView();
                if (temp is Window win)
                {
                    win.DialogResult = true;
                }

                await TryCloseAsync();
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
                win.DialogResult = false;
            return TryCloseAsync();
        }
    }
}

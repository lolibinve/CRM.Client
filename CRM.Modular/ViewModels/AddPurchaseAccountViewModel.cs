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
        public bool WasSuccessful { get; private set; }

        /// <summary>编辑时若现金余额或账期/诚意赊余额任一非 0，则采购账号名称只读。</summary>
        public bool IsAccountNameReadOnly { get; private set; }

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
            }

            if (isModify)
            {
                var cash = Account.BalanceCash ?? 0;
                var debt = Account.BalanceDebt ?? 0;
                IsAccountNameReadOnly = cash != 0 || debt != 0;
            }
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

            WasSuccessful = true;

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
            target.NameRaw = source.NameRaw;
            target.ProcurementAccountRaw = source.ProcurementAccountRaw;
            target.Remark = source.Remark;
            target.BalanceCash = source.BalanceCash;
            target.BalanceDebt = source.BalanceDebt;
            target.IsCheck = source.IsCheck;
        }
    }
}

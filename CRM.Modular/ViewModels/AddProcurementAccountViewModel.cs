using Caliburn.Micro;
using CRM.Model;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddProcurementAccountViewModel : Screen
    {
        public ProcurementAccountLstModel Account { get; set; } = new ProcurementAccountLstModel();
        public string Title { get; set; }

        public AddProcurementAccountViewModel(ProcurementAccountLstModel data, bool isModify = false)
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
            target.ProcurementAccount = source.ProcurementAccount;
            target.TypeRaw = source.TypeRaw;
            target.Remark = source.Remark;
            target.BalanceCash = source.BalanceCash;
            target.BalanceDebt = source.BalanceDebt;
            target.IsCheck = source.IsCheck;
        }
    }
}

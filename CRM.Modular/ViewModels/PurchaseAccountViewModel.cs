using Caliburn.Micro;
using CRM.Model;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountViewModel : Screen
    {
        private readonly IWindowManager windowManager;
        private List<ProcurementAccountLstModel> _allItems = new List<ProcurementAccountLstModel>();

        public BindableCollection<ProcurementAccountLstModel> AccountLst { get; set; } = new BindableCollection<ProcurementAccountLstModel>();

        public ProcurementAccountLstModel SelectItem { get; set; }

        public DateTime? SelectedStartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime? SelectedEndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public PurchaseAccountViewModel(IWindowManager manager)
        {
            windowManager = manager;
            Query();
        }

        /// <summary>
        /// 从 <c>accountList</c> 拉取数据后按日期区间筛选展示。
        /// </summary>
        public async void Query()
        {
            var data = await CRMRequest.PurchaseAccountList(1, 1000);
            _allItems = data?.AccountLst ?? new List<ProcurementAccountLstModel>();
            ApplyDateFilter();
        }

        private void ApplyDateFilter()
        {
            var start = SelectedStartDate?.Date;
            var end = SelectedEndDate?.Date;

            IEnumerable<ProcurementAccountLstModel> q = _allItems;

            if (start.HasValue)
            {
                q = q.Where(x => x.Date.HasValue && x.Date.Value.Date >= start.Value);
            }

            if (end.HasValue)
            {
                q = q.Where(x => x.Date.HasValue && x.Date.Value.Date <= end.Value);
            }

            AccountLst = new BindableCollection<ProcurementAccountLstModel>(q.OrderByDescending(x => x.Date));
        }

        public async void Add()
        {
            AddProcurementAccountViewModel vm = new AddProcurementAccountViewModel(null, false);
            var result = await windowManager.ShowDialogAsync(vm);
            if (result == true)
            {
                var item = vm.Account;
                item.Id = _allItems.Any() ? _allItems.Max(x => x.Id) + 1 : 1;
                _allItems.Insert(0, item);
                ApplyDateFilter();
                SelectItem = item;
            }
        }

        public async void Account_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectItem == null)
            {
                return;
            }
            await ModifyBase();
        }

        private async Task ModifyBase()
        {
            AddProcurementAccountViewModel vm = new AddProcurementAccountViewModel(SelectItem, true);
            var result = await windowManager.ShowDialogAsync(vm);
            if (result == true)
            {
                var edit = vm.Account;
                var source = _allItems.FirstOrDefault(x => x.Id == SelectItem.Id);
                if (source != null)
                {
                    source.Date = edit.Date;
                    source.Amount = edit.Amount;
                    source.ProcurementAccount = edit.ProcurementAccount;
                    source.FundType = edit.FundType;
                    source.Remark = edit.Remark;
                }

                ApplyDateFilter();
                SelectItem = _allItems.FirstOrDefault(x => x.Id == edit.Id) ?? edit;
            }
        }
    }
}

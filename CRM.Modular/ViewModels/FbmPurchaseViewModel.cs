using Aipark.Wpf.Controls;
using Caliburn.Micro;
using CRM.Modular.Models;
using CRM.Model;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// FBM 采购列表：筛选、分页、对接 <c>fbmList</c>。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class FbmPurchaseViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel() { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        public DateTime? SelectedStartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime? SelectedEndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public string OrderIdSearch { get; set; }

        public ObservableCollection<RoleData> RoleSource { get; set; } = new ObservableCollection<RoleData>();
        public RoleData SelectRole { get; set; }

        public ObservableCollection<string> AccountFilterList { get; set; } = new ObservableCollection<string>();
        public string SelectedFilterAccount { get; set; }

        public bool IsAdmin { get; set; }

        public BindableCollection<FbmPurchaseRecordModel> RecordLst { get; set; } = new BindableCollection<FbmPurchaseRecordModel>();

        public FbmPurchaseRecordModel SelectItem { get; set; }

        public FbmPurchaseViewModel(IWindowManager manager)
        {
            windowManager = manager;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await InitRoleSource();
            await InitAccountFilter();
            await QueryBase(1);
        }

        private async Task InitRoleSource()
        {
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

        private async Task InitAccountFilter()
        {
            AccountFilterList = new ObservableCollection<string>();
            AccountFilterList.Add("全部");
            var acc = await CRMRequest.PurchaseAccountList(1, 2000);
            if (acc?.AccountLst != null)
            {
                foreach (var name in acc.AccountLst.Select(x => x.ProcurementAccount).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(x => x))
                {
                    AccountFilterList.Add(name);
                }
            }

            SelectedFilterAccount = "全部";
        }

        public async void Query()
        {
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
                var buyerName = SelectRole?.Name;
                if (SelectRole == null)
                {
                    var info = IoC.Get<CacheInfo>();
                    if (!info.IsAdmin)
                    {
                        buyerName = info.LoginAccount;
                    }
                    else
                    {
                        buyerName = "";
                    }
                }
                else if (buyerName == "全部")
                {
                    buyerName = "";
                }

                var purchaseAccount = string.IsNullOrEmpty(SelectedFilterAccount) || SelectedFilterAccount == "全部"
                    ? ""
                    : SelectedFilterAccount;

                var result = await CRMRequest.FbmList(
                    pageNum,
                    PageSizeConst,
                    purchaseAccount,
                    buyerName,
                    OrderIdSearch,
                    SelectedStartDate?.ToString("yyyy-MM-dd"),
                    SelectedEndDate?.ToString("yyyy-MM-dd"));

                if (result != null)
                {
                    RecordLst = new BindableCollection<FbmPurchaseRecordModel>(result.List ?? new List<FbmPurchaseRecordModel>());
                    var pages = (int)Math.Ceiling((result.Count * 1.0) / PageSizeConst);
                    PageInfo = new PageInfoModel
                    {
                        Total = result.Count,
                        PageNum = pageNum,
                        PageSize = PageSizeConst,
                        PagesCount = pages < 1 ? 1 : pages,
                    };
                }
                else
                {
                    RecordLst = new BindableCollection<FbmPurchaseRecordModel>();
                    PageInfo = new PageInfoModel
                    {
                        Total = 0,
                        PageNum = 1,
                        PageSize = PageSizeConst,
                        PagesCount = 1,
                    };
                }
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        public async void Add()
        {
            var vm = new AddFbmPurchaseViewModel(null, false);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }

        public async void Record_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectItem == null)
            {
                return;
            }

            var vm = new AddFbmPurchaseViewModel(SelectItem, true);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }
    }
}

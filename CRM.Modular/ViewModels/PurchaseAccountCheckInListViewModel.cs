using Aipark.Wpf.Controls;
using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 采购账号入账流水：<c>accountCheckInList</c>，支持起止时间、资金类型、分页。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class PurchaseAccountCheckInListViewModel : Screen
    {
        public const int PageSizeConst = 20;

        /// <summary>与主表 <c>purchase_account.name</c> 一致，作为 <c>name</c> 查询参数。</summary>
        public string AccountName { get; }

        public string WindowTitle { get; }

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        public BindableCollection<PurchaseAccountCheckInRecordModel> RecordLst { get; set; } =
            new BindableCollection<PurchaseAccountCheckInRecordModel>();

        /// <summary>筛选：开始日期（含当日）。</summary>
        public DateTime? FilterStartDate { get; set; }

        /// <summary>筛选：结束日期（含当日）。</summary>
        public DateTime? FilterEndDate { get; set; }

        public BindableCollection<FundTypeFilterItem> FundTypeFilters { get; set; }

        public FundTypeFilterItem SelectedFundType { get; set; }

        /// <summary>现金存入合计（接口 <c>sumCheckInCash</c>）。</summary>
        public decimal SumCheckInCash { get; set; }

        /// <summary>账期/诚意赊存入合计（接口 <c>sumCheckInDebt</c>）。</summary>
        public decimal SumCheckInDebt { get; set; }

        public PurchaseAccountCheckInListViewModel(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
                throw new ArgumentException("采购账号名称不能为空。", nameof(accountName));

            AccountName = accountName.Trim();
            WindowTitle = $"资金流水 — {AccountName}";
            DisplayName = WindowTitle;

            FundTypeFilters = new BindableCollection<FundTypeFilterItem>(new[]
            {
                new FundTypeFilterItem { Value = -1, Label = "全部" },
                new FundTypeFilterItem { Value = 0, Label = "现金" },
                new FundTypeFilterItem { Value = 1, Label = "到期诚意赊" },
            });
            SelectedFundType = FundTypeFilters[0];

            _ = QueryBase(1);
        }

        /// <summary>资金类型筛选项。</summary>
        public class FundTypeFilterItem
        {
            public int Value { get; set; }
            public string Label { get; set; }
        }

        /// <summary>按当前筛选条件从第 1 页查询（与分页控件 <c>pageNum</c>、<c>pageSize</c> 一并提交）。</summary>
        public async void Query()
        {
            await QueryBase(1);
        }

        public async void Pagination_OnPageNumberChanged(Pagination pagination, NumberChangedEventArgs e)
        {
            if (pagination == null)
                return;
            await QueryBase(pagination.PageNumber, pagination.PageSize);
        }

        private async Task QueryBase(int pageNum = 1, int? pageSizeOverride = null)
        {
            if (FilterStartDate.HasValue && FilterEndDate.HasValue && FilterStartDate.Value.Date > FilterEndDate.Value.Date)
            {
                MessageBox.Show("开始日期不能晚于结束日期。");
                return;
            }

            var pageSize = pageSizeOverride ?? (PageInfo?.PageSize > 0 ? PageInfo.PageSize : PageSizeConst);
            if (pageSize < 1)
                pageSize = PageSizeConst;

            var type = SelectedFundType?.Value ?? -1;
            var startStr = FilterStartDate.HasValue
                ? FilterStartDate.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null;
            var endStr = FilterEndDate.HasValue
                ? FilterEndDate.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null;

            IsProgressIndeterminate = true;
            try
            {
                var result = await CRMRequest.PurchaseAccountCheckInList(
                    AccountName,
                    pageNum,
                    pageSize,
                    type,
                    startStr,
                    endStr);

                if (result != null)
                {
                    RecordLst = new BindableCollection<PurchaseAccountCheckInRecordModel>(result.List ?? new List<PurchaseAccountCheckInRecordModel>());
                    SumCheckInCash = result.SumCheckInCash;
                    SumCheckInDebt = result.SumCheckInDebt;
                    var pages = (int)Math.Ceiling((result.Count * 1.0) / pageSize);
                    PageInfo = new PageInfoModel
                    {
                        Total = result.Count,
                        PageNum = pageNum,
                        PageSize = pageSize,
                        PagesCount = pages < 1 ? 1 : pages,
                    };
                }
                else
                {
                    RecordLst = new BindableCollection<PurchaseAccountCheckInRecordModel>();
                    SumCheckInCash = 0;
                    SumCheckInDebt = 0;
                    PageInfo = new PageInfoModel
                    {
                        Total = 0,
                        PageNum = 1,
                        PageSize = pageSize,
                        PagesCount = 1,
                    };
                }
            }
            finally
            {
                IsProgressIndeterminate = false;
            }
        }

        public Task CloseWindow()
        {
            return TryCloseAsync();
        }
    }
}

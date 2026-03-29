using Aipark.Wpf.Controls;
using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 备货采购列表：<c>stockList</c>。公共筛选：采购批次、业务员、产品编码；
    /// <c>type</c> 区分模块2～5：采购运输 / 到仓 / 滞销 / 售罄（与 <see cref="StockPurchaseConstants"/> 一致）。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class StockPurchaseViewModel : Screen
    {
        public const int PageSizeConst = 20;

        private readonly IWindowManager windowManager;

        public PageInfoModel PageInfo { get; set; } = new PageInfoModel { PageNum = 1, PageSize = PageSizeConst };

        public bool IsProgressIndeterminate { get; set; }

        /// <summary>筛选：采购批次 <c>purId</c>。</summary>
        public string FilterPurId { get; set; }

        /// <summary>筛选：业务员 <c>buyer_name</c>。</summary>
        public string FilterBuyerName { get; set; }

        /// <summary>筛选：产品编码 <c>p_id</c>。</summary>
        public string FilterProductCode { get; set; }

        /// <summary>筛选：采购账号（与 <c>fbmList</c> 的 <c>purchaseAccount</c> 一致）。</summary>
        public BindableCollection<string> AccountFilterList { get; set; } = new BindableCollection<string>();

        public string SelectedFilterAccount { get; set; }

        /// <summary>筛选：列表必填 <c>type</c>（模块2～5 库存视图）。</summary>
        public int FilterShipmentType { get; set; } = StockPurchaseConstants.StockListInTransit;

        /// <summary>库存视图芯片：与 <c>FilterShipmentType</c> 同步（默认模块2）。</summary>
        public bool statusTransit { get; set; } = true;
        public bool statusWarehouse { get; set; }
        public bool statusDeadstock { get; set; }
        public bool statusSoldOut { get; set; }

        /// <summary>角标占位，与订单管理 Badged 一致；暂无分项统计时可保持空字符串。</summary>
        public string StockViewBadge0 { get; set; } = "";
        public string StockViewBadge1 { get; set; } = "";
        public string StockViewBadge2 { get; set; } = "";
        public string StockViewBadge3 { get; set; } = "";

        /// <summary>模块3～5 列表为只读查看；模块2 可新增与编辑。</summary>
        [DependsOn(nameof(FilterShipmentType))]
        public bool IsReadOnlyStockView =>
            FilterShipmentType == StockPurchaseConstants.StockListArrivedWarehouse
            || FilterShipmentType == StockPurchaseConstants.StockListDeadstock
            || FilterShipmentType == StockPurchaseConstants.StockListSoldOut;

        public BindableCollection<StockPurchaseRecordModel> RecordLst { get; set; } = new BindableCollection<StockPurchaseRecordModel>();

        public StockPurchaseRecordModel SelectItem { get; set; }

        public StockPurchaseViewModel(IWindowManager manager)
        {
            windowManager = manager;
            var info = IoC.Get<CacheInfo>();
            FilterBuyerName = info?.LoginAccount ?? "";
            _ = QueryBase(1);
        }

        public async void Query()
        {
            await QueryBase(1);
        }

        /// <summary>切换库存视图类型并重新查询第一页（与订单管理 <c>StateQuery</c> 类似）。</summary>
        public async void StockTypeQuery(int type)
        {
            FilterShipmentType = type;
            statusTransit = type == StockPurchaseConstants.StockListInTransit;
            statusWarehouse = type == StockPurchaseConstants.StockListArrivedWarehouse;
            statusDeadstock = type == StockPurchaseConstants.StockListDeadstock;
            statusSoldOut = type == StockPurchaseConstants.StockListSoldOut;
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
                var result = await CRMRequest.StockList(
                    FilterShipmentType,
                    pageNum,
                    PageSizeConst,
                    FilterProductCode,
                    FilterBuyerName,
                    FilterPurId);

                if (result != null)
                {
                    RecordLst = new BindableCollection<StockPurchaseRecordModel>(result.List ?? new List<StockPurchaseRecordModel>());
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
                    RecordLst = new BindableCollection<StockPurchaseRecordModel>();
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
            var vm = new AddStockPurchaseViewModel(null, false, viewOnly: false);
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

            var vm = new AddStockPurchaseViewModel(SelectItem, true, viewOnly: IsReadOnlyStockView);
            var ok = await windowManager.ShowDialogAsync(vm);
            if (ok == true)
            {
                await QueryBase(PageInfo?.PageNum ?? 1);
            }
        }
    }
}

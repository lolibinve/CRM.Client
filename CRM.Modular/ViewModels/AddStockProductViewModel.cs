using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 产品库存新增/编辑：<c>stockManageEdit</c>，仅手动维护产品编码、名称；自动计算字段不在本界面展示。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class AddStockProductViewModel : Screen
    {
        private bool _isSubmitting;

        public StockProductRecordModel Record { get; set; } = new StockProductRecordModel();

        public string Title { get; set; }

        public AddStockProductViewModel(StockProductRecordModel data, bool isModify)
        {
            Title = isModify ? "修改产品库存" : "新增产品库存";
            if (isModify && data != null)
            {
                Record = Clone(data);
            }
            else
            {
                Record = new StockProductRecordModel();
            }
        }

        public async void Sure()
        {
            if (_isSubmitting)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Record.ProductCode))
            {
                MessageBox.Show("请输入产品编码");
                return;
            }

            if (string.IsNullOrWhiteSpace(Record.ProductName))
            {
                MessageBox.Show("请输入产品名称");
                return;
            }

            _isSubmitting = true;
            try
            {
                var login = IoC.Get<CacheInfo>()?.LoginAccount;
                var ok = await CRMRequest.StockManageEdit(Record, login);
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
            finally
            {
                _isSubmitting = false;
            }
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

        private static StockProductRecordModel Clone(StockProductRecordModel s)
        {
            return new StockProductRecordModel
            {
                Id = s.Id,
                ProductCode = s.ProductCode,
                ProductName = s.ProductName,
            };
        }
    }
}

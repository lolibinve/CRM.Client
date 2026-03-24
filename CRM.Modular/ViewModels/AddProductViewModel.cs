using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Help;
using CRM.Modular.Models;
using HttpLib;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace CRM.Modular.ViewModels
{

    public class AddProductViewModel : Screen
    {
        public RoleData SelectRole { set; get; }

        private ProductData _product = new ProductData();
        public ProductData product
        {
            get { return _product; }
            set { Set(ref _product, value); }
        }

        private bool _IsProgressIndeterminate;
        public bool IsProgressIndeterminate
        {
            get { return _IsProgressIndeterminate; }
            set { Set(ref _IsProgressIndeterminate, value); }
        }
        
        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }
        public AddProductViewModel(ProductData data,bool IsModify=false)
        {
            Title = IsModify ? "修改产品配置" : "新增产品";
           
            if (!IsModify)
            {
                var info = IoC.Get<CacheInfo>();
                this.product.Operator = info?.LoginAccount;
                this.product.Date= DateTime.Now;
            }
            else
            {
                this.product.Clone(data);
            }
        }


        public async void Sure()
        {
            IsProgressIndeterminate = true;
            var result = await CRMRequest.ModifyProduct(product);
            if (result != null)
            {
                product.Clone(result);
                var temp = this.GetView();
                if (temp is Window win)
                {
                    win.DialogResult = true;
                }
                IsProgressIndeterminate = false;
                await TryCloseAsync();
            }
        }

        public Task CloseForm()
        {
            var temp = this.GetView();
            if (temp is Window win)
            {
                win.DialogResult = false;
            }
            return TryCloseAsync();
        }


        public void AddPic(object sender)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = " 图片 |*.jpg;*.png;*.bmp";
            dialog.Title = "导入";

            if (dialog.ShowDialog() == true)
            {
                this.product.ImageBase64Str = Base64ToImageExtensions.ImageCompressToBase64(50, 50, dialog.FileName);
            }
        }
    }
}

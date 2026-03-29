using Caliburn.Micro;
using CLog;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 上架产品管理  已废除
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class ListManageViewModel : Caliburn.Micro.Screen
    {
        public const int PageSizeConst = 15;

        public BindableCollection<WareLstModel> WareLst { set; get; } = new BindableCollection<WareLstModel>();

        public ObservableCollection<RoleData> RoleSource { set; get; } = new ObservableCollection<RoleData>();
        public bool IsAdmin { set; get; }
        private readonly IWindowManager windowManager;
        public bool IsProgressIndeterminate { set; get; }
        public PageInfoModel PageInfo { set; get; } = new PageInfoModel() { PageNum = 1 };

        public WareLstModel SelectWare { set; get; }

        public DateTime? SelectedStartDate { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime? SelectedEndDate { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public RoleData SelectRole { set; get; }
        public string SKU { set; get; }

        //#region 导出参数
        //public ObservableCollection<string> Countries { set; get; } = new ObservableCollection<string>();
        //public string UPCPar { set; get; } = "UPC";
        //public bool IsUpdate { set; get; }
        //#endregion


        public ListManageViewModel(IWindowManager manager)
        {
            this.windowManager = manager;
            _ = InitRoleSource();
            _ = QueryBase(PageInfo.PageNum);
            InitCountries();
        }

        public async Task InitRoleSource()
        {
            var source = await CRMRequest.RoleList(null);
            RoleSource = new ObservableCollection<RoleData>(source.Orderlst);
            RoleSource.Insert(0, new RoleData()
            {
                Name = "全部",
            });

            var info = IoC.Get<CacheInfo>();
            IsAdmin = info.IsAdmin;
            //IsUpdate = true;
        }

        public async void InitCountries()
        {
            var source = await CRMRequest.GetCountryCurrencyInfo();
            if (source != null && source.PriceLst.Count > 0)
            {
                var lst = source.PriceLst.Select(x => x.Country).ToList();
                //Countries = new ObservableCollection<string>(lst);
            }
        }


        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public async void Query()
        {
            await QueryBase(1);
        }

        public async void Pagination_OnPageNumberChanged(Aipark.Wpf.Controls.Pagination arg1, Aipark.Wpf.Controls.NumberChangedEventArgs arg2)
        {
            await QueryBase(arg1.PageNumber);
        }



        private async Task QueryBase(int pageNum = 1)
        {
            IsProgressIndeterminate = true;

            var saleMan = SelectRole?.Name;
            if (SelectRole == null)
            {
                var info = IoC.Get<CacheInfo>();
                if (info.IsAdmin == false)
                {
                    saleMan = info.LoginAccount;
                    SelectRole = new RoleData() { Admin = 0, Name = info.LoginAccount };
                }
            }
            else
            {
                if (saleMan == "全部")
                {
                    saleMan = "";
                }
            }
            var result = await CRMRequest.GetWareLst(saleMan, SelectedStartDate, SelectedEndDate, SKU, pageNum, pageSize: PageSizeConst);

            if (result != null)
            {
                UpdateView(result, PageInfo.PageNum);
            }
            IsProgressIndeterminate = false;
        }


        /// <summary>
        /// 新增商品
        /// </summary>
        public async void AdddWare()
        {
            AddWareViewModel addOrderViewModel = new AddWareViewModel(windowManager, null, false);
            var result = await windowManager.ShowDialogAsync(addOrderViewModel);
            if (result == true)
            {
                await QueryBase(PageInfo.PageNum);
            }
        }

        /// <summary>
        /// 双击修改商品信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Ware_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectWare != null)
            {
                await ModifyBase();
            }
        }

        public void Item_CheckedClick(object sender, RoutedEventArgs e)
        {
            //if (WareLst != null && WareLst.Count > 0 && sender != null)
            //{
            //    if (((FrameworkElement)sender).DataContext is WareLstModel data)
            //    {
            //        foreach (var item in WareLst)
            //        {
            //            if (item.Id != data.Id && item.IsCheck)
            //            {
            //                item.IsCheck = false;
            //            }
            //        }
            //    }

            //}
        }

        /// <summary>
        /// 编辑商品
        /// </summary>
        /// <returns></returns>
        private async Task ModifyBase()
        {
            AddWareViewModel AddWareViewModel = new AddWareViewModel(windowManager, SelectWare, true);
            var result = await windowManager.ShowDialogAsync(AddWareViewModel);
            if (result == true)
            {
                await QueryBase(PageInfo.PageNum);
            }
        }

        private void UpdateView(WareModel result, int pageNum)
        {
            var lst = result.Warelst;
            if (lst == null)
            {
                WareLst = new BindableCollection<WareLstModel>();
                PageInfo = new PageInfoModel();
                return;
            }
            WareLst = new BindableCollection<WareLstModel>(lst);
            PageInfo = new PageInfoModel()
            {
                Total = result.Count,
                PageNum = pageNum,
                PageSize = PageSizeConst,
                PagesCount = (int)Math.Ceiling((result.Count * 1.0) / PageSizeConst),
            };
        }


        public async void Export()
        {

            if (!WareLst.Any(x => x.IsCheck))
            {
                MessageBox.Show("请勾选！");
                return;
            }

            //var first = WareLst.First(x => x.IsCheck);

            List<string> ctemp = new List<string>();
            foreach (var item in WareLst)
            {
                if ( item.IsCheck)
                {
                    ctemp.Add(item.Id.ToString());
                }
            }
            string ids = string.Join(",", ctemp);
            //if (string.IsNullOrEmpty(first.Sku))
            //{
            //    MessageBox.Show("sku不能为空！");
            //    return;
            //}

            ListExpertDesignViewModel lstExport = new ListExpertDesignViewModel();
            var result = await windowManager.ShowDialogAsync(lstExport);


            if (result == true)
            {
                //国家
                List<string> temp = new List<string>();
                foreach (var item in lstExport.SelectedContries)
                {
                    temp.Add(item.ToString());
                }
                string countries = string.Join(",", temp);


                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "导出目录";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string folderPath = dialog.SelectedPath;
                    try
                    {
                        string fileName = folderPath + "\\数据" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip";
                        var fileByte = await CRMRequest.ExportFileDownLoad(fileName,ids, countries, lstExport.UPCPar,lstExport.BrandName,lstExport.RecommendedBrowseNodes, lstExport.IsUpdate);

                        if (fileByte != null)
                        {
                            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create);
                            BinaryWriter bw = new BinaryWriter(fs);
                            bw.Write(fileByte);
                            bw.Close();
                            fs.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        TLog.Error(e);
                    }
                }

            }
        }



        //public async void Export1()
        //{
        //    if (WareLst.Any(x => x.IsCheck))
        //    {
        //        var first = WareLst.First(x => x.IsCheck);
        //        if (string.IsNullOrEmpty(first.Sku))
        //        {
        //            MessageBox.Show("sku不能为空！");
        //            return;
        //        }

        //        System.Windows.Controls.ListBox listBoxCountry = ((FrameworkElement)this.GetView()).FindName("CountriesListBox") as System.Windows.Controls.ListBox;
        //        if (listBoxCountry.SelectedItems == null || listBoxCountry.SelectedItems.Count == 0)
        //        {
        //            MessageBox.Show("请选择国家！");
        //            return;
        //        }

        //        List<string> temp = new List<string>();
        //        foreach (var item in listBoxCountry.SelectedItems)
        //        {
        //            temp.Add(item.ToString());
        //        }
        //        string countries = string.Join(",", temp);


        //        FolderBrowserDialog dialog = new FolderBrowserDialog();
        //        dialog.Description = "导出目录";

        //        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            string folderPath = dialog.SelectedPath;
        //            try
        //            {
        //                string fileName = folderPath + "\\数据" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip";
        //                var result = await CRMRequest.ExportFileDownLoad(fileName, first.Sku, countries, UPCPar, IsUpdate);

        //                if (result != null)
        //                {
        //                    FileStream fs = new FileStream(fileName, System.IO.FileMode.Create);
        //                    BinaryWriter bw = new BinaryWriter(fs);
        //                    bw.Write(result);
        //                    bw.Close();
        //                    fs.Close();
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                TLog.Error(e);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("请勾选产品！");
        //        return;
        //    }
        //}

        //void StreamToFile(string fileName, Stream stream)
        //{
        //    // 把 Stream 转换成 byte[] 
        //    byte[] bytes = new byte[stream.Length];
        //    stream.Read(bytes, 0, bytes.Length);
        //    // 设置当前流的位置为流的开始 
        //    stream.Seek(0, SeekOrigin.Begin);

        //    // 把 byte[] 写入文件 
        //    FileStream fs = new FileStream(fileName, System.IO.FileMode.Create);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(bytes);
        //    bw.Close();
        //    fs.Close();
        //}
    }
}

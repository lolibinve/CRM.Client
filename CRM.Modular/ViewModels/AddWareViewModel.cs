using Caliburn.Micro;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Model;
using HttpLib;
using CRM.Modular.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using CRM.Modular.Views;
using static OfficeOpenXml.FormulaParsing.EpplusExcelDataProvider;
using MaterialDesignThemes.Wpf;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CRM.Modular.ViewModels
{
    /// <summary>
    /// 新增上架产品、编辑上架产品
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class AddWareViewModel : Screen
    {
        public WareData Ware { set; get; } = new WareData();
        public string Title { set; get; }
        public bool IsModify { set; get; }
        public bool ChildVisibility { set; get; }

        //原始列表信息
        private WareLstModel WareLst { set; get; }
        //public string Colors { set; get; }
        //public string Sizes { set; get; }

        public ChildModel SelectChild { set; get; }
        public ObservableCollection<ChildModel> Child { set; get; } = new ObservableCollection<ChildModel>();

        private readonly IWindowManager windowManager;

        #region 语言
        public WareDetailViewModel English { set; get; }
        public WareDetailViewModel France { set; get; }
        public WareDetailViewModel German { set; get; }
        public WareDetailViewModel Italy { set; get; }
        public WareDetailViewModel Spain { set; get; }
        public WareDetailViewModel Korean { set; get; }
        public WareDetailViewModel Portuguese { set; get; }
        public WareDetailViewModel Polski { set; get; }
        public WareDetailViewModel Svenska { set; get; }
        public WareDetailViewModel Dutch { set; get; }
        public WareDetailViewModel Arabic { set; get; }
        public WareDetailViewModel Japan { set; get; }
        public WareDetailViewModel Chinese { set; get; }
        #endregion

        public AddWareViewModel(IWindowManager manager, WareLstModel data, bool isModify = false)
        {
            this.windowManager = manager;
            this.WareLst = data;
            this.IsModify = isModify;
            Title = isModify ? "修改" : "新增";
            InitView();
        }

        private async void InitView()
        {
            this.ChildVisibility = false;
            //新增
            if (!IsModify)
            {
                this.Ware = new WareData();
                this.Ware.Id = 0;
                this.Ware.FeedProductType = "home";

                var source = await CRMRequest.GetCountryCurrencyInfo();
                var temp = new ObservableCollection<PriceModel>(source.PriceLst);
                this.Ware.BasePrice = new ObservableCollection<PriceModel>(temp);
            }
            else
            {
                this.Ware = await CRMRequest.GetWareInfo(this.WareLst.Sku);
                if (this.Ware == null)
                {
                    return;
                }
                if (this.Ware?.Child != null && this.Ware.Child.Count > 0)
                {
                    Child = this.Ware.Child;
                    this.ChildVisibility = true;
                }
            }
            this.Ware.Operator = IoC.Get<CacheInfo>().LoginAccount;

            English = new WareDetailViewModel(Ware.English);
            France = new WareDetailViewModel(Ware.France);
            German = new WareDetailViewModel(Ware.German);
            Italy = new WareDetailViewModel(Ware.Italy);
            Spain = new WareDetailViewModel(Ware.Spain);
            Chinese = new WareDetailViewModel(Ware.Chinese);
            Japan = new WareDetailViewModel(Ware.Japan);
            Arabic = new WareDetailViewModel(Ware.Arabic);
            Dutch = new WareDetailViewModel(Ware.Dutch);
            Svenska = new WareDetailViewModel(Ware.Svenska);
            Polski = new WareDetailViewModel(Ware.Polski);
            Portuguese = new WareDetailViewModel(Ware.Portuguese);
            Korean = new WareDetailViewModel(Ware.Korean);
        }





        public async void Sure()
        {
            this.Ware.Child = this.Child;
            if (Child != null && Child.Count != 0)
            {
                var first = Child.FirstOrDefault();
                Ware.MainImageUrl = first.MainImageUrl;
            }

            this.Ware.English = English.LanModel;
            this.Ware.France = France.LanModel;
            this.Ware.German = German.LanModel;
            this.Ware.Italy = Italy.LanModel;
            this.Ware.Spain = Spain.LanModel;
            this.Ware.Chinese = Chinese.LanModel;
            this.Ware.Japan = Japan.LanModel;
            this.Ware.Arabic = Arabic.LanModel;
            this.Ware.Dutch = Dutch.LanModel;
            this.Ware.Svenska = Svenska.LanModel;
            this.Ware.Polski = Polski.LanModel;
            this.Ware.Portuguese = Portuguese.LanModel;
            this.Ware.Korean = Korean.LanModel;

            var result = await CRMRequest.AddWare(Ware);
            if (result)
            {
                var temp = this.GetView();
                if (temp is Window win)
                {
                    win.DialogResult = true;
                }
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

        #region 生成变体
        /// <summary>
        /// 颜色变体
        /// </summary>
        public void CreateColors()
        {
            if (!string.IsNullOrEmpty(Ware.Colors))
            {
                Child.Clear();
                ChildVisibility = true;
                List<string> temps = Ware.Colors.Split(' ').ToList();
                temps.RemoveAll(string.IsNullOrWhiteSpace);
                int childid = 0;
                foreach (var item in temps)
                {
                    childid = ++childid;
                    Child.Add(new ChildModel()
                    {
                        ChildId = childid.ToString(),
                        Quantity = 100.ToString(),
                        Color = item,
                    });
                }
            }
        }

        //尺寸变体
        public void CreateSizes()
        {
            if (!string.IsNullOrEmpty(Ware.Sizes))
            {
                Child.Clear();
                ChildVisibility = true;
                List<string> temps = Ware.Sizes.Split(' ').ToList();
                temps.RemoveAll(string.IsNullOrWhiteSpace);
                int childid = 0;
                foreach (var item in temps)
                {
                    childid = ++childid;
                    Child.Add(new ChildModel()
                    {
                        ChildId = childid.ToString(),
                        Quantity = 100.ToString(),
                        //ChildId = Guid.NewGuid().ToString().Replace("-", ""),
                        Size = item,
                    });
                }
            }
        }


        //颜色尺寸变体
        public void CreateColorsSizes()
        {
            if (string.IsNullOrEmpty(Ware.Colors) || string.IsNullOrEmpty(Ware.Sizes))
            {
                return;
            }

            ChildVisibility = true;
            Child.Clear();
            List<string> tempsColor = Ware.Colors.Split(' ').ToList();
            List<string> tempsSize = Ware.Sizes.Split(' ').ToList();
            tempsColor.RemoveAll(string.IsNullOrWhiteSpace);
            tempsSize.RemoveAll(string.IsNullOrWhiteSpace);

            int childid = 0;
            foreach (var color in tempsColor)
            {
                foreach (var size in tempsSize)
                {
                    childid = ++childid;
                    Child.Add(new ChildModel()
                    {
                        ChildId = childid.ToString(),
                        Quantity = 100.ToString(),
                        Size = size,
                        Color = color,
                    });
                }
            }
        }

        #endregion


        /// <summary>
        /// 删除变体行
        /// </summary>
        public void DeletePrice(object sender, RoutedEventArgs e)
        {
            if (SelectChild != null && !string.IsNullOrEmpty(SelectChild.ChildId))
            {
                var first = Child.First(x => x.ChildId == SelectChild.ChildId);
                if (first != null)
                {
                    Child.Remove(first);
                }
            }
        }

        /// <summary>
        /// 修改变体
        /// </summary>
        public async void EditPrice(object sender, RoutedEventArgs e)
        {
            StandardPriceViewModel priceViewModel = new StandardPriceViewModel(SelectChild,Ware);
            var result = await windowManager.ShowDialogAsync(priceViewModel);
            if (result == true)
            {
                foreach (var item in Child)
                {
                    if (item.ChildId != null && item.ChildId == SelectChild.ChildId)
                    {
                        if (item.StandardPrice == null)
                        {
                            item.StandardPrice = new ObservableCollection<PriceModel>();
                        }
                        item.Clone(priceViewModel.Child);
                        item.StandardPrice = new ObservableCollection<PriceModel>(priceViewModel.PriceLst);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 同步图片
        /// </summary>
        public void SyncPictures()
        {
            if (Child == null || Child.Count == 0)
            {
                return;
            }

            var first = Child.FirstOrDefault();

            foreach (var item in Child)
            {
                if (item.ChildId == first.ChildId)
                {
                    continue;
                }
                item.MainImageUrl = string.IsNullOrEmpty(item.MainImageUrl) ? first.MainImageUrl : item.MainImageUrl;
                item.OtherImageUrl1 = string.IsNullOrEmpty(item.OtherImageUrl1)? first.OtherImageUrl1 : item.OtherImageUrl1;
                item.OtherImageUrl2 = string.IsNullOrEmpty(item.OtherImageUrl2) ? first.OtherImageUrl2 : item.OtherImageUrl2;
                item.OtherImageUrl3 = string.IsNullOrEmpty(item.OtherImageUrl3) ? first.OtherImageUrl3 : item.OtherImageUrl3;
                item.OtherImageUrl4 = string.IsNullOrEmpty(item.OtherImageUrl4) ? first.OtherImageUrl4 : item.OtherImageUrl4;
                item.OtherImageUrl5 = string.IsNullOrEmpty(item.OtherImageUrl5) ? first.OtherImageUrl5 : item.OtherImageUrl5;
                item.OtherImageUrl6 = string.IsNullOrEmpty(item.OtherImageUrl6) ? first.OtherImageUrl6 : item.OtherImageUrl6;
                item.OtherImageUrl7 = string.IsNullOrEmpty(item.OtherImageUrl7) ? first.OtherImageUrl7 : item.OtherImageUrl7;
            }
        }

        public void SyncPartCode()
        {
            if (Child == null || Child.Count == 0)
            {
                return;
            }

            var first = Child.FirstOrDefault();
            if (string.IsNullOrEmpty(first.PartNumber)) return;



            string src = first.PartNumber;
            Regex reg = new Regex(@"(\d+)$");
            Match match = reg.Match(src);
            if (match.Success)
            {
                string matchStr = match.Groups[1].Value;
                int len = matchStr.Length;
                //匹配的数字
                int number = int.Parse(matchStr);

                //匹配的字母
                string letters = src.Substring(0, src.LastIndexOf(matchStr));


                foreach (var item in Child)
                {
                    if (item.ChildId == first.ChildId)
                    {
                        continue;
                    }
                    int num = ++number;
                    string result = num.ToString();
                    if (num.ToString().Length < len)
                    {
                        for (int i = 0; i < len - num.ToString().Length; i++)
                        {
                            result = "0" + result;
                        }
                    }
                    item.PartNumber = letters + result;
                }

            }

           
        }
    }
}

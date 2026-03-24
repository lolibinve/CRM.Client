using Caliburn.Micro;
using CLog;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MessageBox = System.Windows.MessageBox;
using Screen = Caliburn.Micro.Screen;
using TextBox = System.Windows.Controls.TextBox;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class OrderManageViewModel : Screen
    {
        public const int PageSizeConst = 15;
        public int OrderQueryStatus { get; set; } = 0;
        public bool statusAll { set; get; }

        public bool statusNew { set; get; }
        public bool statusPrepare { set; get; }
        public bool statusSend { set; get; }
        public bool statusWrong { set; get; }
        public bool statusDone { set; get; }

        public string SKUStr { set; get; }
        //public string Country { set; get; }
        public string OrderNumber { set; get; }
        public string Store { set; get; }
        public OrderData SelectOrder { set; get; }
        public List<CountryCodeMode> Countries { set; get; }
        public string SelectCountry { set; get; }
        
        #region 属性
        public BindableCollection<OrderData> OrderLst { set; get; } = new BindableCollection<OrderData>();
        public DateTime? SelectedStartDate { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
        public DateTime? SelectedEndDate { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public PageInfoModel PageInfo { set; get; } = new PageInfoModel() { PageNum = 1 };
        public bool IsProgressIndeterminate { set; get; }
        public ObservableCollection<RoleData> RoleSource { set; get; } = new ObservableCollection<RoleData>();
        public RoleData SelectRole { set; get; }
        public bool IsAdmin { set; get; }

        public string Badged { set; get; }
        public string Badged0 { set; get; }
        public string Badged1 { set; get; }
        public string Badged2 { set; get; }
        public string Badged3 { set; get; }
        public string Badged4 { set; get; }

        public double ProfitRatio { set; get; }
        public string TotalSales { set; get; }
        public string TotalCost { set; get; }
        public string TotalTrans { set; get; }
        public string TotalProfit { set; get; }

        public bool ImportAnimation { set; get; } = false;
        public bool ExportAnimation { set; get; } = false;
        #endregion

        private readonly IWindowManager windowManager;
        public OrderManageViewModel(IWindowManager manager)
        {
            this.windowManager = manager;
            SetCountriesSource();
            _ = InitRoleSource();
            _ = QueryBase(PageInfo.PageNum, OrderQueryStatus);
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
        }

        public void SetCountriesSource()
        {
            Countries = new List<CountryCodeMode>()
            {
                new CountryCodeMode{ Code="", Country="全部" },
                new CountryCodeMode{ Code="US", Country="美国" },
                new CountryCodeMode{ Code="CA", Country="加拿大" },
                new CountryCodeMode{ Code="MX", Country="墨西哥" },
                new CountryCodeMode{ Code="BR", Country="巴西" },
                new CountryCodeMode{ Code="JP", Country="日本" },

                new CountryCodeMode{ Code="AU", Country="澳大利亚" },
                new CountryCodeMode{ Code="SG", Country="新加坡" },
                new CountryCodeMode{ Code="UK", Country="英国" },
                new CountryCodeMode{ Code="FR", Country="法国" },
                new CountryCodeMode{ Code="DE", Country="德国" },

                new CountryCodeMode{ Code="IT", Country="意大利" },
                new CountryCodeMode{ Code="ES", Country="西班牙" },
                new CountryCodeMode{ Code="SE", Country="瑞典" },
                new CountryCodeMode{ Code="PL", Country="波兰" },
                new CountryCodeMode{ Code="NL", Country="荷兰" },

                new CountryCodeMode{ Code="AE", Country="阿联酋" },
                new CountryCodeMode{ Code="SA", Country="沙特阿拉伯" },
            };
        }


        public async void CopyAdd()
        {
            var model = OrderLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                AddOrderViewModel addOrderViewModel = new AddOrderViewModel(model);
                var result = await windowManager.ShowDialogAsync(addOrderViewModel);
                if(result == true)
                {
                    await QueryBase(PageInfo.PageNum, OrderQueryStatus);
                }
            }
            else
            {
                MessageBox.Show("亲，请先勾选订单");
            }
        }

        public async void Import()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = " 变量 |*.csv;*.xls;*.xlsx";
            dialog.Title = "导入";

            if (dialog.ShowDialog() == true)
            {
                var file = dialog.FileName;
                if (string.IsNullOrEmpty(file))
                {
                    return;
                }
                try
                {
                    ImportAnimation = true;
                    var info = IoC.Get<CacheInfo>();
                    var result = await CRMRequest.PostFile(file, info?.LoginAccount);
                    if(!result)
                    {
                        MessageBox.Show("导入失败,请重新导入");
                    }
                    else
                    {
                        MessageBox.Show("导入成功");
                        await QueryBase(PageInfo.PageNum, OrderQueryStatus);
                    }
                    ImportAnimation = false;
                }
                catch (Exception e)
                {
                    TLog.Error(e);
                }
            }
        }

        public async void Export()
        {
            //IsProgressIndeterminate = true;
            ExportAnimation = true;
            ExcelHelp help = new ExcelHelp();

            var result = await GetExportData();
            //help.CreateExcel(OrderLst.ToList());
            try
            {
                help.CreateExcel(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ExportAnimation = false;
            //IsProgressIndeterminate = false;
        }

        private async Task<List<OrderData>> GetExportData(int size = 100)
        {
            var serchTimes = (int)Math.Ceiling((PageInfo.Total * 1.0) / size);
            List<OrderData> mergeList = new List<OrderData>();

            for (int i = 1; i < serchTimes + 1; i++)
            {
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
                var result = await CRMRequest.GetOrderLst(saleMan, SelectedStartDate, SelectedEndDate, SKUStr, OrderNumber, SelectCountry, Store, pageNum: i, pageSize: size, OrderQueryStatus);
                if (result != null && result.OrderDatalst.Count > 0)
                {
                    mergeList = mergeList.Concat(result.OrderDatalst).ToList();
                }
            }

            return mergeList;
        }


        public async void Modify()
        {
            var model = OrderLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                await ModifyBase();
            }
            else
            {
                MessageBox.Show("请先勾选订单");
            }
        }

        public async void Delete()
        {
            var model = OrderLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                var result = await CRMRequest.DeleteOrder(model.Id.ToString());
                if(result)
                {
                    await QueryBase(PageInfo.PageNum, OrderQueryStatus);
                }
            }
            else
            {
                MessageBox.Show("请先勾选订单");
            }
        }


        public async void OrderLst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if(sender is System.Windows.Controls.DataGrid data)
            {
                string colName = data.CurrentColumn?.Header.ToString();
                if(colName == "订单号" || colName == "SKU")
                {
                    return;
                }
            }
            if (SelectOrder != null)
            {
                await ModifyBase();
            }
        }


        private async Task ModifyBase()
        {
            AddOrderViewModel addOrderViewModel = new AddOrderViewModel(SelectOrder, true);
            var result = await windowManager.ShowDialogAsync(addOrderViewModel);
            if (result == true)
            {
                await QueryBase(PageInfo.PageNum, OrderQueryStatus);
            }
        }

        public void OrderItem_CheckedClick(object sender, RoutedEventArgs e)
        {
            if (OrderLst != null && OrderLst.Count > 0 && sender!=null)
            {
                if (((FrameworkElement)sender).DataContext is OrderData data)
                {
                    foreach (var item in OrderLst)
                    {
                        if (item.Id != data.Id && item.IsCheck)
                        {
                            item.IsCheck = false;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 流转
        /// </summary>
        public async void Complete()
        {
            var model = OrderLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                if(model.State == OrderState.退款)
                {
                    MessageBox.Show("退款订单无法妥投");
                    return;
                }
                var result = await CRMRequest.ModifyState(model.Id.ToString(), 3);
                if (result)
                {
                    MessageBox.Show("妥投成功");
                    await QueryBase(PageInfo.PageNum, OrderQueryStatus);
                }
            }
        }

        public async void Send()
        {
            var model = OrderLst.FirstOrDefault(x => x.IsCheck);
            if (model != null)
            {
                if (model.State != OrderState.备货)
                {
                    MessageBox.Show("非备货订单无法发货");
                    return;
                }
                var result = await CRMRequest.ModifyState(model.Id.ToString(), 2);
                if (result)
                {
                    MessageBox.Show("发货成功");
                    await QueryBase(PageInfo.PageNum, OrderQueryStatus);
                }
            }
        }


        public async void Pagination_OnPageNumberChanged(Aipark.Wpf.Controls.Pagination arg1, Aipark.Wpf.Controls.NumberChangedEventArgs arg2)
        {
            await QueryBase(arg1.PageNumber, OrderQueryStatus);
        }

        public async void StateQuery(int status = -1)
        {
            OrderQueryStatus = status;
            await QueryBase(1, status);
        }


        public async void Query()
        {
            await QueryBase(1);
        }


        private async Task QueryBase(int pageNum = 1, int status = 0)
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
            var result = await CRMRequest.GetOrderLst(saleMan, SelectedStartDate, SelectedEndDate, SKUStr, OrderNumber, SelectCountry, Store, pageNum, pageSize: PageSizeConst, status);

            if (result != null)
            {
                OrderQueryStatus = status;
                UpdateView(result, PageInfo.PageNum);
            }
            IsProgressIndeterminate = false;
        }


        public void UpdateView(OrderModel order, int pageNum)
        {
            this.ProfitRatio = order.ProfitRatio;
            this.TotalSales = order.TotalSales.ToString();
            this.TotalCost = order.TotalCost.ToString();
            this.TotalTrans = order.TotalTrans.ToString();
            this.TotalProfit = order.TotalProfit.ToString();

            this.Badged = (order.OrderNew + order.OrderPrepare + order.OrderSend + order.OrderWrong + order.OrderDone).ToString();
            this.Badged0 = order.OrderNew.ToString();
            this.Badged1 = order.OrderPrepare.ToString();
            this.Badged2 = order.OrderSend.ToString();
            this.Badged3 = order.OrderWrong.ToString();
            this.Badged4 = order.OrderDone.ToString();

            if (order.OrderDatalst != null)
            {
                OrderLst = new BindableCollection<OrderData>(order.OrderDatalst);
            }
            else
            {
                OrderLst = new BindableCollection<OrderData>();
            }

            PageInfo = new PageInfoModel()
            {
                Total = order.Count,
                PageNum = pageNum,
                PageSize = PageSizeConst,
                PagesCount = (int)Math.Ceiling((order.Count * 1.0) / PageSizeConst),
            };

            SetState();

            void SetState()
            {
                if(OrderQueryStatus == -1)
                {
                    statusAll = true;
                }
                else if(OrderQueryStatus == 0)
                {
                    statusNew = true;
                }
                else if (OrderQueryStatus == 1)
                {
                    statusPrepare = true;
                }
                else if (OrderQueryStatus == 2)
                {
                    statusSend = true;
                }
                else if (OrderQueryStatus == 3)
                {
                    statusDone = true;
                }
                else if (OrderQueryStatus == 4)
                {
                    statusWrong = true;
                }
            }
        }


    }


    public class ExcelHelp
    {

        public void CreateExcel(List<OrderData> orderModel)
        {

            DataTable dt = GetDataTable(orderModel);
            if (dt.Rows.Count == 0)
            {
                return;
            }

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("订单数据");

                int rowIndex = 1;   // 起始行为 1
                int colIndex = 1;   // 起始列为 1

                int stateCol = 0;   
                int timeCol = 0;

                //设置列名
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;
                    //字体
                    worksheet.Cells[rowIndex, colIndex + i].Style.Font.Name = "Arial";
                    //字体加粗
                    worksheet.Cells[rowIndex, colIndex + i].Style.Font.Bold = true;
                    //字体大小
                    worksheet.Cells[rowIndex, colIndex + i].Style.Font.Size = 12;
                    //自动调整列宽，也可以指定最小宽度和最大宽度
                    worksheet.Column(colIndex + i).AutoFit();

                    if(dt.Columns[i].ColumnName == "状态")
                    {
                        stateCol = i;
                    }
                    else if(dt.Columns[i].ColumnName == "日期")
                    {
                        timeCol = i;
                    }
                }

                // 跳过第一列列名
                rowIndex++;

                //写入数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j == 0)
                        {
                            worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j];
                        }
                        else
                        {
                            var temp = dt.Rows[i][j];
                            if(j == stateCol )
                            {
                                if(temp is int state)
                                {
                                    temp = Enum.GetName(typeof(OrderState), (OrderState)state);
                                }
                            }
                            else if(j == timeCol)
                            {
                                if (temp is DateTime time_data)
                                {
                                    temp = time_data.ToString("D");
                                }
                            }
                            worksheet.Cells[rowIndex + i, colIndex + j].Value = temp;
                        }
                    }

                    //自动调整行高
                    worksheet.Row(rowIndex + i).CustomHeight = true;
                }

                //worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //水平居中
                //worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //单元格是否自动换行
                //worksheet.Cells.Style.WrapText = false;
                //单元格自动适应大小
                //worksheet.Cells.Style.ShrinkToFit = true;
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                worksheet.View.FreezePanes(2, 1);
                worksheet.Calculate();

                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "导出目录";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    string folderPath = dialog.SelectedPath;

                    try
                    {
                        string filePath = folderPath + "\\订单数据" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                        FileStream fileStream = new FileStream(filePath, FileMode.Create);
                        package.SaveAs(fileStream);
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        TLog.Error(e);
                    }

                }

            }
        }

        private DataTable GetDataTable(List<OrderData> staffList)
        {
            DataTable dt = new DataTable();
            if (staffList == null || staffList.Count < 1)
            {
                return dt;
            }

            //var staffs = staffList.Where(x => x.IsSelect).ToList();
            var staffs = staffList.ToList();
            if (staffs == null || staffs.Count < 1)
            {
                return dt;
            }

            var dickey_field = FieldToName();
            //var nameStr = dickey_field.Keys.ToList();
            dt = ToDataTable(staffs);

            List<DataColumn> delete_columns = new List<DataColumn>();
            //替换列名
            foreach (DataColumn col in dt.Columns)
            {
                if (dickey_field.ContainsKey(col.ToString()))
                {
                    col.ColumnName = dickey_field[col.ToString()];
                }
                else
                {
                    delete_columns.Add(col);
                }
            }
            
            //删除不需要的列
            foreach (var item in delete_columns)
            {
                dt.Columns.Remove(item);
            }

            return dt;
        }

        private Dictionary<string, string> FieldToName()
        {
            Dictionary<string, string> CompareDic = new Dictionary<string, string>();
            CompareDic.Add("Buyer", "购买人姓名");
            CompareDic.Add("Phone", "联系方式");
            CompareDic.Add("SaleDate", "日期");
            CompareDic.Add("OrderNumber", "订单号");
            CompareDic.Add("Account", "业务员");
            CompareDic.Add("SKU", "SKU");
            CompareDic.Add("Country", "国家");
            CompareDic.Add("QuantityPurchased", "购买数量");
            CompareDic.Add("MoneyType", "货币");
            CompareDic.Add("SettleAmount", "结算金额");
            //CompareDic.Add("ExchangeRafe", "外汇汇率");
            //CompareDic.Add("ExchangeAmount", "外汇金额");
            //CompareDic.Add("BackExchange", "回款汇率");
            CompareDic.Add("SalesVolume", "销售额（¥）");
            CompareDic.Add("Cost", "成本（¥）");
            CompareDic.Add("TransExpense", "运费（¥）");
            CompareDic.Add("BackAmount", "退款（¥）");
            CompareDic.Add("Profit", "利润（¥）");
            CompareDic.Add("ProfitRate", "利润率%");
            CompareDic.Add("Store", "店铺");
            //CompareDic.Add("ExchangeLose", "汇损");
            CompareDic.Add("State", "状态");

            return CompareDic;
        }

        public DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
            {
                propertyNameList.AddRange(propertyName);
            }
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        Type colType = pi.PropertyType;
                        if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        result.Columns.Add(pi.Name, colType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                        {
                            result.Columns.Add(pi.Name, pi.PropertyType);
                        }
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

    }
}

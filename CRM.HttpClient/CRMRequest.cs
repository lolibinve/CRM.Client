using HttpLib.HttpClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using CRM.Model;
using System.IO;
using System.Reflection;
using CLog;

namespace HttpLib
{
    public static partial class CRMRequest
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="cts"></param>
        /// <returns>1-管理员 0-业务员</returns>
        public static async Task<int?> LoginAsync(string account, string password, CancellationTokenSource cts)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"uname",account},
                {"psw",password},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/login", parameters, cts: cts);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<int?>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        #region 角色

        public static async Task<RoleModel> RoleList(string name)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"name",name},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/roleList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<RoleModel>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
        public static async Task<RoleData> ModifyRole(RoleData role)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"name",role.Name},
                {"psw",role.PassWord},
                {"admin",role.Admin.ToString()},
                {"skuTag",role.SkuTag},
                {"id",role.Id.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/roleEdit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<RoleData>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
        public static async Task<bool> DeleteRole(string id)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",id.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/roleDel", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<bool>>(result.Content);
                if (response.State == 0)
                {

                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        #region 产品

        public static async Task<ProductModel> ProductList(string sku, string uname, DateTime? startDate, DateTime? endDate, int pageNum = 1, int pageSize = 50)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"sku",sku},
                {"operator",uname},
                {"pageNum",pageNum.ToString()},
                {"pageSize",pageSize.ToString()},
                {"startDate",startDate?.ToString("yyyy-MM-dd")},
                {"endDate",endDate?.ToString("yyyy-MM-dd")},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/productList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<ProductModel>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
        public static async Task<ProductData> ModifyProduct(ProductData model)
        {
            var par1 = new Dictionary<string, string>()
            {
                {"id",model.Id.ToString()},
                {"image",model.ImageBase64Str},
                {"sku",model.Sku},
                {"remark",model.Remark},
                {"operator",model.Operator},
                {"date",model.Date?.ToString("yyyy-MM-dd")},
            };

            string json = JsonHelper.SerializeObject(model);
            //FileStream fs = new FileStream(model.Image, FileMode.Open, FileAccess.Read);
            HttpResult result = await CRMHttpClient.PostAsync($"crm/order/productEdit", json, formDataStrDic: par1);
            if (result.IsSuccess || !string.IsNullOrEmpty(result.Content))
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<ProductData>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
        public static async Task<bool> DeleteProduct(string id)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",id.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/productDel", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<bool>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        #region 汇率配置

        public static async Task<ExchangeModel> ExchangeList()
        {
            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/transRatioList", null);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<ExchangeModel>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
        public static async Task<ExchangeData> ModifyExchange(ExchangeData model)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",model.Id.ToString()},
                {"country",model.Country},
                {"ratio",model.Ratio},
                {"backRatio",model.BackRatio},

                {"initRatio",model.InitRatio},
                {"initBackRatio",model.InitBackRatio},
                {"midCurrency",model.MidCurrency},
                {"dv",model.Dv},
                {"isDvPercent",model.IsDvPercent?"1":"0"},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/transRatioEdit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<ExchangeData>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
        public static async Task<bool> DeleteExchange(string id)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",id.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/transRatioDel", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<bool>>(result.Content);
                if (response.State == 0)
                {

                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        #region 订单管理

        public static async Task<OrderModel> GetOrderLst(string uname, DateTime? startDate, DateTime? endDate, string sku, string orderNumber, string country, string store, int pageNum = 1, int pageSize = 50, int status = -1)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"uname",uname},
                {"pageNum",pageNum.ToString()},
                {"pageSize",pageSize.ToString()},
                {"startDate",startDate?.ToString("yyyy-MM-dd")},
                {"endDate",endDate?.ToString("yyyy-MM-dd")},
                {"sku",sku},
                {"orderId",orderNumber},
                {"country",country},
                {"status",status.ToString()},
                {"store",store},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/list", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<OrderModel>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        public static async Task<bool> PostFile(string filePath, string uname)
        {
            var par1 = new Dictionary<string, string>()
            {
                {"file",filePath},
            };

            var par2 = new Dictionary<string, string>()
            {
                {"uname",uname}
            };

            HttpResult result = await CRMHttpClient.PostAsync($"crm/order/import", null, formDataFileDic: par1, formDataStrDic: par2);
            if (result.IsSuccess || !string.IsNullOrEmpty(result.Content))
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<List<OrderData>>>(result.Content);
                if (response.State == 0)
                {

                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        public static async Task<OrderData> ModifyOrder(OrderData model)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",model.Id.ToString()},
                {"cost",model.Cost.ToString()},
                {"transExpense",model.TransExpense.ToString()},
                {"backAmount",model.BackAmount.ToString()},
                {"remark",model.Remark},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/edit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<OrderData>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        public static async Task<bool> ModifyState(string id, int state)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",id.ToString()},
                {"status",state.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/changeStatus", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<OrderData>>(result.Content);
                if (response.State == 0)
                {

                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        public static async Task<bool> AddOrder(OrderData model)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",model.Id.ToString()},
                {"store",model.Store},
                {"date",model.SaleDate?.ToString("yyyy-MM-dd")},
                {"orderId",model.OrderNumber},
                {"sku",model.SKU},
                {"uname",model.Account},
                {"country",model.Country},
                {"moneyType",model.MoneyType},
                {"settleAmount",model.SettleAmount.ToString()},
                {"exchangeRafe",model.ExchangeRafe.ToString()},
                {"exchangeAmount",model.ExchangeAmount.ToString()},
                {"backExchange",model.BackExchange.ToString()},
                {"salesVolume",model.SalesVolume.ToString()},
                {"cost",model.Cost.ToString()},
                {"transExpense",model.TransExpense.ToString()},
                {"backAmount",model.BackAmount.ToString()},
                {"profit",model.Profit.ToString()},
                {"profitRate",model.ProfitRate.ToString()},
                {"type",((int)model.State).ToString()},
                {"remark",model.Remark},
                {"buyerName",model.Buyer},
                {"phone",model.Phone},
                {"image",model.ImageBase64Str},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/edit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<OrderData>>(result.Content);
                if (response.State == 0)
                {

                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        public static async Task<bool> DeleteOrder(string id)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id",id.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/del", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion


        public static async Task<WareModel> GetWareLst(string uname, DateTime? startDate, DateTime? endDate, string sku, int pageNum = 1, int pageSize = 50)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"uname",uname},
                {"pageNum",pageNum.ToString()},
                {"pageSize",pageSize.ToString()},
                {"startDate",startDate?.ToString("yyyy-MM-dd")},
                {"endDate",endDate?.ToString("yyyy-MM-dd")},
                {"sku",sku},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/launchList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<WareModel>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        public static async Task<WareData> GetWareInfo(string sku)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"sku",sku},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/launchConfigInfo", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<WareData>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        public static async Task<PriceLstModel> GetCountryCurrencyInfo()
        {
            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/countryCurrencyInfo");
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<PriceLstModel>>(result.Content);
                if (response.State == 0)
                {

                    return response.Value;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        public static async Task<bool> AddWare(WareData model)
        {
            string json = JsonHelper.SerializeObject(model);

            HttpResult result = await CRMHttpClient.PostAsync($"crm/order/launchConfig", json);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<WareData>>(result.Content);
                if (response.State == 0)
                {

                    return true;
                }
                else
                {
                    MessageBox.Show(response.Desc);
                }
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #region 采购账户

        /// <summary>
        /// 采购账号分页列表，对应 GET <c>crm/purchase/accountList</c>（<c>PurchaseAccountListRequest</c>：<c>pageNum</c>、<c>pageSize</c>）。
        /// </summary>
        public static async Task<PurchaseAccountModel> PurchaseAccountList(int pageNum = 1, int pageSize = 20)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"pageNum", pageNum.ToString()},
                {"pageSize", pageSize.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/accountList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<PurchaseAccountModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value ?? new PurchaseAccountModel { AccountLst = new List<ProcurementAccountLstModel>(), Count = 0 };
                }

                MessageBox.Show(response.Desc ?? "获取采购账号列表失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        /// <summary>
        /// 采购账号新增/编辑，GET <c>crm/purchase/accountEdit</c>（<c>PurchaseAccountEditRequest</c>：<c>id</c>、<c>addTime</c>、<c>moneyIn</c>、<c>name</c>、<c>type</c>、<c>remark</c>）。
        /// 始终传 <c>id</c>：新增为 <c>0</c>，修改为实际主键；<c>addTime</c> 与 <c>ModifyProduct</c> 中 <c>date</c> 相同，为 <c>yyyy-MM-dd</c> 字符串。
        /// </summary>
        public static async Task<bool> PurchaseAccountEdit(ProcurementAccountLstModel model)
        {
            if (model == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.ProcurementAccount))
            {
                MessageBox.Show("请输入采购账号");
                return false;
            }

            var name = model.ProcurementAccount.Trim();
            var addTimeStr = (model.Date ?? DateTime.Now.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var parameters = new Dictionary<string, string>()
            {
                {"id", model.Id.ToString()},
                {"addTime", addTimeStr},
                {"moneyIn", model.Amount.ToString(CultureInfo.InvariantCulture)},
                {"name", name},
                {"type", model.AccountType.ToString(CultureInfo.InvariantCulture)},
                {"remark", model.Remark ?? ""},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/accountEdit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "保存采购账号失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        /// <summary>
        /// 采购账号删除：GET <c>crm/purchase/accountDel</c>，Query 参数 <c>id</c>（与 <c>/crm/purchase/accountDel?id=1</c> 一致）。
        /// </summary>
        public static async Task<bool> PurchaseAccountDelete(int id)
        {
            if (id <= 0)
            {
                MessageBox.Show("无效的记录 id，无法删除。");
                return false;
            }

            var parameters = new Dictionary<string, string>()
            {
                {"id", id.ToString(CultureInfo.InvariantCulture)},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/accountDel", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "删除失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        #region FBM 采购

        /// <summary>
        /// FBM（现金采购）分页列表，GET <c>crm/purchase/fbmList</c>。
        /// </summary>
        public static async Task<FbmPurchaseListModel> FbmList(int pageNum, int pageSize,
            string purchaseAccount = null, string buyerName = null, string orderId = null,
            string startDate = null, string endDate = null)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"pageNum", pageNum.ToString()},
                {"pageSize", pageSize.ToString()},
                {"purchaseAccount", purchaseAccount ?? ""},
                {"buyerName", buyerName ?? ""},
                {"orderId", orderId ?? ""},
                {"startDate", startDate ?? ""},
                {"endDate", endDate ?? ""},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/fbmList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<FbmPurchaseListModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value ?? new FbmPurchaseListModel { Count = 0, List = new List<FbmPurchaseRecordModel>() };
                }

                MessageBox.Show(response.Desc ?? "获取 FBM 采购列表失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        /// <summary>
        /// FBM 采购新增/编辑，GET <c>crm/purchase/fbmEdit</c>（<c>PurchaseEditRequest</c>：<c>orderId</c>、<c>addTime</c>、<c>expense</c> 等）。
        /// 始终传 <c>id</c>：新增为 <c>0</c>，修改为实际主键。
        /// </summary>
        public static async Task<bool> FbmPurchaseEdit(FbmPurchaseRecordModel model)
        {
            if (model == null)
            {
                return false;
            }

            var parameters = new Dictionary<string, string>()
            {
                {"id", model.Id.ToString()},
                {"orderId", model.OrderId ?? ""},
                {"addTime", model.PurchaseDateEdit?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""},
                {"expense", model.Expense.ToString(CultureInfo.InvariantCulture)},
                {"Uname", model.BuyerName ?? ""},
                {"accountName", model.PurchaseAccount ?? ""},
                {"payment", model.Payment.ToString(CultureInfo.InvariantCulture)},
                {"remark", model.Remark ?? ""},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/fbmEdit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "保存失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        /// <summary>
        /// FBM 采购删除：GET <c>crm/purchase/fbmDel</c>，Query 参数 <c>id</c>（与 <c>/crm/purchase/fbmDel?id=100</c> 一致）。
        /// </summary>
        public static async Task<bool> FbmPurchaseDelete(int id)
        {
            if (id <= 0)
            {
                MessageBox.Show("无效的记录 id，无法删除。");
                return false;
            }

            var parameters = new Dictionary<string, string>()
            {
                {"id", id.ToString(CultureInfo.InvariantCulture)},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/fbmDel", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "删除失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        #region 备货汇总（产品库存）

        /// <summary>
        /// 备货汇总分页列表，GET <c>crm/purchase/stockManageList</c>。
        /// </summary>
        public static async Task<StockProductListModel> StockManageList(int pageNum = 1, int pageSize = 20)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"pageNum", pageNum.ToString()},
                {"pageSize", pageSize.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/stockManageList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<StockProductListModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value ?? new StockProductListModel { Count = 0, List = new List<StockProductRecordModel>() };
                }

                MessageBox.Show(response.Desc ?? "获取产品库存列表失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        /// <summary>
        /// 备货汇总新增/编辑，GET <c>crm/purchase/stockManageEdit</c>（<c>pId</c>、<c>pName</c>）。
        /// 始终传 <c>id</c>：新增为 <c>0</c>，修改为实际主键。
        /// </summary>
        public static async Task<bool> StockManageEdit(StockProductRecordModel model)
        {
            if (model == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.ProductCode))
            {
                MessageBox.Show("请输入产品编码");
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                MessageBox.Show("请输入产品名称");
                return false;
            }

            var parameters = new Dictionary<string, string>()
            {
                {"id", model.Id.ToString()},
                {"pId", model.ProductCode.Trim()},
                {"pName", model.ProductName.Trim()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/stockManageEdit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "保存失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        /// <summary>
        /// 备货汇总删除（若后端未提供此路由，请按实际文档修改路径或移除此方法）。
        /// </summary>
        public static async Task<bool> StockManageDelete(int id)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"id", id.ToString()},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/stockManageDel", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "删除失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        #region 备货采购（备货流水 stockList / stockEdit）

        /// <summary>
        /// 备货流水分页列表，GET <c>crm/purchase/stockList</c>。<paramref name="type"/> 必填且不能为 0；
        /// 与前端库存视图一致时常用：<see cref="StockPurchaseConstants.StockListInTransit"/>（模块2）、<see cref="StockPurchaseConstants.StockListArrivedWarehouse"/>（模块3）、<see cref="StockPurchaseConstants.StockListDeadstock"/>（模块4）。
        /// </summary>
        public static async Task<StockPurchaseListModel> StockList(int type, int pageNum = 1, int pageSize = 20,
            string productCode = null, string buyerName = null, string purId = null)
        {
            if (type == 0)
            {
                MessageBox.Show("请选择库存/货件类型筛选条件");
                return null;
            }

            var parameters = new Dictionary<string, string>()
            {
                {"type", type.ToString(CultureInfo.InvariantCulture)},
                {"pageNum", pageNum.ToString(CultureInfo.InvariantCulture)},
                {"pageSize", pageSize.ToString(CultureInfo.InvariantCulture)},
                {"p_id", productCode ?? ""},
                {"buyer_name", buyerName ?? ""},
                {"purId", purId ?? ""},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/stockList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<StockPurchaseListModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value ?? new StockPurchaseListModel { Count = 0, List = new List<StockPurchaseRecordModel>() };
                }

                MessageBox.Show(response.Desc ?? "获取备货采购列表失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        /// <summary>
        /// 备货采购新增/编辑，GET <c>crm/purchase/stockEdit</c>。
        /// <c>addTime</c> 与 <c>fbmEdit</c> 一致：日期字符串 <c>yyyy-MM-dd</c>（由 <see cref="StockPurchaseRecordModel.PurchaseDate"/> 格式化；无日期则为空串）。
        /// <c>instockTime</c> 为到仓时间，<strong>精确到自然日</strong>，提交 <c>yyyy-MM-dd</c>。
        /// </summary>
        public static async Task<bool> StockPurchaseEdit(StockPurchaseRecordModel model)
        {
            if (model == null)
            {
                return false;
            }

            model.RecalculateUnitFieldsForSave();

            var parameters = new Dictionary<string, string>()
            {
                {"id", model.Id.ToString(CultureInfo.InvariantCulture)},
                {"purId", model.PurId ?? ""},
                {"pId", model.ProductCode ?? ""},
                {"pName", model.ProductName ?? ""},
                {"quantity", model.Quantity.ToString(CultureInfo.InvariantCulture)},
                {"expense", model.Expense.ToString(CultureInfo.InvariantCulture)},
                {"unitValue", model.UnitValue.ToString(CultureInfo.InvariantCulture)},
                {"payment", model.Payment.ToString(CultureInfo.InvariantCulture)},
                {"transFee", model.TransFee.ToString(CultureInfo.InvariantCulture)},
                {"unitTransFee", model.UnitTransFee.ToString(CultureInfo.InvariantCulture)},
                {"unitCost", model.UnitCost.ToString(CultureInfo.InvariantCulture)},
                {"userName", model.UserName ?? ""},
                {"type", model.ShipmentType.ToString(CultureInfo.InvariantCulture)},
                {"addTime", model.PurchaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""},
                {"instockTime", model.InstockDateTime.HasValue ? model.InstockDateTime.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : ""},
                {"remark", model.Remark ?? ""},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/stockEdit", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "保存失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 下载文件接口
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="country"></param>
        /// <param name="externalProductType">UPC</param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static async Task<byte[]> ExportFileDownLoad(string dirPath,string ids, string country, string externalProductType, string brandName, string recommendedBrowseNodes, bool IsUpdate = true)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"ids",ids},
                {"country",country},
                {"brandName",brandName},
                {"recommendedBrowseNodes",recommendedBrowseNodes},
                {"update",IsUpdate?"Update":"Delete"},
                {"externalProductType",externalProductType},
            };

            HttpResult result = await CRMHttpClient.GetAsync($"crm/order/launchExport", parameters, isStream: true);
            if (result.IsSuccess)
            {
                return result.Bytes;
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }
    }
}

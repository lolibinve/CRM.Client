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

        /// <summary>
        /// 订单新增/编辑，GET <c>crm/order/edit</c>。
        /// 编辑（<c>id&gt;0</c>）时若用户在页面上更改过采购方式，传 <c>useStock=1</c>，否则 <c>useStock=0</c>。
        /// </summary>
        public static async Task<bool> AddOrder(OrderData model, int useStock = 0)
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
                {"purchaseMethod", model.PurchaseMethod.ToString(CultureInfo.InvariantCulture)},
                {"purId", model.PurId ?? ""},
                {"shipQuantity", model.ShipQuantity.ToString(CultureInfo.InvariantCulture)},
            };

            // 订单“修改”页面提交额外字段：purchaseType / purchaseId / sendAmount
            if (model.Id > 0)
            {
                parameters["purchaseType"] = model.PurchaseMethod.ToString(CultureInfo.InvariantCulture);
                parameters["purchaseId"] = model.PurId ?? "";
                parameters["sendAmount"] = model.ShipQuantity.ToString(CultureInfo.InvariantCulture);
                parameters["useStock"] = useStock.ToString(CultureInfo.InvariantCulture);
            }

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
        /// 触发采购账号余额刷新任务，GET <c>crm/login/taskPurchaseAccountBalance</c>。
        /// </summary>
        public static async Task<bool> TaskPurchaseAccountBalance()
        {
            HttpResult result = await CRMHttpClient.GetAsync($"crm/login/taskPurchaseAccountBalance", null);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "刷新余额任务失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

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
        /// 采购账号新增/编辑，GET <c>crm/purchase/accountEdit</c>（<c>id</c>、<c>addTime</c>、<c>name</c>、<c>remark</c>；不再传 <c>moneyIn</c>、<c>type</c>）。
        /// 始终传 <c>id</c>：新增为 <c>0</c>，修改为实际主键；<c>addTime</c> 为 <c>yyyy-MM-dd</c> 字符串。
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
                {"name", name},
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

        /// <summary>
        /// 采购账号入账，GET <c>crm/purchase/accountCheckIn</c>（<c>PurchaseAccountCheckInRequest</c>：<c>id</c>、<c>amount</c>、<c>type</c>、<c>remark</c>）。
        /// 写入 <c>purchase_account_check_in</c> 并重算余额（异步）。
        /// </summary>
        public static async Task<bool> PurchaseAccountCheckIn(int accountId, long amount, int type, string remark = null)
        {
            if (accountId <= 0)
            {
                MessageBox.Show("无效的采购账号 id。");
                return false;
            }

            if (amount <= 0)
            {
                MessageBox.Show("入账金额必须大于 0。");
                return false;
            }

            if (type != 0 && type != 1)
            {
                MessageBox.Show("资金类型无效。");
                return false;
            }

            var parameters = new Dictionary<string, string>
            {
                {"id", accountId.ToString(CultureInfo.InvariantCulture)},
                {"amount", amount.ToString(CultureInfo.InvariantCulture)},
                {"type", type.ToString(CultureInfo.InvariantCulture)},
            };
            if (!string.IsNullOrWhiteSpace(remark))
                parameters["remark"] = remark.Trim();

            HttpResult result = await CRMHttpClient.GetAsync("crm/purchase/accountCheckIn", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "入账失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

        /// <summary>
        /// 采购账号入账流水分页，GET <c>crm/purchase/accountCheckInList</c>（<c>PurchaseAccountCheckInListRequest</c>）。
        /// <paramref name="type"/> 为 <c>-1</c> 时不按资金类型过滤。
        /// <paramref name="startDate"/>、<paramref name="endDate"/> 为 <c>yyyy-MM-dd</c>，非空时传入。
        /// </summary>
        public static async Task<PurchaseAccountCheckInListModel> PurchaseAccountCheckInList(
            string name,
            int pageNum = 1,
            int pageSize = 20,
            int type = -1,
            string startDate = null,
            string endDate = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("采购账号名称不能为空。");
                return null;
            }

            var parameters = new Dictionary<string, string>
            {
                {"name", name.Trim()},
                {"pageNum", pageNum.ToString(CultureInfo.InvariantCulture)},
                {"pageSize", pageSize.ToString(CultureInfo.InvariantCulture)},
                {"type", type.ToString(CultureInfo.InvariantCulture)},
            };
            if (!string.IsNullOrWhiteSpace(startDate))
                parameters["startDate"] = startDate.Trim();
            if (!string.IsNullOrWhiteSpace(endDate))
                parameters["endDate"] = endDate.Trim();

            HttpResult result = await CRMHttpClient.GetAsync("crm/purchase/accountCheckInList", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<PurchaseAccountCheckInListModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value ?? new PurchaseAccountCheckInListModel
                    {
                        Count = 0,
                        List = new List<PurchaseAccountCheckInRecordModel>(),
                    };
                }

                MessageBox.Show(response.Desc ?? "获取入账流水失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
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
                {"pageNum", pageNum.ToString(CultureInfo.InvariantCulture)},
                {"pageSize", pageSize.ToString(CultureInfo.InvariantCulture)},
            };
            if (!string.IsNullOrWhiteSpace(purchaseAccount))
                parameters["purchaseAccount"] = purchaseAccount.Trim();
            if (!string.IsNullOrWhiteSpace(buyerName))
                parameters["buyerName"] = buyerName.Trim();
            if (!string.IsNullOrWhiteSpace(orderId))
                parameters["orderId"] = orderId.Trim();
            if (!string.IsNullOrWhiteSpace(startDate))
                parameters["startDate"] = startDate.Trim();
            if (!string.IsNullOrWhiteSpace(endDate))
                parameters["endDate"] = endDate.Trim();

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
                {"uName", model.BuyerName ?? ""},
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
        /// 触发备货汇总库存更新任务，GET <c>crm/login/taskStockManageSummary</c>。
        /// </summary>
        public static async Task<bool> TaskStockManageSummary()
        {
            HttpResult result = await CRMHttpClient.GetAsync($"crm/login/taskStockManageSummary", null);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<object>>(result.Content);
                if (response.State == 0)
                {
                    return true;
                }

                MessageBox.Show(response.Desc ?? "更新库存任务失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return false;
        }

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

        /// <summary>备货接口金额类参数：以「元」为单位、固定两位小数字符串（与 <see cref="StockPurchaseRecordModel"/> 约定一致）。</summary>
        private static string FormatStockYuanQuery(decimal yuan) =>
            yuan.ToString("F2", CultureInfo.InvariantCulture);

        /// <summary>
        /// 备货流水分页列表，GET <c>crm/purchase/stockList</c>。<paramref name="type"/> 必填；
        /// 与前端库存视图一致时常用：<see cref="StockShipmentStatus.InTransit"/>（模块2）、<see cref="StockShipmentStatus.ArrivedWarehouse"/>（模块3）、<see cref="StockShipmentStatus.Deadstock"/>（模块4）、<see cref="StockShipmentStatus.SoldOut"/>（模块5）。
        /// </summary>
        public static async Task<StockPurchaseListModel> StockList(int type, int pageNum = 1, int pageSize = 20,
            string productCode = null, string buyerName = null, string purId = null)
        {
            if (type < 0)
            {
                MessageBox.Show("请选择库存/货件类型筛选条件");
                return null;
            }

            var parameters = new Dictionary<string, string>()
            {
                {"type", type.ToString(CultureInfo.InvariantCulture)},
                {"pageNum", pageNum.ToString(CultureInfo.InvariantCulture)},
                {"pageSize", pageSize.ToString(CultureInfo.InvariantCulture)},
            };
            if (!string.IsNullOrWhiteSpace(productCode))
                parameters["pId"] = productCode.Trim();
            if (!string.IsNullOrWhiteSpace(buyerName))
                parameters["buyerName"] = buyerName.Trim();
            if (!string.IsNullOrWhiteSpace(purId))
                parameters["purId"] = purId.Trim();

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
                {"expense", FormatStockYuanQuery(model.Expense)},
                {"unitValue", FormatStockYuanQuery(model.UnitValue)},
                {"payment", model.Payment.ToString(CultureInfo.InvariantCulture)},
                {"transFee", FormatStockYuanQuery(model.TransFee)},
                {"unitTransFee", FormatStockYuanQuery(model.UnitTransFee)},
                {"unitCost", FormatStockYuanQuery(model.UnitCost)},
                {"userName", model.UserName ?? ""},
                {"type", model.ShipmentType.ToString(CultureInfo.InvariantCulture)},
                {"addTime", model.PurchaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""},
                {"instockTime", model.InstockDateTime.HasValue ? model.InstockDateTime.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : ""},
                {"remark", model.Remark ?? ""},
            };
            if (model.ShipmentTypeOld.HasValue)
                parameters["typeOld"] = model.ShipmentTypeOld.Value.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(model.PurchaseAccount))
                parameters["purchaseAccount"] = model.PurchaseAccount.Trim();

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

        /// <summary>
        /// 按订单号查 FBM 现金采购，GET <c>crm/purchase/fbmInfoByOrderId</c>。
        /// 成功 <c>data</c> 为单条 <c>PurchaseRecordFbm</c>（<c>expense</c> 为采购金额）；见 <c>PurchaseController-API(3).md</c> §15。
        /// </summary>
        public static async Task<FbmPurchaseRecordModel> FbmInfoByOrderId(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return null;
            }

            var parameters = new Dictionary<string, string> { { "orderId", orderId.Trim() } };
            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/fbmInfoByOrderId", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<FbmPurchaseRecordModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value;
                }

                MessageBox.Show(response.Desc ?? "未找到 FBM 采购信息");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        /// <summary>
        /// 按采购批次查备货，GET <c>crm/purchase/stockInfoByPurId</c>。
        /// 成功 <c>data</c> 为单条 <c>StockRecord</c>（含 <c>stayQuantity</c>、<c>unitCost</c> 等），与 <c>stockList</c> 列表行同形；见 <c>PurchaseController-API(3).md</c> §14。
        /// </summary>
        public static async Task<StockPurchaseRecordModel> StockInfoByPurId(string purId)
        {
            if (string.IsNullOrWhiteSpace(purId))
            {
                return null;
            }

            var parameters = new Dictionary<string, string> { { "purId", purId.Trim() } };
            HttpResult result = await CRMHttpClient.GetAsync($"crm/purchase/stockInfoByPurId", parameters);
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<StockPurchaseRecordModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value;
                }

                MessageBox.Show(response.Desc ?? "未找到该采购批次备货信息");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
        }

        /// <summary>
        /// 滞留剩余库存采购批次列表，GET <c>crm/purchase/stockStalePurIdList</c>。
        /// 成功 <c>data</c> 为 <c>{ list: string[] }</c>。
        /// </summary>
        public static async Task<List<string>> StockStalePurIdList()
        {
            HttpResult result = await CRMHttpClient.GetAsync("crm/purchase/stockStalePurIdList", new Dictionary<string, string>());
            if (result.IsSuccess)
            {
                var response = JsonHelper.DeserializeObject<CRMHttpResponse<StockStalePurIdListModel>>(result.Content);
                if (response.State == 0)
                {
                    return response.Value?.List ?? new List<string>();
                }

                MessageBox.Show(response.Desc ?? "获取滞留库存采购批次失败");
            }
            else
            {
                MessageBox.Show(result.ErrorMessage());
            }

            return null;
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

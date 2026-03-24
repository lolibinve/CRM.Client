using HttpLib.HttpClient;
using System;
using System.Collections.Generic;
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

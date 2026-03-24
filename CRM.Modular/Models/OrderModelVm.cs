using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Help;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CRM.Modular.Models
{
    [AddINotifyPropertyChangedInterface]
    public class OrderModelVm
    {
        /// <summary>
        /// 店铺
        /// </summary>
        [ExcelColumn("店铺")]
        public string Store { get; set; }

        [ExcelColumn("日期")]
        public DateTime? SaleDate { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [ExcelColumn("订单号")]
        public string OrderNumber { get; set; }

        [ExcelColumn("SKU")]
        public string SKU { get; set; }


        [ExcelColumn("业务员")]
        public string Account { get; set; }


        [ExcelColumn("国家")]
        public string Country { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [ExcelColumn("货币")]
        public string MoneyType { get; set; }

        /// <summary>
        /// 结算金额
        /// </summary>
        [ExcelColumn("结算金额")]
        public float? SettelAmount { get; set; }

        /// <summary>
        /// 外汇汇率
        /// </summary>
        [ExcelColumn("外汇汇率")]
        public float? ExchangeRafe { get; set; }

        /// <summary>
        /// 外汇金额
        /// </summary>
        [ExcelColumn("外汇金额")]
        public float? ExchangeAmount { get; set; }

        /// <summary>
        /// 回款汇率
        /// </summary>
        [ExcelColumn("回款汇率")]
        public float? BackExchange { get; set; }

        /// <summary>
        /// 销售额（¥）
        /// </summary>
        [ExcelColumn("销售额（¥）")]
        public float? SalesVolume { get; set; }

        /// <summary>
        /// 成本（¥）
        /// </summary>
        [ExcelColumn("成本（¥）")]
        public float? Cost { get; set; }

        /// <summary>
        /// 运费（¥）
        /// </summary>
        [ExcelColumn("运费（¥）")]
        public float? TransExpense { get; set; }

        /// <summary>
        /// 退款（¥）
        /// </summary>
        [ExcelColumn("退款（¥）")]
        public float? BackAmount { get; set; }

        /// <summary>
        /// 利润（¥）
        /// </summary>
        [ExcelColumn("利润（¥）")]
        public float? Profit { get; set; }

        /// <summary>
        /// 利润率%
        /// </summary>
        [ExcelColumn("利润率%")]
        public float? ProfitRate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ExcelColumn("状态")]
        public string State { get; set; }

        /// <summary>
        /// 汇损
        /// </summary>
        [ExcelColumn("备注")]
        public string Remark { get; set; }
    }


    [AddINotifyPropertyChangedInterface]
    public class PageInfoModel
    {
        public int PageNum { set; get; }
        public int Total { set; get; }
        public int PageSize { set; get; }
        public int PagesCount { set; get; }

    }
}

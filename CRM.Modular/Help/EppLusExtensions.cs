using CLog;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Modular.Help
{

    public static class EppLusExtensions
    {
        /// <summary>
        /// 获取标签对应excel的Index
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int GetColumnByName(this ExcelWorksheet ws, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            if (ws.Cells["1:1"].Any(c => c.Value?.ToString() == columnName))
            {
                return ws.Cells["1:1"].First(c => c.Value?.ToString() == columnName).Start.Column;
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// 扩展方法
        /// </summary>
        /// <param name="worksheet"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        {

            Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(CRM.Model.ExcelColumn);
            var columns = typeof(T)
                .GetProperties()
                .Where(x => x.CustomAttributes.Any(columnOnly))
                .Select(p => new
                {
                    Property = p,
                    Column = p.GetCustomAttributes<CRM.Model.ExcelColumn>().First().ColumnName
                }).ToList();


            var OrderNumberIndex = GetColumnByName(worksheet, "订单号");
            int rowCount = worksheet.AutoFilterAddress.Rows;

            int validCount = 1;
            for (int i = 1; i < rowCount; i++)
            {
                var val = worksheet.Cells[i, OrderNumberIndex];
                if(val == null || val.Text == "0")
                {
                    validCount = i;
                    break;
                }
            }
            var rows = worksheet.Cells
                   .Select(cell => cell.Start.Row)
                   .Distinct()
                   .OrderBy(x => x).Skip(1).Take(validCount - 2);

            List<T> temp = new List<T>();
            foreach (var row in rows)
            {
                var tnew = new T();
                foreach (var col in columns)
                {
                    int index = GetColumnByName(worksheet, col.Column);
                    if (index < 0)
                    {
                        continue;
                    }

                    var val = worksheet.Cells[row, index];
                    try
                    {
                        if (val.Value == null)
                        {
                            col.Property.SetValue(tnew, null);
                            continue;
                        }
                        else if (col.Property.PropertyType == typeof(float?))
                        {
                            col.Property.SetValue(tnew, val.GetValue<float?>());
                            continue;
                        }
                        else if (col.Property.PropertyType == typeof(bool))
                        {
                            col.Property.SetValue(tnew, val.GetValue<bool>());
                            continue;
                        }
                        else if (col.Property.PropertyType == typeof(DateTime?))
                        {
                            col.Property.SetValue(tnew, val.GetValue<DateTime?>());
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    col.Property.SetValue(tnew, val.GetValue<string>());
                }
                temp.Add(tnew);
            }

            return temp;


            //var collection = rows
            //    .Select(row =>
            //    {
            //        var tnew = new T();
            //        columns.ForEach(col =>
            //        {
            //            int index = GetColumnByName(worksheet, col.Column);
            //            if (index < 0)
            //            {
            //                return;
            //            }

            //            var val = worksheet.Cells[row, index];
            //            try
            //            {
            //                if (val.Value == null)
            //                {
            //                    col.Property.SetValue(tnew, null);
            //                    return;
            //                }
            //                else if (col.Property.PropertyType == typeof(float?))
            //                {
            //                    col.Property.SetValue(tnew, val.GetValue<float?>());
            //                    return;
            //                }
            //                else if (col.Property.PropertyType == typeof(bool))
            //                {
            //                    col.Property.SetValue(tnew, val.GetValue<bool>());
            //                    return;
            //                }
            //                else if (col.Property.PropertyType == typeof(DateTime?))
            //                {
            //                    col.Property.SetValue(tnew, val.GetValue<DateTime?>());
            //                    return;
            //                }
            //            }
            //            catch (Exception)
            //            {
            //                return;
            //            }
            //            col.Property.SetValue(tnew, val.GetValue<string>());
            //        });

            //        return tnew;
            //    });
            //return collection;
        }

    }
}

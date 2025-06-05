using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmourgisCOREServices;
using CalliAPI.BusinessLogic;
using CalliAPI.DataAccess;
using CalliAPI.Models;
using CalliAPI.UI;
using Task = System.Threading.Tasks.Task;

namespace CalliAPI.Utilities
{


    public static class ReportLauncher
    {
        private static readonly AMO_Logger _logger = new AMO_Logger("CalliAPI");

        /// <summary>
        /// This method is called from the ClioService to show a report. It takes an IAsyncEnumerable<Matter> and converts it to a DataTable, then displays it
        /// in a ReportForm.
        /// </summary>
        /// <param name="matters"></param>
        /// <returns></returns>
        public static async Task ShowAsync(IAsyncEnumerable<Matter> matters)
        {
            if (matters == null)
            {
                _logger.LogError("matters is null");
                return;
            }

            _logger.Info("ReportLauncher.ShowAsync called");
            // This method is called from the ClioService to show a report
            // It takes an IAsyncEnumerable<Matter> and converts it to a DataTable
            var table = await matters.ToSmartDataTableAsync(); // Defined in MatterFilters.cs, this consumes the stream
            var form = new ReportForm();
            form.SetData(table);
            form.Show();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var table = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in items)
            {
                var row = table.NewRow();
                foreach (var prop in props)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }


    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmourgisCOREServices;
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

    }


}

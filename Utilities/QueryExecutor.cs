using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalliAPI.BusinessLogic;
using CalliAPI.DataAccess;
using CalliAPI.Interfaces;
using CalliAPI.Models;

namespace CalliAPI.Utilities
{

    public class QueryExecutor
    {
        public static async System.Threading.Tasks.Task ExecuteAsync<T>(
        IQueryBuilder<T> builder,
        ClioService clioService,
        ClioApiClient clioApiClient)
        {
            string fields = builder.SelectedFields;
            var filters = builder.SelectedFilters;

            IAsyncEnumerable<T> results = GetResults<T>(clioService, clioApiClient, fields);

            foreach (var filter in filters)
            {
                results = filter(results);
            }

            await ReportLauncher_ShowAsync(results, clioApiClient);
        }

        private static IAsyncEnumerable<T> GetResults<T>(ClioService clioService, ClioApiClient clioApiClient, string fields)
        {
            if (typeof(T) == typeof(Matter))
            {
                return (IAsyncEnumerable<T>)clioService.GetAllMattersAsync(fields);
            }
            else if (typeof(T) == typeof(CalliAPI.Models.Task))
            {
                //return (IAsyncEnumerable<T>)clioService.GetAllTasksAsync(fields); // hypothetical
            }

            throw new NotSupportedException($"Querying for type {typeof(T).Name} is not supported.");
        }

        private static async System.Threading.Tasks.Task ReportLauncher_ShowAsync<T>(IAsyncEnumerable<T> results, ClioApiClient clioApiClient)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results), "Results cannot be null.");
            }

            if (results is IAsyncEnumerable<Matter> matters)
            {
                await ReportLauncher.ShowAsync((IAsyncEnumerable<Matter>)results);
            }
            
        }
    }

}

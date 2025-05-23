﻿// Project: CalliAPI
// Namespace: CalliAPI.BusinessLogic
// FileName: ClioService.cs
// Jacob Hayes
// 2025 05 21

// FastFetch refers to the process of quickly retrieving all matters from Clio through parallization of offset requests.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalliAPI.Interfaces;
using CalliAPI.Models;
using AmourgisCOREServices;
using CalliAPI.Utilities;
using CalliAPI.DataAccess;
using Task = System.Threading.Tasks.Task;

namespace CalliAPI.BusinessLogic
{
    public class ClioService
    {
        private readonly AMO_Logger _logger;

        public static string[] prefileStages =
            [
                "prefile",
                "pif - prefile",
                "case prep",
                "pif - case prep",
                "signing and filing",
                "prefiling",
                "pif - prefiling"
            ];

        private readonly ClioApiClient _clioApiClient;

        public ClioService(ClioApiClient clioApiClient, AMO_Logger logger)
        {
            _clioApiClient = clioApiClient;
            this._logger = logger;
        }

        #region event delegates
        public event Action<int, int> ProgressUpdated
        {
            add => _clioApiClient.ProgressUpdated += value;
            remove => _clioApiClient.ProgressUpdated -= value;
        }
        #endregion

        public async Task<string> VerifyAPI()
        {
            return await _clioApiClient.VerifyAPI();
        }


        public async Task<List<Matter>> GetMattersNotCurrentlyBeingWorked()
        {
            return await _clioApiClient.GetMattersNotCurrentlyBeingWorked();
        }


        public static string GetPrefileStageDescription()
        {
            return $"Prefile stages include: {string.Join(", ", prefileStages)}.";
        }


        #region Classic Reports
        #region reports - open Matters


        public async Task GetAllOpenMatters()
        {
            // Initialize the matter stream and filter it
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllOpenMattersAsync();
            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(matters);
        }

        public async Task GetAllOpen713Matters()
        {
            // Initialize the matter stream and filter it
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllOpenMattersAsync().FilterByPracticeAreaSuffixAsync(new string[] { "7", "13" });
            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(matters);
        }

        public async Task GetUnworked713Matters()
        {

            // Initialize the matter stream and filter it
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllOpenMattersAsync().
                FilterByPracticeAreaSuffixAsync(new string[] { "7", "13" }).
                FilterByStageNameAsync(prefileStages);

            IAsyncEnumerable<Matter> filteredMatters = FilterMattersWithNoOpenTasksAsync(matters);

            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(filteredMatters);
        }


        #endregion

        #region reports - all Matters
        public async Task GetAllMatters()
        {
            // Initialize the matter stream
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllMattersAsync();
            // Filter the matter stream (since this is ALL matters, we don't need to bother with filtering)
            // matters = matters.FilterByWhatever();
            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(matters);
        }



        #endregion

        #region helper methods

        public async Task<bool> MatterHasNoOpenTasksAsync(Matter matter)
        {
            if (!matter.has_tasks)
                return true;

            var tasks = await _clioApiClient.GetTasksForMatterAsync(matter.id);
            return tasks.All(t => t.status.Equals("complete", StringComparison.OrdinalIgnoreCase));
        }


        public async IAsyncEnumerable<Matter> FilterMattersWithNoOpenTasksAsync(IAsyncEnumerable<Matter> matters)
        {
            await foreach (var matter in matters)
            {
                if (await MatterHasNoOpenTasksAsync(matter))
                {
                    _logger.Info($"No tasks found for matter {matter.id.ToString()}.");
                    yield return matter;
                }
            }
        }


        #endregion
        #endregion
        #region FastFetch reports
        public async Task FastFetchAllMatters(DateTime dateSince)
        {
            IAsyncEnumerable<Matter> matters = _clioApiClient.FastFetchMattersSinceAsync(dateSince);

            await ReportLauncher.ShowAsync(matters);
        }
        #endregion

    }
}

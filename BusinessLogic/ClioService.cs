// Project: CalliAPI
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
using Microsoft.Identity.Client;
using System.Net;
using System.Globalization;

namespace CalliAPI.BusinessLogic
{
    public class ClioService(ClioApiClient clioApiClient, AuthService authService)
    {
        
        private readonly AMO_Logger _logger = AMO_Logger.Instance;
        private readonly ClioApiClient _clioApiClient = clioApiClient;
        private readonly AuthService _authService = authService;



        public static IReadOnlyList<string> PrefileStages { get; } =
[
    "prefile",
    "pif - prefile",
    "case prep",
    "pif - case prep",
    "signing and filing",
    "prefiling",
    "pif - prefiling"
];

        #region Authentication Methods
        public string GetAuthorizationUrl()
        {
            return _authService.GetAuthorizationUrl();
        }

        public async Task GetAccessTokenAsync(string authorizationCode)
        {
            await _authService.GetAccessTokenAsync(authorizationCode);
        }

        public string? ValidateAuthorizationCode(string userInput)
        {
            return _authService.ValidateAuthorizationCode(userInput);
        }


        /// <summary>
        /// Checks if the user is authenticated with Clio.
        /// </summary>
        public async Task<bool> IsAuthenticated() => await _authService.TryLoadOrRefreshTokenAsync();
        #endregion

        #region Delegates

        /// <summary>
        /// Tracks the progress of a long-running operation, such as fetching matters or tasks.
        /// </summary>
        public event Action<int, int> ProgressUpdated;

        /// <summary>
        /// Tracks the progress of practice area filtering operations.
        /// </summary>
        public event Action<int, int>? PracticeAreaProgressUpdated;

        #endregion

        public async Task<HttpResponseMessage> VerifyAPI()
        {
            return await _clioApiClient.VerifyAPI(_authService.AccessToken);
        }


        public async Task<List<Matter>> GetMattersNotCurrentlyBeingWorked()
        {
            return await _clioApiClient.GetMattersNotCurrentlyBeingWorked(_authService.AccessToken);
        }


        public static string GetPrefileStageDescription()
        {
            return $"Prefile stages include: {string.Join(", ", PrefileStages)}.";
        }


        #region Custom Report Methods
        #endregion

        #region Classic Reports
        #region reports - open Matters

        public async Task GetUnworked713Matters()
        {
            _logger.Info("ClioService: GetUnworked713Matters called.");
            // Initialize the matter stream and filter it


                var matters = _clioApiClient.GetAllMattersAsync(status: "open,pending")
#if DEBUG
.LogEachAsync(_logger, "All Matters")
#endif
                    .FilterByPracticeAreaSuffixAsync(["7", "13"])
#if DEBUG
.LogEachAsync(_logger, "After Suffix Filter")
#endif
                    .FilterByStageNameAsync([.. PrefileStages])
#if DEBUG
.LogEachAsync(_logger, "After Stage Filter")
#endif
                    ;

            IAsyncEnumerable<Matter> filteredMatters = FilterMattersWithNoOpenTasksParallelAsync(matters);

            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(filteredMatters);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="matters"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Matter> FilterMattersWithNoOpenTasksParallelAsync(IAsyncEnumerable<Matter> matters)
        {
            var semaphore = new SemaphoreSlim(2); // limit to 2 concurrent requests (any more and we trigger retry exponential backoff)
            var tasks = new List<Task<Matter?>>();

            await foreach (var matter in matters)
            {
                await semaphore.WaitAsync();

                tasks.Add(System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        if (!matter.has_tasks)
                        {
                            _logger.Info("No tasks found, including this matter.");
                            return matter;
                        }
                        var tasks = await _clioApiClient.GetTasksForMatterAsync(matter.id);
                        if (tasks.All(t => t.status.Equals("complete", StringComparison.OrdinalIgnoreCase)))
                        {
                            _logger.Info("All tasks complete, including this matter.");
                            return matter;
                        }

                        _logger.Info("There exists an incomplete task, skipping this matter.");
                        return null;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            foreach (var task in tasks)
            {
                var result = await task;
                if (result != null)
                    yield return result;
            }
        }


        #endregion

        #region reports - all Matters
        public async IAsyncEnumerable<Matter> GetAllMattersAsync(string fields = "", string status = "", string addedHtml = "",
            List<long>? practiceAreasSelected = null)
        {
            // Safely convert the list to a comma-separated string, or "null" if it's null
            string practiceAreasSelectedAsString = practiceAreasSelected is null
            ? "null"
            : string.Join(",", practiceAreasSelected);

            _logger.Info($"""
            Beginning GetAllMattersAsync()
            fields: {fields}
            status: {status}
            addedHtml: {addedHtml}
            practiceAreasSelected: {practiceAreasSelectedAsString}
            """);

            // If no practice areas are selected, fall back to the original method
            if (practiceAreasSelected is null || practiceAreasSelected.Count == 0)
            {
                await foreach (var matter in _clioApiClient.GetAllMattersAsync(fields, status, addedHtml))
                {
                    yield return matter;
                }
                yield break; // Exit early so we don't continue to CombinedMatters()
            }


            List<int> pageTotals = [];
            int areaIndex = 0;


            // Calculate the total number of pages for all practice areas selected
            // This is done to provide a progress bar that shows the total number of pages to be processed

            foreach (long practiceAreaId in practiceAreasSelected)
            {
                areaIndex++;
                PracticeAreaProgressUpdated?.Invoke(areaIndex, practiceAreasSelected.Count);

                int pagesForThisArea = 0;
                string htmlWithPracticeArea = $"{addedHtml}&practice_area_id={practiceAreaId}";

                // Use a dummy loop to just get the page count
                await foreach (var _ in _clioApiClient.GetAllMattersAsync(
             fields,
             status,
             htmlWithPracticeArea,
             feedbackTotalPagesForThisArea: pages => pagesForThisArea = pages,
             onProgress: null)) // Don't report progress yet
                {
                    break; // We only need the first page to get the total
                }

                pageTotals.Add(pagesForThisArea);
            }



            int totalPages = pageTotals.Sum();
            int pagesCompleted = 0;

            foreach (long practiceAreaId in practiceAreasSelected)
            {
                string htmlWithPracticeArea = $"{addedHtml}&practice_area_id={practiceAreaId}";

                await foreach (var matter in _clioApiClient.GetAllMattersAsync(
                    fields,
                    status,
                    htmlWithPracticeArea,
                    feedbackTotalPagesForThisArea: null, // Already known
                    onProgress: (page, _) =>
                    {
                        pagesCompleted++;
                        ProgressUpdated?.Invoke(pagesCompleted, totalPages);
                        _logger.Info($"Progress updated: {pagesCompleted}/{totalPages}");
                    }))
                {
                    yield return matter;
                }
            }

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
                    _logger.Info($"No tasks found for matter {matter.id}.");
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

        #region Calendar Methods
        public async Task<List<ClioCalendar>> GetCalendarsAsync()
        {
            return await _clioApiClient.GetCalendarsAsync();
        }

        internal async IAsyncEnumerable<ClioCalendarEvent> GetCalendarEntriesAsync(List<long> selectedCalendars)
        {
            await foreach (var calendarEvent in _clioApiClient.GetCalendarEntriesAsync(selectedCalendars))
            {
                yield return calendarEvent;
            }
        }
        #endregion

        #region Practice Area Methods
        public async Task<List<PracticeArea>> GetAllPracticeAreasAsync()
        {
            return await _clioApiClient.GetAllPracticeAreasAsync();
        }
        #endregion
    }
}

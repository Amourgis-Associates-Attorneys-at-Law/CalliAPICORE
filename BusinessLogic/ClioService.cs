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
    public class ClioService
    {
        
        private readonly AMO_Logger _logger;
        private readonly ClioApiClient _clioApiClient;
        private readonly AuthService _authService;



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

        public ClioService(ClioApiClient clioApiClient, AuthService authService, AMO_Logger logger)
        {
            _clioApiClient = clioApiClient;
            _authService = authService;
            _logger = logger;
        }

        #region Authentication Methods
        public string GetAuthorizationUrl()
        {
            return _authService.GetAuthorizationUrl();
        }

        public async Task InitializeAfterAuthAsync()
        {
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Cannot initialize ClioService before authentication.");
            }

            //await _clioApiClient.LoadCustomFieldNamesAsync();
        }

        public async Task GetAccessTokenAsync(string authorizationCode)
        {
            await _authService.GetAccessTokenAsync(authorizationCode);
        }

        public string ValidateAuthorizationCode(string userInput)
        {
            return _authService.ValidateAuthorizationCode(userInput);
        }
        #endregion

        #region Delegates
        public event Action<int, int> ProgressUpdated
        {
            add => _clioApiClient.ProgressUpdated += value;
            remove => _clioApiClient.ProgressUpdated -= value;
        }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_authService.AccessToken);
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
            return $"Prefile stages include: {string.Join(", ", prefileStages)}.";
        }


        #region Custom Report Methods
        #endregion

        #region Classic Reports
        #region reports - open Matters


        public async Task GetAllOpenMatters()
        {
            // Initialize the matter stream and filter it
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllOpenMattersAsync();
            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(matters, _clioApiClient);
        }

        public async Task GetAllOpen713Matters()
        {
            // Initialize the matter stream and filter it
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllOpenMattersAsync().FilterByPracticeAreaSuffixAsync(new string[] { "7", "13" });
            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(matters, _clioApiClient);
        }

        public async Task GetUnworked713Matters()
        {
            _logger.Info("ClioService: GetUnworked713Matters called.");
            // Initialize the matter stream and filter it


            var matters = _clioApiClient.GetAllOpenMattersAsync()
                .LogEachAsync(_logger, "All Matters")
                .FilterByPracticeAreaSuffixAsync(new[] { "7", "13" })
                .LogEachAsync(_logger, "After Suffix Filter")
                .FilterByStageNameAsync(prefileStages)
                .LogEachAsync(_logger, "After Stage Filter");


            IAsyncEnumerable<Matter> filteredMatters = FilterMattersWithNoOpenTasksAsync(matters);

            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(filteredMatters, _clioApiClient);
        }





        #endregion

        #region reports - all Matters
        public async Task GetAllMattersAsync(string fields="", string status="", string addedHtml="")
        {
            // Initialize the matter stream
            IAsyncEnumerable<Matter> matters = _clioApiClient.GetAllMattersAsync(fields, status, addedHtml);
            // Filter the matter stream (since this is ALL matters, we don't need to bother with filtering)
            // matters = matters.FilterByWhatever();
            // Convert the matter stream to a DataTable and show it
            await ReportLauncher.ShowAsync(matters, _clioApiClient);
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

            await ReportLauncher.ShowAsync(matters, _clioApiClient);
        }
        #endregion

        #region Calendar Methods
        public async Task<List<ClioCalendar>> GetCalendarsAsync()
        {
            return await _clioApiClient.GetCalendarsAsync(_authService.AccessToken);
        }

        internal async IAsyncEnumerable<ClioCalendarEvent> GetCalendarEntriesAsync(List<long> selectedCalendars)
        {
            await foreach (var calendarEvent in _clioApiClient.GetCalendarEntriesAsync(selectedCalendars, _authService.AccessToken))
            {
                yield return calendarEvent;
            }
        }
        #endregion
    }
}

using CalliAPI.Interfaces;
using CalliAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CalliAPI.Properties;
using System.Text.RegularExpressions;
using CalliAPI.Properties;
using CalliAPI.Models;
using Task = System.Threading.Tasks.Task;
using System.Diagnostics.Metrics;
using AmourgisCOREServices;
using Polly;
using System.Net.Sockets;
using System.Net;
using System.Drawing.Printing;
using System.Threading;
using System.Runtime.CompilerServices;
using ClioTask = CalliAPI.Models.Task;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace CalliAPI.DataAccess
{
    public class ClioApiClient : IClioApiClient
    {
        

        private const string MatterFields = @"id,practice_area{name},description,display_number,status,has_tasks,client{id,name},matter_stage{name},custom_field_values{id,value,custom_field}";

        private readonly HttpClient _httpClient;
        private readonly AMO_Logger _logger;
        private string clientSecret = string.Empty;
        private AsyncPolicy<HttpResponseMessage> _retryPolicy;
        //private List<string> practiceAreas = new List<string>
        //{
        //    "AK", "CA", "CB", "CL", "CN", "DY", "TD", "WA", "YO"
        //};


        public ClioApiClient(HttpClient httpClient, AMO_Logger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            clientSecret = Environment.GetEnvironmentVariable("CLIO_CLIENT_SECRET");


        _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => (int)r.StatusCode == 429)
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            onRetry: (outcome, delay, attempt, context) =>
            { 
                if (outcome.Exception != null)
                {
                    _logger.Warn($"Exception on attempt {attempt}: {outcome.Exception.Message}. Retrying in {delay.TotalSeconds} seconds...");
                }
                else
                {
                    _logger.Warn($"Retrying due to HTTP {outcome.Result.StatusCode}. Delay: {delay.TotalSeconds} seconds...");
                }
            });

        }

        #region delegates

        public event Action<int, int> ProgressUpdated;

        #endregion






        public async Task<HttpResponseMessage> VerifyAPI(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string apiUrl = "https://app.clio.com/api/v4/users/who_am_i";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            return response;
        }

        public async Task<string> GetAccessTokenAsync(string authorizationCode)
        {
            string tokenEndpoint = "https://app.clio.com/oauth/token";
            // Package up the request data and send it to the token endpoint so we can grab the access_token string
            var requestData = new Dictionary<string, string>
            {
            { "grant_type", "authorization_code" },
            { "code", authorizationCode },
            { "redirect_uri", "https://www.amourgis.com/" },
            { "client_id", Properties.Settings.Default.ClientId },
            { "client_secret", clientSecret}
            };


            _logger.Info("Requesting access token with data: " +
                            string.Join(", ", requestData.Select(kvp => $"{kvp.Key}={kvp.Value}")));

            var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestData));

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.Info("Token response: " + responseContent);

            try
            {
                var jsonDocument = JsonDocument.Parse(responseContent);
                string accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString();
                return accessToken;
            }
            catch (KeyNotFoundException)
            {
                _logger.Error("Access token not found in response.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error parsing token response: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// This method attempts to get a property from a JsonElement. It checks if the property exists and if its value is an object.
        /// </summary>
        /// <param name="element">The JsonElement</param>
        /// <param name="propertyName">The property to attempt to recover from the JsonElement</param>
        /// <param name="result">The output of the attempt to access the JsonValueKind.Object from the JsonElement</param>
        /// <returns>True if TryGetProperty() && ValueKind == JsonValueKind.Object, else false</returns>
        private bool TryGetObject(JsonElement element, string propertyName, out JsonElement result)
        {
            result = default;
            if (element.TryGetProperty(propertyName, out var temp) && temp.ValueKind == JsonValueKind.Object)
            {
                result = temp;
                return true;
            }
            return false;
        }


        private static string TryGetString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
            ? prop.GetString()
            : null;
        }



        public Matter ParseMatter(JsonElement element)
        {
            try
            {
                Matter matter = JsonSerializer.Deserialize<Matter>(element.GetRawText());

                return matter;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error parsing matter: {ex.Message}");
                throw;
            }
        }


        #region Tasks

        public async Task<List<ClioTask>> GetTasksForMatterAsync(long matterId)
        {
            string url = $"{Properties.Settings.Default.ApiUrl}tasks?matter_id={matterId}&fields=id,subject,status";
            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(responseContent);

            try
            {
                if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement))
                {
                    return dataElement.EnumerateArray().Select(t => new ClioTask
                    {
                        id = t.GetProperty("id").GetInt64(),
                        status = t.GetProperty("status").GetString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error parsing tasks: {ex.Message}");
                throw;
            }
            return new List<ClioTask>();
        }

        #endregion


        #region reporting functions

        /// <summary>
        /// Get a list of all the matters as Matter objects. Use this with MatterFilters.cs to filter the list of matters.
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<Matter> GetAllMattersAsync(string fields = "", string status = "", string addedHttp = "")
        {
            _logger.Info("API CALL START -- GET ALL MATTERS ASYNC --");

            if (string.IsNullOrEmpty(fields)) 
                fields = MatterFields; // Default to the predefined fields if none are provided
            else
                if (!fields.StartsWith("id"))
                    fields = "id," + fields; // Ensure 'id' is always included in the fields

            if (string.IsNullOrEmpty(status)) status = "open,pending,closed"; // Default to all matters if no status is provided
            if (!addedHttp.StartsWith("&")) addedHttp = "&" + addedHttp; // Ensure the additional parameters start with '&'

            string nextPageUrl = $"{Properties.Settings.Default.ApiUrl}matters?" +
                                $"fields={fields}" + // Use the provided fields or default to MatterFields
                                $"&status={status}" + // Use the provided status or default to all
                                $"{addedHttp}" + // Add any additional parameters provided
                                "&order=id(asc)";

            int pageCount = 0;
            int maxPages = 9999;
            int totalRecords;
            int totalPages = 0;

            while (!string.IsNullOrEmpty(nextPageUrl) && pageCount < maxPages) // While there exists another page of results...
            {
                pageCount++;
                _logger.Info($"Fetching page {pageCount}: {nextPageUrl}");


                // Get the next page (retrying with Polly)
                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(nextPageUrl));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error($"Failed to fetch matters: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    yield break;
                }

                var content = await response.Content.ReadAsStringAsync();

                JsonDocument json;

                try // Parse the JSON response
                {
                    json = JsonDocument.Parse(content);
                }
                catch (JsonException ex)
                {
                    _logger.Error($"Failed to parse JSON: {ex.Message}");
                    yield break;
                }

                // Only extract totalRecords on the first page
                if (pageCount == 1 &&
                    json.RootElement.TryGetProperty("meta", out var metaElement) &&
                    metaElement.TryGetProperty("records", out var recordsElement) &&
                    recordsElement.TryGetInt32(out totalRecords))
                {
                    totalPages = (int)Math.Ceiling(totalRecords / 200.0);
                    _logger.Info($"Total records: {totalRecords}, estimated pages: {totalPages}");
                }

                // Update progress bar here (if you pass a callback or use an event)
                ProgressUpdated?.Invoke(pageCount, totalPages);



                // If there's a data element, parse it

                // Using the using block to ensure it's disposed of later...
                using (json)
                {
                    if (json.RootElement.TryGetProperty("data", out var dataElement))
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            yield return ParseMatter(element);
                        }
                    }

                    nextPageUrl = json.RootElement
                        .GetProperty("meta")
                        .GetProperty("paging")
                        .TryGetProperty("next", out var nextElement)
                        ? nextElement.GetString()
                        : null;

                    
                }
            }

            _logger.Info("API CALL END -- GET ALL MATTERS ASYNC --");
        }

        /// <summary>
        /// Get a list of all the open matters as Matter objects. Use this with MatterFilters.cs to filter the list of matters.
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<Matter> GetAllOpenMattersAsync()
        {
            _logger.Info("API CALL START -- GET ALL OPEN MATTERS ASYNC --");

            string nextPageUrl = $"{Properties.Settings.Default.ApiUrl}matters?" +
                                 $"fields={MatterFields}" +
                                 "&status=open&order=id(asc)";

            int pageCount = 0;
            int maxPages = 9999;
            int totalRecords;
            int totalPages = 0;

            while (!string.IsNullOrEmpty(nextPageUrl) && pageCount < maxPages) // While there exists another page of results...
            {
                pageCount++;
                _logger.Info($"Fetching page {pageCount}: {nextPageUrl}");


                // Get the next page (retrying with Polly)
                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(nextPageUrl));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error($"Failed to fetch matters: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    yield break;
                }

                var content = await response.Content.ReadAsStringAsync();

                JsonDocument json;

                try // Parse the JSON response
                {
                    json = JsonDocument.Parse(content);
                }
                catch (JsonException ex)
                {
                    _logger.Error($"Failed to parse JSON: {ex.Message}");
                    yield break;
                }

                // Only extract totalRecords on the first page
                if (pageCount == 1 &&
                    json.RootElement.TryGetProperty("meta", out var metaElement) &&
                    metaElement.TryGetProperty("records", out var recordsElement) &&
                    recordsElement.TryGetInt32(out totalRecords))
                {
                    totalPages = (int)Math.Ceiling(totalRecords / 200.0);
                    _logger.Info($"Total records: {totalRecords}, estimated pages: {totalPages}");
                }

                // Update progress bar here (if you pass a callback or use an event)
                ProgressUpdated?.Invoke(pageCount, totalPages);



                // If there's a data element, parse it

                // Using the using block to ensure it's disposed of later...
                using (json)
                {
                    if (json.RootElement.TryGetProperty("data", out var dataElement))
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            yield return ParseMatter(element);
                        }
                    }

                    nextPageUrl = json.RootElement
                        .GetProperty("meta")
                        .GetProperty("paging")
                        .TryGetProperty("next", out var nextElement)
                        ? nextElement.GetString()
                        : null;


                }
            }

            _logger.Info("API CALL END -- GET ALL OPEN MATTERS ASYNC --");
        }



        /// <summary>
        /// Fetches up to 10,000 Matter records created since the given date using offset-based pagination,
        /// parallelized for performance. Results are streamed back as an IAsyncEnumerable.
        /// </summary>
        public async IAsyncEnumerable<Matter> FastFetchMattersSinceAsync(
         DateTime sinceDate,
         [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Constants for pagination and throttling
            const int PageSize = 200;                   // Clio API max page size
            const int MaxRecords = 10000;               // Safety cap to avoid over-fetching
            const int MaxParallelRequests = 10;         // Limit concurrent requests to avoid rate limiting

            int totalPages = MaxRecords / PageSize;     // Total pages to fetch (e.g., 50 for 10,000 records)
            int completedPages = 0;                     // Tracks how many pages have completed
            int totalRecordsFetched = 0;                // Tracks total records fetched across all pages

            // Build base URL and query parameters
            string baseUrl = $"{Properties.Settings.Default.ApiUrl}matters";
            string fields = MatterFields;
            string isoDate = sinceDate.ToUniversalTime().ToString("o"); // ISO 8601 format

            var throttler = new SemaphoreSlim(MaxParallelRequests); // Controls how many requests run at once

            // Create a list of tasks, one for each page (offset)
            var tasks = Enumerable.Range(0, totalPages).Select(async i =>
            {
                int offset = i * PageSize;
                string url = $"{baseUrl}?fields={Uri.EscapeDataString(fields)}&created_since={Uri.EscapeDataString(isoDate)}&limit={PageSize}&offset={offset}";
                _logger.Info($"Requesting {url}");
                await throttler.WaitAsync(cancellationToken); // Wait for a slot to run
                try
                {
                    // Use Polly to retry transient failures
                    var response = await _retryPolicy.ExecuteAsync(() =>
         _httpClient.GetAsync(url, cancellationToken));

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Warn($"[ParallelFetch] Failed at offset {offset}: {response.StatusCode}");
                        return Enumerable.Empty<Matter>();
                    }

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var json = JsonDocument.Parse(content);

                    var matters = new List<Matter>();
                    if (json.RootElement.TryGetProperty("data", out var dataElement))
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            matters.Add(ParseMatter(element)); // Convert JSON to Matter object
                        }
                    }

                    // Safely increment the total record count
                    Interlocked.Add(ref totalRecordsFetched, matters.Count);
                    return matters;
                }
                catch (Exception ex)
                {
                    _logger.Error($"[ParallelFetch] Exception at offset {offset}: {ex.Message}");
                    return Enumerable.Empty<Matter>();
                }
                finally
                {
                    // Update progress and release the throttler slot
                    int newPageCount = Interlocked.Increment(ref completedPages);
                    ProgressUpdated?.Invoke(newPageCount, totalPages);
                    throttler.Release();
                }
            }).ToList();

            // As each task completes, yield its results
            foreach (var task in tasks)
            {
                var matters = await task;
                foreach (var matter in matters)
                {
                    yield return matter;
                }
            }

            // Warn if we hit the record cap
            if (totalRecordsFetched >= MaxRecords)
            {
                _logger.Warn($"[ParallelFetch] Record cap of {MaxRecords} reached. Results may be incomplete.");
                MessageBox.Show(
                $"Only the first {MaxRecords} records were fetched. The results may be incomplete.\n\n" +
                $"Try narrowing your date range or applying filters.",
                "Record Limit Reached",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            }
        }

        public async IAsyncEnumerable<Matter> FastFetchOpenMattersSinceAsync(
 DateTime sinceDate,
 [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Constants for pagination and throttling
            const int PageSize = 200;                   // Clio API max page size
            const int MaxRecords = 10000;               // Safety cap to avoid over-fetching
            const int MaxParallelRequests = 10;         // Limit concurrent requests to avoid rate limiting

            int totalPages = MaxRecords / PageSize;     // Total pages to fetch (e.g., 50 for 10,000 records)
            int completedPages = 0;                     // Tracks how many pages have completed
            int totalRecordsFetched = 0;                // Tracks total records fetched across all pages

            // Build base URL and query parameters
            string baseUrl = $"{Properties.Settings.Default.ApiUrl}matters";
            string fields = $"{MatterFields}&status=open";
            string isoDate = sinceDate.ToUniversalTime().ToString("o"); // ISO 8601 format

            var throttler = new SemaphoreSlim(MaxParallelRequests); // Controls how many requests run at once

            // Create a list of tasks, one for each page (offset)
            var tasks = Enumerable.Range(0, totalPages).Select(async i =>
            {
                int offset = i * PageSize;
                string url = $"{baseUrl}?fields={Uri.EscapeDataString(fields)}&created_since={Uri.EscapeDataString(isoDate)}&limit={PageSize}&offset={offset}";

                await throttler.WaitAsync(cancellationToken); // Wait for a slot to run
                try
                {
                    // Use Polly to retry transient failures
                    var response = await _retryPolicy.ExecuteAsync(() =>
         _httpClient.GetAsync(url, cancellationToken));

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Warn($"[ParallelFetch] Failed at offset {offset}: {response.StatusCode}");
                        return Enumerable.Empty<Matter>();
                    }

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var json = JsonDocument.Parse(content);

                    var matters = new List<Matter>();
                    if (json.RootElement.TryGetProperty("data", out var dataElement))
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            matters.Add(ParseMatter(element)); // Convert JSON to Matter object
                        }
                    }

                    // Safely increment the total record count
                    Interlocked.Add(ref totalRecordsFetched, matters.Count);
                    return matters;
                }
                catch (Exception ex)
                {
                    _logger.Error($"[ParallelFetch] Exception at offset {offset}: {ex.Message}");
                    return Enumerable.Empty<Matter>();
                }
                finally
                {
                    // Update progress and release the throttler slot
                    int newPageCount = Interlocked.Increment(ref completedPages);
                    ProgressUpdated?.Invoke(newPageCount, totalPages);
                    throttler.Release();
                }
            }).ToList();

            // As each task completes, yield its results
            foreach (var task in tasks)
            {
                var matters = await task;
                foreach (var matter in matters)
                {
                    yield return matter;
                }
            }

            // Warn if we hit the record cap
            if (totalRecordsFetched >= MaxRecords)
            {
                _logger.Warn($"[ParallelFetch] Record cap of {MaxRecords} reached. Results may be incomplete.");
                MessageBox.Show(
                $"Only the first {MaxRecords} records were fetched. The results may be incomplete.\n\n" +
                $"Try narrowing your date range or applying filters.",
                "Record Limit Reached",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            }
        }





        /// <summary>
        /// Get a list of all the Practice Areas as PracticeArea objects
        /// </summary>
        /// <returns></returns>
        public async Task<List<PracticeArea>> GetAllPracticeAreasAsync()
        {
            var practiceAreas = new List<PracticeArea>();
            string url = Properties.Settings.Default.ApiUrl + "practice_areas";

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);

            if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement))
            {
                practiceAreas = dataElement.EnumerateArray().Select(pa => new PracticeArea
                {
                    id = pa.GetProperty("id").GetInt64(),
                    name = pa.GetProperty("name").GetString()
                }).ToList();
            }

            return practiceAreas;
        }


        public List<long> GetRelevantPracticeAreaIds(List<PracticeArea> practiceAreas)
        {
            return practiceAreas
            .Where(pa => pa.name.EndsWith("7") || pa.name.EndsWith("13"))
            .Select(pa => pa.id)
            .ToList();
        }


        public async Task<List<Matter>> GetActiveMattersByPracticeAreaAsync(long practiceAreaId, string accessToken)
        {
            // This line sets the authorization header for the HTTP client using the access token. This is necessary for authenticating API requests.
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var allMatters = new List<Matter>();

            // This line constructs the URL for the API endpoint to fetch matters. The URL is based on the base API URL and the specific endpoint for matters.
            string nextPageUrl = Properties.Settings.Default.ApiUrl + "matters?" +
                "fields=id,practice_area{name},status,has_tasks,client{id,name},matter_stage{name}" +
                "&practice_area_id=" + practiceAreaId.ToString() +
                "&status=open,pending";

            // This loop fetches all pages of matters from the API. It continues until there are no more pages to fetch (i.e., nextPageUrl is null or empty).
            while (!string.IsNullOrEmpty(nextPageUrl))
            {
                var response = await _httpClient.GetAsync(nextPageUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseContent);

                // Check if the "data" property exists
                if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement))
                {
                    // This line calls ParseMatter() on every matter in the response and adds them to the allMatters list.
                    var matters = dataElement.EnumerateArray().Select(ParseMatter).ToList();
                    allMatters.AddRange(matters);
                }
                else
                {
                    Console.WriteLine("The 'data' property was not found in the JSON response.");
                }


                // Check if the "meta" and "paging" properties exist and get the "next" property
                if (jsonDocument.RootElement.TryGetProperty("meta", out JsonElement metaElement) &&
                metaElement.TryGetProperty("paging", out JsonElement pagingElement) &&
                pagingElement.TryGetProperty("next", out JsonElement nextElement))
                {
                    nextPageUrl = nextElement.GetString();
                }
                else
                {
                    nextPageUrl = null;
                    Console.WriteLine("The 'next' property was not found in the JSON response.");
                }

            }
            return allMatters;
        }

        public async Task<List<Matter>> GetAllActive713MattersAsync(string accessToken)
        {
            var practiceAreas = await GetAllPracticeAreasAsync();
            var relevantPracticeAreaIds = GetRelevantPracticeAreaIds(practiceAreas);

            var allMatters = new List<Matter>();

            foreach (var practiceAreaId in relevantPracticeAreaIds)
            {
                var matters = await GetActiveMattersByPracticeAreaAsync(practiceAreaId, accessToken);
                allMatters.AddRange(matters);
            }

            return allMatters;
        }




        /// <summary>
        /// Fetches all matters that are not currently being worked on.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Matter>> GetMattersNotCurrentlyBeingWorked(string accessToken)
        {
            // Implement the logic to fetch matters not currently being worked on - JH 2025-04-21
            // Defined as matters who:
            // 1. have a practice area ending in 7 or 13
            // 2. have a stage in "Prefile" or "PIF - Prefile" or "Case prep" or "PIF - Case prep" or "Signing and Filing"
            // 3. have no tasks outstanding

            List<string> validStages = new List<string>
            {
                "prefile",
                "pif - prefile",
                "case prep",
                "pif - case prep",
                "signing and filing"
            };

            var matters = await GetAllActive713MattersAsync(accessToken); // 1. get all matters with a practice area ending in 7 or 13

            var filteredMatters = matters
                .Where(m => (m.matter_stage_name != null)) // has a stage
                .Where(m => (validStages.Contains(m.matter_stage_name.ToLower()))); // 2. has a stage in "Prefile" or "PIF - Prefile" or "Case prep" or "PIF - Case prep" or "Signing and Filing"


            // Create a list to store matters with no outstanding tasks
            var mattersWithNoOutstandingTasks = new List<Matter>();

            foreach (var matter in filteredMatters)
            {
                string taskUrl = Properties.Settings.Default.ApiUrl + "tasks?matter_id=" + matter.id + "&fields=id,subject,status";
                var response = await _httpClient.GetAsync(taskUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseContent);

                // Check if the "data" property exists so we can grab the tasks
                if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement))
                {
                    var tasks = dataElement.EnumerateArray().ToList();

                    // Check if all tasks have the status "complete"
                    bool allTasksComplete = tasks.All(t => t.GetProperty("status").ToString().Equals("complete", StringComparison.OrdinalIgnoreCase));

                    // If there are no tasks or all tasks are complete, add the matter to the list
                    if (tasks.Count == 0 || allTasksComplete)
                    {
                        mattersWithNoOutstandingTasks.Add(matter);
                    }
                }
                else
                {
                    Console.WriteLine("The 'data' property was not found in the JSON response.");
                }

            }

            return mattersWithNoOutstandingTasks;



        }
        #endregion

    }
}

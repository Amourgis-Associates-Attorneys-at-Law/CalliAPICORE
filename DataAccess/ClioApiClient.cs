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

namespace CalliAPI.DataAccess
{
    public class ClioApiClient : IClioApiClient
    {
        private static readonly AMO_Logger _logger = new AMO_Logger("CalliAPI");

        private readonly HttpClient _httpClient;
        private string accessToken = string.Empty;
        //private List<string> practiceAreas = new List<string>
        //{
        //    "AK", "CA", "CB", "CL", "CN", "DY", "TD", "WA", "YO"
        //};


        public ClioApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        #region retry policies
        /// <summary>
        /// This policy will retry the request with exponential backoff.
        /// </summary>
        private readonly AsyncPolicy<HttpResponseMessage> _retryPolicy = Policy
            .Handle<HttpRequestException>() // Handle HTTP request exceptions
            .Or<SocketException>() // Handle socket exceptions
    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode == 429) //HTTP 429: Too Many Requests
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
                _logger.Warn($"Rate limited. Retrying in {delay.TotalSeconds} seconds...");
            }
        });


        /// <summary>
        /// This policy will return a fallback response if all retries fail.
        /// Example: var response = await _fallbackPolicy.WrapAsync(_retryPolicy).ExecuteAsync(() => _httpClient.GetAsync(nextPageUrl));
        /// </summary>
        private readonly AsyncPolicy<HttpResponseMessage> _fallbackPolicy = Policy<HttpResponseMessage>
         .Handle<HttpRequestException>()
         .OrResult(r => !r.IsSuccessStatusCode)
         .FallbackAsync(
         fallbackAction: (cancellationToken) =>
         {
             _logger.Error("All retries failed. Returning fallback response.");
             return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
         });
        #endregion

        #region delegates

        public event Action<int, int> ProgressUpdated;

        #endregion






        public async Task<string> VerifyAPI()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string apiUrl = "https://app.clio.com/api/v4/users/who_am_i";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                return "Clio API is working!";
            }
            else
            {
                return $"Clio API error: {response.StatusCode}";
            }
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
            { "client_secret", Properties.Settings.Default.ClientSecret }
            };
            _logger.Info($"Requesting access token as {requestData.ToString()}");
            var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestData));
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            _logger.Info(jsonDocument.ToString());
            accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString();
            return accessToken;
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




        public Matter ParseMatter(JsonElement element)
        {
            try
            {
                var matter = new Matter
                {
                    id = element.GetProperty("id").GetInt64()
                };


                // Practice Area
                if (TryGetObject(element, "practice_area", out var practiceAreaElement) &&
         practiceAreaElement.TryGetProperty("name", out var practiceAreaNameElement))
                {
                    matter.practice_area_name = practiceAreaNameElement.GetString();
                }

                // Display Number
                if (TryGetObject(element, "display_number", out var displayNumberElement) && displayNumberElement.ValueKind == JsonValueKind.String)
                {
                    matter.display_number = displayNumberElement.GetString();
                }

                // Status
                if (element.TryGetProperty("status", out var statusElement) && statusElement.ValueKind == JsonValueKind.String)
                {
                    matter.status = statusElement.GetString();
                }

                // Has Tasks
                if (element.TryGetProperty("has_tasks", out var hasTasksElement) && hasTasksElement.ValueKind == JsonValueKind.True || hasTasksElement.ValueKind == JsonValueKind.False)
                {
                    matter.has_tasks = hasTasksElement.GetBoolean();
                }

                // Client
                if (TryGetObject(element, "client", out var clientElement) &&
         clientElement.TryGetProperty("id", out var clientIdElement) &&
         clientElement.TryGetProperty("name", out var clientNameElement))
                {
                    matter.client = new Client
                    {
                        id = clientIdElement.GetInt64(),
                        name = clientNameElement.GetString()
                    };
                }

                // Matter Stage
                if (TryGetObject(element, "matter_stage", out var matterStageElement) &&
         matterStageElement.TryGetProperty("name", out var matterStageNameElement))
                {
                    matter.matter_stage_name = matterStageNameElement.GetString();
                }

                // Custom Fields
                if (element.TryGetProperty("custom_field_values", out var customFieldsElement) &&
         customFieldsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var fieldValue in customFieldsElement.EnumerateArray())
                    {
                        if (TryGetObject(fieldValue, "custom_field", out var customFieldElement) &&
                        customFieldElement.TryGetProperty("id", out var idElement) &&
                        fieldValue.TryGetProperty("value", out var valueElement) &&
                        valueElement.ValueKind == JsonValueKind.String)
                        {
                            long fieldId = idElement.GetInt64();
                            string value = valueElement.GetString();

                            if (CustomFieldMap.TryGetField(fieldId, out var customField))
                            {
                                matter.CustomFields[customField] = value;
                            }
                        }
                    }
                }

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
        public async IAsyncEnumerable<Matter> GetAllMattersAsync()
        {
            _logger.Info("API CALL START -- GET ALL MATTERS ASYNC --");

            string nextPageUrl = $"{Properties.Settings.Default.ApiUrl}matters?" +
                                 "fields=id,practice_area{name},status,has_tasks,client{id,name},matter_stage{name},custom_field_values{id,value,custom_field}&order=id(asc)";

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
                                 "fields=id,practice_area{name},status,has_tasks,client{id,name},matter_stage{name},custom_field_values{id,value,custom_field}&status=open&order=id(asc)";

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
            string fields = "id,practice_area{name},status,has_tasks,client{id,name},matter_stage{name},custom_field_values{id,value,custom_field}";
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
            string fields = "id,practice_area{name},status,has_tasks,client{id,name},matter_stage{name},custom_field_values{id,value,custom_field}&status=open";
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


        public async Task<List<Matter>> GetActiveMattersByPracticeAreaAsync(long practiceAreaId)
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

        public async Task<List<Matter>> GetAllActive713MattersAsync()
        {
            var practiceAreas = await GetAllPracticeAreasAsync();
            var relevantPracticeAreaIds = GetRelevantPracticeAreaIds(practiceAreas);

            var allMatters = new List<Matter>();

            foreach (var practiceAreaId in relevantPracticeAreaIds)
            {
                var matters = await GetActiveMattersByPracticeAreaAsync(practiceAreaId);
                allMatters.AddRange(matters);
            }

            return allMatters;
        }




        /// <summary>
        /// Fetches all matters that are not currently being worked on.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Matter>> GetMattersNotCurrentlyBeingWorked()
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

            var matters = await GetAllActive713MattersAsync(); // 1. get all matters with a practice area ending in 7 or 13

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

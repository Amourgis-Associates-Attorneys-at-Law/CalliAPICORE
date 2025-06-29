﻿using AmourgisCOREServices;
using CalliAPI.Interfaces;
using CalliAPI.Models;
using CalliAPI.Utilities;
using Polly;
using Polly.Retry;
using Polly.Wrap;
using System;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text.Json;
using ClioTask = CalliAPI.Models.Task;

namespace CalliAPI.DataAccess
{
    public class ClioApiClient : IClioApiClient
    {
        

        private const string MatterFields = @"id,practice_area{name},description,display_number,status,has_tasks,client{id,name},matter_stage{name},custom_field_values{id,value,custom_field}";

        private readonly HttpClient _httpClient;
        private readonly AMO_Logger _logger;
        private readonly string clientSecret = string.Empty;
        private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;
        private readonly Dictionary<long, string> _fieldNameCache = [];
        //private List<string> practiceAreas = new List<string>
        //{
        //    "AK", "CA", "CB", "CL", "CN", "DY", "TD", "WA", "YO"
        //};


        public ClioApiClient(HttpClient httpClient, string secret)
        {
            _httpClient = httpClient;
            _logger = AMO_Logger.Instance;
            clientSecret = secret;

            #region deprecated code
            //            // Custom Retry Delays based on the fact that exponential backoff from (2 ^ attempt) seconds was far too short of a delay that Clio never respected
            //            var retryDelays = new[]
            //{
            //    TimeSpan.FromSeconds(30),
            //    TimeSpan.FromSeconds(60),
            //    TimeSpan.FromSeconds(90),
            //    TimeSpan.FromSeconds(120),
            //    TimeSpan.FromSeconds(150)
            //};

            //            _retryPolicy = Policy
            //        .Handle<HttpRequestException>()
            //        .OrResult<HttpResponseMessage>(r => (int)r.StatusCode == 429)
            //        .WaitAndRetryAsync(
            //            retryDelays,
            //            onRetry: (outcome, delay, attempt, context) =>
            //            {
            //                if (outcome.Exception != null)
            //                {
            //                    _logger.Warn($"Exception on attempt {attempt}: {outcome.Exception.Message}. Retrying in {delay.TotalSeconds} seconds...");
            //                }
            //                else
            //                {
            //                    _logger.Warn($"Retrying due to HTTP {outcome.Result.StatusCode}. Delay: {delay.TotalSeconds} seconds...");
            //                }
            //            });
            #endregion

            _pipeline = CreateClioRetryPipeline(_logger);

        }


        #region delegates

        public event Action<int, int> ProgressUpdated;

        #endregion


        #region Polly Retry Policy
        // As of Polly 8, ResiliencePipeline is the new way to create resilience policies
        private static ResiliencePipeline<HttpResponseMessage> CreateClioRetryPipeline(AMO_Logger logger)
        {
            var retryOptions = new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 5,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(r => (int)r.StatusCode == 429),
                DelayGenerator = args =>
                {
                    var response = args.Outcome.Result;
                    if (response != null && response.Headers.TryGetValues("Retry-After", out var values))
                    {
                        var retryAfter = values.FirstOrDefault();
                        if (int.TryParse(retryAfter, out int seconds))
                        {
                            logger.Warn($"Rate limit hit. Retrying after {seconds} seconds.");
                            return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(seconds));
                        }
                        else if (DateTimeOffset.TryParse(retryAfter, out var retryDate))
                        {
                            var delay = retryDate - DateTimeOffset.UtcNow;
                            logger.Warn($"Rate limit hit. Retrying at {retryDate} (in {delay.TotalSeconds:F0} seconds).");
                            return new ValueTask<TimeSpan?>(delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(10));
                        }
                    }

                    logger.Warn("Rate limit hit. Retry-After header missing or invalid. Using default 30s delay.");
                    return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(30));
                },
                OnRetry = args =>
                {
                    logger.Warn($"Retry {args.AttemptNumber + 1} due to 429 Too Many Requests.");
                    return default;
                }
            };

            return new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(retryOptions)
                .Build();
        }


        #endregion



        public async Task<HttpResponseMessage> VerifyAPI(string? accessToken)
        {
            if (accessToken is null) accessToken = RegistrySecretManager.GetClioAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string apiUrl = "https://app.clio.com/api/v4/users/who_am_i";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            return response;
        }

        public async Task<(string accessToken, string refreshToken, DateTime expiresAt)> GetAccessTokenAsync(string authorizationCode)
        {
            var requestData = new Dictionary<string, string>
    {
        { "grant_type", "authorization_code" },
        { "code", authorizationCode },
        { "redirect_uri", Properties.Settings.Default.RedirectUri },
        { "client_id", Properties.Settings.Default.ClientId },
        { "client_secret", clientSecret }
    };

            var response = await _httpClient.PostAsync("https://app.clio.com/oauth/token", new FormUrlEncodedContent(requestData));
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            string? accessToken = json.RootElement.GetProperty("access_token").GetString();
            string? refreshToken = json.RootElement.GetProperty("refresh_token").GetString();
            var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32(); // seconds
            var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

            // Save to registry
            RegistrySecretManager.SetClioAccessToken(accessToken);
            RegistrySecretManager.SetClioRefreshToken(refreshToken);
            RegistrySecretManager.SetClioTokenExpiry(expiresAt);

            return (accessToken, refreshToken, expiresAt);
        }

        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            var requestData = new Dictionary<string, string>
    {
        { "grant_type", "refresh_token" },
        { "refresh_token", refreshToken },
        { "client_id", Properties.Settings.Default.ClientId },
        { "client_secret", clientSecret }
    };

            var response = await _httpClient.PostAsync("https://app.clio.com/oauth/token", new FormUrlEncodedContent(requestData));
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var accessToken = json.RootElement.GetProperty("access_token").GetString();
            var newRefreshToken = json.RootElement.GetProperty("refresh_token").GetString();
            var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();

            var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

            RegistrySecretManager.SetClioAccessToken(accessToken);
            RegistrySecretManager.SetClioRefreshToken(newRefreshToken);
            RegistrySecretManager.SetClioTokenExpiry(expiresAt);

            return accessToken;
        }


        ///// <summary>
        ///// This method attempts to get a property from a JsonElement. It checks if the property exists and if its value is an object.
        ///// </summary>
        ///// <param name="element">The JsonElement</param>
        ///// <param name="propertyName">The property to attempt to recover from the JsonElement</param>
        ///// <param name="result">The output of the attempt to access the JsonValueKind.Object from the JsonElement</param>
        ///// <returns>True if TryGetProperty() and ValueKind == JsonValueKind.Object, else false</returns>
        //private static bool TryGetObject(JsonElement element, string propertyName, out JsonElement result)
        //{
        //    result = default;
        //    if (element.TryGetProperty(propertyName, out var temp) && temp.ValueKind == JsonValueKind.Object)
        //    {
        //        result = temp;
        //        return true;
        //    }
        //    return false;
        //}


        //private static string TryGetString(JsonElement element, string propertyName)
        //{
        //    return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
        //    ? prop.GetString()
        //    : null;
        //}


        #region Parse Methods
        /// <summary>
        /// Attempts to parse a JsonElement into a Matter object.
        /// </summary>
        /// <param name="element">The JSON containing the Matter object to parse.</param>
        /// <returns>The Matter object that was returned from the JSON.</returns>
        public Matter ParseMatter(JsonElement element)
        {
            try
            {
                // Attempt to deserialize the JSON into a Matter object
                Matter? matter = JsonSerializer.Deserialize<Matter>(element.GetRawText());

                // If deserialization fails and returns null, throw a meaningful exception
                return matter is null ? throw new InvalidOperationException("Deserialization returned null for Matter.") : matter;
            }
            catch (JsonException jsonEx)
            {
                _logger.Error($"JSON error parsing Matter: {jsonEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error parsing Matter: {ex.Message}");
                throw;
            }
        }

        private ClioCalendar ParseClioCalendar(JsonElement element)
        {
            try
            {
                ClioCalendar? calendar = JsonSerializer.Deserialize<ClioCalendar>(element.GetRawText());

                return calendar is null ? throw new InvalidOperationException("Deserialization returned null for ClioCalendar.") : calendar;
            }
            catch (JsonException jsonEx)
            {
                _logger.Error($"JSON error parsing ClioCalendar: {jsonEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error parsing ClioCalendar: {ex.Message}");
                throw;
            }
        }


        private ClioCalendarEvent ParseClioCalendarEvent(JsonElement element)
        {
            try
            {
                ClioCalendarEvent? clioCalendarEvent = JsonSerializer.Deserialize<ClioCalendarEvent>(element.GetRawText());

                if (clioCalendarEvent is null)
                {
                    _logger.Error("Deserialization returned null for ClioCalendarEvent.");
                    throw new InvalidOperationException("Failed to deserialize ClioCalendarEvent.");
                }

                return clioCalendarEvent;

            }
            catch (Exception ex)
            {
                _logger.Error($"Error parsing clio calendar event: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Tasks

        public async Task<List<ClioTask>> GetTasksForMatterAsync(long matterId)
        {
            string url = $"{Properties.Settings.Default.ApiUrl}tasks?matter_id={matterId}&fields=id,subject,status";
            var response = await _pipeline.ExecuteAsync(async token =>
            {
                return await _httpClient.GetAsync(url, token);
            });
            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(responseContent);

            try
            {
                if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement))
                {
                    return [.. dataElement.EnumerateArray().Select(t => new ClioTask
                    {
                        id = t.GetProperty("id").GetInt64(),
                        status = t.GetProperty("status").GetString() ?? ""
                    })];
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error parsing tasks: {ex.Message}");
                throw;
            }
            return [];
        }

        #endregion


        #region reporting functions
        #region matters
        /// <summary>
        /// Get a list of all the matters as Matter objects. Use this with MatterFilters.cs to filter the list of matters.
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<Matter> GetAllMattersAsync(string fields = "", string status = "", string addedHttp = "", Action<int>? feedbackTotalPagesForThisArea = null, Action<int, int>? onProgress = null)
        {
            _logger.Info("API CALL START -- GET ALL MATTERS ASYNC --");

            if (string.IsNullOrEmpty(fields)) 
                fields = MatterFields; // Default to the predefined fields if none are provided
            else
                if (!fields.StartsWith("id"))
                    fields = "id," + fields; // Ensure 'id' is always included in the fields

            if (string.IsNullOrEmpty(status)) status = "open,pending,closed"; // Default to all matters if no status is provided
            if (!addedHttp.StartsWith('&')) addedHttp = "&" + addedHttp; // Ensure the additional parameters start with '&'

            string nextPageUrl = $"{Properties.Settings.Default.ApiUrl}matters?" +
                                $"fields={fields}" + // Use the provided fields or default to MatterFields
                                $"&status={status}" + // Use the provided status or default to all
                                $"{addedHttp}" + // Add any additional parameters provided
                                "&order=id(asc)";

            int pageCount = 0;
            int maxPages = 9999;
            int totalPages = 0;

            while (!string.IsNullOrEmpty(nextPageUrl) && pageCount < maxPages) // While there exists another page of results...
            {
                pageCount++;
                _logger.Info($"Fetching page {pageCount}: {nextPageUrl}");


                // Get the next page (retrying with Polly)
                var response = await _pipeline.ExecuteAsync(async token =>
                {
                    return await _httpClient.GetAsync(nextPageUrl, token);
                });
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
                    recordsElement.TryGetInt32(out int totalRecords))
                {
                    totalPages = Math.Max(1, (int)Math.Ceiling(totalRecords / 200.0));
                    _logger.Info($"Total records: {totalRecords}, estimated pages: {totalPages}");
                    feedbackTotalPagesForThisArea?.Invoke(totalPages);
                }

                // Update progress bar here (if you pass a callback or use an event)
                onProgress?.Invoke(pageCount, totalPages);



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
                    else
                    {
                        _logger.Warn("No 'data' property found in the JSON response.");
                    }

#pragma warning disable CS8600
                        nextPageUrl = json.RootElement
                            .GetProperty("meta")
                            .GetProperty("paging")
                            .TryGetProperty("next", out var nextElement)
                            ? nextElement.GetString()
                            : null;
#pragma warning restore CS8600

                }
            }

            _logger.Info("API CALL END -- GET ALL MATTERS ASYNC --");
        }
        #endregion

        #region calendars
        /// <summary>
        /// Get all visible calendars and store them as ClioCalendar objects.
        /// </summary>
        /// <returns>A List of ClioCalendar objects that the user is allowed to view.</returns>
        internal async Task<List<ClioCalendar>> GetCalendarsAsync()
        {
            var calendars = new List<ClioCalendar>();

            string taskUrl = Properties.Settings.Default.ApiUrl + "calendars?fields=visible,id,name,permission&visible=true";

            var response = await _pipeline.ExecuteAsync(async token =>
            {
                return await _httpClient.GetAsync(taskUrl, token);
            });
            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"Failed to fetch calendars: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return calendars;
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                using var json = JsonDocument.Parse(content);
                if (json.RootElement.TryGetProperty("data", out var dataElement))
                {
                    foreach (var element in dataElement.EnumerateArray())
                    {
                        calendars.Add(ParseClioCalendar(element));
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.Error($"Failed to parse JSON: {ex.Message}");
            }

            return calendars;
        }

        internal async IAsyncEnumerable<ClioCalendarEvent> GetCalendarEntriesAsync(List<long> selectedCalendars)
        {

            foreach (long calendarId in selectedCalendars)
            {
                // Build the URL for fetching calendar entries for the selected calendar
                string taskUrl = Properties.Settings.Default.ApiUrl + "calendar_entries" +
                    "?calendar_id=" + calendarId.ToString() +
                    "&fields=id,summary,description,location,start_at,end_at,all_day,matter{id,description,display_number,status,created_at,updated_at}";

                string nextPageUrl = taskUrl; // Initialize the next page URL

                int pageCount = 0;
                int maxPages = 9999;
                int totalPages = 0;

                while (!string.IsNullOrEmpty(nextPageUrl) && pageCount < maxPages) // While there exists another page of results...
                {
                    pageCount++;
                    _logger.Info($"Fetching page {pageCount}: {nextPageUrl}");


                    // Get the next page (retrying with Polly)
                    var response = await _pipeline.ExecuteAsync(async token =>
                    {
                        return await _httpClient.GetAsync(nextPageUrl, token);
                    });
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
                        recordsElement.TryGetInt32(out int totalRecords))
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
                                yield return ParseClioCalendarEvent(element);
                            }
                        }
#pragma warning disable CS8600
                        nextPageUrl = json.RootElement
                            .GetProperty("meta")
                            .GetProperty("paging")
                            .TryGetProperty("next", out var nextElement)
                            ? nextElement.GetString()
                            : null;
#pragma warning restore CS8600

                    }
                }
            }
        }


        #endregion


        #endregion



        #region archived methods

        /// <summary>
        /// Get a list of all the Practice Areas as PracticeArea objects
        /// </summary>
        /// <returns></returns>
        public async Task<List<PracticeArea>> GetAllPracticeAreasAsync()
        {
            _logger.Info($"Getting all Practice Areas...");
            var practiceAreas = new List<PracticeArea>();
            string url = Properties.Settings.Default.ApiUrl + "practice_areas";

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);

            if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement))
            {
                practiceAreas = [.. dataElement.EnumerateArray().Select(pa => new PracticeArea
                {
                    id = pa.GetProperty("id").GetInt64(),
                    practice_area_name = pa.GetProperty("name").GetString() ?? ""
                })];
            }

            return practiceAreas;
        }


        public static List<long> GetRelevantPracticeAreaIds(List<PracticeArea> practiceAreas)
        {
            return [.. practiceAreas
            .Where(pa => pa.practice_area_name.EndsWith('7') || pa.practice_area_name.EndsWith("13"))
            .Select(pa => pa.id)];
        }


        public async Task<List<Matter>> GetActiveMattersByPracticeAreaAsync(long practiceAreaId, string accessToken)
        {
            // This line sets the authorization header for the HTTP client using the access token. This is necessary for authenticating API requests.
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var allMatters = new List<Matter>();

            // This line constructs the URL for the API endpoint to fetch matters. The URL is based on the base API URL and the specific endpoint for matters.
            string? nextPageUrl = Properties.Settings.Default.ApiUrl + "matters?" +
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

            List<string> validStages =
            [
                "prefile",
                "pif - prefile",
                "case prep",
                "pif - case prep",
                "signing and filing"
            ];

            var matters = await GetAllActive713MattersAsync(accessToken); // 1. get all matters with a practice area ending in 7 or 13

            var filteredMatters = matters
                .Where(m => m.matter_stage?.matter_stage_name != null &&
                            validStages.Contains(m.matter_stage.matter_stage_name.ToLower())); // 2. has a stage in "Prefile" or "PIF - Prefile" or "Case prep" or "PIF - Case prep" or "Signing and Filing"


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

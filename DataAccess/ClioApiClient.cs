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
            var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestData));
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString();
            return accessToken;
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
                if (element.TryGetProperty("practice_area", out var practiceAreaElement) &&
         practiceAreaElement.TryGetProperty("name", out var practiceAreaNameElement))
                {
                    matter.practice_area_name = practiceAreaNameElement.GetString();
                }

                // Status
                if (element.TryGetProperty("status", out var statusElement))
                {
                    matter.status = statusElement.GetString();
                }

                // Has Tasks
                if (element.TryGetProperty("has_tasks", out var hasTasksElement))
                {
                    matter.has_tasks = hasTasksElement.GetBoolean();
                }

                // Client
                if (element.TryGetProperty("client", out var clientElement) &&
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
                if (element.TryGetProperty("matter_stage", out var matterStageElement) &&
         matterStageElement.TryGetProperty("name", out var matterStageNameElement))
                {
                    matter.matter_stage_name = matterStageNameElement.GetString();
                }

                // Custom Fields
                if (element.TryGetProperty("custom_field_values", out var customFieldsElement))
                {
                    foreach (var fieldValue in customFieldsElement.EnumerateArray())
                    {
                        if (fieldValue.TryGetProperty("custom_field", out var customFieldElement) &&
                        customFieldElement.TryGetProperty("id", out var idElement) &&
                        fieldValue.TryGetProperty("value", out var valueElement))
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
                Console.WriteLine($"Error parsing element: {ex.Message}");
                throw;
            }
        }



        #region reporting functions

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

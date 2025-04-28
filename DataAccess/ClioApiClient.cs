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
using CalliAPI_Mailer.Models;

namespace CalliAPI.DataAccess
{
    internal class ClioApiClient : IClioApiClient
    {

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


        private Matter ParseMatter(JsonElement element)
        {
            try
            {
                // Log the element for debugging
                Console.WriteLine($"Element: {element}");


                // Declare variables for properties
                JsonElement practiceAreaElement, practiceAreaNameElement;
                JsonElement statusElement;
                JsonElement hasTasksElement;
                JsonElement clientElement, clientIdElement, clientNameElement;
                JsonElement matterStageElement, matterStageNameElement;


                // Check and log each property
                Console.WriteLine($"id: {element.GetProperty("id")}");
                Console.WriteLine($"practice_area: {element.TryGetProperty("practice_area", out practiceAreaElement)}");
                if (practiceAreaElement.ValueKind != JsonValueKind.Null)
                {
                    Console.WriteLine($"practice_area_name: {practiceAreaElement.TryGetProperty("name", out practiceAreaNameElement)}");
                }
                Console.WriteLine($"status: {element.TryGetProperty("status", out statusElement)}");
                Console.WriteLine($"has_tasks: {element.TryGetProperty("has_tasks", out hasTasksElement)}");
                Console.WriteLine($"client: {element.TryGetProperty("client", out clientElement)}");
                if (clientElement.ValueKind != JsonValueKind.Null)
                {
                    Console.WriteLine($"client_id: {clientElement.TryGetProperty("id", out clientIdElement)}");
                    Console.WriteLine($"client_name: {clientElement.TryGetProperty("name", out clientNameElement)}");
                }
                Console.WriteLine($"matter_stage: {element.TryGetProperty("matter_stage", out matterStageElement)}");
                if (matterStageElement.ValueKind != JsonValueKind.Null)
                {
                    Console.WriteLine($"matter_stage_name: {matterStageElement.TryGetProperty("name", out matterStageNameElement)}");
                }

                return new Matter
                {
                    id = element.GetProperty("id").GetInt64(),
                    practice_area_name = practiceAreaElement.ValueKind != JsonValueKind.Null && practiceAreaElement.TryGetProperty("name", out practiceAreaNameElement) ? practiceAreaNameElement.GetString() : null,
                    status = statusElement.GetString(),
                    has_tasks = hasTasksElement.GetBoolean(),
                    client = clientElement.ValueKind != JsonValueKind.Null && clientElement.TryGetProperty("id", out clientIdElement) && clientElement.TryGetProperty("name", out clientNameElement) ? new Client
                    {
                        id = clientIdElement.GetInt64(),
                        name = clientNameElement.GetString()
                    } : null,
                    matter_stage_name = matterStageElement.ValueKind != JsonValueKind.Null && matterStageElement.TryGetProperty("name", out matterStageNameElement) ? matterStageNameElement.GetString() : null
                };
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalliAPI.Interfaces;
using CalliAPI.Properties;
using CalliAPI;

namespace CalliAPI.BusinessLogic
{
    internal class AuthService : IAuthService
    {

        private readonly IClioApiClient _clioApiClient;
        public string AccessToken { get; private set; }

        public AuthService(IClioApiClient clioApiAccess)
        {
            _clioApiClient = clioApiAccess;
        }

        public string GetAuthorizationUrl()
        {
            // Return the authorization URL
            return $"https://app.clio.com/oauth/authorize?response_type=code&" +
                $"client_id={Properties.Settings.Default.ClientId}&" +
                $"redirect_uri={Properties.Settings.Default.RedirectUri}";
        }

        public string ValidateAuthorizationCode(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return null;

            if (userInput.StartsWith("https://www.amourgis.com/"))
            {
                var uri = new Uri(userInput);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                return query["code"];
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(userInput, @"^[a-zA-Z0-9]{20,}$"))
                return userInput;

            return null;
        }

        public async Task GetAccessTokenAsync(string authorizationCode)
        {
            AccessToken = await _clioApiClient.GetAccessTokenAsync(authorizationCode);
        }

    }
}

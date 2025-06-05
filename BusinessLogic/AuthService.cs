using AmourgisCOREServices;
using CalliAPI;
using CalliAPI.DataAccess;
using CalliAPI.Interfaces;
using CalliAPI.Properties;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.BusinessLogic
{
    /// <summary>
    /// AuthService is responsible for handling authentication with the Clio API.
    /// </summary>
    public class AuthService(ClioApiClient clioApiAccess, AMO_Logger logger) : IAuthService
    {
        private readonly AMO_Logger _logger = logger;

        private readonly ClioApiClient _clioApiClient = clioApiAccess;
        public string? AccessToken { get; private set; }

        //private string _codeVerifier;
        //private string _codeChallenge;


        /// <summary>
        /// Generates PKCE (Proof Key for Code Exchange) values. Useful if we can use PKCE for OAuth 2.0 authorization.
        /// </summary>
        //private void GeneratePkceValues()
        //{
        //    using var rng = RandomNumberGenerator.Create();
        //    var bytes = new byte[32];
        //    rng.GetBytes(bytes);
        //    _codeVerifier = Convert.ToBase64String(bytes)
        //        .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        //    using var sha256 = SHA256.Create();
        //    var challengeBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(_codeVerifier));
        //    _codeChallenge = Convert.ToBase64String(challengeBytes)
        //        .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        //}




        public string GetAuthorizationUrl()
        {
            // Return the authorization URL
            return $"https://app.clio.com/oauth/authorize?response_type=code&" +
                $"client_id={Properties.Settings.Default.ClientId}&" +
                $"redirect_uri={Properties.Settings.Default.RedirectUri}";
        }

        public string? ValidateAuthorizationCode(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return null;

            if (userInput.StartsWith("https://www.amourgis.com/"))
            {
                var uri = new Uri(userInput);
                var query = QueryHelpers.ParseQuery(uri.Query);
                return query.TryGetValue("code", out var code) ? code.ToString() : null;
            }

            // Regex fallback: check if it's a raw code
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

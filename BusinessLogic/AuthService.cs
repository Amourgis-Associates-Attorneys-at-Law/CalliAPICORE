using AmourgisCOREServices;
using CalliAPI.DataAccess;
using CalliAPI.Interfaces;
using CalliAPI.Utilities;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace CalliAPI.BusinessLogic
{
    /// <summary>
    /// AuthService is responsible for handling authentication with the Clio API.
    /// </summary>
    /// <param name="clioApiAccess">A ClioApiClient to communicate with the Clio API.</param>
    public class AuthService(ClioApiClient clioApiAccess) : IAuthService
    {
        private readonly AMO_Logger _logger = AMO_Logger.Instance;
        private readonly ClioApiClient _clioApiClient = clioApiAccess;

        public string? AccessToken { get; private set; }

        public string GetAuthorizationUrl()
        {
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

            if (System.Text.RegularExpressions.Regex.IsMatch(userInput, @"^[a-zA-Z0-9]{20,}$"))
                return userInput;

            return null;
        }

        public async Task GetAccessTokenAsync(string authorizationCode)
        {
            var (accessToken, refreshToken, expiresAt) = await _clioApiClient.GetAccessTokenAsync(authorizationCode);
            AccessToken = accessToken;

            RegistrySecretManager.SetClioAccessToken(accessToken);
            RegistrySecretManager.SetClioRefreshToken(refreshToken);
            RegistrySecretManager.SetClioTokenExpiry(expiresAt);
        }

        public async Task<bool> TryLoadOrRefreshTokenAsync()
        {
            var accessToken = RegistrySecretManager.GetClioAccessToken();
            var refreshToken = RegistrySecretManager.GetClioRefreshToken();
            var expiry = RegistrySecretManager.GetClioTokenExpiry();

            if (!string.IsNullOrWhiteSpace(accessToken) && expiry.HasValue && expiry.Value > DateTime.UtcNow)
            {
                _logger.Info("Using cached access token.");
                AccessToken = accessToken;
                return true;
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.Info("Access token expired. Attempting refresh...");
                try
                {
                    AccessToken = await _clioApiClient.RefreshAccessTokenAsync(refreshToken);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Warn($"Token refresh failed: {ex.Message}");
                }
            }

            return false;
        }
    }
}

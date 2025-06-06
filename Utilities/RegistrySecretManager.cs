using Microsoft.Win32;

namespace CalliAPI.Utilities
{
    /// <summary>
    /// Centralized helper for storing and retrieving secrets from the Windows Registry.
    /// </summary>
    public static class RegistrySecretManager
    {
        private const string RegistryPath = @"Software\Amourgis\CalliAPI";

        public static string? GetSecret(string keyName)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            return key?.GetValue(keyName) as string;
        }

        public static void SetSecret(string keyName, string value)
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key?.SetValue(keyName, value);
        }

        public static string? GetClioClientSecret() => GetSecret("CLIO_CLIENT_SECRET");

        public static void SetClioClientSecret(string secret) => SetSecret("CLIO_CLIENT_SECRET", secret);

        public static string? GetGithubToken() => GetSecret("GITHUB_TOKEN");

        public static void SetGithubToken(string token) => SetSecret("GITHUB_TOKEN", token);

        public static string? GetClioAccessToken() => GetSecret("CLIO_ACCESS_TOKEN");
        public static void SetClioAccessToken(string token) => SetSecret("CLIO_ACCESS_TOKEN", token);

        public static string? GetClioRefreshToken() => GetSecret("CLIO_REFRESH_TOKEN");
        public static void SetClioRefreshToken(string token) => SetSecret("CLIO_REFRESH_TOKEN", token);

        public static DateTime? GetClioTokenExpiry()
        {
            var value = GetSecret("CLIO_TOKEN_EXPIRY");
            return DateTime.TryParse(value, out var dt) ? dt : null;
        }

        public static void SetClioTokenExpiry(DateTime expiry)
        {
            SetSecret("CLIO_TOKEN_EXPIRY", expiry.ToString("o")); // ISO 8601
        }


    }
}

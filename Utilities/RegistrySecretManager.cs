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
    }
}

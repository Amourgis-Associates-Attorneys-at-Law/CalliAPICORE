using AmourgisCOREServices;
using CalliAPI.UI.Forms;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace CalliAPI.Utilities
{


    /// <summary>
    /// Provides access to the application's version information as defined in the .csproj file.
    /// </summary>
    public static class VersionHelper
    {
        public static readonly AMO_Logger _logger = AMO_Logger.Instance;

        /// <summary>
                /// Gets the full version from the executing assembly (e.g., 1.2.3.0).
                /// </summary>
        public static Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version!;
        }

        /// <summary>
        /// Gets the version as a formatted string (e.g., "v1.2.3").
        /// </summary>
        public static string GetFormattedVersion(bool includeBuild = false)
        {
            var version = GetVersion();
            return includeBuild
            ? $"v{version.Major}.{version.Minor}.{version.Build}"
            : $"v{version.Major}.{version.Minor}";
        }

        /// <summary>
        /// Gets the version as a string suitable for display in UI (e.g. "CalliAPI v1.2.3").
        /// </summary>
        public static string GetDisplayVersion()
        {
            return $"CalliAPI {GetFormattedVersion(includeBuild: true)}";
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Checks for updates and applies them if available.
        /// </summary>
        /// <returns></returns>
        public static async Task SplashPromptForUpdateAsync(Form splash, string? githubToken)
        {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#if !DEBUG
            try
            {

                // Check if we have a valid GitHub token
                string? token = RegistrySecretManager.GetGithubToken();
                if (string.IsNullOrWhiteSpace(token) || token == "dummy")
                {
                    token = null; // Treat dummy as no token
                }
                _logger.Info($"Checking for updates with token: {(token ?? "none")}");

                // Initialize the UpdateManager with the GitHub source
                var mgr = new UpdateManager(
                    new GithubSource(
                        "https://github.com/Amourgis-Associates-Attorneys-at-Law/CalliAPICORE",
                        accessToken: token,
                        prerelease: false
                    )
                );

                var updateInfo = await mgr.CheckForUpdatesAsync();
                _logger.Info($"Update check completed. Update available: {updateInfo != null}");

                if (updateInfo != null)
                {
                    var result = MessageBox.Show(
                        splash,
                        $"A new version ({updateInfo.TargetFullRelease}) is available. Would you like to update now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        await mgr.DownloadUpdatesAsync(updateInfo);
                        mgr.ApplyUpdatesAndRestart(updateInfo);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Warn($"Update check failed: {ex.Message}");
                // Optional: show a non-blocking message to the user
                MessageBox.Show($"Update check failed: {ex.Message}", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif

        }



        /// <summary>
                /// Determines whether the application is running from a Velopack-installed location.
                /// </summary>
        public static bool IsInstalled()
            {
                try
                {
                    // Velopack installs typically live under LocalAppData or Program Files
                    string exePath = AppContext.BaseDirectory;
                    string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                    return exePath.StartsWith(localAppData, StringComparison.OrdinalIgnoreCase) ||
                    exePath.StartsWith(programFiles, StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            }

        //public static bool ShouldCheckForUpdates()
        //{
        //    var token = RegistrySecretManager.GetGithubToken();

        //    // Only check for updates if:
        //    // - A GitHub token is present and not a dummy
        //    // - The app is installed (not portable)
        //    return !string.IsNullOrWhiteSpace(token) &&
        //        token != "dummy" &&
        //        IsInstalled();

        //}


    }
}

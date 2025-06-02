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

        /// <summary>
                /// Gets the full version from the executing assembly (e.g., 1.2.3.0).
                /// </summary>
        public static Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
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
        /// Gets the version as a string suitable for display in UI.
        /// </summary>
        public static string GetDisplayVersion()
        {
            return $"CalliAPI {GetFormattedVersion(includeBuild: true)}";
        }

        /// <summary>
        /// Checks for updates and applies them if available.
        /// </summary>
        /// <returns></returns>
        public static async Task PromptForUpdateAsync()
        {
#if !DEBUG
            var mgr = new UpdateManager(new GithubSource("https://github.com/Amourgis-Associates-Attorneys-at-Law/CalliAPICORE", accessToken: null, prerelease: false));
            var updateInfo = await mgr.CheckForUpdatesAsync();

            if (updateInfo != null)
            {
                using (Form topmostForm = new Form
                {
                    Size = new Size(0, 0),
                    StartPosition = FormStartPosition.Manual,
                    Location = new Point(-2000, -2000), // Off-screen
                    ShowInTaskbar = false,
                    TopMost = true
                })
                {

                    topmostForm.Show();
                    await Task.Delay(100); // Ensure the form is shown before focusing
                    topmostForm.Focus();

                    var result = MessageBox.Show(
                        topmostForm,
                        $"A new version ({updateInfo.TargetFullRelease.ToString()}) is available. Would you like to update now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);



                    if (result == DialogResult.Yes)
                    {
                        await mgr.DownloadUpdatesAsync(updateInfo);
                        mgr.ApplyUpdatesAndRestart(updateInfo);
                    }
                }


            }
#endif
        }
    }
}

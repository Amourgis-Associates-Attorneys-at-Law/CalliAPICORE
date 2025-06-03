/*
 * CalliAPI 'Program.cs'
 * 2025 Mar 28
 * Jacob Hayes
 * Creates the services and clients that CalliAPI needs to function and passes them to the MainForm.
 */

using AmourgisCOREServices;
using CalliAPI.BusinessLogic;
using CalliAPI.DataAccess;
using CalliAPI.Interfaces;
using CalliAPI.UI;
using CalliAPI.UI.Forms;
using CalliAPI.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Win32;
using Serilog.Core;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Velopack; // for auto-updates
using Velopack.Windows; // for auto-updates

namespace CalliAPI
{
    public static class Program
    {


        /// <summary>
        /// AllocConsole is a Windows API function that allocates a new console for the calling process.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();



        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            VelopackApp.Build().Run(); // <--- must be first line of code in Main!
            // ^ Checks for Updates with Velopack (formerly Squirrel.Clowd, formerly Squirrel.Windows)

            AMO_Logger _logger = new AMO_Logger("CalliAPI");
            _logger.Info("CalliAPI started");



            // Open a console window for debugging purposes
            AllocConsole();



            // Redirect Console output to the new console window to allow Serilog's Console sink to work
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(standardOutput);
            Console.SetError(standardOutput);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();


            SplashForm splash = null;
            var splashThread = new Thread(() =>
            {
                splash = new SplashForm();
                Application.Run(splash);
            });
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            // Do your async work here
            _logger.Info("Checking for updates...");
            await VersionHelper.SplashPromptForUpdateAsync(splash);
            _logger.Info("Update check complete.");




            await Task.Delay(4000);

            // Close splash screen safely
            splash?.Invoke(() => splash.Close());
            splashThread.Join();



            string clientSecret = LoadClientSecretFromRegistry();
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                using (var secretForm = new MissingClioSecret())
                {
                    if (secretForm.ShowDialog() == DialogResult.OK)
                    {
                        SaveClientSecretToRegistry(secretForm.ClientSecret);
                        clientSecret = secretForm.ClientSecret;
                    }
                    else
                    {
                        MessageBox.Show("You must enter a CLIO_CLIENT_SECRET to continue.", "Missing Secret", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }
            }

            // Create the Services we need through Dependency Injection - 2025-04-21
            HttpClient httpClient = new HttpClient();
            ClioApiClient clioApiClient = new ClioApiClient(httpClient, logger: _logger, secret: clientSecret);
            AuthService authService = new AuthService(clioApiClient, logger: _logger);
            ClioService clioService = new ClioService(clioApiClient, authService, logger: _logger);

            _logger.Info("Services created");
            _logger.Info("Loading custom fields from Clio...");


            // Create the MainForm and pass the services to 
            _logger.Info("About to launch MainForm...");
            Application.Run(new MainForm(clioService: clioService, clioApiClient: clioApiClient, logger: _logger));
        }


        public static void SaveClientSecretToRegistry(string secret)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\CalliAPI");
            key.SetValue("ClioClientSecret", secret);
            key.Close();
        }

        public static string LoadClientSecretFromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CalliAPI");
            return key?.GetValue("ClioClientSecret") as string;
        }

    }
}
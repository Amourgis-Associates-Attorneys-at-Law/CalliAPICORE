/*
 * CalliAPI 'Program.cs'
 * 2025 Mar 28
 * Jacob Hayes
 * Creates the services and clients that CalliAPI needs to function and passes them to the MainForm.
 */

using AmourgisCOREServices;
using CalliAPI.BusinessLogic;
using CalliAPI.DataAccess;
using System.Runtime.InteropServices;

namespace CalliAPI
{
    internal static class Program
    {
        private static readonly AMO_Logger _logger = new AMO_Logger("CalliAPI");


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
        static void Main()
        {
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

            // Create the Services we need through Dependency Injection - 2025-04-21
            HttpClient httpClient = new HttpClient();
            ClioApiClient clioApiClient = new ClioApiClient(httpClient);
            AuthService authService = new AuthService(clioApiClient);
            ClioService clioService = new ClioService(clioApiClient, logger: _logger);
            _logger.Info("Services created");

            // Create the MainForm and pass the services to it
            Application.Run(new MainForm(clioService, authService));
        }
    }
}
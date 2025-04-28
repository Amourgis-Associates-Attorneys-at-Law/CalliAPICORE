/*
 * CalliAPI 'Program.cs'
 * 2025 Mar 28
 * Jacob Hayes
 * Creates the services and clients that CalliAPI needs to function and passes them to the MainForm.
 */

using CalliAPI.BusinessLogic;
using CalliAPI.DataAccess;

namespace CalliAPI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Create the Services we need through Dependency Injection - 2025-04-21
            HttpClient httpClient = new HttpClient();
            ClioApiClient clioApiClient = new ClioApiClient(httpClient);
            AuthService authService = new AuthService(clioApiClient);
            ClioService clioService = new ClioService(clioApiClient);

            Application.Run(new MainForm(clioService, authService));
        }
    }
}
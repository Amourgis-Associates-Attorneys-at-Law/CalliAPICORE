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


            // Show splash on the main thread
            Application.Run(new SplashForm(_logger));
        }



    }
}
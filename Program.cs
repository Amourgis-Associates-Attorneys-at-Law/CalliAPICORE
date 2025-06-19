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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Serilog.Core;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Velopack; // for auto-updates
using Velopack.Windows; // for auto-updates

namespace CalliAPI
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public static partial class Program
    {
            ///<summary>
            ///AllocConsole is a Windows API function that allocates a new console for the calling process.
            /// </summary>
            /// <returns>True if the console was successfully allocated; otherwise, false.</returns>
            [LibraryImport("kernel32.dll", EntryPoint = "AllocConsole")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static partial bool AllocConsole();




        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            VelopackApp.Build().Run(); // <--- must be first line of code in Main!
            // ^ Checks for Updates with Velopack (formerly Squirrel.Clowd, formerly Squirrel.Windows)


            // Register the singleton logger
            AMO_Logger.Initialize("CalliAPI");




#if DEBUG
            // Open a console window for debugging purposes
            AllocConsole();



            // Redirect Console output to the new console window to allow Serilog's Console sink to work
            StreamWriter standardOutput = new(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(standardOutput);
            Console.SetError(standardOutput);
#endif
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Resolve and run the splash form
            Application.Run(new SplashForm());

        }



    }
}
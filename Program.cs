/*
 * CalliAPI Mailer
 * 2025 Mar 28
 * Jacob Hayes
 * More detail in Form1.cs
 */

namespace CalliAPI_Mailer
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
            Application.Run(new Form1());
        }
    }
}
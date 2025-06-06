using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Utilities
{
    public static class WebHelper
    {

    public static void OpenUrl(string url)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // This is key for opening URLs in the default browser
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                // Optional: log or display the error
                Console.WriteLine($"Error opening URL: {ex.Message}");
            }
        }

    }
}

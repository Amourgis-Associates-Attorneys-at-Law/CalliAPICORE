using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CalliAPI.Utilities
{
    public static class EmbeddedFontLoader
    {
        private static readonly PrivateFontCollection fontCollection = new();

        /// <summary>
        /// Loads a font from an embedded resource and returns a Font object.
        /// </summary>
        /// <param name="resourceName">The full resource name, e.g., "CalliAPI.Resources.CormorantGaramond-Regular.ttf"</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font style</param>
        /// <returns>A Font object ready to use</returns>
        public static Font LoadFont(string resourceName, float size, FontStyle style = FontStyle.Regular)
        {
            using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Font resource '{resourceName}' not found.");

            byte[] fontData = new byte[fontStream.Length];
            fontStream.Read(fontData, 0, fontData.Length);

            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            fontCollection.AddMemoryFont(fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);

            return new Font(fontCollection.Families[0], size, style);
        }
    }
}

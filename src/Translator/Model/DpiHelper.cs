using System.Runtime.InteropServices;
using System.Windows;

namespace Translator.Model
{
    /// <summary>
    /// Provides helper methods for retrieving DPI information from a window
    /// </summary>
    public static class DpiHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

        /// <summary>
        /// Retrieves the DPI values for the specified window
        /// </summary>
        /// <param name="hwnd">Handle to the window for which to retrieve DPI information</param>
        /// <returns>Point where X represents horizontal DPI and Y represents vertical DPI</returns>
        public static Point GetDpi(IntPtr hwnd)
        {
            var hdc = GetDC(hwnd);
            try
            {
                var dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                var dpiY = GetDeviceCaps(hdc, LOGPIXELSY);
                return new Point(dpiX, dpiY);
            }
            finally
            {
                ReleaseDC(hwnd, hdc);
            }
        }
    }
}

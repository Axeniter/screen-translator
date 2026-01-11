using System.Runtime.InteropServices;
using System.Windows;

namespace Translator.Model
{
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

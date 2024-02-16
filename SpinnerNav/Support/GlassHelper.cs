using System;
using System.Runtime.InteropServices;

namespace SpinnerNav.Support
{
    public class GlassHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };

        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMarInset);
        [DllImport("DwmApi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        public static void GetCurrentDPI(out double x, out double y)
        {
            try
            {
                System.Windows.Media.Matrix m = System.Windows.PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow).CompositionTarget.TransformToDevice;
                x = 96 / m.M11;
                y = 96 / m.M22;
            }
            catch
            {
                y = 96;
                x = 96;
            }
        }

        public static void UseDarkTitleBar(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero) // zero = invalid handle
            {
                if (DwmSetWindowAttribute(hWnd, 19, new[] { 1 }, sizeof(int)) != 0)
                    DwmSetWindowAttribute(hWnd, 20, new[] { 1 }, sizeof(int));
            }
        }
    }
}

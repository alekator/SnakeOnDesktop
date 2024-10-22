using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SnakeOnDesktop
{
    /// <summary>
    /// Класс для взаимодействия с объектами рабочего стола Windows, используя Windows API (в разработке).
    /// </summary>
    public class Desktop
    {
        public class DesktopObject
        {
            public string Title { get; set; }
            public Rectangle Bounds { get; set; }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public List<DesktopObject> ScanDesktop()
        {
            List<DesktopObject> desktopObjects = new List<DesktopObject>();

            IntPtr hwnd = GetForegroundWindow();

            while (hwnd != IntPtr.Zero)
            {
                RECT rect;
                if (GetWindowRect(hwnd, out rect))
                {
                    desktopObjects.Add(new DesktopObject
                    {
                        Title = hwnd.ToString(),
                        Bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top)
                    });
                }
                hwnd = FindWindowEx(IntPtr.Zero, hwnd, null, null);
            }

            return desktopObjects;
        }
    }
}

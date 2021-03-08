using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Ryujinx.Common.Platform
{
    [SupportedOSPlatform("windows")]
    internal class WindowsPlatform : Platform
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();

        private const int SW_SHOW = 5;
        private const int SW_HIDE = 0;

        private uint GetWindowId()
        {
            IntPtr consoleWindow = GetConsoleWindow();

            uint dwProcessId;
            GetWindowThreadProcessId(consoleWindow, out dwProcessId);

            return dwProcessId;
        }

        override public void ShowConsole()
        {
            if (GetCurrentProcessId() == GetWindowId())
            {
                ShowWindow(GetConsoleWindow(), SW_SHOW);
            }
        }

        override public void HideConsole()
        {
            if (GetCurrentProcessId() == GetWindowId())
            {
                ShowWindow(GetConsoleWindow(), SW_HIDE);
            }
        }
    }
}
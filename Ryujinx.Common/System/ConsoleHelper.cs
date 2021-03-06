using Microsoft.Win32.SafeHandles;
using Ryujinx.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.Common.System
{
    public class ConsoleHelper
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;

        private const int SW_SHOW = 5;
        private const int SW_HIDE = 0;

        private static uint GetWindowId()
        {
            IntPtr consoleWindow = GetConsoleWindow();

            GetWindowThreadProcessId(consoleWindow, out uint dwProcessId);

            return dwProcessId;
        }

        public static void ToggleConsole()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (ConfigurationState.Instance.ShowConsole)
                {
                    if (AllocConsole())
                    {
                        IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);

                        SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);

                        Encoding encoding = Encoding.GetEncoding(MY_CODE_PAGE);
                        StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                        standardOutput.AutoFlush = true;
                        Console.SetOut(standardOutput);

                        safeFileHandle.Dispose();
                        standardOutput.Dispose();

                        Console.Title = $"Ryujinx Console {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
                    }
                    else if (GetCurrentProcessId() == GetWindowId())
                    {
                        ShowWindow(GetConsoleWindow(), SW_SHOW);
                    }
                }
                else if (GetCurrentProcessId() == GetWindowId())
                {
                    {
                        ShowWindow(GetConsoleWindow(), SW_HIDE);
                    }
                }
            }
        }
    }
}
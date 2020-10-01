using System;
using System.Windows;
using TeamNotifier.Views;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Principal;
using System.Reflection;


namespace TeamNotifier
{
    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
    
    public partial class App : Application
    {
        public TaskbarIcon NotifyIcon;
        Logger logger = new Logger();

        public static bool IsRunningAsAdministrator()
        {
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!IsRunningAsAdministrator())
            {
                var processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(processStartInfo);
                Environment.Exit(0);
            }

            if (!logger.Create())
            {
                NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                Environment.Exit(0);
            }

            Log.Message("*********************** Application started *************************");

            FirstChanceLog();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var exc = (eventArgs.ExceptionObject as Exception);
                Log.Message("****** UEXCEPTION *******" + exc.ToString());
            };

            base.OnStartup(e);

            MainWindow window = new MainWindow();
            MainWindow = window;
            window.Show();

            HwndSource.FromHwnd((new WindowInteropHelper(window)).Handle).AddHook(new HwndSourceHook(HandleMessages));
            NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }

        [Conditional("DEBUG")]
        private void FirstChanceLog()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                Log.Message("****** FCEXCEPTION *******" + eventArgs.Exception.ToString());
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Message("*********************** Application closed *************************");
            logger.Close();
            NotifyIcon.Dispose();
            base.OnExit(e);
        }

        private IntPtr HandleMessages(IntPtr handle, Int32 message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            if (message == NativeMethods.WM_SHOWME)
            {
                if (MainWindow.Visibility != Visibility.Visible)
                    MainWindow.Show();

                if (MainWindow.WindowState == WindowState.Minimized)
                    MainWindow.WindowState = WindowState.Normal;

                var topmost = MainWindow.Topmost;

                MainWindow.Topmost = true;
                MainWindow.Topmost = topmost;
            }

            return IntPtr.Zero;
        }
    }
}

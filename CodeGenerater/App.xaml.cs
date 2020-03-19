using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace CodeGenerater
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Window mainWindow;
        public const bool DEBUG = false;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new WelcomeW().Show();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            new Mutex(true, assemblyName, out var createdNew);
            if (!createdNew)
            {
                var current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        Win32Helper.SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
                Shutdown();
            }
            else
            {                
                base.OnStartup(e);
            }
        }


        public static void showMainWindow() {
            if (mainWindow != null) {
                mainWindow.Show();       
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
           e.Handled = true;
           MessageBox.Show(sender.ToString() + e.Exception.Message);
        }
    }
}

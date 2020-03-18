using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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

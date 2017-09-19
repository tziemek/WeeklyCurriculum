using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;

namespace WeeklyCurriculum.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILog logger = LogManager.GetLogger(typeof(App));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Application.Current.DispatcherUnhandledException += this.OnCurrentDispatcherUnhandledException;
            var win = new MainWindow();
            var vm = new MainViewModel();
            win.DataContext = vm;
            win.Show();

        }

        private void OnCurrentDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal(e);
        }
    }
}

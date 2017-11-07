using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using log4net;
using WeeklyCurriculum.Components;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILog logger = LogManager.GetLogger(typeof(App));
        private CompositionHost container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Application.Current.DispatcherUnhandledException += this.OnCurrentDispatcherUnhandledException;

            var dlls = Directory
                .GetFiles(Path.GetDirectoryName(typeof(App).Assembly.Location), "Weekly*.dll", SearchOption.TopDirectoryOnly);
            var refAssemblies = dlls
                .Select(Assembly.LoadFrom)
                .ToList();

            var configuration = new ContainerConfiguration()
                        .WithAssembly(typeof(App).GetTypeInfo().Assembly)
                        .WithAssemblies(refAssemblies)
                        ;
            this.container = configuration.CreateContainer();

            var vm = this.container.GetExport<MainViewModel>();
            var win = new MainWindow();
            win.DataContext = vm;
            win.Show();
        }

        private void OnCurrentDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal(e);
        }
    }
}

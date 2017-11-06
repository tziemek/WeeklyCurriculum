using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Composition.Convention;
using System.Composition.Hosting;
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

            var refAssemblies = new List<Assembly>();
            refAssemblies.Add(typeof(Holiday).GetTypeInfo().Assembly);
            refAssemblies.Add(typeof(WeekProvider).GetTypeInfo().Assembly);

            var configuration = new ContainerConfiguration()
                        .WithAssembly(typeof(App).GetTypeInfo().Assembly)
                        .WithAssemblies(refAssemblies)
                        ;
            this.container = configuration.CreateContainer();

            //var assCat = new AssemblyCatalog(typeof(App).Assembly);
            //var cont = new CompositionContainer(assCat);
            //var vm = new MainViewModel();
            //var vm = cont.GetExportedValue<MainViewModel>();
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

using System.ComponentModel.Composition.Hosting;
using System.Linq;
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
        private CompositionContainer container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.container = new CompositionContainer(new ApplicationCatalog(), CompositionOptions.DisableSilentRejection);
            Application.Current.DispatcherUnhandledException += this.OnCurrentDispatcherUnhandledException;
            var win = new MainWindow();
            var vm = this.container.GetExportedValue<MainViewModel>();
            win.DataContext = vm;
            win.Show();
        }

        private void OnCurrentDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal(e);
        }
    }
}

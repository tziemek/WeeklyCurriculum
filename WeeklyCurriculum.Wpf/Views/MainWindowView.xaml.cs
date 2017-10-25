using System.ComponentModel.Composition;
using System.Windows;

namespace WeeklyCurriculum.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export(typeof(MainWindowView))]
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }
    }
}

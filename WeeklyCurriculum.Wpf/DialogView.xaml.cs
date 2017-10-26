using System.Windows;
using System.Windows.Controls;

namespace WeeklyCurriculum.Wpf
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : UserControl
    {
        public DialogView()
        {
            InitializeComponent();
        }

        public object DialogContent
        {
            get { return (object)GetValue(DialogContentProperty); }
            set { SetValue(DialogContentProperty, value); }
        }

        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register(nameof(DialogContent), typeof(object), typeof(DialogView), new PropertyMetadata(null));
    }
}

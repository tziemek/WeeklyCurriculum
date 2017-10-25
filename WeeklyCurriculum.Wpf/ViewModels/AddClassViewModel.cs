using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace WeeklyCurriculum.Wpf.ViewModels
{
    [Export(typeof(AddClassViewModel))]
    public class AddClassViewModel : Screen
    {
        private string name;

        public string Name
        {
            get => this.name;
            set
            {
                this.name = value; this.NotifyOfPropertyChange();
            }
        }

        public void Accept()
        {
            this.TryClose(true);
        }

        public void Cancel()
        {
            this.TryClose(false);
        }
    }
}

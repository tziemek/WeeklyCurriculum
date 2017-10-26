using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace WeeklyCurriculum.Wpf
{
    public class SingleInputDialogViewModel : ViewModelBase
    {
        private string text;
        private ICommand ok;

        public ICommand Ok
        {
            get
            {
                return this.ok ?? (this.ok = new RelayCommand(this.OnOk));
            }
        }

        private void OnOk(object obj)
        {
        }

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.RaisePropertyChanged();
            }
        }
    }
}

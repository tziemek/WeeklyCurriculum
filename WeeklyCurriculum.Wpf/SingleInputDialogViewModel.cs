using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace WeeklyCurriculum.Wpf
{
    public class SingleInputDialogViewModel : ViewModelBase
    {
        private string text;
        private string errorMessage;
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

        public string ErrorMessage
        {
            get => this.errorMessage;
            set
            {
                this.errorMessage = value;
                this.RaisePropertyChanged();
            }
        }
    }
}

using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NodaTime;

namespace WeeklyCurriculum.Wpf
{
    public class NewYearInputDialogViewModel : ViewModelBase
    {
        private string year;
        private string errorMessage;
        private DateTime? start;
        private DateTime? end;

        public string Year
        {
            get => this.year;
            set
            {
                this.year = value;
                this.RaisePropertyChanged();
            }
        }

        public DateTime? Start
        {
            get => this.start;
            set
            {
                this.start = value;
                this.RaisePropertyChanged();
            }
        }

        public DateTime? End
        {
            get => this.end;
            set
            {
                this.end = value;
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

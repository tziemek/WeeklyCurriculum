using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Wpf
{
    public class SingleInputDialogViewModel : ViewModelBase
    {
        private string text;
        private string errorMessage;

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

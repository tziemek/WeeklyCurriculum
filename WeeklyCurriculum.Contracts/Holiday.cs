using NodaTime;

namespace WeeklyCurriculum.Contracts
{
    public class Holiday : ViewModelBase
    {
        private LocalDate start;
        private LocalDate end;
        private string name;

        public LocalDate Start
        {
            get => this.start;
            set
            {
                this.start = value;
                this.RaisePropertyChanged();
            }
        }

        public LocalDate End
        {
            get => this.end;
            set
            {
                this.end = value;
                this.RaisePropertyChanged();
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.RaisePropertyChanged();
            }
        }
    }
}

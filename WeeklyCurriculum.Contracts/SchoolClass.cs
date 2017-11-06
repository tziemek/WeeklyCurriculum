namespace WeeklyCurriculum.Contracts
{
    public class SchoolClass : ViewModelBase
    {
        private string name;
        private bool isMonday;
        private bool isTuesday;
        private bool isWednesday;
        private bool isThursday;
        private bool isFriday;

        public string Name
        {
            get => this.name;
            set
            {
                if (this.name == value)
                    return;
                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsMonday
        {
            get => this.isMonday;
            set
            {
                if (this.isMonday == value)
                    return;
                this.isMonday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsTuesday
        {
            get => this.isTuesday;
            set
            {
                if (this.isTuesday == value)
                    return;
                this.isTuesday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsWednesday
        {
            get => this.isWednesday;
            set
            {
                if (this.isWednesday == value)
                    return;
                this.isWednesday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsThursday
        {
            get => this.isThursday;
            set
            {
                if (this.isThursday == value)
                    return;
                this.isThursday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsFriday
        {
            get => this.isFriday;
            set
            {
                if (this.isFriday == value)
                    return;
                this.isFriday = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
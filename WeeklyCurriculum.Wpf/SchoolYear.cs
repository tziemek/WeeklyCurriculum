using System.Collections.ObjectModel;
using NodaTime;

namespace WeeklyCurriculum.Wpf
{
    public class SchoolYear : ViewModelBase
    {
        private int year;
        private LocalDate yearStart;
        private LocalDate yearEnd;

        public int Year
        {
            get => this.year;
            set
            {
                this.year = value;
                this.RaisePropertyChanged();
            }
        }
        public LocalDate YearStart
        {
            get => this.yearStart;
            set
            {
                this.yearStart = value;
                this.RaisePropertyChanged();
            }
        }

        public LocalDate YearEnd
        {
            get => this.yearEnd;
            set
            {
                this.yearEnd = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<SchoolClass> Classes { get; set; }

        public ObservableCollection<Holiday> Holidays { get; set; }
    }
}

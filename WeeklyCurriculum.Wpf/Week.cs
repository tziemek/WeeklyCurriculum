using System;
using NodaTime;

namespace WeeklyCurriculum.Wpf
{
    public class Week : ViewModelBase
    {
        private int weekNumber;
        private LocalDate? weekStart;
        private LocalDate? weekEnd;

        public int WeekNumber
        {
            get => this.weekNumber;
            set
            {
                if (this.weekNumber == value)
                    return;
                this.weekNumber = value;
                this.RaisePropertyChanged();
            }
        }

        public LocalDate? WeekStart
        {
            get => this.weekStart; set
            {
                if (this.weekStart == value)
                    return;
                this.weekStart = value;
                this.RaisePropertyChanged();
            }
        }

        public LocalDate? WeekEnd
        {
            get => this.weekEnd; set
            {
                if (this.weekEnd == value)
                    return;
                this.weekEnd = value;
                this.RaisePropertyChanged();
            }
        }
        }
}
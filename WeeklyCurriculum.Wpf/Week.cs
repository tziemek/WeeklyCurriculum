using System;
using Caliburn.Micro;
using NodaTime;

namespace WeeklyCurriculum.Wpf
{
    public class Week : PropertyChangedBase
    {
        private int weekNumber;
        private int weekYear;
        private LocalDate weekStart;
        private LocalDate weekEnd;

        public int WeekNumber
        {
            get => this.weekNumber;
            set
            {
                if (this.weekNumber == value)
                    return;
                this.weekNumber = value;
                this.NotifyOfPropertyChange();
            }
        }

        public int WeekYear
        {
            get => this.weekYear;
            set
            {
                if (this.weekYear == value)
                    return;
                this.weekYear = value;
                this.NotifyOfPropertyChange();
            }
        }

        public string WeekStartVisual => this.WeekStart.ToString();

        public string WeekEndVisual => this.WeekEnd.ToString();

        public LocalDate WeekStart
        {
            get => this.weekStart; set
            {
                if (this.weekStart == value)
                    return;
                this.weekStart = value;
                this.NotifyOfPropertyChange();
            }
        }

        public LocalDate WeekEnd
        {
            get => this.weekEnd; set
            {
                if (this.weekEnd == value)
                    return;
                this.weekEnd = value;
                this.NotifyOfPropertyChange();
            }
        }
        }
}
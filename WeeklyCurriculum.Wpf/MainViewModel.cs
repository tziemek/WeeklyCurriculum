using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NodaTime;
using NodaTime.Calendars;

namespace WeeklyCurriculum.Wpf
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Week> availableWeeks;
        private ObservableCollection<SchoolClass> availableClasses;
        private Week selectedWeek;
        private SchoolClass selectedClass;
        private ICommand addClassCommand;

        public MainViewModel()
        {
            this.AvailableWeeks = new ObservableCollection<Week>(this.GetAvailableWeeks(2017));
            this.SelectedWeek = this.SelectCurrentWeek(this.AvailableWeeks, SystemClock.Instance.GetCurrentInstant());
            this.AvailableClasses = this.GetAvailableClasses();
        }

        private Week SelectCurrentWeek(ObservableCollection<Week> availableWeeks, Instant instant)
        {
            foreach (var week in availableWeeks)
            {
                if (instant.InUtc().LocalDateTime.Date >= week.WeekStart && instant.InUtc().LocalDateTime.Date <= week.WeekEnd)
                {
                    return week;
                }
            }
            throw new InvalidOperationException("No week found for current date");
        }

        private ObservableCollection<SchoolClass> GetAvailableClasses()
        {
            return new ObservableCollection<SchoolClass>();
        }

        private List<Week> GetAvailableWeeks(int year)
        {
            var startOfSchoolYear = new LocalDate(2017, 9, 12);
            var endOfSchoolYear = new LocalDate(2018, 7, 31);
            var rule = WeekYearRules.Iso;
            var result = new List<Week>();
            var currentStart = startOfSchoolYear;
            while (currentStart < endOfSchoolYear)
            {
                var currentEnd = currentStart.Next(IsoDayOfWeek.Friday);
                var calendarWeek = rule.GetWeekOfWeekYear(currentStart);
                result.Add(new Week() { WeekYear = currentStart.Year, WeekNumber = calendarWeek, WeekStart = currentStart, WeekEnd = currentEnd });
                currentStart = currentEnd.Next(IsoDayOfWeek.Monday);
            }
            return result;
        }

        public ObservableCollection<Week> AvailableWeeks { get => this.availableWeeks; private set => this.availableWeeks = value; }

        public ObservableCollection<SchoolClass> AvailableClasses { get => this.availableClasses; private set => this.availableClasses = value; }

        public Week SelectedWeek
        {
            get => this.selectedWeek; set
            {
                if (this.selectedWeek == value)
                {
                    return;
                }

                this.selectedWeek = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand AddClass
        {
            get
            {
                return this.addClassCommand ?? (this.addClassCommand = new RelayCommand(this.OnAddClass));
            }
        }

        public SchoolClass SelectedClass
        {
            get => this.selectedClass;
            set
            {
                if (this.selectedClass == value)
                {
                    return;
                }

                this.selectedClass = value;
                this.RaisePropertyChanged();
            }
        }

        private void OnAddClass(object obj)
        {
            var schoolClass = new SchoolClass();
            this.AvailableClasses.Add(schoolClass);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ControlzEx;
using MaterialDesignThemes.Wpf;
using NodaTime;
using NodaTime.Calendars;

namespace WeeklyCurriculum.Wpf
{
    [Export]
    public class MainViewModel : ViewModelBase
    {
        private readonly WeekProvider weekProvider;
        private readonly ISchoolClassProvider schoolClassProvider;
        private ObservableCollection<Week> availableWeeks;
        private ObservableCollection<SchoolClass> availableClasses;
        private Week selectedWeek;
        private SchoolClass selectedClass;
        private ICommand addClassCommand;
        private ICommand dayCheckedCommand;

        [ImportingConstructor]
        public MainViewModel(WeekProvider weekProvider, ISchoolClassProvider schoolClassProvider)
        {
            this.weekProvider = weekProvider;
            this.schoolClassProvider = schoolClassProvider;
            this.AvailableWeeks = new ObservableCollection<Week>(this.weekProvider.GetAvailableWeeks(2017));
            this.SelectedWeek = this.SelectCurrentWeek(this.AvailableWeeks, SystemClock.Instance.GetCurrentInstant());
            this.AvailableClasses = new ObservableCollection<SchoolClass>(this.schoolClassProvider.GetAvailableClasses());
            this.SelectedClass = this.AvailableClasses.FirstOrDefault();
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

        public ICommand DayChecked
        {
            get
            {
                return this.dayCheckedCommand ?? (this.dayCheckedCommand = new RelayCommand(this.OnDayChecked));
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

        private void OnDayChecked(object obj)
        {
        }

        private async void OnAddClass(object obj)
        {
            var addNewClass = new SingleInputDialogViewModel();
            var result = await DialogHost.Show(addNewClass);
            if (result is true && !string.IsNullOrWhiteSpace(addNewClass.Text))
            {
                var schoolClass = new SchoolClass();
                schoolClass.Name = addNewClass.Text;
                this.AvailableClasses.Add(schoolClass);
                this.SelectedClass = schoolClass;
            }
        }
    }
}

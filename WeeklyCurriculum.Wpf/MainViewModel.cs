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
using WeeklyCurriculum.Wpf.Data;

namespace WeeklyCurriculum.Wpf
{
    [Export]
    public class MainViewModel : ViewModelBase
    {
        private readonly WeekProvider weekProvider;
        private readonly ISchoolClassProvider schoolClassProvider;
        private Week selectedWeek;
        private SchoolClassData selectedClass;
        private SchoolClass editingClass;
        private ICommand addClassCommand;
        private ICommand saveClassCommand;
        private ICommand dayCheckedCommand;

        [ImportingConstructor]
        public MainViewModel(WeekProvider weekProvider, ISchoolClassProvider schoolClassProvider)
        {
            this.weekProvider = weekProvider;
            this.schoolClassProvider = schoolClassProvider;
            this.AvailableWeeks = new ObservableCollection<Week>(this.weekProvider.GetAvailableWeeks(2017));
            this.SelectedWeek = this.SelectCurrentWeek(this.AvailableWeeks, SystemClock.Instance.GetCurrentInstant());
            this.AvailableClasses = new ObservableCollection<SchoolClassData>(this.schoolClassProvider.GetAvailableClasses());
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
            // ToDo this will throw on weekends :(
            throw new InvalidOperationException("No week found for current date");
        }

        public ObservableCollection<Week> AvailableWeeks { get; private set; }

        public ObservableCollection<SchoolClassData> AvailableClasses { get; private set; }

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

        public ICommand SaveClass
        {
            get
            {
                return this.saveClassCommand ?? (this.saveClassCommand = new RelayCommand(this.OnSaveClass));
            }
        }

        public ICommand DayChecked
        {
            get
            {
                return this.dayCheckedCommand ?? (this.dayCheckedCommand = new RelayCommand(this.OnDayChecked));
            }
        }

        public SchoolClassData SelectedClass
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
                this.UpdateEditingClass();
            }
        }

        private void UpdateEditingClass()
        {
            if (this.SelectedClass != null && this.SelectedWeek != null)
            {
                if (this.EditingClass == null)
                {
                    this.EditingClass = new SchoolClass();
                }
                this.EditingClass.Name = this.SelectedClass.Name;
                this.EditingClass.IsMonday = this.SelectedClass.IsMonday;
                this.EditingClass.IsTuesday = this.SelectedClass.IsTuesday;
                this.EditingClass.IsWednesday = this.SelectedClass.IsWednesday;
                this.EditingClass.IsThursday = this.SelectedClass.IsThursday;
                this.EditingClass.IsFriday = this.SelectedClass.IsFriday;
                var matchingWeek = this.SelectedClass.SchoolWeeks?.FirstOrDefault(sw => sw.Year == this.SelectedWeek.WeekYear && sw.Week == this.SelectedWeek.WeekNumber);
                this.EditingClass.Monday = matchingWeek?.Monday;
                this.EditingClass.Tuesday = matchingWeek?.Tuesday;
                this.EditingClass.Wednesday = matchingWeek?.Wednesday;
                this.EditingClass.Thursday = matchingWeek?.Thursday;
                this.EditingClass.Friday = matchingWeek?.Friday;
            }
        }

        public SchoolClass EditingClass
        {
            get => this.editingClass;
            set
            {
                this.editingClass = value;
                this.RaisePropertyChanged();
            }
        }

        private void OnDayChecked(object obj)
        {
        }

        private void OnSaveClass(object obj)
        {
            if (this.EditingClass != null)
            {
                var classData = this.AvailableClasses.FirstOrDefault(c => c.Name == this.EditingClass.Name);
                if (classData != null)
                {
                    classData.IsMonday = this.EditingClass.IsMonday;
                    classData.IsTuesday = this.EditingClass.IsTuesday;
                    classData.IsWednesday = this.EditingClass.IsWednesday;
                    classData.IsThursday = this.EditingClass.IsThursday;
                    classData.IsFriday = this.EditingClass.IsFriday;
                    if (classData.SchoolWeeks == null)
                    {
                        classData.SchoolWeeks = new List<SchoolWeekData>();
                    }
                    var matchingWeek = classData.SchoolWeeks.FirstOrDefault(sw => sw.Year == this.SelectedWeek.WeekYear && sw.Week == this.SelectedWeek.WeekNumber);
                    if (matchingWeek == null)
                    {
                        matchingWeek = new SchoolWeekData();
                        matchingWeek.Year = this.SelectedWeek.WeekYear;
                        matchingWeek.Week = this.SelectedWeek.WeekNumber;
                        classData.SchoolWeeks.Add(matchingWeek);
                    }
                    matchingWeek.Monday = this.EditingClass.Monday;
                    matchingWeek.Tuesday = this.EditingClass.Tuesday;
                    matchingWeek.Wednesday = this.EditingClass.Wednesday;
                    matchingWeek.Thursday = this.EditingClass.Thursday;
                    matchingWeek.Friday = this.EditingClass.Friday;
                }
            }
        }

        private async void OnAddClass(object obj)
        {
            var addNewClass = new SingleInputDialogViewModel();
            void OnCloseAddNewClass(object sender, DialogClosingEventArgs closingArgs)
            {
                if (closingArgs.Parameter is false)
                {
                    return;
                }
                if (this.AvailableClasses.Any(c => c.Name == addNewClass.Text))
                {
                    addNewClass.ErrorMessage = "Name already taken";
                    closingArgs.Cancel();
                }
                else
                {
                    addNewClass.ErrorMessage = null;
                }
            }
            var result = await DialogHost.Show(addNewClass, OnCloseAddNewClass);
            if (result is true && !string.IsNullOrWhiteSpace(addNewClass.Text))
            {
                var schoolClass = new SchoolClassData();
                schoolClass.Name = addNewClass.Text;
                this.AvailableClasses.Add(schoolClass);
                this.SelectedClass = schoolClass;
                // ToDo save class
            }
        }
    }
}

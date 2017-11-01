using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ControlzEx;
using MaterialDesignThemes.Wpf;
using NodaTime;
using NodaTime.Calendars;
using WeeklyCurriculum.Wpf.Data;

namespace WeeklyCurriculum.Wpf
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainViewModel : ViewModelBase
    {
        private readonly WeekProvider weekProvider;
        private readonly ISchoolClassProvider schoolClassProvider;
        private readonly ReportGenerator reportGenerator;

        private SchoolClassData selectedClass;
        private SchoolYearData selectedYear;
        private SchoolClass editingClass;
        private ICommand addClassCommand;
        private ICommand saveClassCommand;
        private ICommand dayCheckedCommand;
        private ICommand printCommand;
        private ICommand addYearCommand;

        [ImportingConstructor]
        public MainViewModel(WeekProvider weekProvider, ISchoolClassProvider schoolClassProvider, ReportGenerator reportGenerator)
        {
            this.weekProvider = weekProvider;
            this.schoolClassProvider = schoolClassProvider;
            this.reportGenerator = reportGenerator;
            this.AvailableYears = new ObservableCollection<SchoolYearData>(this.schoolClassProvider.GetSchoolYears());
            this.SelectedYear = this.AvailableYears.FirstOrDefault();
            if (this.SelectedYear != null && this.SelectedYear.Classes?.Count > 0)
            {
                this.SelectedClass = this.SelectedYear.Classes.FirstOrDefault();
            }
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
        public ObservableCollection<SchoolYearData> AvailableYears { get; private set; }

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

        public ICommand AddYear
        {
            get
            {
                return this.addYearCommand ?? (this.addYearCommand = new RelayCommand(this.OnAddYear));
            }
        }

        public ICommand Print
        {
            get
            {
                return this.printCommand ?? (this.printCommand = new RelayCommand(this.OnPrint));
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
        public SchoolYearData SelectedYear
        {
            get => this.selectedYear;
            private set
            {
                this.selectedYear = value;
                this.RaisePropertyChanged();
                this.UpdateEditingClass();
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

        private void OnPrint(object obj)
        {
            this.reportGenerator.Print(this.EditingClass);
        }

        private void UpdateEditingClass()
        {
            if (this.SelectedClass != null && this.SelectedYear != null)
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
            }
        }

        private async void OnAddYear(object obj)
        {
            var addNewYear = new NewYearInputDialogViewModel();
            void OnCloseAddNewYear(object sender, DialogClosingEventArgs closingArgs)
            {
                if (closingArgs.Parameter is false)
                {
                    return;
                }
                var errors = new StringBuilder();
                if (int.TryParse(addNewYear.Year, out var newYear))
                {
                    if (this.AvailableYears.Any(y => y.Year == newYear))
                    {
                        errors.AppendLine("Schuljahr ist bereits vorhanden.");
                    }
                }
                if (addNewYear.Start == null || addNewYear.End == null)
                {
                    errors.AppendLine("Schuljahresanfang und -ende müssen gesetzt sein.");
                }
                if (errors.Length != 0)
                {
                    addNewYear.ErrorMessage = errors.ToString();
                    closingArgs.Cancel();
                }
                else
                {
                    addNewYear.ErrorMessage = null;
                }
            }
            var result = await DialogHost.Show(addNewYear, OnCloseAddNewYear);
            if (result is true)
            {
                var year = new SchoolYearData();
                year.Year = int.Parse(addNewYear.Year);
                year.YearStart = LocalDate.FromDateTime(addNewYear.Start.GetValueOrDefault());
                year.YearEnd = LocalDate.FromDateTime(addNewYear.End.GetValueOrDefault());
                this.AvailableYears.Add(year);
                this.SelectedYear = year;
            }
        }

        private void OnDayChecked(object obj)
        {
        }

        private void OnSaveClass(object obj)
        {
            if (this.EditingClass != null)
            {
                var classData = this.SelectedClass;
                if (classData != null)
                {
                    classData.IsMonday = this.EditingClass.IsMonday;
                    classData.IsTuesday = this.EditingClass.IsTuesday;
                    classData.IsWednesday = this.EditingClass.IsWednesday;
                    classData.IsThursday = this.EditingClass.IsThursday;
                    classData.IsFriday = this.EditingClass.IsFriday;
                }
            }
            this.schoolClassProvider.SaveSchoolYears(this.AvailableYears);
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
                if (this.SelectedYear.Classes.Any(c => c.Name == addNewClass.Text))
                {
                    addNewClass.ErrorMessage = "Klasse ist bereits vorhanden.";
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
                this.SelectedYear.Classes.Add(schoolClass);
                this.SelectedClass = schoolClass;
            }
        }
    }
}

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

        private SchoolClass selectedClass;
        private SchoolYear selectedYear;
        private ICommand addClassCommand;
        private ICommand saveCommand;
        private ICommand dayCheckedCommand;
        private ICommand printCommand;
        private ICommand addYearCommand;

        [ImportingConstructor]
        public MainViewModel(WeekProvider weekProvider, ISchoolClassProvider schoolClassProvider, ReportGenerator reportGenerator)
        {
            this.weekProvider = weekProvider;
            this.schoolClassProvider = schoolClassProvider;
            this.reportGenerator = reportGenerator;
            var schoolYearData = this.schoolClassProvider.GetSchoolYears();
            if (schoolYearData != null)
            {
                this.AvailableYears = new ObservableCollection<SchoolYear>(schoolYearData.Select(this.CreateSchoolYearFromData));
            }
            else
            {
                this.AvailableYears = new ObservableCollection<SchoolYear>();
            }
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
        public ObservableCollection<SchoolYear> AvailableYears { get; private set; }

        public ICommand AddClass
        {
            get
            {
                return this.addClassCommand ?? (this.addClassCommand = new RelayCommand(this.OnAddClass));
            }
        }

        public ICommand Save
        {
            get
            {
                return this.saveCommand ?? (this.saveCommand = new RelayCommand(this.OnSave));
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
        public SchoolYear SelectedYear
        {
            get => this.selectedYear;
            set
            {
                this.selectedYear = value;
                this.RaisePropertyChanged();
            }
        }

        private void OnPrint(object obj)
        {
            this.reportGenerator.Print(this.SelectedYear, this.SelectedClass);
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
                else
                {
                    errors.AppendLine("Schuljahr muss gesetzt sein.");
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
                var year = new SchoolYear();
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

        private void OnSave(object obj)
        {
            var schoolYearData = this.AvailableYears.Select(CreateSchoolYearData);
            this.schoolClassProvider.SaveSchoolYears(schoolYearData);
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
                if (this.SelectedYear.Classes == null)
                {
                    this.SelectedYear.Classes = new ObservableCollection<SchoolClass>();
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
                var schoolClass = new SchoolClass();
                schoolClass.Name = addNewClass.Text;
                this.SelectedYear.Classes.Add(schoolClass);
                this.SelectedClass = schoolClass;
            }
        }

        private SchoolYear CreateSchoolYearFromData(SchoolYearData schoolYearData)
        {
            var result = new SchoolYear();
            result.Year = schoolYearData.Year;
            result.YearStart = schoolYearData.YearStart;
            result.YearEnd = schoolYearData.YearEnd;
            if (schoolYearData.Classes != null)
            {
                result.Classes = new ObservableCollection<SchoolClass>(schoolYearData.Classes.Select(CreateSchoolClassFromData));
            }
            else
            {
                result.Classes = new ObservableCollection<SchoolClass>();
            }
            if ( schoolYearData.Holidays != null )
            {
                result.Holidays = new ObservableCollection<Holiday>(schoolYearData.Holidays.Select(CreateHolidayFromData));
            }
            else
            {
                result.Holidays = new ObservableCollection<Holiday>();
            }
            return result;
        }

        private SchoolClass CreateSchoolClassFromData(SchoolClassData schoolClassData)
        {
            var result = new SchoolClass();
            result.Name = schoolClassData.Name;
            result.IsMonday = schoolClassData.IsMonday;
            result.IsTuesday = schoolClassData.IsTuesday;
            result.IsWednesday = schoolClassData.IsWednesday;
            result.IsThursday = schoolClassData.IsThursday;
            result.IsFriday = schoolClassData.IsFriday;
            return result;
        }

        private SchoolYearData CreateSchoolYearData(SchoolYear schoolYear)
        {
            var result = new SchoolYearData();
            result.Year = schoolYear.Year;
            result.YearStart = schoolYear.YearStart;
            result.YearEnd = schoolYear.YearEnd;
            if (schoolYear.Classes != null)
            {
                result.Classes = new List<SchoolClassData>(schoolYear.Classes.Select(CreateSchoolClassData));
            }
            if ( schoolYear.Holidays != null )
            {
                result.Holidays = new List<HolidayData>(schoolYear.Holidays.Select(CreateHolidayData));
            }
            return result;
        }

        private Holiday CreateHolidayFromData(HolidayData holidayData)
        {
            var result = new Holiday();
            result.Name = holidayData.Name;
            result.Start = holidayData.Start;
            result.End = holidayData.End;
            return result;
        }

        private HolidayData CreateHolidayData(Holiday holiday)
        {
            var result = new HolidayData();
            result.Name = holiday.Name;
            result.Start = holiday.Start;
            result.End = holiday.End;
            return result;
        }

        private SchoolClassData CreateSchoolClassData(SchoolClass schoolClass)
        {
            var result = new SchoolClassData();
            result.Name = schoolClass.Name;
            result.IsMonday = schoolClass.IsMonday;
            result.IsTuesday = schoolClass.IsTuesday;
            result.IsWednesday = schoolClass.IsWednesday;
            result.IsThursday = schoolClass.IsThursday;
            result.IsFriday = schoolClass.IsFriday;
            return result;
        }
    }
}

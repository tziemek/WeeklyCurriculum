using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using NodaTime;
using WeeklyCurriculum.Components;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Wpf
{
    [System.Composition.Export]
    public class MainViewModel : ViewModelBase, IPartImportsSatisfiedNotification
    {
        private SchoolClass selectedClass;
        private SchoolYear selectedYear;
        private ICommand addClassCommand;
        private ICommand saveCommand;
        private ICommand dayCheckedCommand;
        private ICommand printCommand;
        private ICommand addYearCommand;
        private ICommand importHolidaysCommand;
        private readonly WeekProvider weekProvider;
        private readonly ISchoolClassProvider schoolClassProvider;
        private readonly ReportGenerator reportGenerator;
        private readonly IHolidayProvider holidayProvider;
        private readonly HolidayManagement holidayManagement;

        [System.Composition.ImportingConstructor]
        public MainViewModel(WeekProvider weekProvider, ISchoolClassProvider schoolClassProvider, ReportGenerator reportGenerator, IHolidayProvider holidayProvider, HolidayManagement holidayManagement)
        {
            this.weekProvider = weekProvider;
            this.schoolClassProvider = schoolClassProvider;
            this.reportGenerator = reportGenerator;
            this.holidayProvider = holidayProvider;
            this.holidayManagement = holidayManagement;
        }

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

        public ICommand ImportHolidays
        {
            get
            {
                return this.importHolidaysCommand ?? (this.importHolidaysCommand = new RelayCommand(this.OnImportHolidays));
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

        private void OnImportHolidays(object obj)
        {
            if (this.SelectedYear == null)
            {
                throw new InvalidOperationException("A year must be selected.");
            }
            var ofd = new OpenFileDialog();
            ofd.Filter = "Calendar files (*.ics)|*.ics";
            //ofd.InitialDirectory = "";
            if (ofd.ShowDialog() == true)
            {
                var filename = ofd.FileName;
                var holidayData = this.holidayProvider.GetHolidaysFromFile(filename);

                var holidaysToAdd = new List<HolidayData>(this.SelectedYear.Holidays.Select(HolidayFactory.CreateFromHoliday));
                var relevantHolidayData = this.holidayManagement.FilterRelevantHolidays(holidayData, this.SelectedYear.YearStart, this.SelectedYear.YearEnd);
                var consolidatedHolidayData = this.holidayManagement.ConsolidateHolidays(holidaysToAdd.Concat(relevantHolidayData).ToList());

                this.SelectedYear.Holidays.Clear();

                var holidays = consolidatedHolidayData.OrderBy(h => h.Start).Select(HolidayFactory.CreateFromHolidayData);
                foreach (var item in holidays)
                {
                    this.SelectedYear.Holidays.Add(item);
                }
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
                year.Classes = new ObservableCollection<SchoolClass>();
                year.Holidays = new ObservableCollection<Holiday>();
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
            if (schoolYearData.Holidays != null)
            {
                result.Holidays = new ObservableCollection<Holiday>(schoolYearData.Holidays.Select(HolidayFactory.CreateFromHolidayData));
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
            if (schoolYear.Holidays != null)
            {
                result.Holidays = new List<HolidayData>(schoolYear.Holidays.Select(HolidayFactory.CreateFromHoliday));
            }
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

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
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
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeeklyCurriculum.Components;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.UWP
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISchoolClassProvider schoolClassProvider;
        private SchoolYear selectedYear;
        private SchoolClass selectedClass;

        public MainViewModel(ISchoolClassProvider schoolClassProvider)
        {
            this.schoolClassProvider = schoolClassProvider;

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

            this.AddYear = new RelayCommand(this.OnAddYear);
        }

        private void OnAddYear(object obj)
        {
        }

        public ObservableCollection<SchoolYear> AvailableYears { get; private set; }

        public SchoolYear SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.selectedYear == value)
                {
                    return;
                }
                this.selectedYear = value;
                this.RaisePropertyChanged();
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

        public RelayCommand AddYear { get; }

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
    }
}

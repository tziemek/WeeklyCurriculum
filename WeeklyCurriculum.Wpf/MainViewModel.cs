using System;
using System.Collections.ObjectModel;

namespace WeeklyCurriculum.Wpf
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Week> availableWeeks;
        private ObservableCollection<SchoolClass> availableClasses;
        private Week selectedWeek;

        public MainViewModel()
        {
            this.AvailableWeeks = this.GetAvailableWeeks(2017);
            this.SelectedWeek = this.SelectCurrentWeek(this.AvailableWeeks, DateTime.Now);
            this.AvailableClasses = this.GetAvailableClasses();
        }

        private Week SelectCurrentWeek(ObservableCollection<Week> availableWeeks, DateTime now)
        {
            return null;
        }

        private ObservableCollection<SchoolClass> GetAvailableClasses()
        {
            return null;
        }

        private ObservableCollection<Week> GetAvailableWeeks(int year)
        {
            return null;
        }

        public ObservableCollection<Week> AvailableWeeks { get => this.availableWeeks; private set => this.availableWeeks = value; }

        public ObservableCollection<SchoolClass> AvailableClasses { get => this.availableClasses; private set => this.availableClasses = value; }

        public Week SelectedWeek
        {
            get => this.selectedWeek; set
            {
                if (this.selectedWeek == value)
                    return;
                this.selectedWeek = value;
                this.RaisePropertyChanged();
            }
        }

    }
}

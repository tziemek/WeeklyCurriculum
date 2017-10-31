﻿using System.Linq;

namespace WeeklyCurriculum.Wpf
{
    public class SchoolClass : ViewModelBase
    {
        private string name;
        private bool isMonday;
        private bool isTuesday;
        private bool isWednesday;
        private bool isThursday;
        private bool isFriday;
        private string monday;
        private string tuesday;
        private string wednesday;
        private string thursday;
        private string friday;

        public string Name
        {
            get => this.name;
            set
            {
                if (this.name == value)
                    return;
                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsMonday
        {
            get => this.isMonday;
            set
            {
                if (this.isMonday == value)
                    return;
                this.isMonday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsTuesday
        {
            get => this.isTuesday;
            set
            {
                if (this.isTuesday == value)
                    return;
                this.isTuesday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsWednesday
        {
            get => this.isWednesday;
            set
            {
                if (this.isWednesday == value)
                    return;
                this.isWednesday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsThursday
        {
            get => this.isThursday;
            set
            {
                if (this.isThursday == value)
                    return;
                this.isThursday = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsFriday
        {
            get => this.isFriday;
            set
            {
                if (this.isFriday == value)
                    return;
                this.isFriday = value;
                this.RaisePropertyChanged();
            }
        }

        public string Monday
        {
            get => this.monday;
            set
            {
                this.monday = value;
                this.RaisePropertyChanged();
            }
        }
        public string Tuesday
        {
            get => this.tuesday;
            set
            {
                this.tuesday = value;
                this.RaisePropertyChanged();
            }
        }
        public string Wednesday
        {
            get => this.wednesday;
            set
            {
                this.wednesday = value;
                this.RaisePropertyChanged();
            }
        }
        public string Thursday
        {
            get => this.thursday;
            set
            {
                this.thursday = value;
                this.RaisePropertyChanged();
            }
        }
        public string Friday
        {
            get => this.friday;
            set
            {
                this.friday = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
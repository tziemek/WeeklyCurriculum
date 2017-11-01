﻿using System;
using System.Linq;

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
        private int dayCount;

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
                this.UpdateDayCount();
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
                this.UpdateDayCount();
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
                this.UpdateDayCount();
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
                this.UpdateDayCount();
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
                this.UpdateDayCount();
            }
        }

        private void UpdateDayCount()
        {
            var count = 0;
            if (this.IsMonday) count++;
            if (this.IsTuesday) count++;
            if (this.IsWednesday) count++;
            if (this.IsThursday) count++;
            if (this.IsFriday) count++;
            this.DayCount = count;
        }

        public int DayCount
        {
            get => this.dayCount;
            set
            {
                this.dayCount = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
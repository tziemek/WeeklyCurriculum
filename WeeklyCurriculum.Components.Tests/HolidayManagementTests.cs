using System;
using System.Collections.Generic;
using System.Linq;
using WeeklyCurriculum.Components;
using WeeklyCurriculum.Contracts;
using Xunit;

namespace WeeklyCurriculum
{
    public class HolidayManagementTests
    {
        [Fact]
        public void HolidayNotInYearRange()
        {
            var hm = new HolidayManagement();
            var initialData = new List<HolidayData>();
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 1, 1), End = new NodaTime.LocalDate(2017, 1, 7) });
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 3, 1), End = new NodaTime.LocalDate(2017, 3, 7) });
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 9, 1), End = new NodaTime.LocalDate(2017, 9, 1) });
            var holidays = hm.FilterRelevantHolidays(initialData, new NodaTime.LocalDate(2017, 3, 1), new NodaTime.LocalDate(2017, 8, 31));
            Assert.Equal(1, holidays.Count);
        }

        [Fact]
        public void HolidayWithOverlappingYearRange()
        {
            var hm = new HolidayManagement();
            var initialData = new List<HolidayData>();
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 1, 1), End = new NodaTime.LocalDate(2017, 1, 7) });
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 3, 1), End = new NodaTime.LocalDate(2017, 3, 7) });
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 9, 1), End = new NodaTime.LocalDate(2017, 9, 1) });
            var holidays = hm.FilterRelevantHolidays(initialData, new NodaTime.LocalDate(2017, 1, 4), new NodaTime.LocalDate(2017, 8, 31));
            Assert.Equal(2, holidays.Count);
        }

        [Fact]
        public void SingleDayHolidayOnWeekend()
        {
            var hm = new HolidayManagement();
            var initialData = new List<HolidayData>();
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 3, 1), End = new NodaTime.LocalDate(2017, 3, 7) });
            // this is a single saturday
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 7, 1), End = new NodaTime.LocalDate(2017, 7, 1) });
            var holidays = hm.FilterRelevantHolidays(initialData, new NodaTime.LocalDate(2017, 3, 1), new NodaTime.LocalDate(2017, 8, 31));
            Assert.Equal(1, holidays.Count);
        }

        [Fact]
        public void SingleHolidayInFrontIsMerged()
        {
            var hm = new HolidayManagement();
            var initialData = new List<HolidayData>();
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 8, 1), End = new NodaTime.LocalDate(2017, 8, 7) });
            // this is a single monday
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 11, 6), End = new NodaTime.LocalDate(2017, 11, 6), Name="SingleMonday" });
            // rest of the week
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 11, 7), End = new NodaTime.LocalDate(2017, 11, 10), Name="RestOfWeek" });
            var holidays = hm.FilterRelevantHolidays(initialData, new NodaTime.LocalDate(2017, 3, 1), new NodaTime.LocalDate(2017, 12, 31));
            Assert.Equal(3, holidays.Count);
            var merged = hm.ConsolidateHolidays(holidays);
            Assert.Equal(2, merged.Count);
            Assert.Contains(new HolidayData { Start = new NodaTime.LocalDate(2017, 11, 6), End = new NodaTime.LocalDate(2017, 11, 10), Name="RestOfWeek" }, merged);
        }


        [Fact]
        public void SingleHolidayInBackIsMerged()
        {
            var hm = new HolidayManagement();
            var initialData = new List<HolidayData>();
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 8, 1), End = new NodaTime.LocalDate(2017, 8, 7) });
            // this is the start of a week
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 11, 6), End = new NodaTime.LocalDate(2017, 11, 9), Name="StartOfWeek" });
            // single friday
            initialData.Add(new HolidayData { Start = new NodaTime.LocalDate(2017, 11, 10), End = new NodaTime.LocalDate(2017, 11, 10), Name="SingleFriday" });
            var holidays = hm.FilterRelevantHolidays(initialData, new NodaTime.LocalDate(2017, 3, 1), new NodaTime.LocalDate(2017, 12, 31));
            Assert.Equal(3, holidays.Count);
            var merged = hm.ConsolidateHolidays(holidays);
            Assert.Equal(2, merged.Count);
            Assert.Contains(new HolidayData { Start = new NodaTime.LocalDate(2017, 11, 6), End = new NodaTime.LocalDate(2017, 11, 10), Name = "StartOfWeek" }, merged);
        }
    }
}

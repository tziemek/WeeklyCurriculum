using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using NodaTime;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    [Export]
    public class HolidayManagement
    {
        private readonly LocalDate minDate = new LocalDate(1, 1, 1);

        public List<HolidayData> FilterRelevantHolidays(List<HolidayData> holidayData, LocalDate yearStart, LocalDate yearEnd)
        {
            var result = new List<HolidayData>();
            foreach (var holiday in holidayData)
            {
                if (holiday.End < yearStart)
                {
                    continue;
                }
                if (holiday.Start > yearEnd)
                {
                    continue;
                }
                if (holiday.Start == holiday.End && !this.IsNotWeekend(holiday.Start.DayOfWeek))
                {
                    continue;
                }
                result.Add(holiday);
            }
            return result;
        }

        public List<HolidayData> ConsolidateHolidays(List<HolidayData> holidayData)
        {
            var result = new List<HolidayData>();
            foreach (var holiday in holidayData.OrderBy(h => h.Start))
            {
                // contains
                if ( result.Any(h => h.Start <= holiday.Start && h.End >= holiday.End))
                {
                    continue;
                }

                // neighbours
                var neighbour = result.FirstOrDefault(h => h.End.PlusDays(1) == holiday.Start);
                if (neighbour.Start == minDate && neighbour.End == minDate)
                {
                    result.Add(holiday);
                    continue;
                }

                if (neighbour.Start == neighbour.End)
                {
                    var index = result.IndexOf(neighbour);
                    result.RemoveAt(index);
                    neighbour.End = holiday.End;
                    neighbour.Name = holiday.Name;
                    result.Insert(index, neighbour);
                    continue;
                }

                if (holiday.Start == holiday.End)
                {
                    var index = result.IndexOf(neighbour);
                    result.RemoveAt(index);
                    neighbour.End = holiday.End;
                    result.Insert(index, neighbour);
                    continue;
                }
            }
            return result;
        }

        private bool IsNotWeekend(IsoDayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case IsoDayOfWeek.Monday:
                case IsoDayOfWeek.Tuesday:
                case IsoDayOfWeek.Wednesday:
                case IsoDayOfWeek.Thursday:
                case IsoDayOfWeek.Friday:
                    return true;
                default:
                    return false;
            }
        }
    }
}

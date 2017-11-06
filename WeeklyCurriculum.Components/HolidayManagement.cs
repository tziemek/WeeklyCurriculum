using System;
using System.Collections.Generic;
using System.Composition;
using NodaTime;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    [Export]
    public class HolidayManagement
    {
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
            throw new NotImplementedException();
            //var addToList = true;
            //var neighbour = holidaysToAdd.FirstOrDefault(h => h.End.PlusDays(1) == holiday.Start);
            //if (neighbour != null)
            //{
            //    if (neighbour.End == neighbour.Start)
            //    {
            //        neighbour.End = holiday.End;
            //        neighbour.Name = holiday.Name;
            //        addToList = false;
            //    }
            //    else if (holiday.End == holiday.Start)
            //    {
            //        neighbour.End = holiday.End;
            //        addToList = false;
            //    }
            //}
            //if (addToList)
            //{
            //    holidaysToAdd.Add(this.CreateHolidayFromData(holiday));
            //}
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

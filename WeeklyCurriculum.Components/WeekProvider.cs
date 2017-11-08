using System.Collections.Generic;
using System.Composition;
using NodaTime;
using NodaTime.Calendars;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    [Export]
    public class WeekProvider
    {
        public List<WeekData> GetAllWeeks(LocalDate startOfSchoolYear, LocalDate endOfSchoolYear)
        {
            var rule = WeekYearRules.Iso;
            var result = new List<WeekData>();
            var currentStart = startOfSchoolYear;
            if (currentStart.DayOfWeek != IsoDayOfWeek.Monday)
            {
                currentStart = currentStart.Previous(IsoDayOfWeek.Monday);
            }
            while (currentStart < endOfSchoolYear)
            {
                var currentEnd = currentStart.Next(IsoDayOfWeek.Friday);
                var calendarWeek = rule.GetWeekOfWeekYear(currentStart);
                result.Add(new WeekData { WeekYear = currentStart.Year, WeekNumber = calendarWeek, WeekStart = currentStart, WeekEnd = currentEnd });
                currentStart = currentEnd.Next(IsoDayOfWeek.Monday);
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Calendars;

namespace WeeklyCurriculum.Wpf
{
    [Export]
    public class WeekProvider
    {
        public List<Week> GetAvailableWeeks(int year)
        {
            var startOfSchoolYear = new LocalDate(2017, 9, 12);
            var endOfSchoolYear = new LocalDate(2018, 7, 31);
            var rule = WeekYearRules.Iso;
            var result = new List<Week>();
            var currentStart = startOfSchoolYear;
            while (currentStart < endOfSchoolYear)
            {
                var currentEnd = currentStart.Next(IsoDayOfWeek.Friday);
                var calendarWeek = rule.GetWeekOfWeekYear(currentStart);
                result.Add(new Week() { WeekYear = currentStart.Year, WeekNumber = calendarWeek, WeekStart = currentStart, WeekEnd = currentEnd });
                currentStart = currentEnd.Next(IsoDayOfWeek.Monday);
            }
            return result;
        }
    }
}

using System.Collections.Generic;
using NodaTime;

namespace WeeklyCurriculum.Contracts
{
    public struct SchoolYearData
    {
        public int Year { get; set; }
        public LocalDate YearStart { get; set; }
        public LocalDate YearEnd { get; set; }
        public List<SchoolClassData> Classes { get; set; }
        public List<HolidayData> Holidays { get; set; }
    }
}

using System.Collections.Generic;
using NodaTime;

namespace WeeklyCurriculum.Wpf.Data
{
    public class SchoolYearData
    {
        public int Year { get; set; }
        public LocalDate YearStart { get; set; }
        public LocalDate YearEnd { get; set; }
        public List<SchoolClassData> Classes { get; set; }
    }
}

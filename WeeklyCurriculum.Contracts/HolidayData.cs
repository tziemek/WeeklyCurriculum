using NodaTime;

namespace WeeklyCurriculum.Contracts
{
    public class HolidayData
    {
        public LocalDate Start { get; set; }
        public LocalDate End { get; set; }
        public string Name { get; set; }
    }
}
using NodaTime;

namespace WeeklyCurriculum.Contracts
{
    public struct WeekData
    {
        public int WeekNumber { get; set; }

        public int WeekYear { get; set; }

        public LocalDate WeekStart { get; set; }

        public LocalDate WeekEnd { get; set; }
    }
}
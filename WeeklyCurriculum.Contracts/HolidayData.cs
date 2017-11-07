using System.Diagnostics;
using NodaTime;

namespace WeeklyCurriculum.Contracts
{
    [DebuggerDisplay("{Name}: {Start} - {End}")]
    public struct HolidayData
    {
        public LocalDate Start { get; set; }
        public LocalDate End { get; set; }
        public string Name { get; set; }
    }
}
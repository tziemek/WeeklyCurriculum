using System;
using System.Collections.Generic;
using System.Text;

namespace WeeklyCurriculum.Contracts
{
    public static class HolidayFactory
    {
        public static Holiday CreateFromHolidayData(HolidayData holidayData)
        {
            var result = new Holiday();
            result.Name = holidayData.Name;
            result.Start = holidayData.Start;
            result.End = holidayData.End;
            return result;
        }

        public static HolidayData CreateFromHoliday(Holiday holiday)
        {
            return new HolidayData { Name=holiday.Name, Start = holiday.Start, End=holiday.End};
        }
    }
}

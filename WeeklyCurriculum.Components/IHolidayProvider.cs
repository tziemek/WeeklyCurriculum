using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Ical.Net;
using NodaTime;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    public interface IHolidayProvider
    {
        List<HolidayData> GetHolidaysFromFile(string filename);
    }

    [Export(typeof(IHolidayProvider))]
    public class IcalHolidayProvider : IHolidayProvider
    {
        public List<HolidayData> GetHolidaysFromFile(string filename)
        {
            var result = new List<HolidayData>();
            if (File.Exists(filename))
            {
                var calCollection = Calendar.Load(File.ReadAllText(filename));
                var cal = calCollection.FirstOrDefault();
                foreach (var item in cal.Events)
                {
                    var data = new HolidayData();
                    data.Start = new LocalDate(item.Start.Year, item.Start.Month, item.Start.Day);
                    data.End = new LocalDate(item.End.Year, item.End.Month, item.End.Day).Minus(Period.FromDays(1));
                    data.Name = item.Summary;
                    result.Add(data);
                }
            }
            return result;
        }
    }
}

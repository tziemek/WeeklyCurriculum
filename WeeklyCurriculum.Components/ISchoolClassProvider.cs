using System.Collections.Generic;
using System.Composition;
using System.IO;
using Newtonsoft.Json;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    public interface ISchoolClassProvider
    {
        List<SchoolYearData> GetSchoolYears();
        bool SaveSchoolYears(IEnumerable<SchoolYearData> schoolYears);
    }

    [Export(typeof(ISchoolClassProvider))]
    public class FileSchoolClassProvider : ISchoolClassProvider
    {
        public List<SchoolYearData> GetSchoolYears()
        {
            var result = new List<SchoolYearData>();
            if (File.Exists("db.json"))
            {
                var ser = new JsonSerializer();
                using (var sr = new StreamReader(@"db.json"))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        result = ser.Deserialize<List<SchoolYearData>>(reader);
                    }
                }
            }
            return result;
        }

        public bool SaveSchoolYears(IEnumerable<SchoolYearData> schoolYears)
        {
            var result = false;
            var ser = new JsonSerializer();
            ser.NullValueHandling = NullValueHandling.Ignore;
            using (var sw = new StreamWriter(@"db.json"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    ser.Serialize(writer, schoolYears);
                    result = true;
                }
            }
            return result;
        }
    }
}

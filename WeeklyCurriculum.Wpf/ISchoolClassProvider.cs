using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Newtonsoft.Json;
using WeeklyCurriculum.Wpf.Data;

namespace WeeklyCurriculum.Wpf
{
    public interface ISchoolClassProvider
    {
        List<SchoolClassData> GetAvailableClasses();
        bool SaveClasses(IEnumerable<SchoolClassData> classes);
    }

    [Export(typeof(ISchoolClassProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FileSchoolClassProvider : ISchoolClassProvider
    {
        public List<SchoolClassData> GetAvailableClasses()
        {
            var result = new List<SchoolClassData>();
            if (File.Exists("db.json"))
            {
                var ser = new JsonSerializer();
                using (var sr = new StreamReader(@"db.json"))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        result = ser.Deserialize<List<SchoolClassData>>(reader);
                    }
                }
            }
            return result;
        }

        public bool SaveClasses(IEnumerable<SchoolClassData> classes)
        {
            var result = false;
            var ser = new JsonSerializer();
            ser.NullValueHandling = NullValueHandling.Ignore;
            using (var sw = new StreamWriter(@"db.json"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    ser.Serialize(writer, classes);
                    result = true;
                }
            }
            return result;
        }
    }
}

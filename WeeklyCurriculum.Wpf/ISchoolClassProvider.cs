using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeeklyCurriculum.Wpf.Data;

namespace WeeklyCurriculum.Wpf
{
    public interface ISchoolClassProvider
    {
        List<SchoolClassData> GetAvailableClasses();
    }

    [Export(typeof(ISchoolClassProvider))]
    public class DemoSchoolClassProvider : ISchoolClassProvider
    {
        public List<SchoolClassData> GetAvailableClasses()
        {
            var result = new List<SchoolClassData>();
            var demoClass1 = new SchoolClassData();
            demoClass1.Name = "5a";
            result.Add(demoClass1);
            var demoClass2 = new SchoolClassData();
            demoClass2.Name = "6b";
            result.Add(demoClass2);
            return result;
        }
    }

    //[Export(typeof(ISchoolClassProvider))]
    public class FileSchoolClassProvider : ISchoolClassProvider
    {
        public List<SchoolClassData> GetAvailableClasses()
        {
            throw new NotImplementedException();
        }
    }
}

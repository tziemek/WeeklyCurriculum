using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyCurriculum.Wpf
{
    public interface ISchoolClassProvider
    {
        List<SchoolClass> GetAvailableClasses();
    }

    [Export(typeof(ISchoolClassProvider))]
    public class DemoSchoolClassProvider : ISchoolClassProvider
    {
        public List<SchoolClass> GetAvailableClasses()
        {
            var result = new List<SchoolClass>();
            var demoClass1 = new SchoolClass();
            demoClass1.Name = "5a";
            result.Add(demoClass1);
            var demoClass2 = new SchoolClass();
            demoClass2.Name = "6b";
            result.Add(demoClass2);
            return result;
        }
    }

    //[Export(typeof(ISchoolClassProvider))]
    public class FileSchoolClassProvider : ISchoolClassProvider
    {
        public List<SchoolClass> GetAvailableClasses()
        {
            throw new NotImplementedException();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyCurriculum.Wpf.Data
{
    public class SchoolClassData
    {
        public string Name { get; set; }
        public bool IsMonday { get; set; }
        public bool IsTuesday { get; set; }
        public bool IsWednesday { get; set; }
        public bool IsThursday { get; set; }
        public bool IsFriday { get; set; }
        public List<SchoolWeekData> SchoolWeeks {get;set;}
    }
}

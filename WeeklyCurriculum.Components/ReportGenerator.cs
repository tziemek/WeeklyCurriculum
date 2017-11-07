using System.Collections.Generic;
using System.Composition;
using System.Linq;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using NodaTime;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    [Export]
    public class ReportGenerator
    {
        private readonly WeekProvider weekProvider;

        [System.Composition.ImportingConstructor]
        public ReportGenerator(WeekProvider weekProvider)
        {
            this.weekProvider = weekProvider;
        }

        public void Print(SchoolYear schoolYear, SchoolClass schoolClass)
        {
            var dest = @"test.pdf";
            using (var writer = new PdfWriter(dest))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf, PageSize.A4.Rotate()))
            {
                //document.SetMargins(20, 20, 20, 20);

                var columns = new List<UnitValue>();
                columns.Add(new UnitValue(UnitValue.PERCENT, 20));
                var dayCount = this.GetDayCount(schoolClass);
                var columnWidthInPercent = 80.0 / dayCount;
                var columnDefinition = new UnitValue(UnitValue.PERCENT, (float)columnWidthInPercent);
                for (var i = 0; i < dayCount; i++)
                {
                    columns.Add(columnDefinition);
                }

                var allWeeks = this.weekProvider.GetAllWeeks(schoolYear.YearStart, schoolYear.YearEnd);
                var schoolWeeks = this.GetSchoolWeeksWithoutHolidays(allWeeks, schoolYear.Holidays.Select(HolidayFactory.CreateFromHoliday));


                var header = new Paragraph($"Klasse: {schoolClass.Name}");
                document.Add(header);

                //document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                var table = this.CreateBaseTable(columns, schoolClass);


                document.Add(table);
                document.Close();
            }
        }

        private Table CreateBaseTable(List<UnitValue> columns, SchoolClass schoolClass)
        {
            var table = new Table(columns.ToArray());
            table.SetWidthPercent(100);
            table.AddHeaderCell("SW");
            if (schoolClass.IsMonday)
            {
                table.AddHeaderCell("Montag");
            }
            if (schoolClass.IsTuesday)
            {
                table.AddHeaderCell("Dienstag");
            }
            if (schoolClass.IsWednesday)
            {
                table.AddHeaderCell("Mittwoch");
            }
            if (schoolClass.IsThursday)
            {
                table.AddHeaderCell("Donnerstag");
            }
            if (schoolClass.IsFriday)
            {
                table.AddHeaderCell("Freitag");
            }
            return table;
        }

        private int GetDayCount(SchoolClass schoolClass)
        {
            var result = 0;
            if (schoolClass.IsMonday) result++;
            if (schoolClass.IsTuesday) result++;
            if (schoolClass.IsWednesday) result++;
            if (schoolClass.IsThursday) result++;
            if (schoolClass.IsFriday) result++;
            return result;
        }
    }
}

using System;
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
                var allHolidays = this.GetAllHolidayDays(schoolYear.Holidays).ToList();

                var columns = new List<UnitValue>();
                columns.Add(new UnitValue(UnitValue.PERCENT, 20));
                var relevantDaysOfWeek = this.GetDays(schoolClass).ToList();
                var dayCount = relevantDaysOfWeek.Count();
                var columnWidthInPercent = 80.0 / dayCount;
                var columnDefinition = new UnitValue(UnitValue.PERCENT, (float)columnWidthInPercent);
                for (var i = 0; i < dayCount; i++)
                {
                    columns.Add(columnDefinition);
                }

                var allWeeks = this.weekProvider.GetAllWeeks(schoolYear.YearStart, schoolYear.YearEnd);

                var weeksWithDays = allWeeks.Select(this.GetDaysInWeek);
                var weeksWithHolidayInformation = weeksWithDays.Select(o => this.PopulateWithHolidays(o, relevantDaysOfWeek, allHolidays));

                var schoolWeekSections = new List<IEnumerable<(WeekData Week, List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)> Days)>>();
                var list = new List<(WeekData Week, List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)> Days)>();
                foreach (var item in weeksWithHolidayInformation)
                {
                    // skip full holiday weeks
                    if (item.Days.All(d => d.IsHoliday))
                    {
                        if (list.Count != 0)
                        {
                            schoolWeekSections.Add(list);
                            list = new List<(WeekData Week, List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)> Days)>();
                        }
                        continue;
                    }
                    list.Add(item);
                }
                schoolWeekSections.Add(list);

                for (var i = 0; i < schoolWeekSections.Count; i++)
                {
                    var schoolWeekSection = schoolWeekSections[i];
                    this.AddHeader(schoolClass.Name, document);
                    var table = this.CreateBaseTable(columns, schoolClass);
                    foreach (var item in schoolWeekSection)
                    {
                        table.AddCell($"{item.Week.WeekStart}-{item.Week.WeekEnd}");
                        foreach (var day in item.Days)
                        {
                            var cell = new Cell();
                            cell.SetMinHeight(50);
                            if (day.IsHoliday)
                            {
                                cell.SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY);
                            }
                            table.AddCell(cell);
                        }
                    }
                    document.Add(table);
                    if (i < schoolWeekSections.Count - 1)
                    {
                        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    }
                }


                document.Close();
            }
        }

        private (WeekData Week, List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)> Days) PopulateWithHolidays((WeekData Week, IEnumerable<LocalDate> DaysInWeek) weekWithDays, List<IsoDayOfWeek> relevantDaysOfWeek, List<LocalDate> allHolidays)
        {
            var days = new List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)>();
            if (weekWithDays.Week.WeekStart.DayOfWeek == IsoDayOfWeek.Monday)
            {
                // week starts on monday (normal week)
                foreach (var day in weekWithDays.DaysInWeek)
                {
                    if (relevantDaysOfWeek.Contains(day.DayOfWeek))
                    {
                        if (allHolidays.Contains(day))
                        {
                            days.Add((day.DayOfWeek, true));
                        }
                        else
                        {
                            days.Add((day.DayOfWeek, false));
                        }
                    }
                }
                // if all days in week are holidays skip week and make new page
            }
            else
            {
                // week starts not on a monday i.e. first schoolweek
                days.Add((IsoDayOfWeek.Monday, false));
                days.Add((IsoDayOfWeek.Tuesday, false));
                days.Add((IsoDayOfWeek.Wednesday, false));
            }
            return (weekWithDays.Week, days);
        }

        private IEnumerable<LocalDate> GetAllHolidayDays(IEnumerable<Holiday> holidays)
        {
            foreach (var range in holidays)
            {
                var current = range.Start;
                while (current <= range.End)
                {
                    yield return current;
                    current = current.PlusDays(1);
                }
            }
        }

        private (WeekData Week, IEnumerable<LocalDate> DaysInWeek) GetDaysInWeek(WeekData week)
        {
            return (week, GetDays());

            IEnumerable<LocalDate> GetDays()
            {
                var current = week.WeekStart;
                while (current <= week.WeekEnd)
                {
                    yield return current;
                    current = current.PlusDays(1);
                }
            }
        }

        private void AddHeader(string name, Document document)
        {
            var header = new Paragraph($"Klasse: {name}");
            document.Add(header);
        }

        private Table CreateBaseTable(List<UnitValue> columns, SchoolClass schoolClass)
        {
            var table = new Table(columns.ToArray());
            table.SetWidthPercent(100);
            table.SetHeightPercent(100);
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

        private IEnumerable<IsoDayOfWeek> GetDays(SchoolClass schoolClass)
        {
            if (schoolClass.IsMonday) yield return IsoDayOfWeek.Monday;
            if (schoolClass.IsTuesday) yield return IsoDayOfWeek.Tuesday;
            if (schoolClass.IsWednesday) yield return IsoDayOfWeek.Wednesday;
            if (schoolClass.IsThursday) yield return IsoDayOfWeek.Thursday;
            if (schoolClass.IsFriday) yield return IsoDayOfWeek.Friday;
        }
    }
}

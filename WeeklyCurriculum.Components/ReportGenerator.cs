using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using NodaTime;
using NodaTime.Text;
using WeeklyCurriculum.Contracts;

namespace WeeklyCurriculum.Components
{
    [Export]
    public class ReportGenerator
    {
        private readonly WeekProvider weekProvider;
        private readonly LocalDatePattern datePattern;

        [System.Composition.ImportingConstructor]
        public ReportGenerator(WeekProvider weekProvider)
        {
            this.weekProvider = weekProvider;
            this.datePattern = LocalDatePattern.CreateWithCurrentCulture("d");
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
                var weeksWithHolidayInformation = weeksWithDays.Select(o => this.PopulateWithHolidays(o, relevantDaysOfWeek, allHolidays, schoolYear.YearStart));

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

                var weekCount = 1;
                for (var i = 0; i < schoolWeekSections.Count; i++)
                {
                    var schoolWeekSection = schoolWeekSections[i];
                    this.AddHeader(schoolClass.Name, document);
                    var table = this.CreateBaseTable(columns, schoolClass);
                    foreach (var item in schoolWeekSection)
                    {
                        this.AddRowHeader(table, item.Week, weekCount++);
                        foreach (var day in item.Days)
                        {
                            var cell = new Cell();
                            cell.SetMinHeight(54);
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

        private void AddRowHeader(Table table, WeekData week, int weekCount)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SW: {weekCount}");
            sb.Append(($"{this.datePattern.Format(week.WeekStart)} - {this.datePattern.Format(week.WeekEnd)}"));
            var par = new Paragraph(sb.ToString());
            par.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            var cell = new Cell();
            cell.Add(par);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            table.AddCell(cell);
        }

        private (WeekData Week, List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)> Days) PopulateWithHolidays((WeekData Week, IEnumerable<LocalDate> DaysInWeek) weekWithDays, List<IsoDayOfWeek> relevantDaysOfWeek, List<LocalDate> allHolidays, LocalDate startOfYear)
        {
            var days = new List<(IsoDayOfWeek dayOfWeek, bool IsHoliday)>();
            foreach (var day in weekWithDays.DaysInWeek)
            {
                if (relevantDaysOfWeek.Contains(day.DayOfWeek))
                {
                    if (allHolidays.Contains(day))
                    {
                        days.Add((day.DayOfWeek, true));
                    }
                    else if (day < startOfYear)
                    {
                        days.Add((day.DayOfWeek, true));
                    }
                    else
                    {
                        days.Add((day.DayOfWeek, false));
                    }
                }
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
            table.AddHeaderCell("");
            if (schoolClass.IsMonday)
            {
                var par = new Paragraph("Montag");
                par.SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(par);
            }
            if (schoolClass.IsTuesday)
            {
                var par = new Paragraph("Dienstag");
                par.SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(par);
            }
            if (schoolClass.IsWednesday)
            {
                var par = new Paragraph("Mittwoch");
                par.SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(par);
            }
            if (schoolClass.IsThursday)
            {
                var par = new Paragraph("Donnerstag");
                par.SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(par);
            }
            if (schoolClass.IsFriday)
            {
                var par = new Paragraph("Freitag");
                par.SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(par);
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

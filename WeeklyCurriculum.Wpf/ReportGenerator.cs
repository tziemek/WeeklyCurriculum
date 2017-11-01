using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace WeeklyCurriculum.Wpf
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReportGenerator
    {
        public void Print(SchoolClass schoolClass)
        {
            var dest = @"test.pdf";
            using (var writer = new PdfWriter(dest))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf, PageSize.A4.Rotate()))
            {
                var header = new Paragraph($"Klasse: {schoolClass.Name}");
                document.Add(header);

                var columns = new List<UnitValue>();
                columns.Add(new UnitValue(UnitValue.PERCENT, 20));

                //document.SetMargins(20, 20, 20, 20);
                var columnWidthInPercent = 80.0 / schoolClass.DayCount;
                var columnDefinition = new UnitValue(UnitValue.PERCENT, (float)columnWidthInPercent);
                for (var i = 0; i < schoolClass.DayCount; i++)
                {
                    columns.Add(columnDefinition);
                }
                var table = new Table(columns.ToArray());

                table.SetWidthPercent(100);
                if (schoolClass.IsMonday)
                {
                    table.AddHeaderCell(nameof(schoolClass.Monday));
                    table.AddCell(schoolClass.Monday);
                }
                if (schoolClass.IsTuesday)
                {
                    table.AddHeaderCell(nameof(schoolClass.Tuesday));
                    table.AddCell(schoolClass.Tuesday);
                }
                if (schoolClass.IsWednesday)
                {
                    table.AddHeaderCell(nameof(schoolClass.Wednesday));
                    table.AddCell(schoolClass.Wednesday);
                }
                if (schoolClass.IsThursday)
                {
                    table.AddHeaderCell(nameof(schoolClass.Thursday));
                    table.AddCell(schoolClass.Thursday);
                }
                if (schoolClass.IsFriday)
                {
                    table.AddHeaderCell(nameof(schoolClass.Friday));
                    table.AddCell(schoolClass.Friday);
                }

                document.Add(table);
                document.Close();
            }

            //PdfFont font = PdfFontFactory.createFont(FontConstants.HELVETICA);
            //PdfFont bold = PdfFontFactory.createFont(FontConstants.HELVETICA_BOLD);


            //StringTokenizer tokenizer = new StringTokenizer(line, ";");
            //while (tokenizer.hasMoreTokens())
            //{
            //    if (isHeader)
            //    {
            //        table.addHeaderCell(
            //            new Cell().add(
            //                new Paragraph(tokenizer.nextToken()).setFont(font)));
            //    }
            //    else
            //    {
            //        table.addCell(
            //            new Cell().add(
            //                new Paragraph(tokenizer.nextToken()).setFont(font)));
            //    }
            //}
        }
    }
}

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

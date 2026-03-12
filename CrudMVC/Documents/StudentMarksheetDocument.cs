using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class StudentMarksheetDocument : IDocument
{
    private readonly StudentResultVM _student;

    public StudentMarksheetDocument(StudentResultVM student)
    {
        _student = student;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            // Header
            page.Header()
                .AlignCenter()
                .Text("Rajuk Uttara Model College")
                .FontSize(20)
                .Bold();

            // Content
            page.Content().Column(col =>   
            {
                col.Spacing(20);

                // Row: Student Info (Left) + Grade Criteria (Right)
                col.Item().Row(row =>
                {
                    // Left side: Student info
                    row.RelativeItem(2).Column(leftCol =>
                    {
                        leftCol.Spacing(5);
                        leftCol.Item().Text($"Roll: {_student.RollNumber}");
                        leftCol.Item().Text($"Name: {_student.StudentName}");
                        leftCol.Item().Text($"Class: {_student.ClassName}");
                        leftCol.Item().Text($"Grade: {_student.Grade}");
                    });

                    // Right side: Grade Criteria
                    row.RelativeItem(1).Column(rightCol =>
                    {
                        rightCol.Item().Text("Grade Criteria").Bold().FontSize(12);

                        rightCol.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(3).Text("Marks").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(3).Text("Grade").Bold();
                            });

                            // Data
                            var gradeCriteria = new List<(string Range, string Grade)>
                            {
                                ("90 - 100", "A+"),
                                ("76 - 89", "A"),
                                ("60 - 75", "A-"),
                                ("50 - 59", "B"),
                                ("34 - 49", "C"),
                                ("0 - 33", "F")
                            };

                            foreach (var item in gradeCriteria)
                            {
                                table.Cell().Border(1).Padding(3).Text(item.Range);
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(item.Grade);
                            }
                        });
                    });
                });

                // Spacer
                col.Item().PaddingTop(10);

                // Marks Table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Subject Name
                        columns.RelativeColumn(1); // Marks
                        columns.RelativeColumn(1); // Grade
                    });

                    // Header Row
                    table.Header(header =>
                    {
                        header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Subject").Bold();
                        header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Marks").Bold();
                        header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Grade").Bold();
                    });

                    // Data Rows
                    foreach (var subject in _student.SubjectMarks)
                    {
                        table.Cell().Border(1).Padding(5).Text(subject.SubjectName);
                        table.Cell().Border(1).Padding(5).AlignCenter().Text(subject.Marks.ToString());

                        // Calculate grade for each subject
                        string grade;
                        string gradeColor;

                        if (subject.Marks >= 90) { grade = "A+"; gradeColor = Colors.Green.Darken1; }
                        else if (subject.Marks >= 76) { grade = "A"; gradeColor = Colors.Green.Medium; }
                        else if (subject.Marks >= 60) { grade = "A-"; gradeColor = Colors.Green.Lighten2; }
                        else if (subject.Marks >= 50) { grade = "B"; gradeColor = Colors.Orange.Medium; }
                        else if (subject.Marks >= 34) { grade = "C"; gradeColor = Colors.Orange.Lighten2; }
                        else { grade = "F"; gradeColor = Colors.Red.Medium; }

                        table.Cell().Border(1).Padding(5).AlignCenter()
                            .Text(grade)
                            .FontColor(gradeColor)
                            .Bold();
                    }

                    // Total Marks Row
                    table.Cell().Border(1).Padding(5).Text("Total").Bold();
                    table.Cell().Border(1).Padding(5).AlignCenter().Text(_student.TotalMarks.ToString()).Bold();
                    table.Cell().Border(1).Padding(5).Text(""); 
                });
            });

            // Footer
            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
        });
    }
}
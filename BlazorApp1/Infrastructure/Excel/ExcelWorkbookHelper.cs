using ClosedXML.Excel;
using System.Globalization;
using System.Text;

namespace BlazorApp1.Infrastructure.Excel;

/// <summary>
/// Ho tro tao file mau va doc du lieu theo header tu sheet dau tien cua Excel.
/// </summary>
public static class ExcelWorkbookHelper
{
    public static byte[] CreateTemplate(
        string sheetName,
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<string>> sampleRows)
    {
        if (headers.Count == 0)
        {
            throw new ArgumentException("Template phải có ít nhất 1 cột tiêu đề.", nameof(headers));
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet(string.IsNullOrWhiteSpace(sheetName) ? "Template" : sheetName.Trim());

        for (var col = 0; col < headers.Count; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        for (var row = 0; row < sampleRows.Count; row++)
        {
            var sample = sampleRows[row];
            for (var col = 0; col < headers.Count; col++)
            {
                worksheet.Cell(row + 2, col + 1).Value = col < sample.Count ? sample[col] : string.Empty;
            }
        }

        var lastDataRow = Math.Max(sampleRows.Count + 1, 2);
        var headerRange = worksheet.Range(1, 1, 1, headers.Count);
        var dataRange = worksheet.Range(2, 1, lastDataRow, headers.Count);
        var tableRange = worksheet.Range(1, 1, lastDataRow, headers.Count);

        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F8B84");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#0B6A64");
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#0B6A64");

        dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        dataRange.Style.Alignment.WrapText = true;
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D9E3F0");
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E6EDF6");

        for (var row = 2; row <= lastDataRow; row++)
        {
            if ((row % 2) != 0)
            {
                worksheet.Range(row, 1, row, headers.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBFE");
            }
        }

        tableRange.SetAutoFilter();
        worksheet.SheetView.FreezeRows(1);
        worksheet.Row(1).Height = 24;

        worksheet.Columns(1, headers.Count).AdjustToContents(1, lastDataRow, 12, 48);
        for (var col = 1; col <= headers.Count; col++)
        {
            if (worksheet.Column(col).Width < 16)
            {
                worksheet.Column(col).Width = 16;
            }
        }

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    public static IReadOnlyList<ExcelImportRow> ReadRows(Stream stream)
    {
        return ReadRowsWithContext(stream).Rows;
    }

    public static async Task<IReadOnlyList<ExcelImportRow>> ReadRowsAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var result = await ReadRowsWithContextAsync(stream, cancellationToken);
        return result.Rows;
    }

    public static async Task<ExcelImportReadResult> ReadRowsWithContextAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        await using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        return ReadRowsWithContext(memoryStream);
    }

    public static ExcelImportReadResult ReadRowsWithContext(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault();
        if (worksheet is null)
        {
            return new ExcelImportReadResult(string.Empty, []);
        }

        var usedRange = worksheet.RangeUsed();
        if (usedRange is null)
        {
            return new ExcelImportReadResult(worksheet.Name, []);
        }

        var firstRow = usedRange.FirstRow().RowNumber();
        var lastRow = usedRange.LastRow().RowNumber();
        var firstColumn = usedRange.FirstColumn().ColumnNumber();
        var lastColumn = usedRange.LastColumn().ColumnNumber();

        var headers = new List<string>(lastColumn - firstColumn + 1);
        for (var col = firstColumn; col <= lastColumn; col++)
        {
            headers.Add(NormalizeHeader(worksheet.Cell(firstRow, col).GetFormattedString()));
        }

        var rows = new List<ExcelImportRow>();
        for (var row = firstRow + 1; row <= lastRow; row++)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var hasValue = false;

            for (var index = 0; index < headers.Count; index++)
            {
                var header = headers[index];
                if (string.IsNullOrWhiteSpace(header))
                {
                    continue;
                }

                var cellText = worksheet.Cell(row, firstColumn + index).GetFormattedString().Trim();
                values[header] = cellText;
                if (!string.IsNullOrWhiteSpace(cellText))
                {
                    hasValue = true;
                }
            }

            if (hasValue)
            {
                rows.Add(new ExcelImportRow(row, values));
            }
        }

        return new ExcelImportReadResult(worksheet.Name, rows);
    }

    private static string NormalizeHeader(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
}

public sealed record ExcelImportReadResult(string SheetName, IReadOnlyList<ExcelImportRow> Rows);

/// <summary>
/// Dong du lieu import da duoc map theo tieu de cot.
/// </summary>
public sealed record ExcelImportRow(int RowNumber, IReadOnlyDictionary<string, string> Values)
{
    public string Get(string header)
    {
        if (Values.TryGetValue(header, out var value))
        {
            return value;
        }

        var normalizedExpected = NormalizeHeaderKey(header);
        if (string.IsNullOrWhiteSpace(normalizedExpected))
        {
            return string.Empty;
        }

        foreach (var pair in Values)
        {
            if (NormalizeHeaderKey(pair.Key) == normalizedExpected)
            {
                return pair.Value;
            }
        }

        return string.Empty;
    }

    private static string NormalizeHeaderKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var lowered = char.ToLowerInvariant(ch);
            if (lowered == 'đ')
            {
                lowered = 'd';
            }

            if (char.IsLetterOrDigit(lowered))
            {
                result.Append(lowered);
            }
        }

        return result.ToString();
    }
}

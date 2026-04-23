using BlazorApp1.Models.BaoCao;
using BlazorApp1.Models.Common;
using ClosedXML.Excel;

namespace BlazorApp1.Infrastructure.Excel;

/// <summary>
/// Ho tro xuat file Excel cho cac man hinh bao cao.
/// </summary>
public static class BaoCaoExcelExporter
{
    public static byte[] CreateBaoCaoXuatNhapTonWorkbook(
        IReadOnlyList<BaoCaoXuatNhapTonItemVm> items,
        DateTime tuNgay,
        DateTime denNgay)
    {
        ArgumentNullException.ThrowIfNull(items);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Bao cao XNT");

        const int totalColumns = 7;
        const int headerRow = 4;
        var row = WriteReportHeader(
            worksheet,
            totalColumns,
            "BAO CAO XUAT NHAP TON",
            $"Khoang ap dung: {tuNgay:dd/MM/yyyy} - {denNgay:dd/MM/yyyy}");

        WriteHeaderRow(worksheet, row, ["STT", "Ma san pham", "Ten san pham", "SL dau ky", "SL nhap", "SL xuat", "SL cuoi ky"]);

        var dataStartRow = row + 1;
        for (var index = 0; index < items.Count; index++)
        {
            var currentRow = dataStartRow + index;
            var item = items[index];

            worksheet.Cell(currentRow, 1).Value = index + 1;
            worksheet.Cell(currentRow, 2).Value = item.Ma_San_Pham;
            worksheet.Cell(currentRow, 3).Value = item.Ten_San_Pham;
            worksheet.Cell(currentRow, 4).Value = item.SL_Dau_Ky;
            worksheet.Cell(currentRow, 5).Value = item.SL_Nhap;
            worksheet.Cell(currentRow, 6).Value = item.SL_Xuat;
            worksheet.Cell(currentRow, 7).Value = item.SL_Cuoi_Ky;
        }

        var summaryRow = dataStartRow + items.Count;
        worksheet.Cell(summaryRow, 1).Value = "Tong";
        worksheet.Range(summaryRow, 1, summaryRow, 3).Merge();
        worksheet.Cell(summaryRow, 4).Value = items.Sum(x => x.SL_Dau_Ky);
        worksheet.Cell(summaryRow, 5).Value = items.Sum(x => x.SL_Nhap);
        worksheet.Cell(summaryRow, 6).Value = items.Sum(x => x.SL_Xuat);
        worksheet.Cell(summaryRow, 7).Value = items.Sum(x => x.SL_Cuoi_Ky);

        ApplyCommonStyle(
            worksheet,
            headerRow,
            summaryRow,
            totalColumns,
            numericColumns: [4, 5, 6, 7],
            centerColumns: [1],
            summaryRows: [summaryRow],
            preferredColumnWidths: BuildColumnWidthMap(8, 20, 36, 14, 14, 14, 14),
            wrapColumns: [3]);

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    public static byte[] CreateBaoCaoChiTietNhapWorkbook(
        IReadOnlyList<BaoCaoChiTietNhapItemVm> items,
        DateTime tuNgay,
        DateTime denNgay)
    {
        ArgumentNullException.ThrowIfNull(items);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Bao cao nhap");

        const int totalColumns = 10;
        const int headerRow = 4;
        var row = WriteReportHeader(
            worksheet,
            totalColumns,
            "BAO CAO CHI TIET HANG NHAP",
            $"Khoang ap dung: {tuNgay:dd/MM/yyyy} - {denNgay:dd/MM/yyyy}");

        WriteHeaderRow(
            worksheet,
            row,
            ["STT", "Ngay nhap", "So phieu nhap", "NCC", "Ma san pham", "Ten san pham", "Don vi tien", "SL nhap", "Don gia", "Tri gia"]);

        var dataStartRow = row + 1;
        for (var index = 0; index < items.Count; index++)
        {
            var currentRow = dataStartRow + index;
            var item = items[index];

            worksheet.Cell(currentRow, 1).Value = index + 1;
            worksheet.Cell(currentRow, 2).Value = item.Ngay_Nhap;
            worksheet.Cell(currentRow, 3).Value = item.So_Phieu_Nhap;
            worksheet.Cell(currentRow, 4).Value = item.Ten_NCC;
            worksheet.Cell(currentRow, 5).Value = item.Ma_San_Pham;
            worksheet.Cell(currentRow, 6).Value = item.Ten_San_Pham;
            worksheet.Cell(currentRow, 7).Value = DonViTienOptions.Normalize(item.Don_Vi_Tien);
            worksheet.Cell(currentRow, 8).Value = item.SL_Nhap;
            worksheet.Cell(currentRow, 9).Value = item.Don_Gia;
            worksheet.Cell(currentRow, 10).Value = item.Tri_Gia;
        }

        var summaryRows = new List<int>();
        var currentSummaryRow = dataStartRow + items.Count;

        worksheet.Cell(currentSummaryRow, 1).Value = "Tong so luong";
        worksheet.Range(currentSummaryRow, 1, currentSummaryRow, 7).Merge();
        worksheet.Cell(currentSummaryRow, 8).Value = items.Sum(x => x.SL_Nhap);
        summaryRows.Add(currentSummaryRow);
        currentSummaryRow++;

        var totalsByCurrency = items
            .GroupBy(x => DonViTienOptions.Normalize(x.Don_Vi_Tien))
            .Select(group => new TongTienTheoDonViVm
            {
                Don_Vi_Tien = group.Key,
                Tong_Tri_Gia = group.Sum(x => x.Tri_Gia)
            })
            .OrderBy(x => x.Don_Vi_Tien)
            .ToList();

        if (totalsByCurrency.Count == 0)
        {
            worksheet.Cell(currentSummaryRow, 1).Value = "Tong tri gia";
            worksheet.Range(currentSummaryRow, 1, currentSummaryRow, 9).Merge();
            worksheet.Cell(currentSummaryRow, 10).Value = 0m;
            summaryRows.Add(currentSummaryRow);
        }
        else
        {
            foreach (var total in totalsByCurrency)
            {
                worksheet.Cell(currentSummaryRow, 1).Value = $"Tong tri gia ({total.Don_Vi_Tien})";
                worksheet.Range(currentSummaryRow, 1, currentSummaryRow, 9).Merge();
                worksheet.Cell(currentSummaryRow, 10).Value = total.Tong_Tri_Gia;
                summaryRows.Add(currentSummaryRow);
                currentSummaryRow++;
            }
        }

        ApplyCommonStyle(
            worksheet,
            headerRow,
            summaryRows[^1],
            totalColumns,
            numericColumns: [8, 9, 10],
            centerColumns: [1, 2],
            summaryRows,
            preferredColumnWidths: BuildColumnWidthMap(8, 14, 18, 24, 18, 30, 12, 12, 14, 16),
            wrapColumns: [4, 6]);
        worksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    public static byte[] CreateBaoCaoChiTietXuatWorkbook(
        IReadOnlyList<BaoCaoChiTietXuatItemVm> items,
        DateTime tuNgay,
        DateTime denNgay)
    {
        ArgumentNullException.ThrowIfNull(items);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Bao cao xuat");

        const int totalColumns = 9;
        const int headerRow = 4;
        var row = WriteReportHeader(
            worksheet,
            totalColumns,
            "BAO CAO CHI TIET HANG XUAT",
            $"Khoang ap dung: {tuNgay:dd/MM/yyyy} - {denNgay:dd/MM/yyyy}");

        WriteHeaderRow(
            worksheet,
            row,
            ["STT", "Ngay xuat", "So phieu xuat", "Ma san pham", "Ten san pham", "Don vi tien", "SL xuat", "Don gia", "Tri gia"]);

        var dataStartRow = row + 1;
        for (var index = 0; index < items.Count; index++)
        {
            var currentRow = dataStartRow + index;
            var item = items[index];

            worksheet.Cell(currentRow, 1).Value = index + 1;
            worksheet.Cell(currentRow, 2).Value = item.Ngay_Xuat;
            worksheet.Cell(currentRow, 3).Value = item.So_Phieu_Xuat;
            worksheet.Cell(currentRow, 4).Value = item.Ma_San_Pham;
            worksheet.Cell(currentRow, 5).Value = item.Ten_San_Pham;
            worksheet.Cell(currentRow, 6).Value = DonViTienOptions.Normalize(item.Don_Vi_Tien);
            worksheet.Cell(currentRow, 7).Value = item.SL_Xuat;
            worksheet.Cell(currentRow, 8).Value = item.Don_Gia;
            worksheet.Cell(currentRow, 9).Value = item.Tri_Gia;
        }

        var summaryRows = new List<int>();
        var currentSummaryRow = dataStartRow + items.Count;

        worksheet.Cell(currentSummaryRow, 1).Value = "Tong so luong";
        worksheet.Range(currentSummaryRow, 1, currentSummaryRow, 6).Merge();
        worksheet.Cell(currentSummaryRow, 7).Value = items.Sum(x => x.SL_Xuat);
        summaryRows.Add(currentSummaryRow);
        currentSummaryRow++;

        var totalsByCurrency = items
            .GroupBy(x => DonViTienOptions.Normalize(x.Don_Vi_Tien))
            .Select(group => new TongTienTheoDonViVm
            {
                Don_Vi_Tien = group.Key,
                Tong_Tri_Gia = group.Sum(x => x.Tri_Gia)
            })
            .OrderBy(x => x.Don_Vi_Tien)
            .ToList();

        if (totalsByCurrency.Count == 0)
        {
            worksheet.Cell(currentSummaryRow, 1).Value = "Tong tri gia";
            worksheet.Range(currentSummaryRow, 1, currentSummaryRow, 8).Merge();
            worksheet.Cell(currentSummaryRow, 9).Value = 0m;
            summaryRows.Add(currentSummaryRow);
        }
        else
        {
            foreach (var total in totalsByCurrency)
            {
                worksheet.Cell(currentSummaryRow, 1).Value = $"Tong tri gia ({total.Don_Vi_Tien})";
                worksheet.Range(currentSummaryRow, 1, currentSummaryRow, 8).Merge();
                worksheet.Cell(currentSummaryRow, 9).Value = total.Tong_Tri_Gia;
                summaryRows.Add(currentSummaryRow);
                currentSummaryRow++;
            }
        }

        ApplyCommonStyle(
            worksheet,
            headerRow,
            summaryRows[^1],
            totalColumns,
            numericColumns: [7, 8, 9],
            centerColumns: [1, 2],
            summaryRows,
            preferredColumnWidths: BuildColumnWidthMap(8, 14, 18, 18, 30, 12, 12, 14, 16),
            wrapColumns: [5]);
        worksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    private static int WriteReportHeader(
        IXLWorksheet worksheet,
        int totalColumns,
        string title,
        string periodText)
    {
        worksheet.Cell(1, 1).Value = title;
        worksheet.Range(1, 1, 1, totalColumns).Merge();
        worksheet.Cell(2, 1).Value = periodText;
        worksheet.Range(2, 1, 2, totalColumns).Merge();

        var titleRange = worksheet.Range(1, 1, 1, totalColumns);
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 14;
        titleRange.Style.Font.FontColor = XLColor.FromHtml("#0f3b61");

        var periodRange = worksheet.Range(2, 1, 2, totalColumns);
        periodRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        periodRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        periodRange.Style.Font.FontColor = XLColor.FromHtml("#3f5e7a");
        periodRange.Style.Font.Italic = true;
        periodRange.Style.Font.FontSize = 11;

        worksheet.Row(1).Height = 32;
        worksheet.Row(2).Height = 24;

        return 4;
    }

    private static void WriteHeaderRow(IXLWorksheet worksheet, int row, IReadOnlyList<string> headers)
    {
        for (var col = 0; col < headers.Count; col++)
        {
            worksheet.Cell(row, col + 1).Value = headers[col];
        }
    }

    private static void ApplyCommonStyle(
        IXLWorksheet worksheet,
        int headerRow,
        int lastRow,
        int totalColumns,
        IReadOnlyList<int> numericColumns,
        IReadOnlyList<int> centerColumns,
        IReadOnlyList<int> summaryRows,
        IReadOnlyDictionary<int, double>? preferredColumnWidths = null,
        IReadOnlyList<int>? wrapColumns = null)
    {
        var headerRange = worksheet.Range(headerRow, 1, headerRow, totalColumns);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontSize = 11;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0f8b84");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Alignment.WrapText = true;

        var fullRange = worksheet.Range(headerRow, 1, lastRow, totalColumns);
        fullRange.Style.Font.FontSize = 11;
        fullRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        fullRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#c9d8e6");
        fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        fullRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#dce7f3");

        for (var row = headerRow + 1; row <= lastRow; row++)
        {
            if ((row % 2) == 0)
            {
                worksheet.Range(row, 1, row, totalColumns).Style.Fill.BackgroundColor = XLColor.FromHtml("#f8fbfe");
            }
        }

        foreach (var col in numericColumns)
        {
            var range = worksheet.Range(headerRow + 1, col, lastRow, col);
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            range.Style.NumberFormat.Format = "#,##0.##";
        }

        foreach (var col in centerColumns)
        {
            worksheet.Range(headerRow + 1, col, lastRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        if (wrapColumns is not null)
        {
            foreach (var col in wrapColumns)
            {
                worksheet.Range(headerRow + 1, col, lastRow, col).Style.Alignment.WrapText = true;
            }
        }

        var summaryRowSet = summaryRows.ToHashSet();
        foreach (var summaryRow in summaryRows)
        {
            var summaryRange = worksheet.Range(summaryRow, 1, summaryRow, totalColumns);
            summaryRange.Style.Font.Bold = true;
            summaryRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#eef6ff");
        }

        worksheet.Row(headerRow).Height = 30;
        for (var row = headerRow + 1; row <= lastRow; row++)
        {
            worksheet.Row(row).Height = summaryRowSet.Contains(row) ? 27 : 23;
        }

        worksheet.SheetView.FreezeRows(headerRow);
        worksheet.Range(headerRow, 1, lastRow, totalColumns).SetAutoFilter();
        worksheet.Columns(1, totalColumns).AdjustToContents(1, lastRow, 14, 62);

        if (preferredColumnWidths is not null)
        {
            foreach (var (column, minWidth) in preferredColumnWidths)
            {
                if (column < 1 || column > totalColumns)
                {
                    continue;
                }

                if (worksheet.Column(column).Width < minWidth)
                {
                    worksheet.Column(column).Width = minWidth;
                }
            }
        }
    }

    private static IReadOnlyDictionary<int, double> BuildColumnWidthMap(params double[] widths)
    {
        var result = new Dictionary<int, double>(widths.Length);
        for (var index = 0; index < widths.Length; index++)
        {
            if (widths[index] <= 0)
            {
                continue;
            }

            result[index + 1] = widths[index];
        }

        return result;
    }
}

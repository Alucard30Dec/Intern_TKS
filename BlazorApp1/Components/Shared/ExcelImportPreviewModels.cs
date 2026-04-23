namespace BlazorApp1.Components.Shared;

public enum ExcelImportPreviewStatus
{
    Valid = 0,
    Warning = 1,
    Error = 2
}

public sealed record ExcelImportPreviewRow(
    int DisplayIndex,
    int SourceIndex,
    int ExcelRowNumber,
    IReadOnlyList<string> Cells,
    ExcelImportPreviewStatus Status,
    string Message)
{
    public bool CanImport => Status != ExcelImportPreviewStatus.Error;
}

public sealed record ExcelImportPreviewCellContext(
    ExcelImportPreviewRow Row,
    int CellIndex,
    string? CellValue);

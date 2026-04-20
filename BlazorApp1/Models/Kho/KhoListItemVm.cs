namespace BlazorApp1.Models.Kho;

/// <summary>
/// Du lieu hien thi danh sach kho.
/// </summary>
public sealed class KhoListItemVm
{
    public int Kho_ID { get; set; }
    public string Ten_Kho { get; set; } = string.Empty;
    public string? Ghi_Chu { get; set; }
    public bool Is_Active { get; set; }
}

namespace BlazorApp1.Models.NhaCungCap;

/// <summary>
/// Du lieu hien thi danh sach nha cung cap.
/// </summary>
public sealed class NhaCungCapListItemVm
{
    public int NCC_ID { get; set; }
    public string Ma_NCC { get; set; } = string.Empty;
    public string Ten_NCC { get; set; } = string.Empty;
    public string? Ghi_Chu { get; set; }
    public bool Is_Active { get; set; }
}

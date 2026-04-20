namespace BlazorApp1.Models.LoaiSanPham;

/// <summary>
/// Du lieu hien thi danh sach loai san pham.
/// </summary>
public sealed class LoaiSanPhamListItemVm
{
    public int Loai_San_Pham_ID { get; set; }
    public string Ma_LSP { get; set; } = string.Empty;
    public string Ten_LSP { get; set; } = string.Empty;
    public string? Ghi_Chu { get; set; }
}

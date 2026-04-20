namespace BlazorApp1.Models.LoaiSanPham;

/// <summary>
/// Du lieu hien thi danh sach loai san pham.
/// </summary>
public sealed class LoaiSanPhamListItemVm
{
    public int Id { get; set; }
    public string MaLsp { get; set; } = string.Empty;
    public string TenLsp { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
}


namespace BlazorApp1.Models.SanPham;

/// <summary>
/// Du lieu hien thi danh sach san pham.
/// </summary>
public sealed class SanPhamListItemVm
{
    public int San_Pham_ID { get; set; }
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public int Loai_San_Pham_ID { get; set; }
    public string Ten_LSP { get; set; } = string.Empty;
    public int Don_Vi_Tinh_ID { get; set; }
    public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;
    public string? Ghi_Chu { get; set; }
    public bool Is_Active { get; set; }
}

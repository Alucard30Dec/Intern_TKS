namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu in phieu xuat kho (bai 14).
/// </summary>
public sealed class XuatKhoPrintVm
{
    public int Xuat_Kho_ID { get; set; }
    public string So_Phieu_Xuat_Kho { get; set; } = string.Empty;
    public DateTime Ngay_Xuat_Kho { get; set; }
    public string Ten_Kho { get; set; } = string.Empty;
    public string Don_Vi_Tien { get; set; } = BlazorApp1.Models.Common.DonViTienOptions.Vnd;
    public string? Ghi_Chu { get; set; }
    public IReadOnlyList<XuatKhoPrintLineVm> Lines { get; set; } = [];
    public decimal Tong_Tri_Gia => Lines.Sum(x => x.Tri_Gia);
}

/// <summary>
/// Du lieu tung dong chi tiet cho phieu in xuat kho.
/// </summary>
public sealed class XuatKhoPrintLineVm
{
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;
    public decimal SL_Xuat { get; set; }
    public decimal Don_Gia_Xuat { get; set; }
    public decimal Tri_Gia => SL_Xuat * Don_Gia_Xuat;
}

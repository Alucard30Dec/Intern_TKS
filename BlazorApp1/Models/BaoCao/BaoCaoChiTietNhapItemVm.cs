namespace BlazorApp1.Models.BaoCao;

/// <summary>
/// Dong du lieu bao cao chi tiet hang nhap (bai 15).
/// </summary>
public sealed class BaoCaoChiTietNhapItemVm
{
    public DateTime Ngay_Nhap { get; set; }
    public string So_Phieu_Nhap { get; set; } = string.Empty;
    public string Ten_NCC { get; set; } = string.Empty;
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public string Don_Vi_Tien { get; set; } = BlazorApp1.Models.Common.DonViTienOptions.Vnd;
    public decimal SL_Nhap { get; set; }
    public decimal Don_Gia { get; set; }
    public decimal Tri_Gia => SL_Nhap * Don_Gia;
}

namespace BlazorApp1.Models.BaoCao;

/// <summary>
/// Dong du lieu bao cao chi tiet hang xuat (bai 16).
/// </summary>
public sealed class BaoCaoChiTietXuatItemVm
{
    public DateTime Ngay_Xuat { get; set; }
    public string So_Phieu_Xuat { get; set; } = string.Empty;
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public string Don_Vi_Tien { get; set; } = BlazorApp1.Models.Common.DonViTienOptions.Vnd;
    public decimal SL_Xuat { get; set; }
    public decimal Don_Gia { get; set; }
    public decimal Tri_Gia => SL_Xuat * Don_Gia;
}

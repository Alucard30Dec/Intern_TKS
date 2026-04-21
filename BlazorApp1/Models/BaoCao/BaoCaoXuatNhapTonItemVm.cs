namespace BlazorApp1.Models.BaoCao;

/// <summary>
/// Dong du lieu bao cao xuat nhap ton (bai 17).
/// </summary>
public sealed class BaoCaoXuatNhapTonItemVm
{
    public int San_Pham_ID { get; set; }
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public decimal SL_Dau_Ky { get; set; }
    public decimal SL_Nhap { get; set; }
    public decimal SL_Xuat { get; set; }
    public decimal SL_Cuoi_Ky => SL_Dau_Ky + SL_Nhap - SL_Xuat;
}

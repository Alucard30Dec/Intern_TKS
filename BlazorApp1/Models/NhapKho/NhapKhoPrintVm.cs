namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu in phieu nhap kho (bai 10).
/// </summary>
public sealed class NhapKhoPrintVm
{
    public int Nhap_Kho_ID { get; set; }
    public string So_Phieu_Nhap_Kho { get; set; } = string.Empty;
    public DateTime Ngay_Nhap_Kho { get; set; }
    public string Ten_Kho { get; set; } = string.Empty;
    public string Ten_NCC { get; set; } = string.Empty;
    public string? Ghi_Chu { get; set; }
    public IReadOnlyList<NhapKhoPrintLineVm> Lines { get; set; } = [];
    public decimal Tong_Tri_Gia => Lines.Sum(x => x.Tri_Gia);
}

/// <summary>
/// Du lieu tung dong chi tiet cho phieu in.
/// </summary>
public sealed class NhapKhoPrintLineVm
{
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;
    public decimal SL_Nhap { get; set; }
    public decimal Don_Gia_Nhap { get; set; }
    public decimal Tri_Gia => SL_Nhap * Don_Gia_Nhap;
}


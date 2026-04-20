namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu hien thi chi tiet cua mot phieu nhap kho.
/// </summary>
public sealed class NhapKhoDetailListItemVm
{
    public int Nhap_Kho_Raw_Data_ID { get; set; }
    public int San_Pham_ID { get; set; }
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public decimal SL_Nhap { get; set; }
    public decimal Don_Gia_Nhap { get; set; }
    public decimal Tri_Gia => SL_Nhap * Don_Gia_Nhap;
}

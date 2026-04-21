namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu hien thi chi tiet cua mot phieu xuat kho.
/// </summary>
public sealed class XuatKhoDetailListItemVm
{
    public int Xuat_Kho_Raw_Data_ID { get; set; }
    public int San_Pham_ID { get; set; }
    public string Ma_San_Pham { get; set; } = string.Empty;
    public string Ten_San_Pham { get; set; } = string.Empty;
    public decimal SL_Xuat { get; set; }
    public decimal Don_Gia_Xuat { get; set; }
    public decimal Tri_Gia => SL_Xuat * Don_Gia_Xuat;
}

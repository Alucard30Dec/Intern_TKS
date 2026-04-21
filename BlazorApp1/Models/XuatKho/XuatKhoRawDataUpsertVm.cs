using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu nhap cho tung dong chi tiet phieu xuat kho.
/// </summary>
public sealed class XuatKhoRawDataUpsertVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không được để trống.")]
    public int San_Pham_ID { get; set; }

    public decimal SL_Xuat { get; set; } = 1m;

    public decimal Don_Gia_Xuat { get; set; }

    public string SL_Xuat_Text { get; set; } = "1";

    public string Don_Gia_Xuat_Text { get; set; } = string.Empty;
}

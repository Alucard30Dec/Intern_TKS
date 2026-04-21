using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu them moi 1 dong chi tiet phieu xuat kho (bai 13).
/// </summary>
public sealed class XuatKhoDetailCreateVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không được để trống.")]
    public int San_Pham_ID { get; set; }

    public decimal SL_Xuat { get; set; }

    public decimal Don_Gia_Xuat { get; set; }
}

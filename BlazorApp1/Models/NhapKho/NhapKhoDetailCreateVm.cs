using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu them moi 1 dong chi tiet phieu nhap kho (bai 9).
/// </summary>
public sealed class NhapKhoDetailCreateVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không được để trống.")]
    public int San_Pham_ID { get; set; }

    public decimal SL_Nhap { get; set; }

    public decimal Don_Gia_Nhap { get; set; }
}


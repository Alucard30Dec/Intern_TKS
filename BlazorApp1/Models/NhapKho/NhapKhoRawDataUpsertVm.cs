using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu nhap cho tung dong chi tiet phieu nhap kho.
/// </summary>
public sealed class NhapKhoRawDataUpsertVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không được để trống.")]
    public int San_Pham_ID { get; set; }

    public decimal SL_Nhap { get; set; } = 1m;

    public decimal Don_Gia_Nhap { get; set; }

    public string SL_Nhap_Text { get; set; } = "1";

    public string Don_Gia_Nhap_Text { get; set; } = string.Empty;
}

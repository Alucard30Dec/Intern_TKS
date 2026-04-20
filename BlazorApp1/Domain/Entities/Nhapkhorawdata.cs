using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the chi tiet phieu nhap kho trong bai 7.
/// </summary>
public sealed class NhapKhoRawData
{
    public int Nhap_Kho_Raw_Data_ID { get; set; }

    [Required]
    public int Nhap_Kho_ID { get; set; }

    [Required]
    public int San_Pham_ID { get; set; }

    [Required]
    public decimal SL_Nhap { get; set; }

    [Required]
    public decimal Don_Gia_Nhap { get; set; }

    public bool Is_Active { get; set; } = true;

    public NhapKho? Nhap_Kho { get; set; }
    public SanPham? San_Pham { get; set; }
}

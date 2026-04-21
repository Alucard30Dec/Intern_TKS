using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the chi tiet phieu xuat kho trong bai 11.
/// </summary>
public sealed class XuatKhoRawData
{
    public int Xuat_Kho_Raw_Data_ID { get; set; }

    [Required]
    public int Xuat_Kho_ID { get; set; }

    [Required]
    public int San_Pham_ID { get; set; }

    [Required]
    public decimal SL_Xuat { get; set; }

    [Required]
    public decimal Don_Gia_Xuat { get; set; }

    public bool Is_Active { get; set; } = true;

    public XuatKho? Xuat_Kho { get; set; }
    public SanPham? San_Pham { get; set; }
}

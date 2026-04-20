using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the san pham trong danh muc bai 3.
/// </summary>
public sealed class SanPham
{
    public int San_Pham_ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string Ma_San_Pham { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Ten_San_Pham { get; set; } = string.Empty;

    [Required]
    public int Loai_San_Pham_ID { get; set; }

    [Required]
    public int Don_Vi_Tinh_ID { get; set; }

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }

    public bool Is_Active { get; set; } = true;

    public LoaiSanPham? Loai_San_Pham { get; set; }
    public DonViTinh? Don_Vi_Tinh { get; set; }
}

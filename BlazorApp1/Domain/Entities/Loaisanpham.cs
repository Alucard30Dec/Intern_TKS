using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the loai san pham trong danh muc bai 2.
/// </summary>
public sealed class LoaiSanPham
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string MaLsp { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string TenLsp { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? GhiChu { get; set; }
}


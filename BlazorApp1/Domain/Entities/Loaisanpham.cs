using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the loai san pham trong danh muc bai 2.
/// </summary>
public sealed class LoaiSanPham
{
    public int Loai_San_Pham_ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string Ma_LSP { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Ten_LSP { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }
}

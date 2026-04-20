using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the don vi tinh trong danh muc bai 1.
/// </summary>
public sealed class DonViTinh
{
    public int Don_Vi_Tinh_ID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }
}

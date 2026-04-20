using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the don vi tinh trong danh muc bai 1.
/// </summary>
public sealed class DonViTinh
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string TenDonViTinh { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? GhiChu { get; set; }
}

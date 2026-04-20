using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the nha cung cap trong danh muc bai 4.
/// </summary>
public sealed class NhaCungCap
{
    public int NCC_ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string Ma_NCC { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Ten_NCC { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }

    public bool Is_Active { get; set; } = true;
}

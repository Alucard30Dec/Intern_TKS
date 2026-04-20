using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the kho trong danh muc bai 5.
/// </summary>
public sealed class Kho
{
    public int Kho_ID { get; set; }

    [Required]
    [MaxLength(150)]
    public string Ten_Kho { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }

    public bool Is_Active { get; set; } = true;
}

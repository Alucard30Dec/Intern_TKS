using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the phan quyen kho-user trong bai 6.
/// </summary>
public sealed class KhoUser
{
    public int Kho_User_ID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Ma_Dang_Nhap { get; set; } = string.Empty;

    [Required]
    public int Kho_ID { get; set; }

    public bool Is_Active { get; set; } = true;

    public Kho? Kho { get; set; }
}
